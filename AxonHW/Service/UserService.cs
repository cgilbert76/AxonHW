using AxonHW.Entity;
using AxonHW.Interface;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Author: Colin Gilbert
namespace AxonHW.Service
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly HttpClient _httpClient;
        
        public UserService(HttpClient httpClient, ILogger<UserService> logger) 
        {
            _httpClient = httpClient;
            _logger = logger;
            _logger.LogDebug("Initializing User Service");
        }

        public bool RetrieveUserInfo(string url, out string response)
        {
            HttpResponseMessage r = _httpClient.GetAsync(url).Result;
            response = r.Content.ReadAsStringAsync().Result;

            if (r.IsSuccessStatusCode)
            {
                _logger.LogDebug("User retrieved successfully with response: {Response}", response);
                return true;
            }
            else
            {
                _logger.LogDebug("User retrieval failed with code: {code}", r.StatusCode);
                return false;
            }
        }

        public User CreateUser(string userInfo)
        {
            try
            {
                JObject o = JObject.Parse(userInfo);
                return new User()
                {
                    LastName = o["results"][0]["name"]["last"].ToString(),
                    FirstName = o["results"][0]["name"]["first"].ToString(),
                    City = o["results"][0]["location"]["city"].ToString(),
                    Email = o["results"][0]["email"].ToString(),
                    Age = Convert.ToInt32(o["results"][0]["dob"]["age"].ToString())
                };
            }
            catch(JsonReaderException ex) 
            {
                _logger.LogError($"Error parsing user info: {userInfo}");
                return null;
            }
        }
    }
}
