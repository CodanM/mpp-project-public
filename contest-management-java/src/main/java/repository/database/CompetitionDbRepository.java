package repository.database;

import model.Competition;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import repository.CompetitionRepository;
import utils.JdbcUtils;

import java.sql.*;
import java.util.*;

public class CompetitionDbRepository implements CompetitionRepository {
    private JdbcUtils dbUtils;

    private static final Logger logger = LogManager.getLogger();

    public CompetitionDbRepository(Properties props) {
        logger.info("Initialising CompetitionDbRepository with properties: {}", props);
        dbUtils = new JdbcUtils(props);
    }

    @Override
    public Iterable<Competition> findAll() {
        logger.traceEntry();
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
            logger.error(e);
            System.err.println("Error DB" + e);
        }
        logger.traceExit();
        return competitions;
    }

    @Override
    public Competition find(Long aLong) {
        logger.traceEntry();
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
            logger.error(e);
            System.err.println("Error DB" + e);
        }
        logger.traceExit();
        return comp;
    }

    @Override
    public Long add(Competition entity) {
        logger.traceEntry("saving task {}", entity);
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
            logger.error(e);
            System.err.println("Error DB " + e);
        }
        logger.traceExit();
        return null;
    }

    @Override
    public void remove(Long aLong) {
        logger.traceEntry("removing task {}", aLong);
        Connection conn = dbUtils.getConnection();
        try (PreparedStatement prepStmt = conn.prepareStatement("delete from Competitions where id = ?")) {
            prepStmt.setLong(1, aLong);
            int result = prepStmt.executeUpdate();
        } catch (SQLException e) {
            logger.error(e);
            System.err.println("Error DB " + e);
        }
        logger.traceExit();
    }

    @Override
    public void update(Competition entity) {
        logger.traceEntry("updating task {}", entity);
        Connection conn = dbUtils.getConnection();
        try (PreparedStatement prepStmt = conn
                .prepareStatement("update Competitions set competition_type = ?, age_category = ? where id = ?")) {
            prepStmt.setString(1, entity.getCompetitionType());
            prepStmt.setString(2, entity.getAgeCategory());
            prepStmt.setLong(3, entity.getId());
            int result = prepStmt.executeUpdate();
        } catch (SQLException e) {
            logger.error(e);
            System.err.println("Error DB " + e);
        }
        logger.traceExit();
    }

    @Override
    public int countParticipantsForCompetition(Long competitionId) {
        logger.traceEntry();
        Connection conn = dbUtils.getConnection();
        int count = 0;
        try (PreparedStatement prepStmt = conn.prepareStatement(
                "select cnt from " +
                        "(select id, competition_type, age_category, count(competition_id) cnt " +
                        "from Competitions C " +
                        "left join Registrations R on C.id = R.competition_id " +
                        "group by competition_id, competition_type, age_category)" +
                        "where id = ?")) {
            prepStmt.setLong(1, competitionId);
            try (ResultSet result = prepStmt.executeQuery()) {
                if (result.next())
                    count = result.getInt("cnt");
            }
        } catch (SQLException e) {
            logger.error(e);
            System.err.println("Error DB " + e);
        }
        logger.traceExit();
        return count;
    }

    @Override
    public Iterable<String> findTypesByString(String competitionType) {
        logger.traceEntry();
        Connection conn = dbUtils.getConnection();
        List<String> competitionTypes = new ArrayList<>();

        try (PreparedStatement prepStmt = conn.prepareStatement(
                "select distinct competition_type " +
                        "from Competitions " +
                        "where competition_type like '%' || ? || '%'")) {
            prepStmt.setString(1, competitionType);
            try (ResultSet result = prepStmt.executeQuery()) {
                while (result.next()) {
                    String compType = result.getString("competition_type");
                    competitionTypes.add(compType);
                }
            }
        } catch (SQLException e) {
            logger.error(e);
            System.err.println("Error DB" + e);
        }
        logger.traceExit();
        return competitionTypes;
    }

    @Override
    public Iterable<String> findAgeCategories(String competitionType) {
        logger.traceEntry();
        Connection conn = dbUtils.getConnection();
        List<String> ageCategories = new ArrayList<>();

        try (PreparedStatement prepStmt = conn.prepareStatement(
                "select age_category " +
                        "from Competitions " +
                        "where competition_type = ?")) {
            prepStmt.setString(1, competitionType);
            try (ResultSet result = prepStmt.executeQuery()) {
                while (result.next()) {
                    String ageCategory = result.getString("age_category");
                    ageCategories.add(ageCategory);
                }
            }
        } catch (SQLException e) {
            logger.error(e);
            System.err.println("Error DB" + e);
        }
        logger.traceExit();
        return ageCategories;
    }

    @Override
    public Competition findByProps(String competitionType, String ageCategory) {
        logger.traceEntry();
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
            logger.error(e);
            System.err.println("Error DB" + e);
        }
        logger.traceExit();
        return c;
    }
}
