package repository;

import model.Registration;
import model.Tuple;

public interface RegistrationRepository extends CrudRepository<Tuple<Long, Long>, Registration> {
    Iterable<Registration> findByParticipant(long participantId);
}
