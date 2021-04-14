package contestmgmt.persistence.repository;

import contestmgmt.model.Competition;
import contestmgmt.model.Participant;

public interface IParticipantRepository extends ICrudRepository<Long, Participant> {
    Iterable<Participant> findParticipantsByCompetition(Competition competition);

    Iterable<Long> findParticipantIdsByName(String firstName, String lastName);
}
