package contestmgmt.persistence.repository.jdbc;

import contestmgmt.model.Competition;
import contestmgmt.model.Tuple;
import contestmgmt.persistence.repository.ICompetitionRepository;

import java.sql.*;
import java.util.*;

public class CompetitionDbRepository implements ICompetitionRepository {
    private JdbcUtils dbUtils;

    public CompetitionDbRepository(Properties props) {
        dbUtils = new JdbcUtils(props);
    }

    @Override
    public Iterable<Competition> findAll() {
        Connection conn = dbUtils.getConnection();
        List<Competition> competitions = new ArrayList<>();

        try (PreparedStatement prepStmt = conn.prepareStatement("select * from Competitions")) {
            try (ResultSet result = prepStmt.executeQuery()) {
                while (result.next()) {
                    long id = result.getLong("id");
                    String competitionType = result.getString("competition_type");
                    String ageCategory = result.getString("age_category");
                    Competition comp = new Competition(competitionType, ageCategory);
                    comp.setId(id);
                    competitions.add(comp);
                }
            }
        } catch (SQLException e) {
            System.err.println("Error DB" + e);
        }
        return competitions;
    }

    @Override
    public Competition find(Long aLong) {
        Connection conn = dbUtils.getConnection();
        Competition comp = null;
        try (PreparedStatement prepStmt = conn.prepareStatement("select * from Competitions where id = ?")) {
            prepStmt.setLong(1, aLong);
            try (ResultSet result = prepStmt.executeQuery()) {
                if (result.next()) {
                    long id = result.getLong("id");
                    String competitionType = result.getString("competition_type");
                    String ageCategory = result.getString("age_category");
                    comp = new Competition(competitionType, ageCategory);
                    comp.setId(id);
                }
            }
        } catch (SQLException e) {
            System.err.println("Error DB" + e);
        }
        return comp;
    }

    @Override
    public Long add(Competition entity) {
        Connection conn = dbUtils.getConnection();
        try (PreparedStatement prepStmt = conn.prepareStatement(
                "insert into Competitions " +
                        "(competition_type, age_category) " +
                        "values (?, ?)", Statement.RETURN_GENERATED_KEYS)) {
            prepStmt.setString(1, entity.getCompetitionType());
            prepStmt.setString(2, entity.getAgeCategory());

            int result = prepStmt.executeUpdate();
            ResultSet resultSet = prepStmt.getGeneratedKeys();
            if (resultSet.next())
                return resultSet.getLong(1);
        } catch (SQLException e) {
            System.err.println("Error DB " + e);
        }
        return null;
    }

    @Override
    public void remove(Long aLong) {
        Connection conn = dbUtils.getConnection();
        try (PreparedStatement prepStmt = conn.prepareStatement("delete from Competitions where id = ?")) {
            prepStmt.setLong(1, aLong);
            int result = prepStmt.executeUpdate();
        } catch (SQLException e) {
            System.err.println("Error DB " + e);
        }
    }

    @Override
    public void update(Competition entity) {
        Connection conn = dbUtils.getConnection();
        try (PreparedStatement prepStmt = conn
                .prepareStatement("update Competitions set competition_type = ?, age_category = ? where id = ?")) {
            prepStmt.setString(1, entity.getCompetitionType());
            prepStmt.setString(2, entity.getAgeCategory());
            prepStmt.setLong(3, entity.getId());
            int result = prepStmt.executeUpdate();
        } catch (SQLException e) {
            System.err.println("Error DB " + e);
        }
    }

    @Override
    public Map<Long, Tuple<Competition, Integer>> countParticipantsForEachCompetition(String competitionType,
                                                                                      String ageCategory) {
        Connection conn = dbUtils.getConnection();
        Map<Long, Tuple<Competition, Integer>> map = new HashMap<>();
        try (PreparedStatement prepStmt = conn.prepareStatement(
                "select *, count(competition_id) cnt " +
                        "from Competitions C " +
                        "left join Registrations R on C.id = R.competition_id " +
                        "where competition_type like '%' || ? || '%' and age_category like '%' || ? || '%' " +
                        "group by id, competition_id, competition_type, age_category")) {
            prepStmt.setString(1, competitionType);
            prepStmt.setString(2, ageCategory);
            try (ResultSet result = prepStmt.executeQuery()) {
                while (result.next()) {
                    long id = result.getLong("id");
                    String compType = result.getString("competition_type");
                    String ageCat = result.getString("age_category");
                    int count = result.getInt("cnt");
                    Competition c = new Competition(compType, ageCat);
                    c.setId(id);
                    map.put(id, new Tuple<>(c, count));
                }
            }
        } catch (SQLException e) {
            System.err.println("Error DB " + e);
        }
        return map;
    }

    @Override
    public Iterable<Competition> findCompetitionsByString(String competitionType, String ageCategory) {
        Connection conn = dbUtils.getConnection();
        List<Competition> competitions = new ArrayList<>();

        try (PreparedStatement prepStmt = conn.prepareStatement(
                "select * from Competitions " +
                        "where competition_type like '%' || ? || '%' and age_category like '%' || ? || '%'")) {
            prepStmt.setString(1, competitionType);
            prepStmt.setString(2, ageCategory);
            try (ResultSet result = prepStmt.executeQuery()) {
                while (result.next()) {
                    Long id = result.getLong("id");
                    String compType = result.getString("competition_type");
                    String ageCat = result.getString("age_category");
                    Competition c = new Competition(compType, ageCat);
                    c.setId(id);
                    competitions.add(c);
                }
            }
        } catch (SQLException e) {
            System.err.println("Error DB" + e);
        }
        return competitions;
    }

    @Override
    public Iterable<String> findAgeCategoriesFromCompetitionType(String competitionType) {
        Connection conn = dbUtils.getConnection();
        List<String> ageCategories = new ArrayList<>();
        try (PreparedStatement prepStmt = conn.prepareStatement(
                "select age_category from Competitions " +
                        "where competition_type = ?")) {
            prepStmt.setString(1, competitionType);
            try (ResultSet result = prepStmt.executeQuery()) {
                while (result.next()) {
                    String ageCategory = result.getString("age_category");
                    ageCategories.add(ageCategory);
                }
            }
        } catch (SQLException e) {
            System.err.println("Error DB" + e);
        }
        return ageCategories;
    }

    @Override
    public Competition findByProps(String competitionType, String ageCategory) {
        Connection conn = dbUtils.getConnection();
        Competition c = null;
        try (PreparedStatement prepStmt = conn.prepareStatement(
                "select * from Competitions " +
                        "where competition_type = ? and age_category = ?")) {
            prepStmt.setString(1, competitionType);
            prepStmt.setString(2, ageCategory);
            try (ResultSet result = prepStmt.executeQuery()) {
                if (result.next()) {
                    Long id = result.getLong("id");
                    String compType = result.getString("competition_type");
                    String ageCat = result.getString("age_category");
                    c = new Competition(compType, ageCat);
                    c.setId(id);
                }
            }
        } catch (SQLException e) {
            System.err.println("Error DB" + e);
        }
        return c;
    }
}
