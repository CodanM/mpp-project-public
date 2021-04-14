package contestmgmt.services;

public class ContestManagementException extends Exception {
    public ContestManagementException() {
    }

    public ContestManagementException(String message) {
        super(message);
    }

    public ContestManagementException(String message, Throwable cause) {
        super(message, cause);
    }
}
