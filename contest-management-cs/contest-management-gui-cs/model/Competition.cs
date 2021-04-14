namespace contest_management_gui_cs.model
{
    public class Competition : Entity<long>
    {
        public string CompetitionType { get; set; }
        
        public string AgeCategory { get; set; }
    }
}