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

        CreateMap<StripeCustomerAddressDto, Address>()
            .ForMember(dest => dest.Line1, opt => opt.MapFrom(src => src.Street))
            .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.Zip))
            ;
        CreateMap<StripeCustomerAddressDto, Shipping>()
            .ForPath(dest => dest.Address.Line1, opt => opt.MapFrom(src => src.Street))
            .ForPath(dest => dest.Address.City, opt => opt.MapFrom(src => src.City))
            .ForPath(dest => dest.Address.PostalCode, opt => opt.MapFrom(src => src.Zip))
            .ForPath(dest => dest.Address.State, opt => opt.MapFrom(src => src.State))
            ;
        CreateMap<StripeCustomerAddressDto, AddressOptions>()
            .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.Zip))
            .ForMember(dest => dest.Line1, opt => opt.MapFrom(src => src.Street))
            ;
        CreateMap<StripeCustomerAddressDto, ShippingOptions>()
            .ForPath(dest => dest.Address.PostalCode, opt => opt.MapFrom(src => src.Zip))
            .ForPath(dest => dest.Address.Line1, opt => opt.MapFrom(src => src.Street))
            ;
    }
}