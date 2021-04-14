package contestmgmt.server;

import contestmgmt.model.*;
import contestmgmt.persistence.repository.ICompetitionRepository;
import contestmgmt.persistence.repository.IOrganiserRepository;
import contestmgmt.persistence.repository.IParticipantRepository;
import contestmgmt.persistence.repository.IRegistrationRepository;
import contestmgmt.services.ContestManagementException;
import contestmgmt.services.IContestManagementObserver;
import contestmgmt.services.IContestManagementServices;

import java.util.List;
import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.stream.Collectors;
import java.util.stream.StreamSupport;

public class ContestManagementServicesImpl implements IContestManagementServices {
    private IParticipantRepository participantRepository;

    private ICompetitionRepository competitionRepository;

    private IOrganiserRepository organiserRepository;

    private IRegistrationRepository registrationRepository;

    private Map<String, IContestManagementObserver> loggedOrganisers = new ConcurrentHashMap<>();

    public ContestManagementServicesImpl(IParticipantRepository participantRepository,
                                         ICompetitionRepository competitionRepository,
                                         IOrganiserRepository organiserRepository,
                                         IRegistrationRepository registrationRepository) {
        this.participantRepository = participantRepository;
        this.competitionRepository = competitionRepository;
        this.organiserRepository = organiserRepository;
        this.registrationRepository = registrationRepository;
    }

    @Override
    public synchronized void login(Organiser organiser, IContestManagementObserver client)
            throws ContestManagementException {
        Organiser o = organiserRepository.find(organiser.getId());
        if (o == null || !o.getPassword().equals(organiser.getPassword()))
            throw new ContestManagementException("Incorrect username or password!");

        if (loggedOrganisers.get(o.getUsername()) != null)
            throw new ContestManagementException("Organiser already logged in!");

        loggedOrganisers.put(o.getUsername(), client);
    }

    @Override
    public synchronized void logout(Organiser organiser, IContestManagementObserver client) throws ContestManagementException {
        IContestManagementObserver cl = loggedOrganisers.remove(organiser.getId());
        if (cl == null)
            throw new ContestManagementException("User %s is not logged in".formatted(organiser.getUsername()));
    }

//    @Override
//    public synchronized Participant findParticipantById(Long participantId) {
//        return participantRepository.find(participantId);
//    }

    @Override
    public synchronized List<Competition> getCompetitionsByString(String competitionType, String ageCategory) {
        Iterable<Competition> competitionTypes = competitionRepository
                .findCompetitionsByString(competitionType, ageCategory);
        return StreamSupport.stream(competitionTypes.spliterator(), false)
                .collect(Collectors.toList());
    }

    @Override
    public synchronized List<Participant> getParticipantsByCompetition(Competition competition) {
        competition = competitionRepository.findByProps(competition.getCompetitionType(), competition.getAgeCategory());
        Iterable<Participant> participants = participantRepository.findParticipantsByCompetition(competition);
        return StreamSupport.stream(participants.spliterator(), false)
                .collect(Collectors.toList());
    }

    @Override
    public synchronized Map<Long, Tuple<Competition, Integer>> countParticipantsForEachCompetition(
            String competitionType, String ageCategory) {
        return competitionRepository.countParticipantsForEachCompetition(competitionType, ageCategory);
    }

    @Override
    public synchronized List<Long> getParticipantIdsByName(String firstName, String lastName) {
        Iterable<Long> participantIds = participantRepository.findParticipantIdsByName(firstName, lastName);
        return StreamSupport.stream(participantIds.spliterator(), false)
                .collect(Collectors.toList());
    }

    @Override
    public synchronized List<String> getAgeCategoriesForParticipant(Participant participant, String competitionType) {
        if (participant.getAge() == 0)
            participant = participantRepository.find(participant.getId());
        int age = participant.getAge();

        return StreamSupport.stream(competitionRepository.findAgeCategoriesFromCompetitionType(competitionType)
                .spliterator(), false)
                .filter(s -> {
                    String[] limits = s.split("-");
                    int min = Integer.parseInt(limits[0]);
                    int max = Integer.parseInt(limits[1]);
                    return age >= min && age <= max;
                })
                .collect(Collectors.toList());
    }

    @Override
    public synchronized void addRegistration(Registration registration)
            throws ContestManagementException {
        Participant participant = registration.getParticipant();
        Competition competition = registration.getCompetition();
        if (participant.getId() == null) {
            Long id = participantRepository.add(new Participant(participant.getFirstName(),
                    participant.getLastName(), participant.getAge()));
            participant.setId(id);
        }
        participant = participantRepository.find(participant.getId());

        competition = competitionRepository.findByProps(competition.getCompetitionType(), competition.getAgeCategory());

        String[] ageLimits = competition.getAgeCategory().split("-");
        int minAge = Integer.parseInt(ageLimits[0]);
        int maxAge = Integer.parseInt(ageLimits[1]);
        if (participant.getAge() < minAge || participant.getAge() > maxAge)
            throw new ContestManagementException("Participant age isn't in the allowed range for the competition!");


        Iterable<Registration> registrations = registrationRepository.findByParticipant(participant.getId());
        int rCount = 0;
        for (var r : registrations) {
            if (r.getId().getLeft().equals(participant.getId()) &&
                    r.getCompetition().getCompetitionType().equals(competition.getCompetitionType()) &&
                    r.getCompetition().getAgeCategory().equals(competition.getAgeCategory()))
                throw new ContestManagementException("Registration already exists!");
            rCount++;
        }
        if (rCount >= 2)
            throw new ContestManagementException("Participant already registered in 2 competitions!");

        Registration newReg = new Registration(participant, competition);
        registrationRepository.add(newReg);
        notifyNewRegistration(newReg);
    }

    private static final int defaultThreadsNo = 5;

    private void notifyNewRegistration(Registration registration) {
        ExecutorService executor = Executors.newFixedThreadPool(defaultThreadsNo);
        for (String username : loggedOrganisers.keySet()) {
            IContestManagementObserver client = loggedOrganisers.get(username);
            if (loggedOrganisers.get(username) != null) {
                executor.execute(() -> {
                    try {
                        System.out.printf("Notifying Organiser '%s' about new registration\n", username);
                        client.newRegistration(registration);
                    } catch (ContestManagementException e) {
                        System.err.println("Error notifying organiser" + e);
                    }
                });
            }
        }
        executor.shutdown();
    }
}
