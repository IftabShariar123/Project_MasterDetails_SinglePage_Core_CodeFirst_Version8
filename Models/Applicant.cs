using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Evidence_MDetails_SinglePage_Core_CF.Models
{
    public partial class Applicant
    {
        public Applicant()
        {
            Experience = new List<Experience>();
        }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public int? Age { get; set; }
        public string Qualification { get; set; }
        public int? TotalExperience { get; set; }
        public string PhotoUrl { get; set; }


        [NotMapped]
        public IFormFile ProfilePhoto { get; set; }
        public virtual List<Experience> Experience { get; set; }
    }
}
