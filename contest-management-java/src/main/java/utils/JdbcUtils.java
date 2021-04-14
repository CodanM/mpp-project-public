package utils;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;
import java.util.Properties;

public class JdbcUtils {
    private Properties jdbcProps;

    private static final Logger logger = LogManager.getLogger();

    public JdbcUtils(Properties props) {
        jdbcProps = props;
    }

    private Connection instance = null;

    private Connection getNewConnection() {
        logger.traceEntry();

        String url = jdbcProps.getProperty("jdbc.url");
        String user = jdbcProps.getProperty("jdbc.user");
        String password = jdbcProps.getProperty("jdbc.pass");
        logger.info("Trying to connect to database ... {}", url);
        logger.info("user: {}", user);
        logger.info("password: {}", password);

        Connection conn = null;
        try {
            if (user != null && password != null)
                conn = DriverManager.getConnection(url, user, password);
            else
                conn = DriverManager.getConnection(url);
        } catch (SQLException e) {
            logger.error(e);
            System.err.println("Error getting connection " + e);
        }
        return conn;
    }

    public Connection getConnection() {
        logger.traceEntry();
        try {
            if (instance == null || instance.isClosed())
                instance = getNewConnection();
        } catch (SQLException e) {
            logger.error(e);
            System.err.println("Error DB " + e);
        }
        logger.traceExit(instance);
        return instance;
    }
}
