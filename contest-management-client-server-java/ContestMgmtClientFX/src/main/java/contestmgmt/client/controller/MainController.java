package contestmgmt.client.controller;

import contestmgmt.model.*;
import contestmgmt.networking.dto.CompetitionCountDTO;
import contestmgmt.networking.dto.DTOUtils;
import contestmgmt.services.ContestManagementException;
import contestmgmt.services.IContestManagementObserver;
import contestmgmt.services.IContestManagementServices;
import javafx.application.Platform;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.concurrent.Task;
import javafx.concurrent.WorkerStateEvent;
import javafx.event.ActionEvent;
import javafx.event.EventHandler;
import javafx.fxml.FXML;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.scene.control.*;
import javafx.scene.control.cell.PropertyValueFactory;
import javafx.scene.input.KeyEvent;
import javafx.scene.input.MouseEvent;
import javafx.scene.layout.Pane;
import javafx.stage.Stage;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.Optional;
import java.util.stream.Collectors;

public class MainController implements IContestManagementObserver {
    @FXML
    public TextField competitionTypeTextField;
    @FXML
    public TextField ageCategoryTextField;
    @FXML
    public TableView<CompetitionCountDTO> competitionsTableView;
    @FXML
    public TableColumn<CompetitionCountDTO, Long> competitionIdColumn;
    @FXML
    public TableColumn<CompetitionCountDTO, String> competitionTypeColumn;
    @FXML
    public TableColumn<CompetitionCountDTO, String> ageCategoryColumn;
    @FXML
    public TableColumn<CompetitionCountDTO, Integer> countColumn;
    @FXML
    public Button searchButton;
    @FXML
    public TableView<Participant> participantsTableView;
    @FXML
    public TableColumn<Participant, Long> participantIdColumn;
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
    @FXML
    public Label searchingLabel;

    private IContestManagementServices server;

    private Parent registrationParent;

    private RegistrationController registrationController;

    private Organiser organiser;

    private ObservableList<CompetitionCountDTO> competitions = FXCollections.observableArrayList();

    private ObservableList<Participant> participants = FXCollections.observableArrayList();

    public void setServer(IContestManagementServices server) {
        this.server = server;
    }

    public void setRegistrationParent(Parent registrationParent) {
        this.registrationParent = registrationParent;
    }

    public void setRegistrationController(RegistrationController registrationController) {
        this.registrationController = registrationController;
    }

    public void setOrganiser(Organiser organiser) {
        this.organiser = organiser;
    }

    @FXML
    public void initialize() {
        participantIdColumn.setCellValueFactory(new PropertyValueFactory<>("id"));
        firstNameColumn.setCellValueFactory(new PropertyValueFactory<>("firstName"));
        lastNameColumn.setCellValueFactory(new PropertyValueFactory<>("lastName"));
        ageColumn.setCellValueFactory(new PropertyValueFactory<>("age"));

        participantsTableView.setItems(participants);

        competitionIdColumn.setCellValueFactory(new PropertyValueFactory<>("id"));
        competitionTypeColumn.setCellValueFactory(new PropertyValueFactory<>("competitionType"));
        ageCategoryColumn.setCellValueFactory(new PropertyValueFactory<>("ageCategory"));
        countColumn.setCellValueFactory(new PropertyValueFactory<>("count"));

        competitionsTableView.setItems(competitions);
    }

    private volatile String searchTypeStr = null;

    private volatile String ageCategoryStr = null;

    private Thread initModelWorker;

    public void initModel() {
        searchingLabel.setText("Searching...");
        searchTypeStr = competitionTypeTextField.getText();
        ageCategoryStr = ageCategoryTextField.getText();
        if (initModelWorker == null || !initModelWorker.isAlive()) {
            Task<Map<Long, Tuple<Competition, Integer>>> searchTask = new Task<>() {
                    @Override
                    protected Map<Long, Tuple<Competition, Integer>> call() throws Exception {
                        Map<Long, Tuple<Competition, Integer>> comps = null;
                        while (searchTypeStr != null && ageCategoryStr != null) {
                            String compType = searchTypeStr;
                            String ageCat = ageCategoryStr;
                            searchTypeStr = null;
                            ageCategoryStr = null;
                            try {
                                comps = server.countParticipantsForEachCompetition(compType, ageCat);
                            } catch (ContestManagementException e) {
                                MessageAlert.showErrorMessage(null, e.getMessage());
                            }
                        }
                        return comps;
                    }
                };
            searchTask.addEventHandler(WorkerStateEvent.WORKER_STATE_SUCCEEDED, event -> {
                var res = searchTask.getValue();
                competitions.setAll(res.values().stream()
                        .map(value -> DTOUtils.getDTO(value.getLeft(), value.getRight()))
                        .collect(Collectors.toList()));
                searchingLabel.setText("");
            });
            initModelWorker = new Thread(searchTask);
            initModelWorker.start();
        }
    }

    @FXML
    public void handleSearchParticipants(ActionEvent actionEvent) {
        CompetitionCountDTO ccDTO = competitionsTableView.getSelectionModel().getSelectedItem();
        Competition c = new Competition(ccDTO.getCompetitionType(), ccDTO.getAgeCategory());
        c.setId(ccDTO.getId());
        Platform.runLater(() -> {
            try {
                participants.setAll(server.getParticipantsByCompetition(c));
            } catch (ContestManagementException e) {
                MessageAlert.showErrorMessage(null, e.getMessage());
            }
        });

    }

    @FXML
    public void searchCompetitionTypes(KeyEvent actionEvent) {
        ageCategoryTextField.setText("");
        searchCompetitions();
    }

    @FXML
    public void searchAgeCategories(KeyEvent keyEvent) {
        searchCompetitions();
    }

    private void searchCompetitions() {
        searchingLabel.setText("Searching...");
        searchTypeStr = competitionTypeTextField.getText();
        ageCategoryStr = ageCategoryTextField.getText();
        initModel();
    }

    void logout() {
        try {
            server.logout(organiser, this);
        } catch (ContestManagementException e) {
            MessageAlert.showErrorMessage(null, e.getMessage());
        }
    }

    @FXML
    public void handleNewRegistration(ActionEvent actionEvent) throws Exception {
        openRegistrationWindow();
    }

    private void openRegistrationWindow() throws Exception {
        Stage registrationStage = new Stage();
        registrationStage.setTitle("New Registration");

        FXMLLoader registrationLoader = new FXMLLoader(getClass().getClassLoader().getResource("views/registrationWindow.fxml"));
        Parent registrationRoot = registrationLoader.load();
        RegistrationController registrationController = registrationLoader.getController();
        registrationController.setServer(server);
        setRegistrationParent(registrationRoot);
        setRegistrationController(registrationController);

        registrationStage.setScene(new Scene(registrationParent));

        registrationStage.setOnShowing(event -> registrationController.initModel());

        registrationStage.show();
    }

    @Override
    public void newRegistration(Registration r) {
        Optional<CompetitionCountDTO> ccDTO = competitions.stream()
                .filter(cc -> cc.getId().equals(r.getCompetition().getId()))
                .findFirst();
        ccDTO.ifPresent(c -> c.setCount(c.getCount() + 1));
        Platform.runLater(() -> competitionsTableView.refresh());
    }
}
