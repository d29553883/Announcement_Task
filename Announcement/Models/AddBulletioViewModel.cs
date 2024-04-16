using System.ComponentModel.DataAnnotations;

namespace Announcement.Models
{
    public class AddBulletioViewModel
    {
        public int Id { get; set; }
        public string Category { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public DateTime PublishDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public bool PinTop { get; set; }
        public string Content { get; set; }

        public int Sort { get; set; }
    }
}
