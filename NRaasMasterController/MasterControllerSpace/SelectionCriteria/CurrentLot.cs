using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    [Persistable]
    public class CurrentLot : SelectionOption
    {
        Lot mLot;

        public CurrentLot()
        {}
        public CurrentLot(Lot lot)
        {
            mLot = lot;
        }

        public override string GetTitlePrefix()
        {
            return "Criteria.CurrentLot";
        }

        protected override bool Allow(SimDescription me, IMiniSimDescription actor)
        {
            if (me.CreatedSim == null) return false;

            SimDescription actorDesc = actor as SimDescription;
            if (actorDesc == null) return false;

            if (actorDesc.CreatedSim == null) return false;

            if (mLot == null)
            {
                return (me.CreatedSim.LotCurrent == actorDesc.CreatedSim.LotCurrent);
            }
            else
            {
                return (me.CreatedSim.LotCurrent == mLot);
            }
        }
    }
}
