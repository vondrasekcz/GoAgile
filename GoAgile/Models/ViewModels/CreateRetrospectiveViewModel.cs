using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace GoAgile.Models
{
    public class CreateRetrospectiveViewModel
    {
        [Required]
        [Display(Name = "Retrospective Name*")]
        [StringLength(maximumLength: 30)]
        public string RetrospectiveName { get; set; }

        [Required]
        [Display(Name = "Project*")]
        [StringLength(maximumLength: 30)]
        public string Project { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Planned Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DatePlanned { get; set; }

        [StringLength(maximumLength: 255)]
        public string Comment { get; set; }

        [Display(Name = "Enable Votes")]
        public bool EnableVotes { get; set; }

        [Display(Name = "Max Votes per person")]
        [Range(0, 20, ErrorMessage = "Votes must be between 0 and 20")]
        public int MaxVotes { get; set; }
    }
}