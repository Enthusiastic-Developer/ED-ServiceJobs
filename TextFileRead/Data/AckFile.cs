using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextFileRead.Data
{
    public class AckFile
    {
        public string? FileName { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public List<DataInFile>? DataInFiles { get; set; }
    }

    public class DataInFile
    {
        public string? TextFileName { get; set; }
        public string? ProcessedDateTime { get; set; }
        public string? FileName { get; set; }
        public string? RecordCode { get; set; }
        public string? FileSize { get; set; }
        public string? StatusCode { get; set; }
        public string? ProcessingStatus { get; set; }
    }
}
