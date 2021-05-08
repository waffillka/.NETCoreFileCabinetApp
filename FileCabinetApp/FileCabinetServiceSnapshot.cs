using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetServiceSnapshot
    {
        private FileCabinetRecord[] records;
        private FileCabinetRecordCsvWriter writerCsv;

        public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
        {
            this.records = records;
        }

        public void SaveToCsv(StreamWriter sw)
        {
            this.writerCsv = new FileCabinetRecordCsvWriter(sw);
            foreach (var record in this.records)
            {
                this.writerCsv.Write(record);
            }
        }
    }
}
