package controller;

import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.FXMLLoader;
import javafx.scene.Scene;
import javafx.scene.control.*;
import javafx.scene.control.cell.PropertyValueFactory;
import javafx.scene.input.KeyEvent;
import javafx.scene.input.MouseEvent;
import javafx.scene.layout.Pane;
import javafx.stage.Stage;
import model.Organiser;
import model.Participant;
import service.ContestManagementService;

import java.util.stream.Collectors;
import java.util.stream.StreamSupport;

public class MainWindowController {
    @FXML
    public TextField competitionTypeTextField;
    @FXML
    public ListView<String> competitionTypesListView;
    @FXML
    public ListView<String> ageCategoriesListView;
    @FXML
    public Button searchButton;
    @FXML
    public TableView<Participant> participantsTableView;
    @FXML
    public TableColumn<Participant, String> idColumn;
    @FXML
    public TableColumn<Participant, String> firstNameColumn;
    @FXML
    public TableColumn<Participant, String> lastNameColumn;
    @FXML
    public TableColumn<Participant, String> ageColumn;
    @FXML
    public Button newRegistrationButton;
    @FXML
    public Label registeredCountLabel;
    @FXML
    public Button logoutButton;

    private ContestManagementService service;

    private ObservableList<String> competitionTypes = FXCollections.observableArrayList();

    private ObservableList<String> ageCategories = FXCollections.observableArrayList();

    private ObservableList<Participant> participants = FXCollections.observableArrayList();

    public void setService(ContestManagementService service) {
        this.service = service;
        initModel();
    }

    @FXML
    public void initialize() {
        idColumn.setCellValueFactory(new PropertyValueFactory<>("id"));
        firstNameColumn.setCellValueFactory(new PropertyValueFactory<>("firstName"));
        lastNameColumn.setCellValueFactory(new PropertyValueFactory<>("lastName"));
        ageColumn.setCellValueFactory(new PropertyValueFactory<>("age"));
        competitionTypesListView.setItems(competitionTypes);
        ageCategoriesListView.setItems(ageCategories);
        participantsTableView.setItems(participants);
    }

    public void initModel() {
        competitionTypes.setAll(
                StreamSupport.stream(service.getCompetitionTypesByString("").spliterator(), false)
                        .collect(Collectors.toList()));
    }

    @FXML
    public void handleCompetitionTypeSelected(MouseEvent mouseEvent) {
        String competitionType = competitionTypesListView.getSelectionModel().getSelectedItem();
        registeredCountLabel.setText("");
        ageCategories.setAll(StreamSupport.stream(
                service.getAgeCategoriesByCompetitionType(competitionType).spliterator(), false)
                        .collect(Collectors.toList()));
    }

    @FXML
    public void handleAgeCategorySelected(MouseEvent mouseEvent) {
        String competitionType = competitionTypesListView.getSelectionModel().getSelectedItem();
        String ageCategory = ageCategoriesListView.getSelectionModel().getSelectedItem();
        registeredCountLabel.setText("%d registered".formatted(
                service.countParticipantsForCompetition(competitionType, ageCategory)));
    }

    @FXML
    public void handleSearchParticipants(ActionEvent actionEvent) {
        String competitionType = competitionTypesListView.getSelectionModel().getSelectedItem();
        String ageCategory = ageCategoriesListView.getSelectionModel().getSelectedItem();
        participants.setAll(StreamSupport.stream(
                service.getParticipantsByCompetition(competitionType, ageCategory).spliterator(),false)
                        .collect(Collectors.toList()));
    }

    @FXML
    public void searchCompetitionTypes(KeyEvent actionEvent) {
        competitionTypes.setAll(
                StreamSupport.stream(service.getCompetitionTypesByString(competitionTypeTextField.getText()).spliterator(), false)
                        .collect(Collectors.toList()));
    }

    public void handleLogout(ActionEvent actionEvent) throws Exception {
        openLoginWindow();
    }

    private void openLoginWindow() throws Exception {
        FXMLLoader loginLoader = new FXMLLoader();
        loginLoader.setLocation(getClass().getResource("/views/loginWindow.fxml"));
        Pane loginLayout = loginLoader.load();
        ((LoginWindowController) loginLoader.getController()).setService(service);

        Stage loginStage = new Stage();
        loginStage.setScene(new Scene(loginLayout));
        loginStage.setTitle("Login");
        loginStage.show();

        ((Stage) logoutButton.getScene().getWindow()).close();
    }

    public void handleNewRegistration(ActionEvent actionEvent) throws Exception {
        openRegistrationWindow();
    }

    private void openRegistrationWindow() throws Exception {
        FXMLLoader registrationLoader = new FXMLLoader();
        registrationLoader.setLocation(getClass().getResource("/views/registrationWindow.fxml"));
        Pane registrationLayout = registrationLoader.load();
        ((RegistrationWindowController) registrationLoader.getController()).setService(service);

        Stage registrationStage = new Stage();
        registrationStage.setScene(new Scene(registrationLayout));
        registrationStage.setTitle("New Registration");
        registrationStage.show();
    }
}
