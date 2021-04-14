package service;

import model.*;
import repository.CompetitionRepository;
import repository.OrganiserRepository;
import repository.ParticipantRepository;
import repository.RegistrationRepository;

import java.util.Arrays;
import java.util.Map;
import java.util.Optional;
import java.util.stream.Collectors;
import java.util.stream.StreamSupport;

public class ContestManagementService {
    private ParticipantRepository participantRepository;

    private CompetitionRepository competitionRepository;

    private OrganiserRepository organiserRepository;

    private RegistrationRepository registrationRepository;

    private Organiser currentOrganiser = null;

    public ContestManagementService(ParticipantRepository participantRepository, CompetitionRepository competitionRepository, OrganiserRepository organiserRepository, RegistrationRepository registrationRepository) {
        this.participantRepository = participantRepository;
        this.competitionRepository = competitionRepository;
        this.organiserRepository = organiserRepository;
        this.registrationRepository = registrationRepository;
    }

    public Organiser getCurrentOrganiser() {
        return currentOrganiser;
    }

    public void login(String username, String password) {
        Organiser o = organiserRepository.find(username);
        if (o == null || !o.getPassword().equals(password))
            throw new ServiceException("Incorrect username or password!");
        currentOrganiser = o;
    }

    public Iterable<String> getCompetitionTypesByString(String str) {
        return competitionRepository.findTypesByString(str);
    }

    public Iterable<String> getAgeCategoriesByCompetitionType(String competitionType) {
        return competitionRepository.findAgeCategories(competitionType);
    }

    public Iterable<Participant> getParticipantsByCompetition(String competitionType, String ageCategory) {
        Competition comp = competitionRepository.findByProps(competitionType, ageCategory);
        return participantRepository.findParticipantsByCompetition(comp);
    }

    public int countParticipantsForCompetition(String competitionType, String ageCategory) {
        Competition comp = competitionRepository.findByProps(competitionType, ageCategory);
        return competitionRepository.countParticipantsForCompetition(comp.getId());
    }

    public Iterable<Long> findParticipantIdsByName(String firstName, String lastName) {
        return participantRepository.findParticipantIdsByName(firstName, lastName);
    }

    public int getParticipantAge(Long participantId) {
        return participantRepository.find(participantId).getAge();
    }

    public Iterable<String> getAgeCategoriesForParticipant(int age, String competitionType) {
        return StreamSupport.stream(getAgeCategoriesByCompetitionType(competitionType).spliterator(), false)
                .filter(s -> {
                    String[] limits = s.split("-");
                    int min = Integer.parseInt(limits[0]);
                    int max = Integer.parseInt(limits[1]);
                    return age >= min && age <= max;
                })
                .collect(Collectors.toList());
    }

    public void addRegistration(Long participantId, String firstName, String lastName, int age,
                                String competitionType, String ageCategory) {
        if (participantId == null)
            participantId = participantRepository.add(new Participant(firstName, lastName, age));

        Participant p = participantRepository.find(participantId);
        Competition c = competitionRepository.findByProps(competitionType, ageCategory);

        String[] ageLimits = c.getAgeCategory().split("-");
        int minAge = Integer.parseInt(ageLimits[0]);
        int maxAge = Integer.parseInt(ageLimits[1]);
        if (age < minAge || age > maxAge)
            throw new ServiceException("Participant age isn't in the allowed range for the competition!");


        Iterable<Registration> registrations = registrationRepository.findByParticipant(participantId);
        int rCount = 0;
        for (var r : registrations) {
            if (r.getId().getLeft().equals(participantId) &&
                    r.getCompetition().getCompetitionType().equals(competitionType) &&
                    r.getCompetition().getAgeCategory().equals(ageCategory))
                throw new ServiceException("Registration already exists!");
            rCount++;
        }
        if (rCount >= 2)
            throw new ServiceException("Participant already registered in 2 competitions!");

        registrationRepository.add(new Registration(p, c));
    }
}
