package contestmgmt.networking.dto;

import contestmgmt.model.Competition;
import contestmgmt.model.Organiser;
import contestmgmt.model.Participant;
import contestmgmt.model.Registration;

public class DTOUtils {
    public static StringStringDTO getDTO(String first, String second) {
        return new StringStringDTO(first, second);
    }

    public static Participant getFromDTO(ParticipantDTO participantDTO) {
        Long id = participantDTO.getId();
        String firstName = participantDTO.getFirstName();
        String lastName = participantDTO.getLastName();
        int age = participantDTO.getAge();
        Participant p = new Participant(firstName, lastName, age);
        p.setId(id);
        return p;
    }

    public static ParticipantDTO getDTO(Participant participant) {
        Long id = participant.getId();
        String firstName = participant.getFirstName();
        String lastName = participant.getLastName();
        int age = participant.getAge();
        return new ParticipantDTO(id, firstName, lastName, age);
    }

    public static Competition getFromDTO(CompetitionDTO competitionDTO) {
        Long id = competitionDTO.getId();
        String competitionType = competitionDTO.getCompetitionType();
        String ageCategory = competitionDTO.getAgeCategory();
        Competition c = new Competition(competitionType, ageCategory);
        c.setId(id);
        return c;
    }

    public static CompetitionDTO getDTO(Competition competition) {
        Long id = competition.getId();
        String competitionType = competition.getCompetitionType();
        String ageCategory = competition.getAgeCategory();
        return new CompetitionDTO(id, competitionType, ageCategory);
    }

    public static CompetitionCountDTO getDTO(Competition competition, Integer count) {
        Long id = competition.getId();
        String competitionType = competition.getCompetitionType();
        String ageCategory = competition.getAgeCategory();
        return new CompetitionCountDTO(id, competitionType, ageCategory, count);
    }

    public static Organiser getFromDTO(OrganiserDTO organiserDTO) {
        String username = organiserDTO.getUsername();
        String password = organiserDTO.getPassword();
        return new Organiser(username, password, null, null);
    }

    public static OrganiserDTO getDTO(Organiser organiser) {
        String username = organiser.getUsername();
        String password = organiser.getPassword();
        return new OrganiserDTO(username, password);
    }

    public static RegistrationDTO getDTO(Registration registration) {
        Long participantId = registration.getParticipant().getId();
        String firstName = registration.getParticipant().getFirstName();
        String lastName = registration.getParticipant().getLastName();
        int age = registration.getParticipant().getAge();

        Long competitionId = registration.getCompetition().getId();
        String competitionType = registration.getCompetition().getCompetitionType();
        String ageCategory = registration.getCompetition().getAgeCategory();
        RegistrationDTO regDTO = new RegistrationDTO();

        regDTO.setParticipantId(participantId);
        regDTO.setFirstName(firstName);
        regDTO.setLastName(lastName);
        regDTO.setAge(age);

        regDTO.setCompetitionId(competitionId);
        regDTO.setCompetitionType(competitionType);
        regDTO.setAgeCategory(ageCategory);

        return regDTO;
    }

    public static Registration getFromDTO(RegistrationDTO regDTO) {
        Long participantId = regDTO.getParticipantId();
        String firstName = regDTO.getFirstName();
        String lastName = regDTO.getLastName();
        int age = regDTO.getAge();

        Long competitionId = regDTO.getCompetitionId();
        String competitionType = regDTO.getCompetitionType();
        String ageCategory = regDTO.getAgeCategory();

        Participant p = new Participant(firstName, lastName, age);
        p.setId(participantId);
        Competition c = new Competition(competitionType, ageCategory);
        c.setId(competitionId);
        return new Registration(p, c);
    }

    public static ParticipantStringDTO getDTO(Participant participant, String string) {
        return new ParticipantStringDTO(getDTO(participant), string);
    }

    public static IdCompetitionIntegerDTO getDTO(Long id, Competition competition, Integer count) {
        String competitionType = competition.getCompetitionType();
        String ageCategory = competition.getAgeCategory();
        return new IdCompetitionIntegerDTO(id, competitionType, ageCategory, count);
    }
}
