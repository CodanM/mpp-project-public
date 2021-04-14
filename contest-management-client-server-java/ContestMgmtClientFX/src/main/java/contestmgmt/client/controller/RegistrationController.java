package contestmgmt.client.controller;

import contestmgmt.model.Competition;
import contestmgmt.model.Participant;
import contestmgmt.model.Registration;
import contestmgmt.services.ContestManagementException;
import contestmgmt.services.IContestManagementServices;
import javafx.application.Platform;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.scene.control.*;
import javafx.scene.input.KeyEvent;
import javafx.scene.input.MouseEvent;

import java.util.stream.Collectors;

public class RegistrationController {
    @FXML
    public CheckBox existingCheckBox;
    @FXML
    public TextField firstNameTextField;
    @FXML
    public TextField lastNameTextField;
    @FXML
    public ComboBox<Long> idComboBox;
    @FXML
    public Spinner<Integer> ageSpinner;
    @FXML
    public ComboBox<String> competitionTypesComboBox;
    @FXML
    public ComboBox<String> ageCategoriesComboBox;
    @FXML
    public Button addButton;
    @FXML
    public Label ageLabel;

    private IContestManagementServices server;

    private ObservableList<Long> participantIds = FXCollections.observableArrayList();

    private ObservableList<String> competitionTypes = FXCollections.observableArrayList();

    private ObservableList<String> ageCategories = FXCollections.observableArrayList();

    public void setServer(IContestManagementServices server) {
        this.server= server;
    }

    @FXML
    public void initialize() {
        idComboBox.setItems(participantIds);
        competitionTypesComboBox.setItems(competitionTypes);
        ageCategoriesComboBox.setItems(ageCategories);
    }

    void initModel() {
        Platform.runLater(() ->
        {
            try {
                competitionTypes.setAll(server.getCompetitionsByString("", "").stream()
                        .map(Competition::getCompetitionType)
                        .distinct()
                        .collect(Collectors.toList()));
            } catch (ContestManagementException e) {
                MessageAlert.showErrorMessage(null, e.getMessage());
            }
        });
    }

    public void handleExists(ActionEvent actionEvent) {
        idComboBox.setVisible(existingCheckBox.isSelected());
        ageSpinner.setVisible(!existingCheckBox.isSelected());
        ageLabel.setVisible(!existingCheckBox.isSelected());
        if (!existingCheckBox.isSelected())
            idComboBox.setValue(null);
        else
            ageSpinner.getValueFactory().setValue(0);
    }

    public void handleSearchIds(MouseEvent mouseEvent) {
        String firstName = firstNameTextField.getText();
        String lastName = lastNameTextField.getText();
        //Platform.runLater(() -> {
            try {
                participantIds.setAll(server.getParticipantIdsByName(firstName, lastName));
            } catch (ContestManagementException e) {
                MessageAlert.showErrorMessage(null, e.getMessage());
            }
        //});
    }

//    public void handleIdSelected(ActionEvent actionEvent) {
//        Long participantId = idComboBox.getValue();
//        String firstName = firstNameTextField.getText();
//
//        ageSpinner.getValueFactory().setValue(server.getParticipantAge(participantId));
//    }

    public void handleCompetitionTypeSelected(ActionEvent actionEvent) {
        Long id = idComboBox.getValue();
        String competitionType = competitionTypesComboBox.getValue();
        String firstName = firstNameTextField.getText();
        String lastName = lastNameTextField.getText();
        int age = 0;
        if (!existingCheckBox.isSelected())
            age = ageSpinner.getValue();
        Participant p = new Participant(firstName, lastName, age);
        p.setId(id);

        Platform.runLater(() -> {
            try {
                ageCategories.setAll(server.getAgeCategoriesForParticipant(p, competitionType));
            } catch (ContestManagementException e) {
                MessageAlert.showErrorMessage(null, e.getMessage());
            }
        });
    }

    public void handleAdd(ActionEvent actionEvent) {
        Long participantId = idComboBox.getValue();
        String firstName = firstNameTextField.getText();
        String lastName = lastNameTextField.getText();
        int age = 0;
        if (!existingCheckBox.isSelected())
            age = ageSpinner.getValue();
        Participant p = new Participant(firstName, lastName, age);
        p.setId(participantId);

        String competitionType = competitionTypesComboBox.getValue();
        String ageCategory = ageCategoriesComboBox.getValue();
        Competition c = new Competition(competitionType, ageCategory);

        try {
            server.addRegistration(new Registration(p, c));
            MessageAlert.showMessage(null, Alert.AlertType.INFORMATION, "Success", "Registration added successfully!");
        } catch (ContestManagementException e) {
            MessageAlert.showErrorMessage(null, e.getMessage());
        }
    }

    public void nameChanged(KeyEvent keyEvent) {
        competitionTypesComboBox.setValue(null);
        ageCategoriesComboBox.setValue(null);
    }
}
