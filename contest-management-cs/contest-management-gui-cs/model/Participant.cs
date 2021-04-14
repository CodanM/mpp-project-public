namespace contest_management_gui_cs.model
{
    public class Participant : Entity<long>
    {
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public int Age { get; set; }

        // public override string ToString()
        // {
        //     return $"Participant: {{ Id: {Id}, First Name: {FirstName}, Last Name: {LastName}, Age: {Age} }}";
        // }
    }
}