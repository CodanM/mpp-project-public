package contestmgmt.persistence.repository.jdbc;

import javax.swing.*;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;
import java.util.Properties;

public class JdbcUtils {
    private Properties jdbcProps;

    public JdbcUtils(Properties props) {
        jdbcProps = props;
    }

    private Connection instance = null;

    private Connection getNewConnection() {

        String url = jdbcProps.getProperty("contestmgmt.jdbc.url");

        Connection conn = null;
        try {
            conn = DriverManager.getConnection(url);
        } catch (SQLException e) {
            System.err.println("Error getting connection " + e);
        }
        return conn;
    }

    public Connection getConnection() {
        try {
            if (instance == null || instance.isClosed())
                instance = getNewConnection();
        } catch (SQLException e) {
            System.err.println("Error DB " + e);
        }
        return instance;
    }
}
