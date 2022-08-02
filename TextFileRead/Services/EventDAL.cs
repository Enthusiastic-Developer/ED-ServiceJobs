using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextFileRead.Services
{
    public class EventDAL
    {
        private readonly ILogger _logger = (new NLogLoggerFactory()).CreateLogger<EventDAL>();
        private readonly string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Test;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False";

        public void BulkInsertEnforcementAckDetails(DataTable dt)
        {
            _logger.LogInformation("InsertEnforcementAckDetails DAL method calling started " + DateTime.Now.ToString());
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                    {
                        bulkCopy.DestinationTableName = "[dbo].[EnforcementAcknowledgement_foreign_Load]";
                        bulkCopy.WriteToServer(dt);
                    }
                    conn.Close();
                }
            }
            catch (Exception? ex)
            {
                throw new Exception(string.Format("Error in InsertEnforcementAckDetails DAL method : " + ex.Message + ex?.StackTrace!.ToString()));
            }
        }
    }
}
