package contestmgmt.server;

import contestmgmt.networking.utils.AbstractServer;
import contestmgmt.networking.utils.ContestMgmtRpcConcurrentServer;
import contestmgmt.persistence.repository.ICompetitionRepository;
import contestmgmt.persistence.repository.IOrganiserRepository;
import contestmgmt.persistence.repository.IParticipantRepository;
import contestmgmt.persistence.repository.IRegistrationRepository;
import contestmgmt.persistence.repository.jdbc.CompetitionDbRepository;
import contestmgmt.persistence.repository.jdbc.OrganiserDbRepository;
import contestmgmt.persistence.repository.jdbc.ParticipantDbRepository;
import contestmgmt.persistence.repository.jdbc.RegistrationDbRepository;
import contestmgmt.services.IContestManagementServices;

import java.io.IOException;
import java.rmi.ServerException;
import java.util.Properties;

public class StartRpcServer {
    private static int defaultPort=55555;

    public static void main(String[] args) {
        Properties serverProps = new Properties();
        try {
            serverProps.load(StartRpcServer.class.getResourceAsStream("/contestmgmt-server.properties"));
            System.out.println("System properties set.");
            serverProps.list(System.out);
        } catch (IOException e) {
            System.err.println("Cannot find contestmgmt-server.properties");
            return;
        }

        IParticipantRepository participantRepository = new ParticipantDbRepository(serverProps);
        ICompetitionRepository competitionRepository = new CompetitionDbRepository(serverProps);
        IOrganiserRepository organiserRepository = new OrganiserDbRepository(serverProps);
        IRegistrationRepository registrationRepository = new RegistrationDbRepository(serverProps);

        IContestManagementServices service = new ContestManagementServicesImpl(participantRepository,
                competitionRepository, organiserRepository, registrationRepository);

        int port = defaultPort;
        try {
            port = Integer.parseInt(serverProps.getProperty("contestmgmt.server.port"));
        } catch (NumberFormatException e) {
            System.err.println("Wrong port number " + e.getMessage());
            System.err.println("Using default port " + defaultPort);
        }

        System.out.println("Starting server on port: " + port);
        AbstractServer server = new ContestMgmtRpcConcurrentServer(port, service);
        try {
            server.start();
        } catch (ServerException e) {
            System.err.println("Error starting the server " + e.getMessage());
        } finally {
            try {
                server.stop();
            } catch (ServerException e) {
                System.err.println("Error stopping the server " + e.getMessage());
            }
        }
    }
}
