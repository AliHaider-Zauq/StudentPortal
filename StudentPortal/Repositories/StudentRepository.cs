using StudentPortal.Data;
using StudentPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace StudentPortal.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly StudentDbContext _context;

        public StudentRepository(StudentDbContext context)
        {
            _context = context;
        }

        public void Add(Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
        }

        public Student ViewDetails(int id)
        {
            return _context.Students.Find(id);
        }

        public void Delete(int id)
        {
            var student = _context.Students.Find(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                _context.SaveChanges();
            }
        }

        
        public List<Student> GetAll()
        {
            return _context.Students.ToList();
        }

        public Student GetStudentById(int id)
        {
            return _context.Students.FirstOrDefault(s => s.Id == id);
        }

        public void Update(Student student)
        {
            _context.Students.Update(student);
            _context.SaveChanges();
        }

        public void SaveChanges() // SaveChanges کو implement کریں
        {
            _context.SaveChanges();
        }

        public List<Student> GetStudentBySemesterTerm(string semesterTerm)
        {
          return  _context.Students.Where(S => S.SemesterTerm == semesterTerm).ToList();
        }

    }
}