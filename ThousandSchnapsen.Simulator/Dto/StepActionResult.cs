using ThousandSchnapsen.Common.Interfaces;

namespace ThousandSchnapsen.Simulator.Dto
{
    public class StepActionResult
    {
        public bool Done { get; set; }
        public IPlayerState PlayerState { get; set; }
    }
}