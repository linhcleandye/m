using AutoMapper;
using Infrastructure.Extensions;
using Shared.DTOs.Customer;

namespace Customer.API;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Entities.Customer, CustomerDto>();
        CreateMap<CreateCustomerDto, Entities.Customer>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.GetUserName()));
        CreateMap<UpdateCustomerDto, Entities.Customer>()
            .IgnoreAllNonExisting();

        CreateMap<Stripe.Customer, StripeCustomerDto>()
            .ForMember(dest => dest.Shipping, opt => opt.MapFrom(src => src.Shipping.Address))
            ;
        CreateMap<Stripe.Address, StripeCustomerAddressDto>();
        CreateMap<Stripe.Shipping, StripeCustomerAddressDto>();
    }
}