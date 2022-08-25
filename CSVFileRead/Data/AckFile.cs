using CsvHelper.Configuration.Attributes;

namespace CSVFileRead.Data
{
    public class AckFile
    {
        public string? FileName { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public List<DataInFile>? DataInFiles { get; set; }

    }
    public class DataInFile
    {

        [Index(0)]
        public string? HcTaskId { get; set; }
        [Index(1)]
        public string? HcSchId { get; set; }
        [Index(2)]
        public string? ResponsibleTeam { get; set; }
        [Index(3)]
        public string? ExecutionDate { get; set; }
        [Index(4)]
        public string? ExecutionTime { get; set; }
        [Index(5)]
        public string? RecordCount { get; set; }
        [Index(6)]
        public string? Priority { get; set; }
        [Index(7)]
        public string? CreatedDate { get; set; }
        [Index(8)]
        public string? CreatedUser { get; set; }
    }
}

