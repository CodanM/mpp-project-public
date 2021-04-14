using System.Collections.Generic;
using contest_management_gui_cs.model;

namespace contest_management_gui_cs.repository
{
    public interface IParticipantRepository : ICrudRepository<long, Participant>
    {
        IEnumerable<Participant> FindParticipantsByCompetition(long competitionId);
        
        Participant FindParticipantByName(string firstName, string lastName);
    }
}