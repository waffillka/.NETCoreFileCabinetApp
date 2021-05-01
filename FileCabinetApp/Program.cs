using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FileCabinetApp
{
    public static class Program
    {
        private const string DeveloperName = "George Sebik";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static int id;
        private static string firstName;
        private static string lastName;
        private static DateTime dateOfBirth;
        private static char gender;
        private static short numberOfReviews;
        private static decimal salary;

        private static bool isRunning = true;

        private static FileCabinetService fileCabinetService = new FileCabinetService();

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
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

        public static void Main(string[] args)
        {
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

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
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
            UserData();

            int id = Program.fileCabinetService.CreateRecord(firstName, lastName, dateOfBirth, gender, numberOfReviews, salary);
            Console.WriteLine($"Record #{id} is created.");
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
                if (!int.TryParse(parameters, out id))
                {
                    throw new ArgumentException("Invalid input");
                }

                if (id > Program.fileCabinetService.GetStat() || id < 1)
                {
                    throw new ArgumentException($"#{parameters} record in not found. ");
                }

                Program.UserData();
                Program.fileCabinetService.EditRecord(id, firstName, lastName, dateOfBirth, gender, numberOfReviews, salary);
                Console.WriteLine($"Record #{parameters} is updated.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void UserData()
        {
            do
            {
                Console.Write("First name: ");
                firstName = Console.ReadLine();
            }
            while (firstName.Trim().Length < 2 || firstName.Trim().Length > 60);

            do
            {
                Console.Write("Last name: ");
                lastName = Console.ReadLine();
            }
            while (lastName.Trim().Length is < 2 or > 60);

            bool w;
            do
            {
                Console.Write("Date of birth: ");
                w = DateTime.TryParse(Console.ReadLine(), out dateOfBirth);
            }
            while (!w || (dateOfBirth > DateTime.Now || dateOfBirth < new DateTime(1950, 1, 1)));

            do
            {
                Console.Write("Gender (M/W): ");
                gender = char.ToUpper(Console.ReadKey().KeyChar, new CultureInfo("en-US"));
            }
            while (gender != 'M' && gender != 'W');

            do
            {
                Console.Write("\nNumber of reviews: ");
                w = short.TryParse(Console.ReadLine(), out numberOfReviews);
            }
            while (!w || numberOfReviews < 0);

            do
            {
                Console.Write("Salary: ");
                w = decimal.TryParse(Console.ReadLine(), out salary);
            }
            while (!w || salary < 0);
        }
    }
}