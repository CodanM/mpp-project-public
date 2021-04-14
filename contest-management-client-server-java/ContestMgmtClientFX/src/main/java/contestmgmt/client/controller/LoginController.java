package contestmgmt.client.controller;

import contestmgmt.model.Organiser;
import contestmgmt.services.ContestManagementException;
import contestmgmt.services.IContestManagementServices;
import javafx.application.Platform;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.scene.control.Button;
import javafx.scene.control.PasswordField;
import javafx.scene.control.TextField;
import javafx.scene.layout.Pane;
import javafx.stage.Stage;

public class LoginController {
    @FXML
    public TextField usernameTextField;
    @FXML
    public PasswordField passwordField;
    @FXML
    public Button loginButton;

    private IContestManagementServices server;

    private MainController mainController;

    private Parent mainParent;

    public void setServer(IContestManagementServices server) {
        this.server = server;
    }

    public void setMainWindowController(MainController mainController) {
        this.mainController = mainController;
    }

    public void setMainParent(Parent mainParent) {
        this.mainParent = mainParent;
    }

    @FXML
    public void handleLogin(ActionEvent actionEvent) throws Exception {
        if (usernameTextField.getText().equals("") || passwordField.getText().equals(""))
            MessageAlert.showErrorMessage(null, "Username and password cannot be null!");
        Organiser o = new Organiser(usernameTextField.getText(), passwordField.getText(), null, null);
        try {
            server.login(o, mainController);
            openMainWindow(o);
        } catch (ContestManagementException e) {
            MessageAlert.showErrorMessage(null, e.getMessage());
        }
    }

    private void openMainWindow(Organiser o) {
        String username = usernameTextField.getText();

        Stage mainStage = new Stage();
        mainStage.setTitle("Organiser %s".formatted(username));
        mainStage.setScene(new Scene(mainParent));

        mainStage.setOnShowing(event -> mainController.initModel());

        mainStage.setOnCloseRequest(event -> {
            mainController.logout();
            System.exit(0);
        });

        mainStage.show();
        mainController.setOrganiser(o);
        loginButton.getScene().getWindow().hide();
    }
}
