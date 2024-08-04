using AutoMapper;
using InfoTrackBooking.Models;

namespace InfoTrackBooking.Mappers
{
    /// <summary>
    /// View model mapper to only get necessary information from entity to display to user
    /// </summary>
    public static class BookingDetailsViewModeMappers
    {
        internal static IMapper Mapper { get; }

        static BookingDetailsViewModeMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<BookingDetailsViewModelMapperProfile>())
                .CreateMapper();
        }

        public static IEnumerable<BookingDetailsViewModel> ToViewModel(this IEnumerable<BookingDetailsDto> dtos)
        {
            return Mapper.Map<IEnumerable<BookingDetailsViewModel>>(dtos);
        }
    }

    public class BookingDetailsViewModelMapperProfile : Profile
    {
        public BookingDetailsViewModelMapperProfile()
        {
            CreateMap<BookingDetailsDto, BookingDetailsViewModel>()
                 .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.BookingId))
                 .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime));
        }
    }
}
