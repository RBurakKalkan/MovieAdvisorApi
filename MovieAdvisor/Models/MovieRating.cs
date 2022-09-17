using System;
namespace MovieAdvisor.Models
{
    public class MovieRating
    {
        public int MovieRatingId { get; set; }
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime RecordDate { get; set; }
    }
}
