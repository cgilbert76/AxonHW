using AxonHW.Entity;
using AxonHW.Interface;
using AxonHW.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// requirements from initial POC copied below.
/// <summary>
/// We have provided you the code below for a proof of concept (PoC) console application that satisfies the following requirements:
/// - Reads random users from an API endpoint 5 times. 
/// - All responses from the API should be written to a file.
/// - All successful responses should be represented as a list of users with the following properties
///     - last name
///     - first name
///     - age
///     - city
///     - email
///   and be written to file as valid JSON.
/// 
/// There are now new requirements for this application, and we should like for you to update this console 
/// application to incorporate the following new requirements while satisfing the original requirements:
/// - Update this code so it follows .Net best practices and principles. The code should be extensible, reusable, and easy to test using Unit Tests.
/// - Add Unit tests.
/// - Make the the output file names configurable.
/// - Make the number of API calls configurable instead of 5.
/// - Add logging
/// </summary>

// Author: Colin Gilbert
namespace AxonHW
{
    internal class Program
    {
        private static readonly HttpClient httpClient = new HttpClient();

        static void Main(string[] args)
        {
            //point config to appsettings.json file
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            //build up service collection
            var serviceProvider = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information))
                .AddSingleton<IFileService, FileService>()
                .AddSingleton<IConfiguration>(config)
                .BuildServiceProvider();

            //services
            var userServiceLogger = serviceProvider.GetRequiredService<ILogger<UserService>>();
            var userService = new UserService(httpClient, userServiceLogger);
            var fileService = serviceProvider.GetRequiredService<IFileService>();
            var logger      = serviceProvider.GetRequiredService<ILogger<Program>>();

            //retrieve config values
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var userUrl = configuration.GetValue<string>("UserRetrievalURL") ?? "https://randomuser.me/api/";
            var numApiCalls = configuration.GetValue<int>("APICalls");
            if (numApiCalls == 0)
                numApiCalls = 5; //default to 5

            logger.LogInformation("Begin process");
            logger.LogInformation("Retrieving info for {Count} users from {API}", numApiCalls, userUrl);
            string apiResponse = string.Empty;
            List<User> users = new List<User>();
            for (int i = 0; i < numApiCalls; i++)
            {
                //retrieve user info and build out list
                if (userService.RetrieveUserInfo(userUrl, out apiResponse))
                {
                    //create and add user to list if API returns success
                    var user = userService.CreateUser(apiResponse);
                    if (user != null)
                    {
                        logger.LogInformation("User '{FirstName} {LastName}' added to list", user.FirstName, user.LastName);
                        users.Add(user);
                    }
                }
                //store response anyway even if API indicates non-success
                logger.LogDebug("Saving API response");
                fileService.WriteApiResponseFile(apiResponse);
            }
            //write all users to file
            if (users.Any())
            {
                logger.LogInformation("Saving {Count} users to file", users.Count);
                fileService.WriteUsersFile(users);
            }
            logger.LogInformation("Process complete");
        }
    }
}


