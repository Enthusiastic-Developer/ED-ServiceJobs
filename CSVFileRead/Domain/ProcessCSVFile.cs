using CSVFileRead.Services;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace CSVFileRead.Domain
{
    internal class ProcessCSVFile
    {
        private readonly ILogger _logger = new NLogLoggerFactory().CreateLogger<ProcessCSVFile>();
        public void CSVFileProcess()
        {
            _logger.LogInformation("Processing csvfile started");
            Helper helper = new Helper();
            try
            {
                helper.CSVFileProcess();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing csvfile");
            }
            _logger.LogInformation("Processing csvfile Ended");
        }
    }
}
