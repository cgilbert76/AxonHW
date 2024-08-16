using AxonHW.Service;
using AxonHW.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

// Author: Colin Gilbert
namespace AxonHW.Tests
{
    public class FileServiceTests
    {
        private readonly IConfiguration _configuration;
        private readonly string _testFilesDir = "c:\\temp\\";
        private readonly string _testApiResponseFileName = "TestApiResponses.txt";
        private readonly string _testUserListFileName = "TestUserList.json";

        public FileServiceTests()
        {
            var settings = new Dictionary<string, string> {
                {"APIResponseOutputFileName", _testApiResponseFileName},
                {"UserListOutputFileName", _testUserListFileName},
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();
        }

        [Fact]
        public void WriteApiResponseFile_WritesExpectedUserInfoToFile()
        {
            string testFilePath = _testFilesDir + _testApiResponseFileName;
            var apiResponse = TestHelpers.ReadFile("ApiResponse.txt");
            string expectedApiResponse = apiResponse + "\r\n";

            var serviceLogger = new Mock<ILogger<FileService>>();
            var service = new FileService(_configuration, serviceLogger.Object);

            service.WriteApiResponseFile(apiResponse);
            
            var fileContent = File.ReadAllText(testFilePath);
            Assert.Equal(expectedApiResponse, fileContent);

            File.Delete(testFilePath);
        }

        [Fact]
        public void WriteUsersFile_WritesValidJSONToFile()
        {
            string testFilePath = _testFilesDir + _testUserListFileName;

            var serviceLogger = new Mock<ILogger<FileService>>();
            var service = new FileService(_configuration, serviceLogger.Object);

            List<User> users = new List<User>();
            users.Add(new User()
            {
                LastName = "Doe",
                FirstName = "John",
                Age = 30,
                City = "Anywhere",
                Email = "johndoe@example.com"
            });
            users.Add(new User()
            {
                LastName = "Doe",
                FirstName = "Jane",
                Age = 30,
                City = "Anywhere",
                Email = "janedoe@example.com"
            });

            service.WriteUsersFile(users);

            var fileContent = File.ReadAllText(testFilePath);
            var verifyJson = JsonConvert.SerializeObject(users);
            Assert.Equal(verifyJson, fileContent);

            File.Delete(testFilePath);
        }
    }
}
