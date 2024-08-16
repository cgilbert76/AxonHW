using AxonHW.Entity;

// Author: Colin Gilbert
namespace AxonHW.Interface
{
    public interface IUserService
    {
        bool RetrieveUserInfo(string url, out string response);
        User CreateUser(string userInfo);
    }
}
