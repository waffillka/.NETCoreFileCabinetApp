﻿using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// A class that is intended for processing user data and further processing it.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "George Sebik";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static FileCabinetServiceContext fileCabinetServiceContext = new FileCabinetServiceContext();
        private static bool isRunning = true;

        private static FileCabinetService fileCabinetService = new (new DefaultValidator());

        private static readonly Tuple<string, Action<string>>[] Commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints statistics", "The 'stat' command prints statistics." },
            new string[] { "create", "creating a new record in the filing cabinet", "The 'create' command creating a new record in the filing cabinet." },
            new string[] { "list", "prints records", "The 'list' command prints records." },
            new string[] { "edit", "edits a record", "The 'edit' command edits a record." },
            new string[] { "find", "find record by a known value", "The 'find' command find record by a known value" },
        };

        private static bool isCorrect;

        /// <summary>
        /// Point of entry.
        /// </summary>
        public static void Main(string[] args)
        {
            Console.Write("Validations rules: ");
            var validationsRules = Console.ReadLine().Trim(' ').Split(new char[] { ' ', '=' }, StringSplitOptions.RemoveEmptyEntries);
            var longDescription = "--validation-rules";
            var shortDescription = "-v";
            if (validationsRules.Length == 0)
            {
                Console.WriteLine("Using default validation rules.");
            }
            else if (string.Compare(validationsRules[0], longDescription, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(validationsRules[0], shortDescription, StringComparison.OrdinalIgnoreCase) == 0)
            {
                string parameter = "custom";
                if (string.Equals(validationsRules[1], parameter, StringComparison.OrdinalIgnoreCase))
                {
                    fileCabinetService = new FileCabinetService(new CustomValidator());
                    Console.WriteLine("Using custom validation rules.");
                }
                else
                {
                    Console.WriteLine("Using default validation rules.");
                }
            }

            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(Commands, 0, Commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    Commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            string repeatIfDataIsNotCorrect = parameters;
            try
            {
                UserData();
                int id = Program.fileCabinetService.CreateRecord(fileCabinetServiceContext);
                Console.WriteLine($"Record #{id} is created.");
            }
            catch (Exception ex) when (ex is ArgumentException || ex is FormatException || ex is OverflowException || ex is ArgumentNullException)
            {
                Console.WriteLine(ex.Message);
                isCorrect = false;
            }
            finally
            {
                if (!isCorrect)
                {
                    Console.WriteLine("Your data is incorrect, please try again");
                    isCorrect = true;
                    Create(repeatIfDataIsNotCorrect);
                }
            }
        }

        private static void List(string parameters)
        {
            FileCabinetRecord[] listRecords = Program.fileCabinetService.GetRecords();

            foreach (FileCabinetRecord record in listRecords)
            {
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth.ToString("yyy-MMM-dd", CultureInfo.InvariantCulture)}" +
                    $", {record.Gender}, {record.NumberOfReviews}, {record.Salary}");
            }
        }

        private static void Find(string parameters)
        {
            try
            {
                string parameterValue = parameters.Split(' ').Last().Trim('"');
                string[] parameterArray = parameters.Split(' ');
                var parameterName = parameterArray[^2];
                switch (parameterName.ToLower(CultureInfo.CurrentCulture))
                {
                    case "firstname":
                        ListRecord(Program.fileCabinetService.FindByFirstName(parameterValue));
                        break;
                    case "lastname":
                        ListRecord(Program.fileCabinetService.FindByLastName(parameterValue));
                        break;
                    case "dateofbirth":
                        ListRecord(Program.fileCabinetService.FindByDateOfBirth(parameterValue));
                        break;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ArgumentException argEx)
            {
                Console.WriteLine(argEx.Message);
            }
        }

        private static void ListRecord(FileCabinetRecord[] listRecordsInService)
        {
            for (int i = 0; i < listRecordsInService.Length; i++)
            {
                var builder = new StringBuilder();
                builder.Append($"{listRecordsInService[i].Id}, ");
                builder.Append($"{listRecordsInService[i].FirstName}, ");
                builder.Append($"{listRecordsInService[i].LastName}, ");
                builder.Append($"{listRecordsInService[i].DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, ");
                builder.Append($"{listRecordsInService[i].Gender}, ");
                builder.Append($"{listRecordsInService[i].NumberOfReviews}, ");
                builder.Append($"{listRecordsInService[i].Salary}");
                Console.WriteLine("#" + builder.ToString());
            }
        }

        private static void Edit(string parameters)
        {
            try
            {
                int getNumberEditRecord = int.Parse(parameters, CultureInfo.CurrentCulture);
                if (getNumberEditRecord > Program.fileCabinetService.GetStat() || getNumberEditRecord < 1)
                {
                    throw new ArgumentException($"#{getNumberEditRecord} record in not found. ");
                }

                Program.UserData();
                Program.fileCabinetService.EditRecord(getNumberEditRecord, fileCabinetServiceContext);
                Console.WriteLine($"Record #{parameters} is updated.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void UserData()
        {
            Console.Write("First name: ");
            fileCabinetServiceContext.FirstName = ReadInput(StringConverter, FirstNameValidator);
            Console.Write("Last Name: ");
            fileCabinetServiceContext.LastName = ReadInput(StringConverter, LastNameValidator);
            Console.Write("Date of birth: ");
            fileCabinetServiceContext.DateOfBirth = ReadInput(DateOfBirthConverter, DateOfBirthValidator);
            Console.Write("Gender (M/W): ");
            fileCabinetServiceContext.Gender = ReadInput(GenderConverter, GenderValidator);
            Console.Write("Number of reviews: ");
            fileCabinetServiceContext.NumberOfReviews = ReadInput(NumberOfReviewsConverter, NumberOfReviewsValidator);
            Console.Write("Salary: ");
            fileCabinetServiceContext.Salary = ReadInput(SalaryConverter, SalaryValidator);
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }

        private static Tuple<bool, string> FirstNameValidator(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return new Tuple<bool, string>(false, "Empty string");
            }
            else if (!char.IsLetter(val, val.Length - 1))
            {
                return new Tuple<bool, string>(false, "Check input FirstName");
            }
            else
            {
                return new Tuple<bool, string>(true, val);
            }
        }

        private static Tuple<bool, string, string> StringConverter(string val)
        {
            return new Tuple<bool, string, string>(true, val, val);
        }

        private static Tuple<bool, string> LastNameValidator(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return new Tuple<bool, string>(false, "Empty string");
            }
            else if (!char.IsLetter(val, val.Length - 1))
            {
                return new Tuple<bool, string>(false, "Check input LastName");
            }
            else
            {
                return new Tuple<bool, string>(true, val);
            }
        }

        private static Tuple<bool, string> DateOfBirthValidator(DateTime val)
        {
            if (val == DateTime.MinValue)
            {
                return new Tuple<bool, string>(false, "Сheck input DateOfBirth");
            }
            else
            {
                return new Tuple<bool, string>(true, string.Empty);
            }
        }

        private static Tuple<bool, string, DateTime> DateOfBirthConverter(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return new Tuple<bool, string, DateTime>(false, "Empty field", DateTime.MinValue);
            }

            if (DateTime.TryParse(val, out DateTime result))
            {
                return new Tuple<bool, string, DateTime>(true, val, result);
            }
            else
            {
                return new Tuple<bool, string, DateTime>(false, val, result);
            }
        }

        private static Tuple<bool, string> GenderValidator(char val)
        {
            if (val == char.MaxValue)
            {
                return new Tuple<bool, string>(false, "Empty string");
            }
            else
            {
                return new Tuple<bool, string>(true, string.Empty);
            }
        }

        private static Tuple<bool, string, char> GenderConverter(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return new Tuple<bool, string, char>(false, "Empty field", char.MinValue);
            }

            if (char.TryParse(val, out char result))
            {
                return new Tuple<bool, string, char>(true, val, result);
            }
            else
            {
                return new Tuple<bool, string, char>(false, "The symbol is not of the char type", char.MinValue);
            }
        }

        private static Tuple<bool, string> NumberOfReviewsValidator(short val)
        {
            if (val <= 0)
            {
                return new Tuple<bool, string>(false, "Number Of Reviews can't be negative");
            }
            else
            {
                return new Tuple<bool, string>(true, string.Empty);
            }
        }

        private static Tuple<bool, string, short> NumberOfReviewsConverter(string val)
        {
            if (short.TryParse(val, out short result))
            {
                return new Tuple<bool, string, short>(true, val, result);
            }
            else
            {
                return new Tuple<bool, string, short>(false, val, 0);
            }
        }

        private static Tuple<bool, string, decimal> SalaryConverter(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return new Tuple<bool, string, decimal>(false, "Empty string", 0);
            }
            else if (decimal.TryParse(val, out decimal result))
            {
                return new Tuple<bool, string, decimal>(true, val, result);
            }
            else
            {
                return new Tuple<bool, string, decimal>(false, val, 0);
            }
        }

        private static Tuple<bool, string> SalaryValidator(decimal val)
        {
            if (val <= 0)
            {
                return new Tuple<bool, string>(false, "Salary can't be negative");
            }
            else
            {
                return new Tuple<bool, string>(true, string.Empty);
            }
        }
    }
}