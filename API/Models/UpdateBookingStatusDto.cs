using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class UpdateBookingStatusDto
    {
        [Required]
        public string Action { get; set; }
    }
}
