using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortfolioApi.Models
{
    public class ProjectItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public List<string> TechStack { get; set; } = new();

        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        [StringLength(500)]
        public string Image { get; set; } = string.Empty;
    }
}