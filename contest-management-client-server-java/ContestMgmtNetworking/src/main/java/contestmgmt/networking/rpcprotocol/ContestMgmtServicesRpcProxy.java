package contestmgmt.networking.rpcprotocol;

import contestmgmt.model.*;
import contestmgmt.networking.dto.*;
import contestmgmt.services.ContestManagementException;
import contestmgmt.services.IContestManagementObserver;
import contestmgmt.services.IContestManagementServices;

import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.net.Socket;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.concurrent.BlockingQueue;
import java.util.concurrent.LinkedBlockingQueue;
import java.util.stream.Collectors;

public class ContestMgmtServicesRpcProxy implements IContestManagementServices {
    private final String host;

    private final int port;

    private IContestManagementObserver client;

    private ObjectInputStream input;

    private ObjectOutputStream output;

    private Socket connection;

    private BlockingQueue<Response> responseQueue = new LinkedBlockingQueue<>();;

    private volatile boolean finished;

    public ContestMgmtServicesRpcProxy(String host, int port) {
        this.host = host;
        this.port = port;
    }

    @Override
    public void login(Organiser organiser, IContestManagementObserver client) throws ContestManagementException {
        initializeConnection();
        OrganiserDTO organiserDTO = DTOUtils.getDTO(organiser);

        Request request = new Request.Builder().type(RequestType.Login).data(organiserDTO).build();
        sendRequest(request);

        Response response = readResponse();
        if (response.getType() == ResponseType.Error) {
            closeConnection();
            throw new ContestManagementException(response.getData().toString());
        }

        this.client = client;
    }

    @Override
    public void logout(Organiser organiser, IContestManagementObserver client) throws ContestManagementException {
        OrganiserDTO organiserDTO = DTOUtils.getDTO(organiser);

        Request request = new Request.Builder().type(RequestType.Logout).data(organiserDTO).build();
        sendRequest(request);

        Response response = readResponse();
        closeConnection();
        if (response.getType() == ResponseType.Error)
            throw new ContestManagementException(response.getData().toString());
    }

//    @Override
//    public Participant findParticipantById(Long participantId) {
//        return null;
//    }

    @Override
    public List<Competition> getCompetitionsByString(String competitionType, String ageCategory) throws ContestManagementException {
        StringStringDTO strDTO = DTOUtils.getDTO(competitionType, ageCategory);
        Request request = new Request.Builder().type(RequestType.GetCompetitionsByString).data(strDTO).build();
        sendRequest(request);

        Response response = readResponse();
        if (response.getType() == ResponseType.Error)
            throw new ContestManagementException(response.getData().toString());

        return Arrays.stream((CompetitionDTO[]) response.getData())
                .map(DTOUtils::getFromDTO)
                .collect(Collectors.toList());
    }

    @Override
    public List<Participant> getParticipantsByCompetition(Competition competition) throws ContestManagementException {
        CompetitionDTO competitionDTO = DTOUtils.getDTO(competition);
        Request request = new Request.Builder().type(RequestType.GetParticipantsByCompetition).data(competitionDTO)
                .build();
        sendRequest(request);

        Response response = readResponse();
        if (response.getType() == ResponseType.Error)
            throw new ContestManagementException(response.getData().toString());

        return Arrays.stream((ParticipantDTO[]) response.getData())
                .map(DTOUtils::getFromDTO)
                .collect(Collectors.toList());
    }

    @Override
    public Map<Long, Tuple<Competition, Integer>> countParticipantsForEachCompetition(
            String competitionType, String AgeCategory) throws ContestManagementException {
        StringStringDTO strDTO = DTOUtils.getDTO(competitionType, AgeCategory);
        Request request = new Request.Builder().type(RequestType.CountParticipantsForEachCompetition).data(strDTO).build();
        sendRequest(request);

        Response response = readResponse();
        if (response.getType() == ResponseType.Error)
            throw new ContestManagementException(response.getData().toString());

        IdCompetitionIntegerDTO[] dtos = (IdCompetitionIntegerDTO[]) response.getData();
        return Arrays.stream(dtos)
                .collect(Collectors.toMap(IdCompetitionIntegerDTO::getCompetitionId,
                        dto -> {
                            Long id = dto.getCompetitionId();
                            String compType = dto.getCompetitionType();
                            String ageCat = dto.getAgeCategory();
                            Competition c = new Competition(compType, ageCat);
                            c.setId(id);

                            Integer count = dto.getCount();
                            return new Tuple<>(c, count);
                        }));
    }

    @Override
    public List<Long> getParticipantIdsByName(String firstName, String lastName) throws ContestManagementException {
        StringStringDTO stringStringDTO = DTOUtils.getDTO(firstName, lastName);
        Request request = new Request.Builder().type(RequestType.GetParticipantIdsByName).data(stringStringDTO).build();
        sendRequest(request);

        Response response = readResponse();
        if (response.getType() == ResponseType.Error)
            throw new ContestManagementException(response.getData().toString());

        return Arrays.asList((Long[]) response.getData());
    }

    @Override
    public List<String> getAgeCategoriesForParticipant(Participant participant, String competitionType) throws ContestManagementException {
        ParticipantStringDTO psDTO = DTOUtils.getDTO(participant, competitionType);
        Request request = new Request.Builder().type(RequestType.GetAgeCategoriesForParticipant).data(psDTO).build();
        sendRequest(request);

        Response response = readResponse();
        if (response.getType() == ResponseType.Error)
            throw new ContestManagementException(response.getData().toString());

        return Arrays.asList((String[]) response.getData());
    }

    @Override
    public void addRegistration(Registration registration) throws ContestManagementException {
        RegistrationDTO regDTO = DTOUtils.getDTO(registration);
        Request request = new Request.Builder().type(RequestType.AddRegistration).data(regDTO).build();
        sendRequest(request);

        Response response = readResponse();
        if (response.getType() == ResponseType.Error)
            throw new ContestManagementException(response.getData().toString());
    }

    private void initializeConnection() throws ContestManagementException {
        try {
            connection = new Socket(host, port);
            output = new ObjectOutputStream(connection.getOutputStream());
            output.flush();
            input = new ObjectInputStream(connection.getInputStream());
            finished = false;
            startReader();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    private void closeConnection() {
        finished = true;
        try {
            input.close();
            output.close();
            connection.close();
            client = null;
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    private void startReader() {
        Thread wt = new Thread(new ReaderThread());
        wt.start();
    }

    private class ReaderThread implements Runnable {
        @Override
        public void run() {
            while (!finished) {
                try {
                    Object resp = input.readObject();
                    System.out.println("Response received " + resp);
                    Response response = (Response) resp;
                    if (isUpdate(response))
                        handleUpdate(response);
                    else  {
                        try {
                            responseQueue.put(response);
                        } catch (InterruptedException e) {
                            e.printStackTrace();
                        }
                    }
                } catch (IOException | ClassNotFoundException e) {
                    System.out.println("Error reading " + e);
                }
            }
        }
    }

    private boolean isUpdate(Response response) {
        return response.getType() == ResponseType.NewRegistration;
    }

    private void handleUpdate(Response response) {
        if (response.getType() == ResponseType.NewRegistration) {
            RegistrationDTO regDTO = (RegistrationDTO) response.getData();
            Registration registration = DTOUtils.getFromDTO(regDTO);

            System.out.println("New registration" + registration);

            try {
                client.newRegistration(registration);
            } catch (ContestManagementException e) {
                e.printStackTrace();
            }
        }
    }

    private void sendRequest(Request request) throws ContestManagementException {
        try {
            output.writeObject(request);
            output.flush();
        } catch (IOException e) {
            throw new ContestManagementException("Error sending object " + e);
        }
    }

    private Response readResponse() throws ContestManagementException {
        Response response = null;
        try {
            response = responseQueue.take();
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
        return response;
    }
}
