using AxonHW.Entity;
using AxonHW.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

// Author: Colin Gilbert
namespace AxonHW.Service
{
    public class FileService : IFileService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileService> _logger;
        private readonly string _filePath = "c:\\temp\\";

        public string ResponseFilePath { get; set; }
        public string UserFilePath { get; set; }

        public FileService(IConfiguration configuration, ILogger<FileService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _logger.LogDebug("Initializing File Service");

            //set file paths
            var responseFileName = _configuration.GetValue<string>("APIResponseOutputFileName") ?? "MyTest.txt";
            ResponseFilePath = _filePath + responseFileName;
            _logger.LogDebug("Response file path set to {ResponsePath}", ResponseFilePath);

            var userFileName = _configuration.GetValue<string>("UserListOutputFileName") ?? "MyTest.json";
            UserFilePath = _filePath + userFileName;
            _logger.LogDebug("User file path set to {UserPath}", UserFilePath);
        }

        public void WriteApiResponseFile(string apiResponse)
        {
            File.AppendAllText(ResponseFilePath, apiResponse);
            File.AppendAllText(ResponseFilePath, Environment.NewLine);
        }

        public void WriteUsersFile(List<User> users)
        {
            File.AppendAllText(UserFilePath, JsonConvert.SerializeObject(users));
        }
    }
}
