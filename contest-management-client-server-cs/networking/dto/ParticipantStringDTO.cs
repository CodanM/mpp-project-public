#nullable disable
using System;
using model;

namespace networking.dto
{
    [Serializable]
    public class ParticipantStringDTO
    {
        public long ParticipantId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public int? Age { get; set; }
        
        public string CompetitionType { get; set; }

        public static explicit operator (Participant, string)(ParticipantStringDTO psDto)
        {
            return
            (
                new Participant
                {
                    Id = psDto.ParticipantId,
                    FirstName = psDto.FirstName,
                    LastName = psDto.LastName,
                    Age = psDto.Age
                },
                psDto.CompetitionType
            );
        }

        public static explicit operator ParticipantStringDTO((Participant, string) ps)
        {
            var (part, compType) = ps;
            return new ParticipantStringDTO
            {
                ParticipantId = part.Id,
                FirstName = part.FirstName,
                LastName = part.LastName,
                Age = part.Age,
                CompetitionType = compType
            };
        }
    }
}