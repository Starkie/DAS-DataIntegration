﻿namespace DataLoader
{
    using System;
    using System.Globalization;
    using System.IO;
    using CsvHelper;
    using DataLoader.Entities;
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

            csvReader.Configuration.HasHeaderRecord = true;
            csvReader.Configuration.AllowComments = true;

            while (csvReader.Read())
            {
                string id = csvReader.GetField(0);
                Gender gender = ParseGender(csvReader.GetField(1));
                bool ageParseSucceeded = !int.TryParse(csvReader.GetField(2), out int age);
                string country = csvReader.GetField(3);
                DateTime registrationDate = DateTime.ParseExact(csvReader.GetField(4), "MMM d, yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

                User user = new User
                {
                    Id = id,
                    Gender = gender,
                    Age = ageParseSucceeded && age != 0 ? (int?)age : null,
                    Country = country,
                    RegistrationDate = registrationDate,
                };
            }
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