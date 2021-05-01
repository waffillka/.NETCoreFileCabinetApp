using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

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
            this.list[id - 1].FirstName = firstName;
            this.list[id - 1].LastName = lastName;
            this.list[id - 1].DateOfBirth = dateOfBirth;
            this.list[id - 1].Gender = gender;
            this.list[id - 1].NumberOfReviews = numberOfReviews;
            this.list[id - 1].Salary = salary;
        }

        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            var listFileCabinetRecord = new List<FileCabinetRecord>();

            foreach (var temp in this.list)
            {
                if (temp.FirstName == firstName)
                {
                    listFileCabinetRecord.Add(temp);
                }
            }

            return listFileCabinetRecord.ToArray();
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

            var listFileCabinetRecord = new List<FileCabinetRecord>();

            foreach (var temp in this.list)
            {
                if (temp.DateOfBirth == date)
                {
                    listFileCabinetRecord.Add(temp);
                }
            }

            return listFileCabinetRecord.ToArray();
        }
    }
}
