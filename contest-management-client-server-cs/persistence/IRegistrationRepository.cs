using System.Collections.Generic;
using model;

namespace persistence
{
    public interface IRegistrationRepository : ICrudRepository<(long, long), Registration>
    {
        IEnumerable<Registration> FindByParticipant(long participantId);
    }
}