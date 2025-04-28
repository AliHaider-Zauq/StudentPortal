using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Data;
using StudentPortal.Models;
using StudentPortal.Repositories;
using StudentPortal.ViewModel;

namespace StudentPortal.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly IStudentRepository _studentRepository;

       public StudentController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var students = _studentRepository.GetAll();
            return View(students);
        }


        [HttpGet]
        [Authorize(Roles ="Admin, HOD")]
        [Authorize(Policy = "CanCreateStudent")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(StudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                string? uniqueFileName = null;

                // Image upload logic
                if (model.ImagePath != null)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImagePath.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.ImagePath.CopyTo(fileStream);
                    }

                }

                // Convert ViewModel to Model
                var student = new Student
                {
                    Name = model.Name,
                    Gmail = model.Gmail,
                    SemesterTerm = model.SemesterTerm,
                    ImagePath = uniqueFileName != null ? uniqueFileName : null // Store relative path
                };

                _studentRepository.Add(student); // Add to database
                _studentRepository.SaveChanges(); // Save changes

                return RedirectToAction("Index", "student"); 
            }

            
            return View(model);
        }


        public IActionResult ViewDetails(int id)
        {
            var student = _studentRepository.ViewDetails(id);   
            return View(new List<Student> { student }); 
        }

        [Authorize(Roles ="Admin, HOD")]
        [Authorize(Policy ="example")]
        public IActionResult Delete(int id)
        {
            _studentRepository.Delete(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles ="Admin, Teacher, HOD")]
        [Authorize(Policy = "CanEditRoles")]
        public IActionResult Edit(int id)
        {
            var student = _studentRepository.GetStudentById(id);
            if (student == null)
            {
                return NotFound();
            }

            var model = new StudentViewModel
            {
                Id = student.Id,
                Name = student.Name,
                Gmail = student.Gmail,
                SemesterTerm = student.SemesterTerm,
                ImagePath = null 
            };

            ViewBag.ExistingImage = student.ImagePath;
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(StudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var student = _studentRepository.GetStudentById(model.Id);
                if (student == null)
                {
                    return NotFound();
                }

                // Keep existing image unless a new one is uploaded
                string uniqueFileName = student.ImagePath;

                if (model.ImagePath != null)
                {
                    string uploadsFolder = Path.Combine("wwwroot/images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImagePath.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.ImagePath.CopyTo(fileStream);
                    }

                    // Delete old image if it exists
                    if (!string.IsNullOrEmpty(student.ImagePath))
                    {
                        string oldFilePath = Path.Combine("wwwroot/images", student.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    student.ImagePath = "/images/" + uniqueFileName;
                }

                // Update other details
                student.Name = model.Name;
                student.Gmail = model.Gmail;
                student.SemesterTerm = model.SemesterTerm;

                _studentRepository.Update(student);
                _studentRepository.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.ExistingImage = _studentRepository.GetStudentById(model.Id)?.ImagePath; // Reload existing image
            return View(model);
        }
        //ACCESS DENIED PAGE
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}