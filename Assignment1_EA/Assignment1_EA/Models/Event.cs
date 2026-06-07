namespace Assignment1_EA.Models
{
    public class Event
    {
        public int id { get; set; }
        public string title { get; set; }
        public DateTime date { get; set; }

        public string location { get; set; }

        public List<UserHandler> Attendees { get; set; } = new List<UserHandler>();

    }
}
