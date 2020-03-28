using AutoMapper;
using ThousandSchnapsen.Common.States;
using ThousandSchnapsen.Simulator.Dto;

namespace ThousandSchnapsen.Simulator.Profiles
{
    public class PlayerStateProfile : Profile
    {
        public PlayerStateProfile()
        {
            CreateMap<PlayerState, PlayerStateDto>();
        }
    }
}