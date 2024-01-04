using Application.Common.Models;
using Application.Common.Models.ResponseModels;

namespace Application.Common.Abstractions;

public interface IUamServiceAdapter
{
    Task<HttpRequestResponse<EcapServiceResponse>> CreateUserAsync(UserCreationModel user, string token);
    Task<HttpRequestResponse<EcapServiceResponse>> UpdateUserAsync(UserUpdateModel user, string token);
    Task<HttpRequestResponse<EcapServiceResponse>> DeleteProfilePermanentlyAsync(UserDeletionModel userDeletionModel, string token);
}