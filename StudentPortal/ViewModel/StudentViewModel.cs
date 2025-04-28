namespace StudentPortal.ViewModel
{
    public class StudentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Gmail { get; set; }
        public string SemesterTerm { get; set; }
        public IFormFile ImagePath { get; set; }
    }
}

