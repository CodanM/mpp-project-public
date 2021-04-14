package contestmgmt.client.controller;

import javafx.scene.control.Alert;
import javafx.stage.Stage;

public class MessageAlert {
    public static void showMessage(Stage owner, Alert.AlertType type, String headerText, String contentText) {
        Alert alert = new Alert(type);
        alert.setHeaderText(headerText);
        alert.setContentText(contentText);
        alert.initOwner(owner);
        alert.showAndWait();
    }

    public static void showErrorMessage(Stage owner, String contentText) {
        Alert alert = new Alert(Alert.AlertType.ERROR);
        alert.initOwner(owner);
        alert.setHeaderText("Error!");
        alert.setContentText(contentText);
        alert.showAndWait();
    }
}
