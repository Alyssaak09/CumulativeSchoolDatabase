namespace CumulativeSchoolDatabase.Models
{
    public class Courses
    {

        public int CourseId { get; set; }

        public string? CourseCode { get; set; }

        public int TeacherId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime FinishDate { get; set; }

        public string? CourseNumber { get; set; }

    }
}
