using ThousandSchnapsen.Common.States;

namespace ThousandSchnapsen.Simulator.Dto
{
    public class ActionResultDto
    {
        public PlayerState State { get; set; }
        public bool Done { get; set; }
        public InfoDto Info { get; set; }
    }
}