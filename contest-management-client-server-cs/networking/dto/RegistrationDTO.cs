#nullable disable
using System;
using model;

namespace networking.dto
{
    [Serializable]
    public class RegistrationDTO
    {
        public long ParticipantId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Age { get; set; }

        public long CompetitionId { get; set; }

        public string CompetitionType { get; set; }

        public string AgeCategory { get; set; }

        public static explicit operator Registration(RegistrationDTO registrationDto)
        {
            return new
            (
                new Participant
                {
                    Id = registrationDto.ParticipantId,
                    FirstName = registrationDto.FirstName,
                    LastName = registrationDto.LastName,
                    Age = registrationDto.Age
                },
                new Competition
                {
                    Id = registrationDto.CompetitionId,
                    CompetitionType = registrationDto.CompetitionType,
                    AgeCategory = registrationDto.AgeCategory
                }
            );
        }

        public static explicit operator RegistrationDTO(Registration registration)
        {
            var participant = registration.Participant;
            var competition = registration.Competition;
            return new RegistrationDTO
            {
                ParticipantId = participant.Id,
                FirstName = participant.FirstName,
                LastName = participant.LastName,
                Age = (int) participant.Age!,
                CompetitionId = competition.Id,
                CompetitionType = competition.CompetitionType,
                AgeCategory = competition.AgeCategory
            };
        }
    }
}