package contestmgmt.networking.dto;

import java.io.Serializable;

public class ParticipantDTO implements Serializable {
    private Long id;

    private String firstName;

    private String lastName;

    private int age;

    public ParticipantDTO(Long id, String firstName, String lastName, int age) {
        this.id = id;
        this.firstName = firstName;
        this.lastName = lastName;
        this.age = age;
    }

    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public String getFirstName() {
        return firstName;
    }

    public void setFirstName(String firstName) {
        this.firstName = firstName;
    }

    public String getLastName() {
        return lastName;
    }

    public void setLastName(String lastName) {
        this.lastName = lastName;
    }

    public int getAge() {
        return age;
    }

    public void setAge(int age) {
        this.age = age;
    }

    @Override
    public String toString() {
        return "ParticipantDTO { ID: %s, firstName: %s, lastName: %s, age: %d }".formatted(id.toString(), firstName, lastName, age);
    }
}
