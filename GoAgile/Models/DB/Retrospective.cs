using System;
using System.ComponentModel.DataAnnotations;

namespace GoAgile.Models.DB
{
    public class Retrospective
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Owner { get; set; }

        [Required]       
        [StringLength(255)]
        public string RetrospectiveName { get; set; }

        [Required]
        [StringLength(255)]
        public string Project { get; set; }

        public DateTime? DatePlanned { get; set; }

        public DateTime? DateStared { get; set; }

        public DateTime? DateFinished { get; set; }   

        [StringLength(255)]
        public string Comment { get; set; }

        public EventState State { get; set; }
    }
}