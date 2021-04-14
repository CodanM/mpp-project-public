package contestmgmt.networking.dto;

import java.io.Serializable;

public class ParticipantStringDTO implements Serializable {
    private ParticipantDTO participantDTO;

    private String string;

    public ParticipantStringDTO(ParticipantDTO participantDTO, String string) {
        this.participantDTO = participantDTO;
        this.string = string;
    }

    public ParticipantDTO getParticipantDTO() {
        return participantDTO;
    }

    public void setParticipantDTO(ParticipantDTO participantDTO) {
        this.participantDTO = participantDTO;
    }

    public String getString() {
        return string;
    }

    public void setString(String string) {
        this.string = string;
    }
}
