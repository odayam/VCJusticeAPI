namespace VCJusticeAPI.Models
{
    public class User
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string password { get; set; }
        public bool isAdmin { get; set; }
        public string jobTitle { get; set; }
        public string username { get; set; }
        public string connectionId { get; set; }
    }
}
