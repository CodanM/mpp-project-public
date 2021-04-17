using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using model;
using persistence;
using services;

namespace server_console_remoting
{
    public class ContestMgmtServerImpl : MarshalByRefObject, IContestMgmtServices
    {
        private readonly IParticipantRepository _participantRepository;

        private readonly ICompetitionRepository _competitionRepository;

        private readonly IOrganiserRepository _organiserRepository;

        private IRegistrationRepository _registrationRepository;

        private readonly IDictionary<string, IContestMgmtObserver> _loggedOrganisers =
            new Dictionary<string, IContestMgmtObserver>();

        public ContestMgmtServerImpl(IParticipantRepository participantRepository,
            ICompetitionRepository competitionRepository, IOrganiserRepository organiserRepository,
            IRegistrationRepository registrationRepository)
        {
            _participantRepository = participantRepository;
            _competitionRepository = competitionRepository;
            _organiserRepository = organiserRepository;
            _registrationRepository = registrationRepository;
        }

        public void Login(Organiser organiser, IContestMgmtObserver client)
        {
            var org = _organiserRepository.Find(organiser.Username);
            if (org == null || org.Password != organiser.Password)
                throw new ContestMgmtException("Incorrect username or password!");

            if (_loggedOrganisers.ContainsKey(organiser.Username))
                throw new ContestMgmtException("Organiser already logged in!");

            _loggedOrganisers[organiser.Username] = client;
        }

        public void Logout(Organiser organiser, IContestMgmtObserver client)
        {
            if (!_loggedOrganisers.ContainsKey(organiser.Username))
                throw new ContestMgmtException("Organiser is not logged in!");
            
            _loggedOrganisers.Remove(organiser.Username);
        }

        public IList<string> GetCompetitionTypes(string competitionTypeStr)
        {
            return _competitionRepository.FindCompetitionTypes(competitionTypeStr).ToList();
        }

        public IList<Participant> GetParticipantsByCompetition(Competition competition)
        {
            return _participantRepository.FindParticipantsByCompetition(competition.Id).ToList();
        }

        public IDictionary<long, (Competition, int)> GetCompetitionsAndCounts(string competitionTypeStr,
            string ageCategoryStr)
        {
            return _competitionRepository.GetCompetitionsAndCounts(competitionTypeStr, ageCategoryStr);
        }

        public IList<long> GetParticipantIdsByName(string firstName, string lastName)
        {
            return _participantRepository.FindParticipantIdsByName(firstName, lastName).ToList();
        }

        public IList<string> GetAgeCategoriesForParticipant(Participant participant, string competitionType)
        {
            var age = participant.Age ?? _participantRepository.Find(participant.Id)?.Age;
            if (age == null)
                throw new ContestMgmtException("Could not get participant age!");
            return _competitionRepository.FindAgeCategoriesFromCompetitionType(competitionType)
                .Where(str =>
                {
                    var limits = str.Split('-');
                    var (minAge, maxAge) = (int.Parse(limits[0]), int.Parse(limits[1]));
                    return age >= minAge && age <= maxAge;
                })
                .ToList();
        }

        public void AddRegistration(Registration registration)
        {
            var (participant, competition) = registration;
            
            if (competition.CompetitionType == null || competition.AgeCategory == null)
                throw new ContestMgmtException("CompetitionType and AgeCategory cannot be null!");
            
            if (participant.Id == 0)
                _participantRepository.Add(participant);

            competition = _competitionRepository.FindByProps(competition.CompetitionType, competition.AgeCategory) ??
                          throw new ContestMgmtException("Competition not found!");

            registration = new Registration(participant, competition);
            
            var regs = _registrationRepository.FindByParticipant(participant.Id).ToArray();
            if (regs.Length >= 2)
                throw new ContestMgmtException(
                    "A Participant cannot be registered to more than 2 competitions at once!");
            if (regs.Any(reg => reg.Participant.Id == participant.Id && reg.Competition.Id == competition.Id))
                throw new ContestMgmtException("Registration already exists!");
            
            _registrationRepository.Add(registration);
            NotifyNewRegistration(registration);
        }

        private void NotifyNewRegistration(Registration registration)
        {
            Console.WriteLine("Notify organisers about new Registration...");
            foreach (var client in _loggedOrganisers.Values)
                Task.Run(() => client.NewRegistration(registration));
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}