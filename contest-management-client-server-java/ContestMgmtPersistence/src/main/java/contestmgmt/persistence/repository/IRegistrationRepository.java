package contestmgmt.persistence.repository;

import contestmgmt.model.Registration;
import contestmgmt.model.Tuple;

public interface IRegistrationRepository extends ICrudRepository<Tuple<Long, Long>, Registration> {
    Iterable<Registration> findByParticipant(long participantId);
}
