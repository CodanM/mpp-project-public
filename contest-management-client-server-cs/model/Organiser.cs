using System;

namespace model
{
    [Serializable]
    public class Organiser : Entity<string>
    {
        public string Username { get => Id; set => Id = value; }
        
        public string? Password { get; set; }
        
        public string? FirstName { get; set; }
        
        public string? LastName { get; set; }
    }
}