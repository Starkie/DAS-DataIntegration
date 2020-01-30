namespace DataLoader
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using CsvHelper;
    using DataLoader.Entities;
    using DataLoader.Persistence;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.EntityFrameworkCore;

    class Program
    {
        static int Main(string[] args)
        {
            CommandLineApplication app = new CommandLineApplication();
            app.HelpOption();

            CommandArgument usersFileOption = app.Argument("-u|--usersFile", "The path to the CSV file containing the user data.");
            CommandArgument userArtistPlaysFileOption = app.Argument("-p|--userArtistPlaysFile", "The path to the CSV file containing the user - artist plays data.");

            app.OnExecute(() =>
            {
                LoadUsersToDatabase(usersFileOption.Value);
                LoadUserArtistsPlaysCsvToDatabase(userArtistPlaysFileOption.Value);
            });

            return app.Execute(args);
        }

        private static void LoadUsersToDatabase(string usersFilePath)
        {
            using StreamReader streamReader = new StreamReader(usersFilePath);
            using CsvReader csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
            using DataLoaderContext context = new DataLoaderContext();
            context.Database.EnsureCreated();

            csvReader.Configuration.HasHeaderRecord = true;
            csvReader.Configuration.AllowComments = true;
            csvReader.Configuration.Delimiter = "\t";

            while (csvReader.Read())
            {
                string id = csvReader.GetField(0);
                Gender gender = ParseGender(csvReader.GetField(1));
                bool ageParseSucceeded = int.TryParse(csvReader.GetField(2), out int age);
                string country = csvReader.GetField(3);
                bool registrationDateParseSuccess = DateTime.TryParseExact(csvReader.GetField(4), "MMM d, yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime registrationDate);

                User user = new User
                {
                    Id = id,
                    Gender = gender,
                    Age = ageParseSucceeded && age != 0 ? age : (int?)null,
                    Country = country,
                    RegistrationDate = registrationDate,
                };

                Console.WriteLine($"READ: {user.Id} - Gender: {user.Gender} Age: {user.Age} Country: {user.Country} RegistrationDate: {user.RegistrationDate}");

                context.Users.Add(user);
            }

            context.SaveChanges();
        }

        private static void LoadUserArtistsPlaysCsvToDatabase(string userArtistPlaysFilePath)
        {
            using StreamReader streamReader = new StreamReader(userArtistPlaysFilePath);
            using CsvReader csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
            using DataLoaderContext context = new DataLoaderContext();
            context.Database.EnsureCreated();

            csvReader.Configuration.HasHeaderRecord = true;
            csvReader.Configuration.AllowComments = true;
            csvReader.Configuration.Delimiter = "\t";

            csvReader.Configuration.BadDataFound = (context) =>
            {
                Console.WriteLine($"BAD RECORD - {context.RawRecord}");
            };

            uint i = 0, createdRegistries = 0;
            User user = context.Users.FirstOrDefault();

            while (csvReader.Read())
            {
                // Increase the read registiries counter.
                i++;

                string userId = csvReader.GetField(0);
                long.TryParse(csvReader.GetField(3), out long userArtistPlaysNumber);
                string artistId = csvReader.GetField(1);
                string artistName = csvReader.GetField(2);

                UserPlaysCsv userPlaysCsv = new UserPlaysCsv
                {
                    Id = i,
                    UserId = userId,
                    ArtistId = artistId,
                    ArtistName = artistName,
                    Plays = userArtistPlaysNumber,
                };

                // Console.WriteLine($"READ: {userArtistPlays.User.Id} - ArtistName: {userArtistPlays.Artist.Name} Plays: {userArtistPlays.PlaysNumber}");

                context.UserPlaysCsvs.Add(userPlaysCsv);

                createdRegistries++;

                if (createdRegistries % 10000 == 0)
                {
                    Console.WriteLine($"Saving Changes... {(i / 17559530.0) * 100:0.00}%  {i}/{17559530.0} rows. ");
                    context.SaveChanges();
                }
            }

            context.SaveChanges();
        }

        public static Gender ParseGender(string genderValue)
        {
            if (genderValue.Equals("m", StringComparison.InvariantCultureIgnoreCase))
            {
                return Gender.MALE;
            }

            if (genderValue.Equals("f", StringComparison.InvariantCultureIgnoreCase))
            {
                return Gender.FEMALE;
            }

            return Gender.UNKNOWN;
        }
    }
}