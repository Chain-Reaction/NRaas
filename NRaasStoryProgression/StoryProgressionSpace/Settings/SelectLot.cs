using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Scoring;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Settings
{
    public class SelectLot : OperationSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            if ((Household.ActiveHousehold != null) &&
                (StoryProgression.Main.GetValue<DreamCatcherOption, bool>(Household.ActiveHousehold)))
            {
                return "MakeActiveDreamCatcher";
            }
            return "MakeActive";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            Lot lot = parameters.mTarget as Lot;
            if (lot == null) return false;

            if (lot.Household == null) return false;

            if (lot.Household.IsActive) return false;

            if (SimTypes.IsSpecial(lot.Household)) return false;

            if (Households.AllSims(lot.Household).Count == 0) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Lot lot = parameters.mTarget as Lot;
            if (lot == null) return OptionResult.Failure;

            if ((lot.Household.AllActors != null) && (lot.Household.AllActors.Count > 0))
            {
                Sim sim = null;
                foreach (Sim member in Households.AllSims(lot.Household))
                {
                    if (member.SimDescription.IsNeverSelectable) continue;

                    sim = member;
                    break;
                }

                StoryProgression.Main.Sims.Select(sim);
                return OptionResult.SuccessClose;
            }

            return OptionResult.Failure;
        }
    }
}

