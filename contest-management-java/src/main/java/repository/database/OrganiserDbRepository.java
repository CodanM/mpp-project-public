package repository.database;

import model.Organiser;
import model.Participant;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import repository.OrganiserRepository;
import utils.JdbcUtils;

import java.sql.*;
import java.util.ArrayList;
import java.util.List;
import java.util.Properties;

public class OrganiserDbRepository implements OrganiserRepository {
    private JdbcUtils dbUtils;

    private static final Logger logger = LogManager.getLogger();

    public OrganiserDbRepository(Properties props) {
        logger.info("Initialising OrganiserDbRepository with properties: {}", props);
        dbUtils = new JdbcUtils(props);
    }

    @Override
    public Iterable<Organiser> findAll() {
        logger.traceEntry();
        Connection conn = dbUtils.getConnection();
        List<Organiser> organisers = new ArrayList<>();

        try (PreparedStatement prepStmt = conn.prepareStatement("select * from Organisers")) {
            try (ResultSet result = prepStmt.executeQuery()) {
                while (result.next()) {
                    String username = result.getString("username");
                    String password = result.getString("password");
                    String firstName = result.getString("first_name");
                    String lastName = result.getString("last_name");
                    Organiser o = new Organiser(username, password, firstName, lastName);
                    organisers.add(o);
                }
            }
        } catch (SQLException e) {
            logger.error(e);
            System.err.println("Error DB" + e);
        }
        logger.traceExit();
        return organisers;
    }

    @Override
    public Organiser find(String s) {
        logger.traceEntry();
        Connection conn = dbUtils.getConnection();
        Organiser o = null;
        try (PreparedStatement prepStmt = conn.prepareStatement("select * from Organisers where username = ?")) {
            prepStmt.setString(1, s);
            try (ResultSet result = prepStmt.executeQuery()) {
                result.next();
                String username = result.getString("username");
                String password = result.getString("password");
                String firstName = result.getString("first_name");
                String lastName = result.getString("last_name");
                o = new Organiser(username, password, firstName, lastName);
            }
        } catch (SQLException e) {
            logger.error(e);
            System.err.println("Error DB" + e);
        }
        logger.traceExit();
        return o;
    }

    @Override
    public String add(Organiser entity) {
        logger.traceEntry("saving task {}", entity);
        Connection conn = dbUtils.getConnection();
        try (PreparedStatement prepStmt = conn.prepareStatement(
                "insert into Organisers " +
                        "(username, password, first_name, last_name) " +
                        "values (?, ?, ?, ?)")) {
            prepStmt.setString(1, entity.getUsername());
            prepStmt.setString(2, entity.getPassword());
            prepStmt.setString(3, entity.getFirstName());
            prepStmt.setString(4, entity.getLastName());

            int result = prepStmt.executeUpdate();
        } catch (SQLException e) {
            logger.error(e);
            System.err.println("Error DB " + e);
        }
        logger.traceExit();
        return null;
    }

    @Override
    public void remove(String s) {
        logger.traceEntry("removing task {}", s);
        Connection conn = dbUtils.getConnection();
        try (PreparedStatement prepStmt = conn
                .prepareStatement("delete from Organisers where username = ?")) {
            prepStmt.setString(1, s);
            int result = prepStmt.executeUpdate();
        } catch (SQLException e) {
            logger.error(e);
            System.err.println("Error DB " + e);
        }
        logger.traceExit();
    }

    @Override
    public void update(Organiser entity) {
        logger.traceEntry("updating task {}", entity);
        Connection conn = dbUtils.getConnection();
        try (PreparedStatement prepStmt = conn.prepareStatement(
                "update Organisers set " +
                        "password = ?, " +
                        "first_name = ?, " +
                        "last_name = ? " +
                        "where username = ?")) {
            prepStmt.setString(1, entity.getPassword());
            prepStmt.setString(2, entity.getFirstName());
            prepStmt.setString(3, entity.getLastName());
            prepStmt.setString(4, entity.getUsername());
            int result = prepStmt.executeUpdate();
        } catch (SQLException e) {
            logger.error(e);
            System.err.println("Error DB " + e);
        }
        logger.traceExit();
    }
}
