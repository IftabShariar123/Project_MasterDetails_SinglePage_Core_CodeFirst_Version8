using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Evidence_MDetails_SinglePage_Core_CF.Models
{
    public partial class Experience
    {
        public int ExperienceId { get; set; }
        public int? ApplicantId { get; set; }
        public string CompanyName { get; set; }
        public string Designation { get; set; }
        public int? YearsWorked { get; set; }

        public virtual Applicant Applicant { get; set; }
    }
}
