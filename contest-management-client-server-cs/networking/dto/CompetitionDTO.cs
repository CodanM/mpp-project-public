#nullable disable
using System;
using model;

namespace networking.dto
{
    [Serializable]
    public class CompetitionDTO
    {
        public long CompetitionId { get; set; }

        public string CompetitionType { get; set; }

        public string AgeCategory { get; set; }

        public static explicit operator Competition(CompetitionDTO competitionDto)
        {
            return new()
            {
                Id = competitionDto.CompetitionId,
                CompetitionType = competitionDto.CompetitionType,
                AgeCategory = competitionDto.CompetitionType
            };
        }

        public static explicit operator CompetitionDTO(Competition competition)
        {
            return new()
            {
                CompetitionId = competition.Id,
                CompetitionType = competition.CompetitionType,
                AgeCategory = competition.AgeCategory
            };
        }
    }
}