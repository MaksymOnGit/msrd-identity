using FluentResults;

namespace MSRD.Identity
{
    public sealed class ResultLogger : IResultLogger
    {
        private readonly ILogger<ResultLogger> logger;

        public ResultLogger(ILogger<ResultLogger> logger)
        {
            this.logger = logger;
        }
        public void Log(string context, string content, ResultBase result, LogLevel logLevel)
        {
            logger.Log(logLevel, "Context: {Context}\r\nContent: {Content}\r\nResults: {Results}", context, content, result);
        }

        public void Log<TContext>(string content, ResultBase result, LogLevel logLevel)
        {
            logger.Log(logLevel, "Context: {Context}\r\nContent: {Content}\r\nResults: {Results}", typeof(TContext), content, result);
        }
    }
}
