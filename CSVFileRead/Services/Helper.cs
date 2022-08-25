using CSVFileRead.Data;
using CsvHelper;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace CSVFileRead.Services
{
    internal class Helper
    {
        private readonly ILogger _logger = new NLogLoggerFactory().CreateLogger<Helper>();
        private readonly string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Test;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False";
        private string FilePath = @"C:\Users\nbanuri\Downloads";
        private readonly string _processedPath = @"C:\Users\nbanuri\Downloads\Processed";
        private readonly string _failedProcessedPath = @"C:\Users\nbanuri\Downloads\NotProcessed";
        public void CSVFileProcess()
        {
            string guid = Guid.NewGuid().ToString();
            List<AckFile> enflist = new List<AckFile>();
            try
            {
                _logger.LogInformation(guid + "Started processing CSV file");
                // Make a reference to a directory.
                DirectoryInfo di = new DirectoryInfo(FilePath);
                _logger.LogInformation(guid + "Started Accessing Directory of file");
                // Get a reference to each directory in that directory.
                DirectoryInfo[] diArr = di.GetDirectories();
                foreach (DirectoryInfo dri in diArr)
                {
                    FileInfo[] files = dri.GetFiles("*.csv");
                    foreach (FileInfo file in files)
                    {
                        try
                        {
                            AckFile AckFile = new AckFile();
                            _logger.LogInformation(guid + "Started processing file");
                            using (var reader = new StreamReader(file.FullName))
                            {
                                using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
                                {
                                    csvReader.Context.RegisterClassMap<CSVMap>();
                                    var records = csvReader.GetRecords<DataInFile>().ToList();
                                    AckFile.DataInFiles = records;
                                }
                            }
                            AckFile.FileName = file.Name;
                            AckFile.ProcessedDate = DateTime.Now;
                            BulkInsertCSVDetails(AckFile.DataInFiles);
                            _logger.LogInformation(guid + "Completed processing file");
                            CreateFolderAndMoveFile(true, dri.FullName, file.Name);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(guid + "Error in processing CSV file " + file.Name + " Error: " + ex.Message);
                            CreateFolderAndMoveFile(false, dri.FullName, file.Name);
                        }
                    }
                }
                _logger.LogInformation(guid + "complete processing CSV file");
            }
            catch (Exception ex)
            {

                throw new Exception("Failed processing CSV file" + ex.Message);
            }
        }

        public void CreateFolderAndMoveFile(bool Processed, string SourceProcessing, string fileName)
        {
            string folderDatePath = DateTime.Today.ToString("yyyy/MM/dd").Replace('-', '/');
            string SourceFilePath = SourceProcessing;
            string ProcessedPath = _processedPath;
            string FailedProcessedPath = _failedProcessedPath;
            if (Processed)
            {
                if (!Directory.Exists(Path.Combine(ProcessedPath, folderDatePath)))
                {
                    Directory.CreateDirectory(Path.Combine(ProcessedPath, folderDatePath));
                    _logger.LogInformation("Created Directory for Processed Path : " + Path.Combine(ProcessedPath, folderDatePath));
                }
                File.Move(Path.Combine(SourceFilePath, fileName), Path.Combine(ProcessedPath, folderDatePath, fileName));
                _logger.LogInformation("Moved File from Source File Path to Processed Path : " + Path.Combine(ProcessedPath, folderDatePath));
            }
            else
            {
                if (!Directory.Exists(Path.Combine(FailedProcessedPath, folderDatePath)))
                {
                    Directory.CreateDirectory(Path.Combine(FailedProcessedPath, folderDatePath));
                    _logger.LogInformation("Created Directory for Failed Processed Path : " + Path.Combine(FailedProcessedPath, folderDatePath));
                }
                File.Move(Path.Combine(SourceFilePath, fileName), Path.Combine(FailedProcessedPath, folderDatePath, fileName));
                _logger.LogInformation("Moved File from Source File Path to Processed Path : " + Path.Combine(FailedProcessedPath, folderDatePath));
            }
        }

        private void BulkInsertCSVDetails(List<DataInFile> dataInFiles)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("HcTaskId", typeof(string));
            dt.Columns.Add("HcSchId", typeof(string));
            dt.Columns.Add("ResponsibleTeam", typeof(string));
            dt.Columns.Add("ExecutionDate", typeof(string));
            dt.Columns.Add("ExecutionTime", typeof(string));
            dt.Columns.Add("RecordCount", typeof(string));
            dt.Columns.Add("Priority", typeof(string));
            dt.Columns.Add("CreatedDate", typeof(string));
            dt.Columns.Add("CreatedUser", typeof(string));

            foreach (DataInFile data in dataInFiles)
            {
                DataRow dr = dt.NewRow();
                dr["HcTaskId"] = data.HcTaskId;
                dr["HcSchId"] = data.HcSchId;
                dr["ResponsibleTeam"] = data.ResponsibleTeam;
                dr["ExecutionDate"] = data.ExecutionDate;
                dr["ExecutionTime"] = data.ExecutionTime;
                dr["RecordCount"] = data.RecordCount;
                dr["Priority"] = data.Priority;
                dr["CreatedDate"] = data.CreatedDate;
                dr["CreatedUser"] = data.CreatedUser;
                dt.Rows.Add(dr);
            }
            #region BulkInsert
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();

                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        //Set the database table name
                        sqlBulkCopy.DestinationTableName = "dbo.AckFile";
                        sqlBulkCopy.BatchSize = dt.Rows.Count;
                        sqlBulkCopy.WriteToServer(dt);
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            #endregion
        }
    }
}
