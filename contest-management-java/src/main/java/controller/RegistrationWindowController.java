package controller;

import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.scene.control.*;
import javafx.scene.input.KeyEvent;
import javafx.scene.input.MouseEvent;
import service.ContestManagementService;
import service.ServiceException;

import java.util.stream.Collectors;
import java.util.stream.StreamSupport;

public class RegistrationWindowController {
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

    private ContestManagementService service;

    private ObservableList<Long> participantIds = FXCollections.observableArrayList();

    private ObservableList<String> competitionTypes = FXCollections.observableArrayList();

    private ObservableList<String> ageCategories = FXCollections.observableArrayList();

    public void setService(ContestManagementService service) {
        this.service = service;
        initModel();
    }

    @FXML
    public void initialize() {
        idComboBox.setItems(participantIds);
        competitionTypesComboBox.setItems(competitionTypes);
        ageCategoriesComboBox.setItems(ageCategories);
    }

    private void initModel() {
        competitionTypes.setAll(StreamSupport.stream(
                service.getCompetitionTypesByString("").spliterator(), false)
                        .collect(Collectors.toList()));
    }

    public void handleExists(ActionEvent actionEvent) {
        idComboBox.setVisible(existingCheckBox.isSelected());
        ageSpinner.setDisable(existingCheckBox.isSelected());
        if (!existingCheckBox.isSelected())
            idComboBox.setValue(null);
    }

    public void handleSearchIds(MouseEvent mouseEvent) {
        String firstName = firstNameTextField.getText();
        String lastName = lastNameTextField.getText();
        participantIds.setAll(StreamSupport.stream(
                service.findParticipantIdsByName(firstName, lastName).spliterator(), false)
                        .collect(Collectors.toList()));
    }

    public void handleIdSelected(ActionEvent actionEvent) {
        Long participantId = idComboBox.getValue();
        ageSpinner.getValueFactory().setValue(service.getParticipantAge(participantId));
    }

    public void handleCompetitionTypeSelected(ActionEvent actionEvent) {
        String competitionType = competitionTypesComboBox.getValue();
        int age = ageSpinner.getValue();
        ageCategories.setAll(StreamSupport.stream(
                service.getAgeCategoriesForParticipant(age, competitionType).spliterator(), false)
                        .collect(Collectors.toList()));
    }

    public void handleAdd(ActionEvent actionEvent) {
        Long participantId = idComboBox.getValue();
        String firstName = firstNameTextField.getText();
        String lastName = lastNameTextField.getText();
        int age = ageSpinner.getValue();
        String competitionType = competitionTypesComboBox.getValue();
        String ageCategory = ageCategoriesComboBox.getValue();
        try {
            service.addRegistration(participantId, firstName, lastName, age, competitionType, ageCategory);
            MessageAlert.showMessage(null, Alert.AlertType.INFORMATION, "Success", "Registration added successfully!");
        } catch (ServiceException e) {
            MessageAlert.showErrorMessage(null, e.getMessage());
        }
    }
}
