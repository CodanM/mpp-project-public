package contestmgmt.persistence.repository.jdbc;

import contestmgmt.model.Organiser;
import contestmgmt.persistence.repository.IOrganiserRepository;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;
import java.util.Properties;

public class OrganiserDbRepository implements IOrganiserRepository {
    private JdbcUtils dbUtils;

    public OrganiserDbRepository(Properties props) {
        dbUtils = new JdbcUtils(props);
    }

    @Override
    public Iterable<Organiser> findAll() {
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
            System.err.println("Error DB" + e);
        }
        return organisers;
    }

    @Override
    public Organiser find(String s) {
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
            System.err.println("Error DB" + e);
        }
        return o;
    }

    @Override
    public String add(Organiser entity) {
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
            System.err.println("Error DB " + e);
        }
        return null;
    }

    @Override
    public void remove(String s) {
        Connection conn = dbUtils.getConnection();
        try (PreparedStatement prepStmt = conn
                .prepareStatement("delete from Organisers where username = ?")) {
            prepStmt.setString(1, s);
            int result = prepStmt.executeUpdate();
        } catch (SQLException e) {
            System.err.println("Error DB " + e);
        }
    }

    @Override
    public void update(Organiser entity) {
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
            System.err.println("Error DB " + e);
        }
    }
}
