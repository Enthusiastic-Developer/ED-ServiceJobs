using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextFileRead.Services;

namespace TextFileRead.Domain
{
    public class ProcessFile
    {
        private readonly ILogger _logger = new NLogLoggerFactory().CreateLogger<ProcessFile>();
        public void TextFileProcess()
        {
            _logger.LogInformation("Processing file started");
            Helper helper = new Helper();
            try
            {
                helper.GetReturnAckFiles();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing file");
            }
            _logger.LogInformation("Processing file ended");
        }
    }
}
