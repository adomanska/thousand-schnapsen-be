using AutoMapper;
using ThousandSchnapsen.Common.Commons;

namespace ThousandSchnapsen.Simulator.Profiles
{
    public class CardsSetProfile : Profile
    {
        public CardsSetProfile()
        {
            CreateMap<CardsSet, int>()
                .ConstructUsing(src => src.Code);
        }
    }
}