using ThousandSchnapsen.Common.States;

namespace ThousandSchnapsen.Simulator.Dto
{
    public class StepActionResult
    {
        public bool Done { get; set; }
        public PlayerState PlayerState { get; set; }
    }
}