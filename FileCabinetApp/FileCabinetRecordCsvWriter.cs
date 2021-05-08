using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Class contains methods for writing records to a file.
    /// </summary>
    public class FileCabinetRecordCsvWriter
    {
        private TextWriter tw;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        /// <param name="tw">Thread.</param>
        public FileCabinetRecordCsvWriter(TextWriter tw)
        {
            this.tw = tw;
        }

        /// <summary>
        /// Method for writes to a record file.
        /// </summary>
        /// <param name="fileCabinetRecord">the record of a <see cref="FileCabinetRecord"/> type.</param>
        public void Write(FileCabinetRecord fileCabinetRecord)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"{fileCabinetRecord.Id}, ");
            builder.Append($"{fileCabinetRecord.FirstName}, ");
            builder.Append($"{fileCabinetRecord.LastName}, ");
            builder.Append($"{fileCabinetRecord.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, ");
            builder.Append($"{fileCabinetRecord.Gender}, ");
            builder.Append($"{fileCabinetRecord.NumberOfReviews}, ");
            builder.Append($"{fileCabinetRecord.Salary}");
            this.tw.WriteLine(builder.ToString());
        }
    }
}
