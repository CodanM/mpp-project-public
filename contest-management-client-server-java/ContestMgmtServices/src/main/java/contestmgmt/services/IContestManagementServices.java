package contestmgmt.services;

import contestmgmt.model.*;

import java.util.List;
import java.util.Map;

public interface IContestManagementServices {
    void login(Organiser organiser, IContestManagementObserver client) throws ContestManagementException;

    void logout(Organiser organiser, IContestManagementObserver client) throws ContestManagementException;

//    Participant findParticipantById(Long participantId);

    List<Competition> getCompetitionsByString(String competitionType, String ageCategory) throws ContestManagementException;

    List<Participant> getParticipantsByCompetition(Competition competition) throws ContestManagementException;

    Map<Long, Tuple<Competition, Integer>> countParticipantsForEachCompetition(
            String competitionType, String ageCategory) throws ContestManagementException;

    List<Long> getParticipantIdsByName(String firstName, String lastName) throws ContestManagementException;

    List<String> getAgeCategoriesForParticipant(Participant participant, String competitionType) throws ContestManagementException;

    void addRegistration(Registration registration) throws ContestManagementException;
}