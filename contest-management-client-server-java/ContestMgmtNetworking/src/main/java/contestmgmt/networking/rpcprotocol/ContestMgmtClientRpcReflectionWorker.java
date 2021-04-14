package contestmgmt.networking.rpcprotocol;

import contestmgmt.model.*;
import contestmgmt.networking.dto.*;
import contestmgmt.services.ContestManagementException;
import contestmgmt.services.IContestManagementObserver;
import contestmgmt.services.IContestManagementServices;

import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.net.Socket;
import java.util.List;
import java.util.Map;

public class ContestMgmtClientRpcReflectionWorker implements Runnable, IContestManagementObserver {
    private IContestManagementServices server;

    private Socket connection;

    private ObjectInputStream input;

    private ObjectOutputStream output;

    private volatile boolean connected;

    public ContestMgmtClientRpcReflectionWorker(IContestManagementServices server, Socket connection) {
        this.server = server;
        this.connection = connection;

        try {
            output = new ObjectOutputStream(connection.getOutputStream());
            output.flush();
            input = new ObjectInputStream(connection.getInputStream());
            connected = true;
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    @Override
    public void run() {
        while (connected) {
            try {
                Object request = input.readObject();
                Response response = handleRequest((Request) request);
                if (response != null)
                    sendResponse(response);
            } catch (IOException | ClassNotFoundException e) {
                e.printStackTrace();
            }

            try {
                Thread.sleep(1000);
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }

        try {
            input.close();
            output.close();
            connection.close();
        } catch (IOException e) {
            System.out.println("Error " + e);
        }
    }

    @Override
    public void newRegistration(Registration r) throws ContestManagementException {
        RegistrationDTO registrationDTO = DTOUtils.getDTO(r);
        Response response = new Response.Builder().type(ResponseType.NewRegistration).data(registrationDTO).build();
        System.out.println(response.getType().toString() + " " + r);
        try {
            sendResponse(response);
        } catch (IOException e) {
            throw new ContestManagementException("Error sending: " + e);
        }
    }

    private static Response okResponse = new Response.Builder().type(ResponseType.Ok).build();

    private Response handleRequest(Request request) {
        Response response = null;
        String handlerName = "handle" + request.getType();
        System.out.println("HandlerName " + handlerName);

        try {
            Method method = getClass().getDeclaredMethod(handlerName, Request.class);
            System.out.println(request.getType().toString() + " request...");
            response = (Response) method.invoke(this, request);
            System.out.println("Method %s invoked".formatted(handlerName));
        } catch (NoSuchMethodException | InvocationTargetException | IllegalAccessException e) {
            e.printStackTrace();
        }

        return response;
    }

    private void sendResponse(Response response) throws IOException {
        System.out.println("Sending response " + response);
        output.writeObject(response);
        output.flush();
    }

    private Response handleLogin(Request request) {
        System.out.println("Login request...");
        OrganiserDTO organiserDTO = (OrganiserDTO) request.getData();
        Organiser organiser = DTOUtils.getFromDTO(organiserDTO);

        try {
            server.login(organiser, this);
            return okResponse;
        } catch (ContestManagementException e) {
            connected = false;
            return new Response.Builder().type(ResponseType.Error).data(e.getMessage()).build();
        }

    }

    private Response handleLogout(Request request) {
        OrganiserDTO organiserDTO = (OrganiserDTO) request.getData();
        Organiser organiser = DTOUtils.getFromDTO(organiserDTO);

        try {
            server.logout(organiser, this);
            connected = false;
            return okResponse;
        } catch (ContestManagementException e) {
            return new Response.Builder().type(ResponseType.Error).data(e.getMessage()).build();
        }
    }

//    private Response handleGetParticipantById(Request request) {
//        System.out.println(request.getType().toString() + " request...");
//        Long participantId = (Long) request.getData();
//        try {
//            Participant p = server.findParticipantById(participantId);
//            return new Response.Builder().type(ResponseType.GetParticipantById).data(p).build();
//        }
//    }

    private Response handleGetCompetitionsByString(Request request) {
        StringStringDTO strDTO = (StringStringDTO) request.getData();

        try {
            List<Competition> competitions = server.getCompetitionsByString(strDTO.getFirst(), strDTO.getSecond());
            CompetitionDTO[] competitionDTOs = competitions.stream()
                    .map(DTOUtils::getDTO)
                    .toArray(CompetitionDTO[]::new);
            return new Response.Builder().type(ResponseType.GetCompetitionsByString).data(competitionDTOs).build();
        } catch (ContestManagementException e) {
            return new Response.Builder().type(ResponseType.Error).data(e.getMessage()).build();
        }
    }

    private Response handleGetParticipantsByCompetition(Request request) {
        CompetitionDTO competitionDTO = (CompetitionDTO) request.getData();
        Competition competition = DTOUtils.getFromDTO(competitionDTO);

        try {
            List<Participant> participants = server.getParticipantsByCompetition(competition);
            ParticipantDTO[] participantDTOs = participants.stream()
                    .map(DTOUtils::getDTO)
                    .toArray(ParticipantDTO[]::new);
            return new Response.Builder().type(ResponseType.GetParticipantsByCompetition).data(participantDTOs).build();
        } catch (ContestManagementException e) {
            return new Response.Builder().type(ResponseType.Error).data(e.getMessage()).build();
        }
    }

    private Response handleCountParticipantsForEachCompetition(Request request) {
        StringStringDTO stringStringDTO = (StringStringDTO) request.getData();
        String competitionType = stringStringDTO.getFirst();
        String ageCategory = stringStringDTO.getSecond();
        try {
            var map = server.countParticipantsForEachCompetition(competitionType, ageCategory);
            IdCompetitionIntegerDTO[] dtos = map.values().stream()
                    .map(competitionIntegerTuple -> {
                        Competition c = competitionIntegerTuple.getLeft();
                        Integer count = competitionIntegerTuple.getRight();
                        return DTOUtils.getDTO(c.getId(), c, count);
                    })
                    .toArray(IdCompetitionIntegerDTO[]::new);
            return new Response.Builder().type(ResponseType.CountParticipantsForEachCompetition).data(dtos).build();
        } catch (ContestManagementException e) {
            return new Response.Builder().type(ResponseType.Error).data(e.getMessage()).build();
        }
    }

    private Response handleGetParticipantIdsByName(Request request) {
        StringStringDTO stringStringDTO = (StringStringDTO) request.getData();

        try {
            Long[] participantIds = server.getParticipantIdsByName(stringStringDTO.getFirst(),
                    stringStringDTO.getSecond()).toArray(new Long[0]);
            return new Response.Builder().type(ResponseType.GetParticipantIdsByName).data(participantIds).build();
        } catch (ContestManagementException e) {
            return new Response.Builder().type(ResponseType.Error).data(e.getMessage()).build();
        }
    }

    private Response handleGetAgeCategoriesForParticipant(Request request) {
        ParticipantStringDTO psDTO = (ParticipantStringDTO) request.getData();
        Participant participant = DTOUtils.getFromDTO(psDTO.getParticipantDTO());
        String competitionType = psDTO.getString();

        try {
            String[] ageCategories = server.getAgeCategoriesForParticipant(participant, competitionType)
                    .toArray(new String[0]);
            return new Response.Builder().type(ResponseType.GetAgeCategoriesForParticipant).data(ageCategories).build();
        } catch (ContestManagementException e) {
            return new Response.Builder().type(ResponseType.Error).data(e.getMessage()).build();
        }
    }

    private Response handleAddRegistration(Request request) {
        RegistrationDTO regDTO = (RegistrationDTO) request.getData();
        Registration registration = DTOUtils.getFromDTO(regDTO);

        try {
            server.addRegistration(registration);
            return okResponse;
        } catch (ContestManagementException e) {
            return new Response.Builder().type(ResponseType.Error).data(e.getMessage()).build();
        }
    }
}
