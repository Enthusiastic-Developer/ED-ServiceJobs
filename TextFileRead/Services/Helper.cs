using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextFileRead.Data;

namespace TextFileRead.Services
{
    public class Helper
    {
        private readonly ILogger _logger = new NLogLoggerFactory().CreateLogger<Helper>();
        private string IncomingAckPath = @"D:\M50\TestDocuments\FESP";
        private readonly string _processedPath = @"D:\M50\TestDocuments\Processed";
        private readonly string _failedProcessedPath = @"D:\M50\TestDocuments\NotProcessed";
        private string? enforcementType;

        private EventDAL eventDal = new EventDAL();

        public List<AckFile> GetReturnAckFiles()
        {
            string guid = Guid.NewGuid().ToString();
            List<AckFile> enflist = new List<AckFile>();
            try
            {
                _logger.LogInformation(guid + "Started processing FESP Incoming ack ");
                // Make a reference to a directory.
                DirectoryInfo di = new DirectoryInfo(IncomingAckPath);
                _logger.LogInformation(guid + "Started Accessing Directory Incoming ack ");
                // Get a reference to each directory in that directory.
                DirectoryInfo[] diArr = di.GetDirectories();
                _logger.LogInformation(guid + "Started Accessing Directory Incoming ack ");
                foreach (DirectoryInfo dri in diArr)
                {
                    FileInfo[] files = dri.GetFiles("*.txt");
                    foreach (FileInfo file in files)
                    {

                        try
                        {
                            AckFile AckFile = new AckFile();
                            AckFile.DataInFiles = new List<DataInFile>();
                            _logger.LogInformation(guid + "Started processing FESP Incoming ack file : " + file.DirectoryName + "//" + file.FullName);
                            using (StreamReader? sr = file.OpenText())
                            {
                                string enf = string.Empty;
                                while ((enf = sr.ReadLine()!) != null)
                                {
                                    DataInFile AckTextFile = new DataInFile();
                                    string[] data = enf.Split(':');
                                    AckTextFile.TextFileName = file.Name;
                                    AckTextFile.ProcessedDateTime = data[0];
                                    AckTextFile.FileName = data[1];
                                    AckTextFile.RecordCode = data[2];
                                    AckTextFile.FileSize = data[3];
                                    AckTextFile.StatusCode = data[4];
                                    AckTextFile.ProcessingStatus = data[5];
                                    AckFile.DataInFiles.Add(AckTextFile);
                                }

                            }
                            AckFile.FileName = file.Name;
                            AckFile.ProcessedDate = DateTime.Now;

                            _logger.LogInformation("InsertEnforcementAckDetails called before with count :  " + AckFile.DataInFiles.Count);
                            enforcementType = "FESP";

                            BulkInsertEnforcementAckDetails(AckFile.DataInFiles);
                        }
                        catch (Exception ex)
                        {
                            CreateFolderAndMoveFile(false, enforcementType!, dri.FullName, file.Name);
                            _logger.LogError(guid + "Error in processing FESP Incoming ack file : " + file.DirectoryName + "//" + file.FullName + " Error : " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed processing FESP Incoming ack file" + ex.Message);
            }
            _logger.LogInformation(guid + "complete processing FESP Incoming ack file  ");

            return enflist;
        }
        public void BulkInsertEnforcementAckDetails(List<DataInFile> lstEnforcementAckFile)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("TextFileName", typeof(string));
            dt.Columns.Add("ProcessedDateTime", typeof(string));
            dt.Columns.Add("FileName", typeof(string));
            dt.Columns.Add("RecordCode", typeof(string));
            dt.Columns.Add("FileSize", typeof(string));
            dt.Columns.Add("StatusCode", typeof(string));
            dt.Columns.Add("ProcessingStatus", typeof(string));
            foreach (DataInFile data in lstEnforcementAckFile)
            {
                DataRow dr = dt.NewRow();
                dr["TextFileName"] = data.TextFileName;
                dr["ProcessedDateTime"] = data.ProcessedDateTime;
                dr["FileName"] = data.FileName;
                dr["RecordCode"] = data.RecordCode;
                dr["FileSize"] = data.FileSize;
                dr["StatusCode"] = data.StatusCode;
                dr["ProcessingStatus"] = data.ProcessingStatus;
                dt.Rows.Add(dr);
            }
            eventDal.BulkInsertEnforcementAckDetails(dt);
        }
        public void CreateFolderAndMoveFile(bool Processed, string enforcementType, string SourceProcessing, string fileName)
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
                File.Copy(Path.Combine(SourceFilePath, fileName), Path.Combine(ProcessedPath, folderDatePath, fileName));
                _logger.LogInformation("Moved File from Source File Path to Processed Path : " + Path.Combine(ProcessedPath, folderDatePath));
            }
            else
            {
                if (!Directory.Exists(Path.Combine(FailedProcessedPath, folderDatePath)))
                {
                    Directory.CreateDirectory(Path.Combine(FailedProcessedPath, folderDatePath));
                    _logger.LogInformation("Created Directory for Failed Processed Path : " + Path.Combine(FailedProcessedPath, folderDatePath));
                }
                File.Copy(Path.Combine(SourceFilePath, fileName), Path.Combine(FailedProcessedPath, folderDatePath, fileName));
                _logger.LogInformation("Moved File from Source File Path to Processed Path : " + Path.Combine(FailedProcessedPath, folderDatePath));
            }
        }
    }
}
