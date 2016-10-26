using System;
using System.ComponentModel.DataAnnotations;

namespace GoAgile.Models
{
    public class CreateRetrospectiveViewModel
    {
        [Required]
        [Display(Name = "Retrospective Name")]
        [StringLength(maximumLength: 30)]
        public string RetrospectiveName { get; set; }

        [Required]
        [StringLength(maximumLength: 30)]
        public string Project { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [StringLength(maximumLength: 255)]
        public string Comment { get; set; }
    }

    public class RetrospectiveViewModel
    {
        public string GuidId { get; set; }

        public int State { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
    }
}