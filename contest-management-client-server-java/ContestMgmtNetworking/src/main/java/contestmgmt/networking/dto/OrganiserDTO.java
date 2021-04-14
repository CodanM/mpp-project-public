package contestmgmt.networking.dto;

import java.io.Serializable;

public class OrganiserDTO implements Serializable {
    private String username;

    private String password;

    public String getUsername() {
        return username;
    }

    public OrganiserDTO(String username, String password) {
        this.username = username;
        this.password = password;
    }

    public void setUsername(String username) {
        this.username = username;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    @Override
    public String toString() {
        return "OrganiserDTO { username: %sn, password: %s }".formatted(username, password);
    }
}
