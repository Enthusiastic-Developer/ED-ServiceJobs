using CSVFileRead.Data;
using CsvHelper.Configuration;

namespace CSVFileRead.Services
{
    public class CSVMap : ClassMap<DataInFile>
    {
        public CSVMap()
        {
            Map(m => m.HcTaskId).Index(0);
            Map(m => m.HcSchId).Index(1);
            Map(m => m.ResponsibleTeam).Index(2);
            Map(m => m.ExecutionDate).Index(3);
            Map(m => m.ExecutionTime).Index(4);
            Map(m => m.RecordCount).Index(5);
            Map(m => m.Priority).Index(6);
            Map(m => m.CreatedDate).Index(7);
            Map(m => m.CreatedUser).Index(8);
        }
    }
}
