using System;

namespace networking
{
    public enum ResponseType
    {
        Ok,
        Error,
        NewRegistration,
        GetCompetitionTypes,
        GetParticipantsByCompetition,
        GetCompetitionsAndCountsByString,
        GetParticipantIdsByName,
        GetAgeCategoriesForParticipant
    }
    
    [Serializable]
    public class Response
    {
        public ResponseType Type { get; set; }
        
        public object? Data { get; set; }

        public bool IsUpdate()
        {
            return Type == ResponseType.NewRegistration;
        }

        public override string ToString()
        {
            return $"Response {{ Type: {Type}, Data: {Data} }}";
        }
    }
}