using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace GoAgile.Models
{
    public class CreateRetrospectiveViewModel : IValidatableObject
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
        public int? MaxVotes { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var pPDate = new[] { "DatePlanned" };
            if (DatePlanned != null && DatePlanned < DateTime.Now.AddDays(-1))
            {
                yield return new ValidationResult("Past Date cannot be accepted.", pPDate);
            }

            var pMaxVotes = new[] { "MaxVotes" };
            if (EnableVotes && ( MaxVotes == null || MaxVotes <= 0 ) )
            {
                yield return new ValidationResult("Maximum Votes is required when Voting is enabled", pMaxVotes);
            }
        }
    }
}