package controller;

import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.FXMLLoader;
import javafx.scene.Scene;
import javafx.scene.control.Button;
import javafx.scene.control.PasswordField;
import javafx.scene.control.TextField;
import javafx.scene.layout.Pane;
import javafx.stage.Stage;
import model.Organiser;
import service.ContestManagementService;
import service.ServiceException;

public class LoginWindowController {
    @FXML
    public TextField usernameTextField;
    @FXML
    public PasswordField passwordField;
    @FXML
    public Button loginButton;

    private ContestManagementService service;

    public void setService(ContestManagementService service) {
        this.service = service;
    }

    @FXML
    public void handleLogin(ActionEvent actionEvent) throws Exception {
        if (usernameTextField.getText().equals("") || passwordField.getText().equals(""))
            MessageAlert.showErrorMessage(null, "Username and password cannot be null!");
        try {
            service.login(usernameTextField.getText(), passwordField.getText());
            openMainWindow();
        } catch (ServiceException e) {
            MessageAlert.showErrorMessage(null, e.getMessage());
        }
    }

    private void openMainWindow() throws Exception {
        FXMLLoader mainLoader = new FXMLLoader();
        mainLoader.setLocation(getClass().getResource("/views/mainWindow.fxml"));
        Pane mainLayout = mainLoader.load();
        ((MainWindowController) mainLoader.getController()).setService(service);

        Organiser o = service.getCurrentOrganiser();

        Stage mainStage = new Stage();
        mainStage.setScene(new Scene(mainLayout));
        mainStage.setTitle("%s %s".formatted(o.getFirstName(), o.getLastName()));
        mainStage.show();

        ((Stage) loginButton.getScene().getWindow()).close();
    }
}
