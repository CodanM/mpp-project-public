using System;
using model;

namespace networking.dto
{
    [Serializable]
    public class ParticipantDTO
    {
        public long ParticipantId { get; set; }

        public string? FirstName { get; set; }
        
        public string? LastName { get; set; }
        
        public int? Age { get; set; }

        public static explicit operator Participant(ParticipantDTO participantDto)
        {
            return new()
            {
                Id = participantDto.ParticipantId,
                FirstName = participantDto.FirstName,
                LastName = participantDto.LastName,
                Age = participantDto.Age
            };
        }

        public static explicit operator ParticipantDTO(Participant participant)
        {
            return new()
            {
                ParticipantId = participant.Id,
                FirstName = participant.FirstName,
                LastName = participant.LastName,
                Age = participant.Age
            };
        }
    }
}