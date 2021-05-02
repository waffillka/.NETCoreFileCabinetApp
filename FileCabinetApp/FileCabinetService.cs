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
        private readonly IRecordValidator contextStrategy;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetService"/> class.
        /// </summary>
        /// <param name="strategy">specific interface representative.</param>
        public FileCabinetService(IRecordValidator strategy)
        {
            this.contextStrategy = strategy;
        }

        /// <summary>
        /// creates a new records.
        /// </summary>
        /// <param name="fileCabinetServiceContext">Input FirstName, LastName, DateOfBirth, Gender, Salary, NumberOfReviews.</param>
        /// <returns>id of the new record.</returns>
        public int CreateRecord(FileCabinetServiceContext fileCabinetServiceContext)
        {
            this.contextStrategy.CheckUsersDataEntry(fileCabinetServiceContext);

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = fileCabinetServiceContext.FirstName,
                LastName = fileCabinetServiceContext.LastName,
                DateOfBirth = fileCabinetServiceContext.DateOfBirth,
                Gender = fileCabinetServiceContext.Gender,
                NumberOfReviews = fileCabinetServiceContext.NumberOfReviews,
                Salary = fileCabinetServiceContext.Salary,
            };

            this.AddInDictionaryDateOfBirth(fileCabinetServiceContext.DateOfBirth, record);
            this.AddInDictionaryFirstName(fileCabinetServiceContext.FirstName, record);
            this.AddInDictionaryLastName(fileCabinetServiceContext.LastName, record);
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
        /// changes data an existing record.
        /// </summary>
        /// <param name="id">id of the record to edit.</param>
        /// <param name="objectParameter">Input new FirstName, LastName, DateOfBirth, Gender, Salary, NumberOfReviews.</param>
        public void EditRecord(int id, FileCabinetServiceContext objectParameter)
        {
            this.contextStrategy.CheckUsersDataEntry(objectParameter);

            FileCabinetRecord oldRecord = this.list[id - 1];
            this.RemoveRecordInFirstNameDictionary(oldRecord);
            this.RemoveRecordInLastNameDictionary(oldRecord);
            this.RemoveRecordInDateOfBirthDictionary(oldRecord);
            foreach (var record in this.list.Where(x => x.Id == id))
            {
                record.Id = id;
                record.FirstName = objectParameter.FirstName;
                record.LastName = objectParameter.LastName;
                record.DateOfBirth = objectParameter.DateOfBirth;
                record.Gender = objectParameter.Gender;
                record.NumberOfReviews = objectParameter.NumberOfReviews;
                record.Salary = objectParameter.Salary;
                this.AddInDictionaryFirstName(objectParameter.FirstName, record);
                this.AddInDictionaryLastName(objectParameter.LastName, record);
                this.AddInDictionaryDateOfBirth(objectParameter.DateOfBirth, record);
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

        private void RemoveRecordInDateOfBirthDictionary(FileCabinetRecord oldRecord)
        {
            if (oldRecord is null)
            {
                throw new ArgumentNullException(nameof(oldRecord));
            }
            else if (this.dateofbirthDictionary.ContainsKey(oldRecord.DateOfBirth))
            {
                this.dateofbirthDictionary[oldRecord.DateOfBirth].Remove(oldRecord);
            }
        }

        private void RemoveRecordInLastNameDictionary(FileCabinetRecord oldRecord)
        {
            if (oldRecord is null)
            {
                throw new ArgumentNullException(nameof(oldRecord));
            }
            else if (this.lastNameDictionary.ContainsKey(oldRecord.LastName))
            {
                this.lastNameDictionary[oldRecord.LastName].Remove(oldRecord);
            }
        }

        private void RemoveRecordInFirstNameDictionary(FileCabinetRecord oldRecord)
        {
            if (oldRecord is null)
            {
                throw new ArgumentNullException(nameof(oldRecord));
            }
            else if (this.firstNameDictionary.ContainsKey(oldRecord.FirstName))
            {
                this.firstNameDictionary[oldRecord.FirstName].Remove(oldRecord);
            }
        }
    }
}
