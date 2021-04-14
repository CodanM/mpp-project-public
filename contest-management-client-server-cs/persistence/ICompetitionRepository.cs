using System.Collections.Generic;
using model;

namespace persistence
{
    public interface ICompetitionRepository : ICrudRepository<long, Competition>
    {
        IDictionary<long, (Competition, int)> GetCompetitionsAndCountsByString(string competitionType,
            string ageCategory);

        IEnumerable<Competition> FindCompetitionsByString(string competitionType, string ageCategory);

        IEnumerable<string> FindAgeCategoriesFromCompetitionType(string competitionType);

        Competition? FindByProps(string competitionType, string ageCategory);
    }
}