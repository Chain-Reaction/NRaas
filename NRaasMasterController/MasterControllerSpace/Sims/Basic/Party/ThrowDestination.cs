using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Basic.Party
{
    public class ThrowDestination : ThrowHome
    {
        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (!UIUtils.IsOkayToStartModalDialog())
            {
                Common.Notify(Common.Localize("Pause:Failure"));
                return OptionResult.Failure;
            }

            return base.Run(parameters);
        }

        public override string Name
        {
            get { return Common.Localize("ThrowDestination:MenuName"); }
        }

        protected override Lot GetVenue(IActor actor, out bool hasExclusiveAccess)
        {
            hasExclusiveAccess = false;

            List<Lot> candidates = new List<Lot>();
            foreach (Lot lot in LotManager.AllLots)
            {
                if ((!lot.IsWorldLot) && (lot.GetMetaAutonomyType != Lot.MetaAutonomyType.Residential) /*&& (lot.IsRentable)*/)
                {
                    candidates.Add(lot);
                }
            }

            if (!UIUtils.IsOkayToStartModalDialog())
            {
                Common.Notify(Common.Localize("Pause:Failure"));
                return null;
            }

            return Sims3.Gameplay.Objects.Electronics.Phone.CallInviteToLot.ChooseLot(candidates, true, out hasExclusiveAccess);
        }
    }
}
