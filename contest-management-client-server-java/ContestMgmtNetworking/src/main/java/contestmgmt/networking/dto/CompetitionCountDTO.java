package contestmgmt.networking.dto;

import java.io.Serializable;

public class CompetitionCountDTO implements Serializable {
    private Long id;

    private String competitionType;

    private String ageCategory;

    private int count;

    public CompetitionCountDTO(Long id, String competitionType, String ageCategory, int count) {
        this.id = id;
        this.competitionType = competitionType;
        this.ageCategory = ageCategory;
        this.count = count;
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

    public int getCount() {
        return count;
    }

    public void setCount(int count) {
        this.count = count;
    }

    @Override
    public String toString() {
        return "CompetitionDTO { id: %s, competitionType: %s, ageCategory: %s, count: %d }".formatted(id.toString(), competitionType, ageCategory, count);
    }
}
