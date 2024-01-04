using Application.Common.Models.ResponseModels;
using AutoMapper;

namespace Application.Common.Mappings;

public class InfrastructureMappingProfile: Profile
{
    public InfrastructureMappingProfile()
    {
        CreateMap(typeof(HttpResponseMessage), typeof(HttpRequestResponse<>))
            .ForMember(nameof(HttpRequestResponse<object>.SuccessResponse), opt => opt.Ignore())
            .ForMember(nameof(HttpRequestResponse<object>.FailedResponse), opt => opt.Ignore());
    }
}