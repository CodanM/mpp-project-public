using System;

namespace model
{
    [Serializable]
    public class Participant : Entity<long>
    {
        public string? FirstName { get; set; }
        
        public string? LastName { get; set; }
        
        public int? Age { get; set; }
    }
}