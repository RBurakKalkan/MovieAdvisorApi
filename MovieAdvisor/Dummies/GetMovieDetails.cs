

namespace MovieAdvisor.Dummies
{
    public class GetMovieDetails
    {
        public int MovieId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Average { get; set; }
        public Ratings[] Ratings { get; set; }
        public Comments[] Comments { get; set; }
    }
    public class Comments 
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Comment { get; set; }
    }
    public class Ratings 
    {
        public int MovieId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Rating { get; set; }
    }
}
