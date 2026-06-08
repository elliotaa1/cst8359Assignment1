namespace Assignment1_EA.Models
{
    //Event class to create events.
    public class Event
    {
        //Unique ID for each event
        public int id { get; set; }
        //Event title
        public string title { get; set; }
        //Event date and time
        public DateTime date { get; set; }
        //Event location
        public string location { get; set; }
        //Store list of registered attendees per event
        public List<UserHandler> Attendees { get; set; } = new List<UserHandler>();

    }
}
