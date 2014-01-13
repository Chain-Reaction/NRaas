using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Checks
{
    public class CheckVehicle : Check<Vehicle>
    {
        protected override bool PrePerform(Vehicle obj, bool postLoad)
        {
            return true;
        }

        protected override bool PostPerform(Vehicle car, bool postLoad)
        {
            if (car.CarRoutingComponent == null)
            {
                car.AddComponent<CarRoutingComponent>(new object[0x0]);
            }
            return true;
        }
    }
}
