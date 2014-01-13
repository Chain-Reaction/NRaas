using NRaas.SleepFreedom;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Beds;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Sims3.Gameplay.Objects.Beds
{
    [Persistable]
    public class TentExpensivePermanentNRaas : Sims3.Gameplay.Objects.Beds.TentExpensivePermanent
    {
        public TentExpensivePermanentNRaas()
        { }

        public override bool CanShareBed(Sim newSim, CommodityKind use, out Sim incompatibleSim)
        {
            return BedController.CanShareBed(this, newSim, use, out incompatibleSim);
        }

        public override bool CanShareBed(Sim s, BedData entryPart, CommodityKind use, out Sim incompatibleSim)
        {
            return BedController.CanShareBed(this, s, use, out incompatibleSim);
        }
    }
}
