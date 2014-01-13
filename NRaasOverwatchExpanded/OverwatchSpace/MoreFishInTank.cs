using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Preload
{
    public class MoreFishInTank : PreloadOption
    {
        public MoreFishInTank()
        { }

        public override string GetTitlePrefix()
        {
            return "MoreFishInTank";
        }

        public override void OnPreLoad()
        {
            try
            {
                Overwatch.Log("Try MoreFishInTank");

                FishTankModern.kFishTankTuning.kMaximumNumberOfFish = 12;

                foreach (FishTankModern tank in Sims3.Gameplay.Queries.GetObjects<FishTankModern>())
                {
                    if (tank.Inventory.mMaxInventoryCapacity < FishTankModern.kFishTankTuning.kMaximumNumberOfFish)
                    {
                        tank.Inventory.mMaxInventoryCapacity = FishTankModern.kFishTankTuning.kMaximumNumberOfFish;

                        if (tank.LotCurrent != null)
                        {
                            Overwatch.Log(" Tank Updated: " + tank.LotCurrent.Name);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(Name, e);
            }
        }
    }
}
