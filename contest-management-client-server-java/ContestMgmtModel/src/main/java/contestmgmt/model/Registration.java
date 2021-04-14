package contestmgmt.model;

public class Registration extends Entity<Tuple<Long, Long>> {
    private Participant participant;
    private Competition competition;

    public Registration(Participant participant, Competition competition) {
        setId(new Tuple<>(participant.getId(), competition.getId()));
        this.participant = participant;
        this.competition = competition;
    }

    public Participant getParticipant() {
        return participant;
    }

    public void setParticipant(Participant participant) {
        this.participant = participant;
    }

    public Competition getCompetition() {
        return competition;
    }

    public void setCompetition(Competition competition) {
        this.competition = competition;
    }
}
