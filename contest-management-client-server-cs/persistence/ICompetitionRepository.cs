using System.Collections.Generic;
using model;

namespace persistence
{
    public interface ICompetitionRepository : ICrudRepository<long, Competition>
    {
        IDictionary<long, (Competition, int)> GetCompetitionsAndCounts(string competitionTypeStr = "",
            string ageCategoryStr = "");

        IEnumerable<string> FindCompetitionTypes(string competitionTypeStr = "");

        IEnumerable<string> FindAgeCategoriesFromCompetitionType(string competitionType);

        Competition? FindByProps(string competitionType, string ageCategory);
    }
}