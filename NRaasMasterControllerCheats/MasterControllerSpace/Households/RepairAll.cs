using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.Fireplaces;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.Insect;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Households
{
    public class RepairAll : HouseholdFromList, IHouseholdOption
    {
        public override string GetTitlePrefix()
        {
            return "RepairAll";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override bool Allow(Lot lot, Household me)
        {
            if (!base.Allow(lot, me)) return false;

            if (lot == null) return false;

            return (true);
        }

        protected override OptionResult Run(Lot lot, Household me)
        {
            if (lot == null) return OptionResult.Failure;

            foreach (GameObject obj in lot.GetObjects<GameObject>())
            {
                if ((obj.InUse) || (!obj.InWorld)) continue;

                if (obj.Charred)
                {
                    obj.Charred = false;
                    if (obj is Windows)
                    {
                        RepairableComponent.CreateReplaceObject(obj);
                    }
                }

                RepairableComponent repairable = obj.Repairable;
                if ((repairable != null) && (repairable.Broken))
                {
                    repairable.ForceRepaired(Sim.ActiveActor);
                }
            }

            LotLocation[] burntTiles = World.GetBurntTiles(lot.LotId, LotLocation.Invalid);
            if (burntTiles.Length > 0x0)
            {
                foreach (LotLocation burnt in burntTiles)
                {
                    if ((lot.LotLocationIsPublicResidential(burnt)) && ((lot.TombRoomManager == null) || !lot.TombRoomManager.IsObjectInATombRoom(burnt)))
                    {
                        World.SetBurnt(lot.LotId, burnt, false);
                    }
                }
            }

            return OptionResult.SuccessClose;
        }
    }
}
