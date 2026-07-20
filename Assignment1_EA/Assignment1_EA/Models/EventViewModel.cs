using Microsoft.AspNetCore.Http;

namespace Assignment1_EA.Models.ViewModels
{
    public class EventViewModel
    {
        public int id { get; set; }

        public string? title { get; set; }

        public string? description { get; set; }

        public DateTime date { get; set; }

        public string? location { get; set; }


        // This is only for uploading
        public IFormFile? BannerImage { get; set; }
    }
}