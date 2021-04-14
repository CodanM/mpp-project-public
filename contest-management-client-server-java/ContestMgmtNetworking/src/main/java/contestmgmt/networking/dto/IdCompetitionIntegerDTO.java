package contestmgmt.networking.dto;

import java.io.Serializable;

public class IdCompetitionIntegerDTO implements Serializable {
    private Long competitionId;

    private String competitionType;

    private String ageCategory;

    private Integer count;

    public IdCompetitionIntegerDTO(Long competitionId, String competitionType, String ageCategory, Integer count) {
        this.competitionId = competitionId;
        this.competitionType = competitionType;
        this.ageCategory = ageCategory;
        this.count = count;
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

    public Integer getCount() {
        return count;
    }

    public void setCount(Integer count) {
        this.count = count;
    }
}
