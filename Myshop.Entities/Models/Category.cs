using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Myshop.Entities.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Name is required")]
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;
    }
}
