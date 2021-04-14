using System;

namespace contest_management_gui_cs.model
{
    public class Registration : Entity<Tuple<long, long>>
    {
        private Participant _participant;
        public Participant RegisteredParticipant
        {
            get => _participant;
            set
            {
                _participant = value;
                Id = new Tuple<long, long>(_participant.Id, _competition?.Id ?? -1);
            }
        }

        private Competition _competition;

        public Competition WhichCompetition
        {
            get => _competition;
            set
            {
                _competition = value;
                Id = new Tuple<long, long>(_participant?.Id ?? -1, _competition.Id);
            }
        }
    }
}