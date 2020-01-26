namespace DataLoader.Entities
{
    public class UserArtistPlays
    {
        public int Id { get; set; }

        public User User { get; set; }

        public Artist Artist { get; set; }

        public long PlaysNumber { get; set; }
    }
}