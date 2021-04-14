package contestmgmt.client;

import contestmgmt.client.controller.LoginController;
import contestmgmt.client.controller.MainController;
import contestmgmt.client.controller.RegistrationController;
import contestmgmt.networking.rpcprotocol.ContestMgmtServicesRpcProxy;
import contestmgmt.services.IContestManagementServices;
import javafx.application.Application;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.stage.Stage;

import java.io.IOException;
import java.util.Properties;

public class StartRpcClientFX extends Application {
    private String primaryStage;

    private static int defaultPort = 55555;

    private static String defaultServer = "localhost";

    public void start(Stage primaryStage) throws Exception {
        System.out.println("In start");

        Properties clientProps = new Properties();
        try {
            clientProps.load(StartRpcClientFX.class.getResourceAsStream("/contestmgmt-client.properties"));
            System.out.println("Client properties set.");
        } catch (IOException e) {
            System.err.println("Could not find contestmgmt-client.propeties " + e);
        }

        String serverIP = clientProps.getProperty("contestmgmt.server.host", defaultServer);
        int serverPort = defaultPort;
        try {
            serverPort = Integer.parseInt(clientProps.getProperty("contestmgmt.server.port"));
        } catch (NumberFormatException e) {
            System.err.println("Wrong port number " + e.getMessage());
            System.err.println("Using default port: " + defaultPort);
        }
        System.out.println("Using server IP: " + serverIP);
        System.out.println("Using server port: " + serverPort);

        IContestManagementServices server = new ContestMgmtServicesRpcProxy(serverIP, serverPort);

        FXMLLoader loginLoader = new FXMLLoader(getClass().getClassLoader().getResource("views/loginWindow.fxml"));
        Parent loginRoot = loginLoader.load();

        LoginController loginController = loginLoader.getController();
        loginController.setServer(server);

        FXMLLoader mainLoader = new FXMLLoader(getClass().getClassLoader().getResource("views/mainWindow.fxml"));
        Parent mainRoot = mainLoader.load();

        MainController mainController = mainLoader.getController();
        mainController.setServer(server);

        loginController.setMainWindowController(mainController);
        loginController.setMainParent(mainRoot);

        primaryStage.setTitle("Contest Management");
        primaryStage.setScene(new Scene(loginRoot));
        primaryStage.show();
    }
}
