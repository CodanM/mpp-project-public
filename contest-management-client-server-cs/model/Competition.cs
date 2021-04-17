using System;

namespace model
{
    [Serializable]
    public class Competition : Entity<long>
    {
        public string? CompetitionType { get; set; }
        
        public string? AgeCategory { get; set; }
    }
}