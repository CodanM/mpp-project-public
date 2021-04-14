package contestmgmt.persistence.repository.jdbc;

import contestmgmt.model.Competition;
import contestmgmt.model.Participant;
import contestmgmt.persistence.repository.IParticipantRepository;

import java.sql.*;
import java.util.ArrayList;
import java.util.List;
import java.util.Properties;

public class ParticipantDbRepository implements IParticipantRepository {
    private JdbcUtils dbUtils;

    public ParticipantDbRepository(Properties props) {
        dbUtils = new JdbcUtils(props);
    }

    @Override
    public Iterable<Participant> findAll() {
        Connection conn = dbUtils.getConnection();
        List<Participant> participants = new ArrayList<>();

        try (PreparedStatement prepStmt = conn.prepareStatement("select * from Participants")) {
            try (ResultSet result = prepStmt.executeQuery()) {
                while (result.next()) {
                    long id = result.getLong("id");
                    String firstName = result.getString("first_name");
                    String lastName = result.getString("last_name");
                    int age = result.getInt("age");
                    Participant p = new Participant(firstName, lastName, age);
                    p.setId(id);
                    participants.add(p);
                }
            }
        } catch (SQLException e) {
            System.err.println("Error DB" + e);
        }
        return participants;
    }

    @Override
    public Participant find(Long aLong) {
        Connection conn = dbUtils.getConnection();
        Participant p = null;
        try (PreparedStatement prepStmt = conn.prepareStatement("select * from Participants where id = ?")) {
            prepStmt.setLong(1, aLong);
            try (ResultSet result = prepStmt.executeQuery()) {
                result.next();
                long id = result.getLong("id");
                String firstName = result.getString("first_name");
                String lastName = result.getString("last_name");
                int age = result.getInt("age");
                p = new Participant(firstName, lastName, age);
                p.setId(id);
            }
        } catch (SQLException e) {
            System.err.println("Error DB" + e);
        }
        return p;
    }

    @Override
    public Long add(Participant entity) {
        Connection conn = dbUtils.getConnection();
        try (PreparedStatement prepStmt = conn.prepareStatement(
                "insert into Participants " +
                        "(first_name, last_name, age) " +
                        "values (?, ?, ?)", Statement.RETURN_GENERATED_KEYS)) {
            prepStmt.setString(1, entity.getFirstName());
            prepStmt.setString(2, entity.getLastName());
            prepStmt.setInt(3, entity.getAge());

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
        try (PreparedStatement prepStmt = conn.prepareStatement("delete from Participants where id = ?")) {
            prepStmt.setLong(1, aLong);
            int result = prepStmt.executeUpdate();
        } catch (SQLException e) {
            System.err.println("Error DB " + e);
        }
    }

    @Override
    public void update(Participant entity) {
        Connection conn = dbUtils.getConnection();
        try (PreparedStatement prepStmt = conn.prepareStatement(
                "update Participants set " +
                        "first_name = ?, " +
                        "last_name = ?, " +
                        "age = ? " +
                        "where id = ?")) {
            prepStmt.setString(1, entity.getFirstName());
            prepStmt.setString(2, entity.getLastName());
            prepStmt.setInt(3, entity.getAge());
            prepStmt.setLong(4, entity.getId());
            int result = prepStmt.executeUpdate();
        } catch (SQLException e) {
            System.err.println("Error DB " + e);
        }
    }

    @Override
    public Iterable<Participant> findParticipantsByCompetition(Competition competition) {
        Connection conn = dbUtils.getConnection();
        List<Participant> participants = new ArrayList<>();

        try (PreparedStatement prepStmt = conn.prepareStatement(
                "select participant_id, first_name, last_name, age from Participants P " +
                        "inner join Registrations R on P.id = R.participant_id " +
                        "where competition_id = ?")) {
            prepStmt.setLong(1, competition.getId());
            try (ResultSet result = prepStmt.executeQuery()) {
                while (result.next()) {
                    long id = result.getLong("participant_id");
                    String firstName = result.getString("first_name");
                    String lastName = result.getString("last_name");
                    int age = result.getInt("age");
                    Participant p = new Participant(firstName, lastName, age);
                    p.setId(id);
                    participants.add(p);
                }
            }
        } catch (SQLException e) {
            System.err.println("Error DB" + e);
        }
        return participants;
    }

    @Override
    public Iterable<Long> findParticipantIdsByName(String firstName, String lastName) {
        Connection conn = dbUtils.getConnection();
        List<Long> participantIds = new ArrayList<>();
        try (PreparedStatement prepStmt = conn.prepareStatement(
                "select id from Participants " +
                        "where first_name = ? and last_name = ?")) {
            prepStmt.setString(1, firstName);
            prepStmt.setString(2, lastName);
            try (ResultSet result = prepStmt.executeQuery()) {
                while (result.next())
                    participantIds.add(result.getLong("id"));
            }
        } catch (SQLException e) {
            System.err.println("Error DB" + e);
        }
        return participantIds;
    }
}
