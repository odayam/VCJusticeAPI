namespace VCJusticeAPI.Models
{
    public class Participant
    {
        public string connectionId { get; set; }
        public User user { get; set; }
        public bool isCurrent { get; set; }


    }
}
