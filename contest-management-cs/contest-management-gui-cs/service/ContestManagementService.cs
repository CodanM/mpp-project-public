using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Authentication;
using contest_management_gui_cs.model;
using contest_management_gui_cs.repository;

namespace contest_management_gui_cs.service
{
    public class ContestManagementService
    {
        public IParticipantRepository ParticipantRepository { private get; init; }
        public ICompetitionRepository CompetitionRepository { private get; init; }
        public IOrganiserRepository OrganiserRepository { private get; init; }
        public IRegistrationRepository RegistrationRepository { private get; init; }

        public Organiser CurrentOrganiser { get; private set; }

        public void Login(string username, string password)
        {
            var o = OrganiserRepository.Find(username);
            
            if (o == null || o.Password != password)
                throw new InvalidCredentialException("Invalid username or password!");

            CurrentOrganiser = o;
        }

        public IEnumerable<string> GetCompetitionsByString(string competitionType)
        {
            return CompetitionRepository.FindTypesByString(competitionType);
        }

        public IEnumerable<string> GetAgeCategoriesByCompetitionType(string competitionType)
        {
            return CompetitionRepository.FindAgeCategories(competitionType);
        }
        
        public IEnumerable<Participant> GetParticipantsByCompetition(string competitionType, string ageCategory)
        {
            var comp = CompetitionRepository.FindByProps(competitionType, ageCategory);
            return ParticipantRepository.FindParticipantsByCompetition(comp.Id);
        }

        public int CountParticipantsForCompetition(string competitionType, string ageCategory)
        {
            var comp = CompetitionRepository.FindByProps(competitionType, ageCategory);
            var dict = CompetitionRepository.CountParticipantsForEachCompetition();
            foreach (var (key, value) in dict)
            {
                if (key.Id == comp.Id)
                    return value;
            }

            return 0;
        }

        public void AddRegistration(string firstName, string lastName, int age, string competitionType,
            string ageCategory)
        {
            var p = ParticipantRepository.FindParticipantByName(firstName, lastName);
            if (p == null)
            {
                ParticipantRepository.Add(new Participant
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Age = age
                });
                p = ParticipantRepository.FindParticipantByName(firstName, lastName);
            }

            var c = CompetitionRepository.FindByProps(competitionType, ageCategory);

            var r = RegistrationRepository.Find(new Tuple<long, long>(p.Id, c.Id));

            if (r == null)
            {
                var regs = RegistrationRepository.FindByParticipant(p.Id);
                if (regs.Count() < 2)
                    RegistrationRepository.Add(new Registration
                    {
                        RegisteredParticipant = p,
                        WhichCompetition = c
                    });
                else throw new Exception("Participant is already registered to 2 competitions!");
            }
            else throw new DuplicateNameException("Registration already exists!");
        }
    }
}