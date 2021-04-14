package contestmgmt.networking.dto;

import java.io.Serializable;

public class StringStringDTO implements Serializable {
    private String first;

    private String second;

    public StringStringDTO(String first, String second) {
        this.first = first;
        this.second = second;
    }

    public String getFirst() {
        return first;
    }

    public void setFirst(String first) {
        this.first = first;
    }

    public String getSecond() {
        return second;
    }

    public void setSecond(String second) {
        this.second = second;
    }

    @Override
    public String toString() {
        return "firstName: %s, lastName: %s".formatted(first, second);
    }
}
