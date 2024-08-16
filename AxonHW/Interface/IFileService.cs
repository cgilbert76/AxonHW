using AxonHW.Entity;

// Author: Colin Gilbert
namespace AxonHW.Interface
{
    public interface IFileService
    {
        void WriteApiResponseFile(string apiResponse);
        void WriteUsersFile(List<User> users);
    }
}
