using model;

namespace services
{
    public interface IContestMgmtObserver
    {
        void NewRegistration(Registration registration);
    }
}