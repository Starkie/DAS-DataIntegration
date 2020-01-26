namespace DataLoader
{
    using System;
    using System.Globalization;
    using System.IO;
    using CsvHelper;
    using DataLoader.Entities;
    using DataLoader.Persistence;
    using McMaster.Extensions.CommandLineUtils;

    class Program
    {
        static int Main(string[] args)
        {
            CommandLineApplication app = new CommandLineApplication();
            app.HelpOption();

            CommandArgument usersFileOption = app.Argument("-u|--usersFile", "The path to the CSV file containing the user data.");

            app.OnExecute(() =>
            {
                LoadCsvToDatabase(usersFileOption.Value);
            });

            return app.Execute(args);
        }

        private static void LoadCsvToDatabase(string usersFilePath)
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