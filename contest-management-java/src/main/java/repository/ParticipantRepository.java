package repository;

import model.Competition;
import model.Participant;

public interface ParticipantRepository extends CrudRepository<Long, Participant> {
    Iterable<Participant> findParticipantsByCompetition(Competition competition);

    Iterable<Long> findParticipantIdsByName(String firstName, String lastName);
}
