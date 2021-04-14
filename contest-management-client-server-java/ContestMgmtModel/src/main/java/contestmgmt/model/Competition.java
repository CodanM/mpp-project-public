package contestmgmt.model;

public class Competition extends Entity<Long> {
    private String competitionType;
    private String ageCategory;

    public Competition(String competitionType, String ageCategory) {
        this.competitionType = competitionType;
        this.ageCategory = ageCategory;
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
}
