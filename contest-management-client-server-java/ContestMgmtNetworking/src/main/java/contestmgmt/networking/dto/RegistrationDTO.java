package contestmgmt.networking.dto;

import java.io.Serializable;

public class RegistrationDTO implements Serializable {
    private Long participantId;
    private String firstName;
    private String lastName;
    private int age;

    private Long competitionId;
    private String competitionType;
    private String ageCategory;

    public Long getParticipantId() {
        return participantId;
    }

    public void setParticipantId(Long participantId) {
        this.participantId = participantId;
    }

    public String getFirstName() {
        return firstName;
    }

    public void setFirstName(String firstName) {
        this.firstName = firstName;
    }

    public String getLastName() {
        return lastName;
    }

    public void setLastName(String lastName) {
        this.lastName = lastName;
    }

    public int getAge() {
        return age;
    }

    public void setAge(int age) {
        this.age = age;
    }

    public Long getCompetitionId() {
        return competitionId;
    }

    public void setCompetitionId(Long competitionId) {
        this.competitionId = competitionId;
    }

    public String getCompetitionType() {
        return competitionType;
    }

    public void setCompetitionType(String competitionType) {
        this.competitionType = competitionType;
    }

    public String getAgeCategory() {
        return ageCategory;
    }

    public void setAgeCategory(String ageCategory) {
        this.ageCategory = ageCategory;
    }

    @Override
    public String toString() {
        return "RegistrationDTO { participantId: %s, firstName: %s, lastName: %s, age: %d, competitionId: %s, competitionType: %s, ageCategory: %s }"
                .formatted(participantId.toString(), firstName, lastName, age, competitionId.toString(), competitionType, ageCategory);
    }
}
