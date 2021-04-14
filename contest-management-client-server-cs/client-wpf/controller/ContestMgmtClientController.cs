using System;
using System.Collections.Generic;
using System.Linq;
using model;
using networking.dto;
using services;

namespace client_wpf.controller
{
    public class ContestMgmtClientController : IContestMgmtObserver
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
            throw new System.NotImplementedException();
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

        public IList<CompetitionCountDTO> GetCompetitionsAndCountsByString(string competitionType, string ageCategory)
        {
            var values = _server.GetCompetitionsAndCountsByString(competitionType, ageCategory).Values;
            return values.Select(t => (CompetitionCountDTO) t).ToList();
        }
        
        public IList<Participant> GetParticipantsByCompetition(Competition competition)
        {
            return _server.GetParticipantsByCompetition(competition);
        }

        private void OnOrganiserEvent(ContestMgmtOrganiserEventArgs e)
        {
            UpdateEvent?.Invoke(this, e);
            Console.WriteLine("Update Event called");
        }
    }
}