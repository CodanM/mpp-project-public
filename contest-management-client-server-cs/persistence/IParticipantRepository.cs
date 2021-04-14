using System.Collections.Generic;
using model;

namespace persistence
{
    public interface IParticipantRepository : ICrudRepository<long, Participant>
    {
        IEnumerable<Participant> FindParticipantsByCompetition(long competitionId);
        
        IEnumerable<long> FindParticipantIdsByName(string firstName, string lastName);
    }
}