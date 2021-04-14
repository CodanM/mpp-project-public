using System;
using System.Collections.Generic;
using contest_management_gui_cs.model;

namespace contest_management_gui_cs.repository
{
    public interface IRegistrationRepository : ICrudRepository<Tuple<long, long>, Registration>
    {
        IEnumerable<Registration> FindByParticipant(long participantId);
    }
}