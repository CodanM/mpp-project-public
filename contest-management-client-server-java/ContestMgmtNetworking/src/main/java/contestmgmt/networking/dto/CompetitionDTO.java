package contestmgmt.networking.dto;

import java.io.Serializable;

public class CompetitionDTO implements Serializable {
    private Long id;

    private String competitionType;

    private String ageCategory;

    public CompetitionDTO(Long id, String competitionType, String ageCategory) {
        this.id = id;
        this.competitionType = competitionType;
        this.ageCategory = ageCategory;
    }

    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
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
        return "CompetitionDTO { id: %s, competitionType: %s, ageCategory: %s }".formatted(id.toString(), competitionType, ageCategory);
    }
}
