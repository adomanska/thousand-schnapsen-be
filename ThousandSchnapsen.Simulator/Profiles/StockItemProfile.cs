using AutoMapper;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Simulator.Dto;

namespace ThousandSchnapsen.Simulator.Profiles
{
    public class StockItemProfile : Profile
    {
        public StockItemProfile()
        {
            CreateMap<StockItem, StockItemDto>()
                .ForMember(dest =>
                        dest.CardId,
                    opt => opt.MapFrom(src => src.Card.CardId));
        }
    }
}