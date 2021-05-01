using System;
using System.Collections.Generic;
using System.Linq;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateofbirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, char gender, short numberOfReviews, decimal salary)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentNullException($"{firstName} cannot be null or empty.");
            }

            if (firstName.Trim().Length < 2 || firstName.Trim().Length > 60)
            {
                throw new ArgumentException($"{nameof(firstName)} cannot be less then 2 and more then 60");
            }

            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentNullException($"{lastName} cannot be null or empty.");
            }

            if (lastName.Trim().Length < 2 || lastName.Trim().Length > 60)
            {
                throw new ArgumentException($"{nameof(lastName)} cannot be less then 2 and more then 60");
            }

            if (gender != 'M' && gender != 'W')
            {
                throw new ArgumentException($"{nameof(lastName)}should be equal to 'M' or 'W'.");
            }

            if (dateOfBirth > DateTime.Now || dateOfBirth < new DateTime(1950, 1, 1))
            {
                throw new ArgumentOutOfRangeException($"{dateOfBirth} must be between the current date and 01-Jan-1950");
            }

            if (numberOfReviews < 0)
            {
                throw new ArgumentOutOfRangeException($"{numberOfReviews} cannot be less then 0.");
            }

            if (salary < 0)
            {
                throw new ArgumentOutOfRangeException($"{salary} cannot be less then 0.");
            }

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Gender = gender,
                NumberOfReviews = numberOfReviews,
                Salary = salary,
            };

            this.AddInDictionaryDateOfBirth(dateOfBirth, record);
            this.AddInDictionaryFirstName(firstName, record);
            this.list.Add(record);

            return record.Id;
        }

        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
        }

        public int GetStat()
        {
            return this.list.Count;
        }

        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, char gender, short numberOfReviews, decimal salary)
        {
            this.firstNameDictionary.Remove(firstName);
            this.dateofbirthDictionary.Remove(dateOfBirth);
            foreach (var record in this.list.Where(x => x.Id == id))
            {
                record.Id = id;
                record.FirstName = firstName;
                record.LastName = lastName;
                record.DateOfBirth = dateOfBirth;
                record.Gender = gender;
                record.NumberOfReviews = numberOfReviews;
                record.Salary = salary;
                this.firstNameDictionary.Add(firstName, new List<FileCabinetRecord> { record });
                this.dateofbirthDictionary.Add(dateOfBirth, new List<FileCabinetRecord> { record });
            }
        }

        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                return this.firstNameDictionary[firstName].ToArray();
            }
            else
            {
                return Array.Empty<FileCabinetRecord>();
            }
        }

        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            var listFileCabinetRecord = new List<FileCabinetRecord>();

            foreach (var temp in this.list)
            {
                if (temp.LastName == lastName)
                {
                    listFileCabinetRecord.Add(temp);
                }
            }

            return listFileCabinetRecord.ToArray();
        }

        public FileCabinetRecord[] FindByDateOfBirth(string dateofbirth)
        {
            DateTime date;
            if (!DateTime.TryParse(dateofbirth, out date))
            {
                throw new ArgumentException("Invalid string with date");
            }

            if (this.dateofbirthDictionary.ContainsKey(date))
            {
                return this.dateofbirthDictionary[date].ToArray();
            }
            else
            {
                return Array.Empty<FileCabinetRecord>();
            }
        }

        public void AddInDictionaryFirstName(string firstName, FileCabinetRecord record)
        {
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                this.firstNameDictionary[firstName].Add(record);
            }
            else
            {
                this.firstNameDictionary.Add(firstName, new List<FileCabinetRecord> { record });
            }
        }

        public void AddInDictionaryDateOfBirth(DateTime dateofbirth, FileCabinetRecord record)
        {
            if (this.dateofbirthDictionary.ContainsKey(dateofbirth))
            {
                this.dateofbirthDictionary[dateofbirth].Add(record);
            }
            else
            {
                this.dateofbirthDictionary.Add(dateofbirth, new List<FileCabinetRecord> { record });
            }
        }
    }
}
