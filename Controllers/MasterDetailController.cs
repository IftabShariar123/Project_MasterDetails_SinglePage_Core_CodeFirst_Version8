using Evidence_MDetails_SinglePage_Core_CF.Data;
using Evidence_MDetails_SinglePage_Core_CF.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Evidence_MDetails_SinglePage_Core_CF.Controllers
{
    public class MasterDetailController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHost;

        public MasterDetailController(ApplicationDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }

        public IActionResult Index()
        {
            List<Applicant> applicants;
            applicants = _context.Applicant.ToList();
            return View(applicants);
        }




        [HttpGet]
        public IActionResult Create()
        {
            Applicant applicant = new Applicant();
            applicant.Experience.Add(new Experience() { ExperienceId = 1 });
            return View(applicant);
        }


        [HttpPost]
        public IActionResult Create(Applicant applicant)
        {
            // Filter out empty experience rows
            applicant.Experience = applicant.Experience
                .Where(e => !string.IsNullOrEmpty(e.CompanyName) &&
                           !string.IsNullOrEmpty(e.Designation) &&
                           e.YearsWorked.HasValue)
                .ToList();

            string uniqueFileName = GetUploadedFileName(applicant);
            applicant.PhotoUrl = uniqueFileName;

            _context.Add(applicant);
            _context.SaveChanges();
            return RedirectToAction("index");
        }




        private string GetUploadedFileName(Applicant applicant)
        {
            string uniqueFileName = null;
            if (applicant.ProfilePhoto != null)
            {
                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "Images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + applicant.ProfilePhoto.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    applicant.ProfilePhoto.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }



        public async Task<IActionResult> EditPopup(int id)
        {
            var applicant = await _context.Applicant
                .Include(a => a.Experience)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (applicant == null)
            {
                return NotFound();
            }

            return PartialView("_EditApplicantPartial", applicant);
        }




        [HttpPost]
        public async Task<IActionResult> EditConfirmed(Applicant applicant)
        {
            var existing = await _context.Applicant
                .Include(a => a.Experience)
                .FirstOrDefaultAsync(a => a.Id == applicant.Id);

            if (existing == null) return NotFound();

            // Basic info
            existing.Name = applicant.Name;
            existing.Date = applicant.Date;
            existing.Gender = applicant.Gender;
            existing.Age = applicant.Age;
            existing.Qualification = applicant.Qualification;
            existing.TotalExperience = applicant.TotalExperience;

            // Handle image update
            if (applicant.ProfilePhoto != null && applicant.ProfilePhoto.Length > 0)
            {
                // Generate unique filename
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(applicant.ProfilePhoto.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);

                // Save file to disk
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await applicant.ProfilePhoto.CopyToAsync(stream);
                }

                // Delete old image if exists (optional)
                if (!string.IsNullOrEmpty(existing.PhotoUrl))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", existing.PhotoUrl);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                // Save new file name
                existing.PhotoUrl = fileName;
            }

            // Replace experiences
            _context.Experience.RemoveRange(existing.Experience);
            existing.Experience = applicant.Experience;

            await _context.SaveChangesAsync();
            return Ok();
        }
        public IActionResult Details(int id)
        {
            var applicant = _context.Applicant
                .Include(a => a.Experience)
                .FirstOrDefault(a => a.Id == id);

            return PartialView("_Details", applicant);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var applicant = _context.Applicant
                .Include(a => a.Experience)
                .FirstOrDefault(a => a.Id == id);

            if (applicant != null)
            {
                _context.Applicant.Remove(applicant);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

    }
}


