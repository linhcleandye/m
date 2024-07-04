using AutoMapper;
using Infrastructure.Extensions;
using Shared.DTOs.Customer;

namespace Customer.API;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Entities.Customer, CustomerDto>();
        CreateMap<CreateCustomerDto, Entities.Customer>();
        CreateMap<UpdateCustomerDto, Entities.Customer>()
            .IgnoreAllNonExisting();
    }
}