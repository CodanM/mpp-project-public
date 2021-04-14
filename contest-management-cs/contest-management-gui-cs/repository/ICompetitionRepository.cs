using System.Collections.Generic;
using contest_management_gui_cs.model;

namespace contest_management_gui_cs.repository
{
    public interface ICompetitionRepository : ICrudRepository<long, Competition>
    {
        IDictionary<Competition, int> CountParticipantsForEachCompetition();
        
        IEnumerable<string> FindTypesByString(string competitionType);

        IEnumerable<string> FindAgeCategories(string competitionType);

        Competition FindByProps(string competitionType, string ageCategory);
    }
}