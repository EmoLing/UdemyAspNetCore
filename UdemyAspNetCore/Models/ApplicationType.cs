using System.ComponentModel.DataAnnotations;

namespace UdemyAspNetCore.Models
{
    public class ApplicationType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
