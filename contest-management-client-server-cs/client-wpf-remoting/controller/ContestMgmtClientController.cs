using System;
using System.Collections.Generic;
using System.Linq;
using model;
using services;

namespace client_wpf_remoting.controller
{
    public class ContestMgmtClientController : MarshalByRefObject, IContestMgmtObserver
    {
        private readonly IContestMgmtServices _server;

        private Organiser _currentOrganiser;

        public event EventHandler<ContestMgmtOrganiserEventArgs> UpdateEvent;

        public ContestMgmtClientController(IContestMgmtServices server)
        {
            _server = server;
        }
        
        public void NewRegistration(Registration registration)
        {
            Console.WriteLine($"New Registration {registration}");
            var organiserEventArgs =
                new ContestMgmtOrganiserEventArgs(ContestMgmtOrganiserEvent.NewRegistration, registration);
            OnOrganiserEvent(organiserEventArgs);
        }

        public void Login(string username, string password)
        {
            var org = new Organiser {Username = username, Password = password};
            _server.Login(org, this);
            Console.WriteLine("Login successful...");

            _currentOrganiser = org;
            Console.WriteLine($"Current Organiser: {org}");
        }

        public void Logout()
        {
            Console.WriteLine("Controller logout");
            _server.Logout(_currentOrganiser, this);
            _currentOrganiser = null;
        }

        public IList<CompetitionCountDTO> GetCompetitionsAndCounts(string competitionTypeStr = "",
            string ageCategoryStr = "")
        {
            Console.WriteLine($"GetCompetitionsAndCountsByString {competitionTypeStr}, {ageCategoryStr}");
            var values = _server.GetCompetitionsAndCounts(competitionTypeStr, ageCategoryStr).Values;
            return values.Select(t => (CompetitionCountDTO) t).ToList();
        }

        public IList<Participant> GetParticipantsByCompetition(Competition competition)
        {
            Console.WriteLine($"GetParticipantsByCompetition {competition}");
            return _server.GetParticipantsByCompetition(competition);
        }

        public IList<string> GetCompetitionTypes(string competitionTypeStr = "")
        {
            Console.WriteLine($"GetCompetitionTypes {competitionTypeStr}");
            return _server.GetCompetitionTypes(competitionTypeStr);
        }

        public IList<string> GetAgeCategoriesForParticipant(string competitionType, long? id, int? age)
        {
            if (id == null && age == null)
                throw new Exception("id and age cannot be both null!");
            var participant = new Participant {Id = id ?? 0, Age = id == null ? age : null};
            
            Console.WriteLine($"GetAgeCategoriesForParticipant {competitionType}, {id}, {age}");
            return _server.GetAgeCategoriesForParticipant(participant, competitionType);
        }

        public IList<long> GetParticipantIdsByName(string firstName, string lastName)
        {
            Console.WriteLine($"GetParticipantIdsByName {firstName}, {lastName}");
            return _server.GetParticipantIdsByName(firstName, lastName);
        }

        public void AddRegistration(long participantId, string firstName, string lastName, int age,
            string competitionType, string ageCategory)
        {
            var participant = new Participant
            {
                Id = participantId,
                FirstName = firstName,
                LastName = lastName,
                Age = age
            };
            var competition = new Competition
            {
                CompetitionType = competitionType,
                AgeCategory = ageCategory
            };
            Console.WriteLine($"AddRegistration {participant}, {competition}");
            _server.AddRegistration(new Registration(participant, competition));
        }

        private void OnOrganiserEvent(ContestMgmtOrganiserEventArgs e)
        {
            UpdateEvent?.Invoke(this, e);
            Console.WriteLine("Update Event called");
        }
    }
}