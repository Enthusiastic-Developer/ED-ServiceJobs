using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using TextFileRead.Services;

namespace TextFileRead.Domain
{
    public class ProcessFile
    {
        private readonly ILogger _logger = new NLogLoggerFactory().CreateLogger<ProcessFile>();
        public void TextFileProcess()
        {
            _logger.LogInformation("Processing Text file started");
            Helper helper = new Helper();
            try
            {
                helper.TXTFileProcess();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing file");
            }
            _logger.LogInformation("Processing Text file ended");
        }
    }
}
