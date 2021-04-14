#nullable disable
using System;
using model;

namespace networking.dto
{
    [Serializable]
    public class CompetitionCountDTO
    {
        public long CompetitionId { get; set; }

        public string CompetitionType { get; set; }

        public string AgeCategory { get; set; }

        public int Count { get; set; }

        public static explicit operator (Competition, int)(CompetitionCountDTO ccDto)
        {
            return
            (
                new Competition
                {
                    Id = ccDto.CompetitionId,
                    CompetitionType = ccDto.CompetitionType,
                    AgeCategory = ccDto.AgeCategory
                }, 
                ccDto.Count
            );
        }

        public static explicit operator CompetitionCountDTO((Competition, int) cc)
        {
            var (comp, count) = cc;
            return new CompetitionCountDTO
            {
                CompetitionId = comp.Id,
                CompetitionType = comp.CompetitionType,
                AgeCategory = comp.AgeCategory,
                Count = count
            };
        }
    }
}