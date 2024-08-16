using AxonHW.Entity;
using AxonHW.Service;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;

// Author: Colin Gilbert
namespace AxonHW.Tests
{
    public class UserServiceTests
    {
        [Fact]
        public void RetrieveUserInfo_ReturnsTrueAndResponse_WhenAPICallReturnsOK()
        {
            var apiResponse = TestHelpers.ReadFile("ApiResponse.txt");

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(apiResponse)
                });
            var httpClient = new HttpClient(handlerMock.Object);
            var serviceLogger = new Mock<ILogger<UserService>>();
            var service = new UserService(httpClient, serviceLogger.Object);

            Assert.True(service.RetrieveUserInfo("https://randomuser.me/api/", out var response));
            Assert.Equal(apiResponse, response);
        }

        [Fact]
        public void RetrieveUserInfo_ReturnsFalseAndResponse_WhenAPICallReturnsNotOK()
        {
            var apiResponse = TestHelpers.ReadFile("ApiResponse.txt");

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent(apiResponse)
                });
            var httpClient = new HttpClient(handlerMock.Object);
            var serviceLogger = new Mock<ILogger<UserService>>();
            var service = new UserService(httpClient, serviceLogger.Object);

            Assert.False(service.RetrieveUserInfo("https://randomuser.me/api/", out var response));
            Assert.Equal(apiResponse, response);
        }

        [Fact]
        public void CreateUser_CreatesExpectedUserFromUserInfo()
        {
            var httpClient = new Mock<HttpClient>();
            var serviceLogger = new Mock<ILogger<UserService>>();

            var service = new UserService(httpClient.Object, serviceLogger.Object);

            var userInfo = TestHelpers.ReadFile("ApiResponse.txt");
            var user = service.CreateUser(userInfo);

            User verifyUser = new User()
            {
                LastName = "Roberts",
                FirstName = "Alex",
                Age = 36,
                City = "Greymouth",
                Email = "alex.roberts@example.com"
            };

            Assert.Equal(verifyUser.LastName, user.LastName);
            Assert.Equal(verifyUser.FirstName, user.FirstName);
            Assert.Equal(verifyUser.Age, user.Age);
            Assert.Equal(verifyUser.City, user.City);
            Assert.Equal(verifyUser.Email, user.Email);
        }
    }
}