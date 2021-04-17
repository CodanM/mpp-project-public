using System.Collections.Generic;
using model;

namespace services
{
    public interface IContestMgmtServices
    {
        void Login(Organiser organiser, IContestMgmtObserver client);

        void Logout(Organiser organiser, IContestMgmtObserver client);

        IList<string> GetCompetitionTypes(string competitionTypeStr = "");

        IList<Participant> GetParticipantsByCompetition(Competition competition);

        IDictionary<long, (Competition, int)> GetCompetitionsAndCounts(string competitionTypeStr = "",
            string ageCategoryStr = "");

        IList<long> GetParticipantIdsByName(string firstName, string lastName);

        IList<string> GetAgeCategoriesForParticipant(Participant participant, string competitionType);

        void AddRegistration(Registration registration);
    }
}