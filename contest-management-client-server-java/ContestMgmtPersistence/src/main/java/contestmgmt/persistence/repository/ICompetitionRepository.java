package contestmgmt.persistence.repository;

import contestmgmt.model.Competition;
import contestmgmt.model.Tuple;

import java.util.Map;

public interface ICompetitionRepository extends ICrudRepository<Long, Competition> {
    public Map<Long, Tuple<Competition, Integer>> countParticipantsForEachCompetition(String competitionType,
                                                                                      String ageCategory);

    Iterable<Competition> findCompetitionsByString(String competitionType, String ageCategory);

    Iterable<String> findAgeCategoriesFromCompetitionType(String competitionType);

    Competition findByProps(String competitionType, String ageCategory);
}
