namespace DataLoader
{
    using System;
    using System.Collections.Generic;
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
                // 1. Load the User entities to the database.
                LoadUsersToDatabase(usersFileOption.Value);

                // 2. Load the UserArtistPlaysCsv to the database, without any FK restriction,
                // so it goes faster.
                LoadUserArtistsPlaysCsvToDatabase(userArtistPlaysFileOption.Value);

                // 3. From the UserArtistPlaysCsv, read the artists and create them.
                LoadArtists();

                // 4. Using the created Users and Artists, load the User Plays registries to the database.
                LoadUserPlays();
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

        private static Gender ParseGender(string genderValue)
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

        private static void LoadArtists()
        {
            using DataLoaderContext context = new DataLoaderContext();

            int i = 0, createdRegistries = 0;

            IQueryable<UserPlaysCsv> userPlaysCsvs = context.UserPlaysCsvs.AsNoTracking();

            var uniqueArtistRecords = userPlaysCsvs
                .Where(a => a.ArtistName != null)
                .AsEnumerable()
                .GroupBy(upc => upc.ArtistId);

            foreach (var uar in uniqueArtistRecords)
            {
                // Increase the read registries counter.
                i += uar.Count();

                var artistRecord = uar.FirstOrDefault();

                Artist artist = new Artist
                {
                    Id = artistRecord.ArtistId,
                    Name = artistRecord.ArtistName,
                };

                context.Artists.Add(artist);

                createdRegistries++;

                if (createdRegistries % 10000 == 0)
                {
                    Console.WriteLine($"Saving Changes... {(i / 11500000.0) * 100:0.000}%  {i}/{11500000.0} rows. ");
                    context.SaveChanges();
                }
            }

            context.SaveChanges();
        }

        private static void LoadUserPlays()
        {
            using DataLoaderContext context = new DataLoaderContext();

            int i = 0, createdRegistries = 0;

            Dictionary<string, User> users = context.Users.ToDictionary(item => item.Id, item => item);
            Dictionary<string, Artist> artists = context.Artists.ToDictionary(item => item.Id, item => item);

            var userPlays = context.UserPlaysCsvs.AsNoTracking()
                .Where(upc => !string.IsNullOrEmpty(upc.UserId) && !string.IsNullOrEmpty(upc.ArtistId))
                .OrderByDescending(upc => upc.Plays)
                .AsEnumerable()
                .GroupBy(upc => upc.UserId);

            foreach (var up in userPlays)
            {
                // Increase the read registries counter.
                i += up.Count();

                // Take only the 10 most played artists.
                foreach (var userPlay in up.Take(10))
                {
                    context.UserArtistPlays.Add(new UserArtistPlays
                    {
                        User = users[userPlay.UserId],
                            Artist = artists[userPlay.ArtistId],
                            PlaysNumber = userPlay.Plays,
                    });

                    createdRegistries++;
                }

                if (createdRegistries % 1000 == 0)
                {
                    context.SaveChanges();

                    Console.WriteLine($"Saving Changes... {(i / 11500000.0) * 100:0.000}%  {i}/{11500000.0} rows. ");
                }
            }

            context.SaveChanges();
        }
    }
}