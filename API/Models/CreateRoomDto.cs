using System.ComponentModel.DataAnnotations;

namespace API.Models;

public class CreateRoomDto
{
    [Required]
    public string Name { get; set; } = "";

    [Required]
    public int Capacity { get; set; }

    [Required]
    public string RoomType { get; set; } = "";
}
