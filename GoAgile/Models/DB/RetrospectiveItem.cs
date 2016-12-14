using System.ComponentModel.DataAnnotations;

namespace GoAgile.Models.DB
{
    public class RetrospectiveItem
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Retrospective { get; set; }

        [Required]
        [StringLength(255)]
        public string UserName { get; set; }

        [Required]
        [StringLength(255)]
        public string Section { get; set; }

        [Required]
        [StringLength(255)]
        public string Text { get; set; }

        [Required]
        public int Votes { get; set; }

        [Required]
        public int Color { get; set; }
    }   
}