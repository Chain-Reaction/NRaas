using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Settings
{
    public class ChangeHouseholdOptions : OperationSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "HouseholdOptions";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!base.Allow(parameters)) return false;

            Sim sim = parameters.mTarget as Sim;
            if (sim != null)
            {
                if (sim.Household == null) return false;
            }
            else
            {
                Lot lot = parameters.mTarget as Lot;
                if (lot == null)
                {
                    if (parameters.mTarget is RoomConnectionObject) return false;

                    lot = parameters.mTarget.LotCurrent;
                    if (lot == null) return false;
                }

                if (lot.Household == null) return false;
            }

            return true;
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Household house = null;

            Lot lot = parameters.mTarget as Lot;
            if (lot != null)
            {
                house = lot.Household;
            }
            else
            {
                Sim sim = parameters.mTarget as Sim;
                if (sim != null)
                {
                    house = sim.Household;
                }
                else if (parameters.mTarget.LotCurrent != null)
                {
                    house = parameters.mTarget.LotCurrent.Household;
                }
            }

            if (house != null)
            {
                StoryProgression.Main.GetHouseOptions(house).ShowOptions(StoryProgression.Main, Common.Localize("HouseholdOptions:MenuName"));
                return OptionResult.SuccessRetain;
            }

            return OptionResult.Failure;
        }
    }
}

