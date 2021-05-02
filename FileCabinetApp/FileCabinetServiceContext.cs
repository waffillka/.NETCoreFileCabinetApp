using System;

namespace FileCabinetApp
{
    /// <summary>
    /// this class is intended to be passed as an object.
    /// </summary>
    public class FileCabinetServiceContext
    {
        /// <summary>Gets or sets the first name.</summary>
        /// <value>First name.</value>
        public string FirstName { get; set; }

        /// <summary>Gets or sets the last name.</summary>
        /// <value>Last name.</value>
        public string LastName { get; set; }

        /// <summary>Gets or sets the date of birth.</summary>
        /// <value>Date of birth.</value>
        public DateTime DateOfBirth { get; set; }

        /// <summary>Gets or sets the sex.</summary>
        /// <value>Gender.</value>
        public char Gender { get; set; }

        /// <summary>Gets or sets the number of reviews.</summary>
        /// <value>Number of reviews.</value>
        public short NumberOfReviews { get; set; }

        /// <summary>Gets or sets the salary.</summary>
        /// <value>Salary.</value>
        public decimal Salary { get; set; }
    }
}
