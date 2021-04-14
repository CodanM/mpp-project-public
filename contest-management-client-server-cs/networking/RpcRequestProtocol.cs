using System;

namespace networking
{
    public enum RequestType
    {
        Login,
        Logout,
        GetCompetitionsByString,
        GetParticipantsByCompetition,
        GetCompetitionsAndCountsByString,
        GetParticipantIdsByName,
        GetAgeCategoriesForParticipant,
        AddRegistration
    }
    
    [Serializable]
    public class Request
    {
        public RequestType Type { get; set; }
        
        public object? Data { get; set; }

        public override string ToString()
        {
            return $"Request {{ Type: {Type}, Data: {Data} }}";
        }
    }
}