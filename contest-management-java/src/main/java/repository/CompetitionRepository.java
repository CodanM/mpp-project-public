package repository;

import model.Competition;

import java.util.Map;

public interface CompetitionRepository extends CrudRepository<Long, Competition> {
    int countParticipantsForCompetition(Long competitionId);

    Iterable<String> findTypesByString(String competitionType);

    Iterable<String> findAgeCategories(String competitionType);

    Competition findByProps(String competitionType, String ageCategory);
}
