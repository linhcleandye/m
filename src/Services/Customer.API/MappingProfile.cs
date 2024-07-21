using AutoMapper;
using Infrastructure.Extensions;
using Shared.DTOs.Customer;
using Shared.DTOs.Customer.Stripe;
using Stripe;

namespace Customer.API;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Entities.Customer, CustomerDto>()
            .ForPath(dest => dest.StripeCustomer, opt => opt.MapFrom(src => src.StripeCustomer))
            ;
        CreateMap<CreateCustomerDto, Entities.Customer>()
            .ForMember(dest => dest.StripeCustomerId, opt => opt.Condition(src => src.StripeCustomerId != null))
            .ForPath(dest => dest.StripeCustomer.Address, opt => opt.MapFrom(src => src.Address))
            .ForPath(dest => dest.StripeCustomer.Shipping.Address, opt => opt.MapFrom(src => src.Shipping))
            .ForPath(dest => dest.StripeCustomer.Name, opt => opt.MapFrom(src => src.FullName()))
            .ForPath(dest => dest.StripeCustomer.Phone, opt => opt.MapFrom(src => src.Phone))
            .IgnoreAllNonExisting()
            ;
        
        CreateMap<UpdateCustomerDto, Entities.Customer>()
            .ForMember(dest => dest.StripeCustomerId, opt => opt.Condition(src => src.StripeCustomerId != null))
            .ForPath(dest => dest.StripeCustomer.Address, opt => opt.MapFrom(src => src.Address))
            .ForPath(dest => dest.StripeCustomer.Shipping.Address, opt => opt.MapFrom(src => src.Shipping))
            .ForPath(dest => dest.StripeCustomer.Name, opt => opt.MapFrom(src => src.FullName))
            .IgnoreAllNonExisting();

        // Stripe Customer

        CreateMap<Stripe.Customer, StripeCustomerDto>()
            .ForPath(dest => dest.Shipping, opt => opt.MapFrom(src => src.Shipping.Address))
            .ForPath(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            ;
        CreateMap<Address, StripeCustomerAddressDto>();
        CreateMap<Shipping, StripeCustomerAddressDto>();
          
        CreateMap<StripeCustomerAddressDto, Address>();
        CreateMap<StripeCustomerAddressDto, Shipping>();
        CreateMap<StripeCustomerAddressDto, AddressOptions>();
        CreateMap<StripeCustomerAddressDto, ShippingOptions>();
    }
}