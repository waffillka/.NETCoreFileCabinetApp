using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetRecordCsvWriter
    {
        private TextWriter tw;

        public FileCabinetRecordCsvWriter(TextWriter tw)
        {
            this.tw = tw;
        }

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
