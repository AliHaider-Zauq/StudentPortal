using StudentPortal.Models;

namespace StudentPortal.Repositories
{
    public interface IStudentRepository
    {
        void Add(Student student);
        Student ViewDetails(int id);
        void Delete(int id);
        List<Student> GetAll();
        Student GetStudentById(int id);
        void Update(Student student);
        void SaveChanges();

        }

    }
