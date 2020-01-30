namespace DataLoader.Entities
{
    public class UserPlaysCsv
    {
        public uint Id { get; set; }

        public string UserId { get; set; }

        public string ArtistId { get; set; }

        public string ArtistName { get; set; }

        public long Plays { get; set; }
    }
}