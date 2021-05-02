using System;
using System.Collections.Generic;
using System.Linq;

namespace FileCabinetApp
{
    /// <summary>
    /// contains services for adding, editing, and modifying records.
    /// </summary>
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new (StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new (StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateofbirthDictionary = new ();

        /// <summary>
        /// Create object of record.
        /// </summary>
        /// <param name="firstName">firstName is object field.</param>
        /// <param name="lastName">lastName is object field.</param>
        /// <param name="dateOfBirth">dateOfBirth is object field.</param>
        /// <param name="gender">gender is object field.</param>
        /// <param name="numberOfReviews">numberOfReviews is object field.</param>
        /// <param name="salary">salary is object field.</param>
        /// <returns>id of record.</returns>
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
            this.AddInDictionaryLastName(lastName, record);
            this.list.Add(record);

            return record.Id;
        }

        /// <summary>
        /// Gets records.
        /// </summary>
        /// <returns>ReadOnlyCollection of records.</returns>
        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
        }

        /// <summary>
        /// Get statistics by records.
        /// </summary>
        /// <returns>Count of records.</returns>
        public int GetStat()
        {
            return this.list.Count;
        }

        /// <summary>
        /// Changes data an existing record.
        /// </summary>
        /// <param name="id"> id of the record to edit.</param>
        /// <param name="firstName">firstName of the record to edit.</param>
        /// <param name="lastName">lastName of the record to edit.</param>
        /// <param name="dateOfBirth">dateOfBirth of the record to edit.</param>
        /// <param name="gender">gender of the record to edit.</param>
        /// <param name="numberOfReviews">numberOfReviews of the record to edit.</param>
        /// <param name="salary">salary of the record to edit.</param>
        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, char gender, short numberOfReviews, decimal salary)
        {
            this.firstNameDictionary.Remove(firstName);
            this.dateofbirthDictionary.Remove(dateOfBirth);
            this.lastNameDictionary.Remove(lastName);
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
                this.lastNameDictionary.Add(lastName, new List<FileCabinetRecord> { record });
            }
        }

        /// <summary>
        /// Find records by FirstName.
        /// </summary>
        /// <param name="firstName">variable by which they are looking.</param>
        /// <returns>ReadOnlyCollection of records.</returns>
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

        /// <summary>
        /// Find records by LastName.
        /// </summary>
        /// <param name="lastName">variable by which they are looking.</param>
        /// <returns>ReadOnlyCollection of records.</returns>
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

        /// <summary>
        /// Find records by DateOfBirth.
        /// </summary>
        /// <param name="dateofbirth">variable by which they are looking.</param>
        /// <returns>ReadOnlyCollection of records.</returns>
        public FileCabinetRecord[] FindByDateOfBirth(string dateofbirth)
        {
            if (!DateTime.TryParse(dateofbirth, out DateTime date))
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

        /// <summary>
        /// Add record in the dictionary.
        /// </summary>
        /// <param name="firstName">key in the dictionary.</param>
        /// <param name="record">Object of record.</param>
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

        /// <summary>
        /// Add record in the dictionary.
        /// </summary>
        /// <param name="lastName">key in the dictionary.</param>
        /// <param name="record">Object of record.</param>
        public void AddInDictionaryLastName(string lastName, FileCabinetRecord record)
        {
            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                this.lastNameDictionary[lastName].Add(record);
            }
            else
            {
                this.lastNameDictionary.Add(lastName, new List<FileCabinetRecord> { record });
            }
        }

        /// <summary>
        /// Add record in the dictionary.
        /// </summary>
        /// <param name="dateofbirth">key in the dictionary.</param>
        /// <param name="record">Object of record.</param>
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
