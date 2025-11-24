using AutoMapper;
using OaigLoan.Application.Users.Models;
using OaigLoan.Domain.Entities;

namespace OaigLoan.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
    }
}
