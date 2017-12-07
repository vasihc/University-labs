using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootingMethod
{
    public class Constants
    {
        public const double L = 5; //1 А = 10e-10 м
        public const double V0 = 25; // Эв
        public const double EhFactor = 0.0367493; //1 эВ = 0,0367493 Eh
        public const double EvFactor = 27.21140266; //1 Eh = 27.21140266 Ev
        public const int NodesCount = 1000;
        public const double StepEnergy = 0.005;
        public const double OffsetEnergy = 0.0001;
        public const int EnergiesCount = 801;
        public const double Threshold = 0.35;
        public const double StepBisection = 0.0000001;
    }
}
