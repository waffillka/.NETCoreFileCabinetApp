using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// Class contains a method for writing data type of <see cref="FileCabinetRecord"/> in the format CSV and XML.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private FileCabinetRecord[] records;
        private FileCabinetRecordCsvWriter writerCsv;
        private FileCabinetRecordXmlWriter writerXml;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// initializes a record.
        /// </summary>
        /// <param name="records">Input record.</param>
        public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
        {
            this.records = records;
        }

        /// <summary>
        /// passing a stream to a class <see cref="FileCabinetRecordCsvWriter"/> and record.
        /// </summary>
        /// <param name="sw">stream.</param>
        public void SaveToCsv(StreamWriter sw)
        {
            this.writerCsv = new FileCabinetRecordCsvWriter(sw);
            foreach (var record in this.records)
            {
                this.writerCsv.Write(record);
            }
        }

        /// <summary>
        /// passing a stream to a class <see cref="FileCabinetRecordXmlWriter"/> and list of record.
        /// </summary>
        /// <param name="sw">stream.</param>
        public void SaveToXml(StreamWriter sw)
        {
            List<FileCabinetRecord> list = new List<FileCabinetRecord>();
            this.writerXml = new FileCabinetRecordXmlWriter(XmlWriter.Create(sw));
            foreach (var record in this.records)
            {
                list.Add(record);
            }

            this.writerXml.Write(list);
        }
    }
}
