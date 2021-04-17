using System;

namespace model
{
    [Serializable]
    public class Registration : Entity<(long, long)>
    {
        private Participant _participant;

        private Competition _competition;

        public Registration(Participant participant, Competition competition)
        {
            Id = (participant.Id, competition.Id);
            _participant = participant;
            _competition = competition;
        }

        public Competition Competition
        {
            get => _competition;
            set
            {
                Id = (_participant.Id, value.Id);
                _competition = value;
            }
        }
        
        public Participant Participant
        {
            get => _participant;
            set
            {
                Id = (value.Id, _competition.Id);
                _participant = value;
            }
        }

        public void Deconstruct(out Participant participant, out Competition competition)
        {
            (participant, competition) = (Participant, Competition);
        }
    }
}