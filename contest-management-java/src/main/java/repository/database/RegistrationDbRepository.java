package repository.database;

import model.*;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import repository.RegistrationRepository;
import utils.JdbcUtils;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.*;

public class RegistrationDbRepository implements RegistrationRepository {
    private JdbcUtils dbUtils;

    private static final Logger logger = LogManager.getLogger();

    public RegistrationDbRepository(Properties props) {
        logger.info("Initialising RegistrationDbRepository with properties: {}", props);
        dbUtils = new JdbcUtils(props);
    }

    @Override
    public Iterable<Registration> findAll() {
        logger.traceEntry();
        Connection conn = dbUtils.getConnection();
        List<Registration> registrations = new ArrayList<>();

        try (PreparedStatement prepStmt = conn.prepareStatement(
                "select * from Registrations as R " +
                        "inner join Participants as P on R.participant_id = P.id " +
                        "inner join Competitions as C on R.participant_id = C.id")) {
            try (ResultSet result = prepStmt.executeQuery()) {
                while (result.next()) {
                    long participantId = result.getLong("participant_id");
                    String firstName = result.getString("first_name");
                    String lastName = result.getString("last_name");
                    int age = result.getInt("age");
                    long competitionId = result.getLong("competition_id");
                    String competitionType = result.getString("competition_type");
                    String ageCategory = result.getString("age_category");
                    Participant p = new Participant(firstName, lastName, age);
                    p.setId(participantId);
                    Competition c = new Competition(competitionType, ageCategory);
                    c.setId(competitionId);
                    Registration r = new Registration(p, c);
                    registrations.add(r);
                }
            }
        } catch (SQLException e) {
            logger.error(e);
            System.err.println("Error DB" + e);
        }
        logger.traceExit();
        return registrations;
    }

    @Override
    public Registration find(Tuple<Long, Long> longLongTuple) {
        logger.traceEntry();
        Connection conn = dbUtils.getConnection();
        Registration r = null;
        try (PreparedStatement prepStmt = conn.prepareStatement(
                "select * from Registrations R " +
                        "inner join Participants P on R.participant_id = P.id " +
                        "inner join Competitions C on C.id = R.competition_id " +
                        "where participant_id = ? and competition_id = ?")) {
            prepStmt.setLong(1, longLongTuple.getLeft());
            prepStmt.setLong(2, longLongTuple.getRight());
            try (ResultSet result = prepStmt.executeQuery()) {
                result.next();
                long participantId = result.getLong("participant_id");
                String firstName = result.getString("first_name");
                String lastName = result.getString("last_name");
                int age = result.getInt("age");
                var p = new Participant(firstName, lastName, age);
                p.setId(participantId);

                long competitionId = result.getLong("competition_id");
                String competitionType = result.getString("competition_type");
                String ageCategory = result.getString("age_category");
                var c = new Competition(competitionType, ageCategory);
                c.setId(competitionId);

                r = new Registration(p, c);
            }
        } catch (SQLException e) {
            logger.error(e);
            System.err.println("Error DB" + e);
        }
        logger.traceExit();
        return r;
    }

    @Override
    public Tuple<Long, Long> add(Registration entity) {
        logger.traceEntry("saving task {}", entity);
        Connection conn = dbUtils.getConnection();
        try (PreparedStatement prepStmt = conn.prepareStatement(
                        "insert into Registrations (participant_id, competition_id) " +
                                "values (?, ?)")) {
            prepStmt.setLong(1, entity.getParticipant().getId());
            prepStmt.setLong(2, entity.getCompetition().getId());
            int result = prepStmt.executeUpdate();
        } catch (SQLException e) {
            logger.error(e);
            System.err.println("Error DB " + e);
        }
        logger.traceExit();
        return null;
    }

    @Override
    public void remove(Tuple<Long, Long> longLongTuple) {
        logger.traceEntry("removing task {}", longLongTuple);
        Connection conn = dbUtils.getConnection();
        try (PreparedStatement prepStmt = conn.prepareStatement(
                "delete from Registrations " +
                        "where participant_id = ? and competition_id = ?")) {
            prepStmt.setLong(1, longLongTuple.getLeft());
            prepStmt.setLong(2, longLongTuple.getRight());
            int result = prepStmt.executeUpdate();
        } catch (SQLException e) {
            logger.error(e);
            System.err.println("Error DB " + e);
        }
        logger.traceExit();
    }

    @Override
    public void update(Registration entity) {

    }

    @Override
    public Iterable<Registration> findByParticipant(long participantId) {
        logger.traceEntry();
        Connection conn = dbUtils.getConnection();
        List<Registration> registrations = new ArrayList<>();
        try (PreparedStatement prepStmt = conn.prepareStatement(
                "select * from Registrations R " +
                        "inner join Participants P on R.participant_id = P.id " +
                        "inner join Competitions C on C.id = R.competition_id " +
                        "where participant_id = ?")) {
            prepStmt.setLong(1, participantId);
            try (ResultSet result = prepStmt.executeQuery()) {
                while (result.next()) {
                    long partId = result.getLong("participant_id");
                    String firstName = result.getString("first_name");
                    String lastName = result.getString("last_name");
                    int age = result.getInt("age");
                    var p = new Participant(firstName, lastName, age);
                    p.setId(participantId);

                    long competitionId = result.getLong("competition_id");
                    String competitionType = result.getString("competition_type");
                    String ageCategory = result.getString("age_category");
                    var c = new Competition(competitionType, ageCategory);
                    c.setId(competitionId);

                    Registration r = new Registration(p, c);
                    registrations.add(r);
                }
            }
        } catch (SQLException e) {
            logger.error(e);
            System.err.println("Error DB" + e);
        }
        logger.traceExit();
        return registrations;
    }
}
