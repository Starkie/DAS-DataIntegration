using System.ComponentModel.DataAnnotations;

namespace DataLoader.Entities
{
    public class UserArtistPlays
    {
        public int Id { get; set; }

        [Required]
        public User User { get; set; }

        [Required]
        public Artist Artist { get; set; }

        [Required]
        public long PlaysNumber { get; set; }
    }
}