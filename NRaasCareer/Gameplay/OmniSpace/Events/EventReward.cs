using NRaas.CareerSpace.Booters;
using NRaas.Gameplay.Careers;
using NRaas.Gameplay.Rewards;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Rewards;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NRaas.Gameplay.OmniSpace.Events
{
    public class EventReward : Career.EventReward
    {
        // Methods
        public EventReward()
        { }
        public EventReward(XmlDbRow row, Dictionary<string, Dictionary<int, CareerLevel>> careerLevels, string careerName)
            : base(row, careerLevels, careerName)
        {
            mYesRewardsList = OpportunityBooter.ParseRewards(row, sYesRewardColumns, kNumRewards);
        }

        public override void GiveRewards(bool bDialogResult, Sim sim, string icon, Sims3.Gameplay.ActorSystems.Origin origin)
        {
            base.GiveRewards(bDialogResult, sim, icon, origin);

            RewardInfoEx.GiveRewards(sim, null, mYesRewardsList);
        }
    }
}
