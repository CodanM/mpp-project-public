#nullable disable
using System;
using model;

namespace networking.dto
{
    [Serializable]
    public class OrganiserDTO
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public static explicit operator Organiser(OrganiserDTO organiserDto)
        {
            return new()
            {
                Username = organiserDto.Username,
                Password = organiserDto.Password,
                FirstName = organiserDto.FirstName,
                LastName = organiserDto.LastName
            };
        }

        public static explicit operator OrganiserDTO(Organiser organiser)
        {
            return new()
            {
                Username = organiser.Username,
                Password = organiser.Password,
                FirstName = organiser.FirstName,
                LastName = organiser.LastName
            };
        }
    }
}