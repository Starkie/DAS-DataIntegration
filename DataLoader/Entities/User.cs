namespace DataLoader.Entities
{
    using System;

    public class User
    {
        public string Id { get; set; }

        public Gender Gender { get; set; }

        public int? Age { get; set; }

        public string Country { get; set; }

        public DateTime RegistrationDate { get; set; }
    }
}