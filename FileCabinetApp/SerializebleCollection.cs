using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// class for serialize cillection.
    /// </summary>
    [XmlInclude(typeof(FileCabinetRecord))]
    public class SerializebleCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializebleCollection"/> class.
        /// </summary>
        public SerializebleCollection()
        {
            this.Records = new List<FileCabinetRecord>();
        }

        /// <summary>
        /// Gets collection for serialize.
        /// </summary>
        /// <value>The Records property gets/sets the value.</value>
        [XmlArray("Records")]
        [XmlArrayItem("Record")]
        public List<FileCabinetRecord> Records { get; }
    }
}
