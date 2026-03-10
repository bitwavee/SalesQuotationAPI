using AutoMapper;
using SalesQuotation.Application.Dtos;
using SalesQuotation.Domain.Entities;

namespace SalesQuotation.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User -> UserDto
        CreateMap<User, UserDto>()
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()));

        // Enquiry -> EnquiryDto
        CreateMap<Enquiry, EnquiryDto>()
            .ForMember(d => d.AssignedStaff, o => o.MapFrom(s => s.AssignedStaff))
            .ForMember(d => d.MeasurementsCount, o => o.MapFrom(s => s.Measurements != null ? s.Measurements.Count : 0))
            .ForMember(d => d.QuotationsCount, o => o.MapFrom(s => s.Quotations != null ? s.Quotations.Count : 0));

        // Quotation -> QuotationDto
        CreateMap<Quotation, QuotationDto>()
            .ForMember(d => d.Items, o => o.MapFrom(s => s.Items));

        // QuotationItem -> QuotationItemDto
        CreateMap<QuotationItem, QuotationItemDto>()
            .ForMember(d => d.MaterialName, o => o.MapFrom(s => s.Material != null ? s.Material.Name : string.Empty));

        // Material -> MaterialDto
        CreateMap<Material, MaterialDto>();

        // Measurement -> MeasurementDto
        CreateMap<Measurement, MeasurementDto>()
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Category));

        // MeasurementCategory -> MeasurementCategoryDto
        CreateMap<MeasurementCategory, MeasurementCategoryDto>();

        // EnquiryStatusConfig -> EnquiryStatusConfigDto
        CreateMap<EnquiryStatusConfig, EnquiryStatusConfigDto>()
            .ForMember(d => d.StatusKey, o => o.MapFrom(s => s.StatusValue))
            .ForMember(d => d.Color, o => o.MapFrom(s => s.ColorHex))
            .ForMember(d => d.RequiredFields, o => o.MapFrom(s => s.RequiredFields))
            .ForMember(d => d.FieldPermissions, o => o.MapFrom(s => s.FieldPermissions));

        // EnquiryProgress -> EnquiryProgressDto
        CreateMap<EnquiryProgress, EnquiryProgressDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.NewStatus))
            .ForMember(d => d.Notes, o => o.MapFrom(s => s.Comment))
            .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.CreatedBy));
    }
}
