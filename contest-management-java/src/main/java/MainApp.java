import controller.LoginWindowController;
import javafx.application.Application;
import javafx.fxml.FXMLLoader;
import javafx.scene.Scene;
import javafx.scene.layout.Pane;
import javafx.stage.Stage;
import repository.*;
import repository.database.CompetitionDbRepository;
import repository.database.OrganiserDbRepository;
import repository.database.ParticipantDbRepository;
import repository.database.RegistrationDbRepository;
import service.ContestManagementService;

import java.io.FileReader;
import java.io.IOException;
import java.util.Properties;

public class MainApp extends Application {
    private static ContestManagementService service;

    public static void main(String[] args) {
        Properties props = new Properties();
        try {
            props.load(new FileReader("contest-mgmt-db.config"));
        } catch (IOException e) {
            System.out.println("Cannot find contest-mgmt-db.config" + e);
        }

        ParticipantRepository participantRepository = new ParticipantDbRepository(props);
        CompetitionRepository competitionRepository = new CompetitionDbRepository(props);
        OrganiserRepository organiserRepository = new OrganiserDbRepository(props);
        RegistrationRepository registrationRepository = new RegistrationDbRepository(props);

        service = new ContestManagementService(
                participantRepository,
                competitionRepository,
                organiserRepository,
                registrationRepository
        );

        launch(args);
    }

    @Override
    public void start(Stage primaryStage) throws Exception {
        FXMLLoader loginLoader = new FXMLLoader();
        loginLoader.setLocation(getClass().getResource("/views/loginWindow.fxml"));
        Pane loginLayout = loginLoader.load();
        ((LoginWindowController) loginLoader.getController()).setService(service);

        primaryStage.setScene(new Scene(loginLayout));
        primaryStage.setTitle("Login");
        primaryStage.show();
    }
}