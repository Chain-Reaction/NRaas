extern alias SP;

using SimPersonality = SP::NRaas.StoryProgressionSpace.Personalities.SimPersonality;

using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Progression
{
    public class SetLeader : PersonalityBase
    {
        protected override int GetMaxSelection()
        {
            return 1;
        }

        protected override bool IsLeader
        {
            get { return true; }
        }

        protected override bool Allow(SimDescription me, SimPersonality personality)
        {
            if (personality.Me == me) return false;

            return !personality.IsLeaderless;
        }

        protected override bool Run(SimDescription me, SimPersonality personality)
        {
            if (personality.Me != null)
            {
                if (!AcceptCancelDialog.Show(Common.Localize("SetLeader:Prompt", personality.IsFemaleLocalization(), personality.Me.IsFemale, new object[] { personality.GetLocalizedName(), personality.Me })))
                {
                    return false;
                }
            }

            personality.SetLeader(me, true);
            return true;
        }

        public class ListingOption : PersonalityListing<SetLeader>
        {
            public override string GetTitlePrefix()
            {
                return "SetLeader";
            }

            public override ITitlePrefixOption ParentListingOption
            {
                get { return null; }
            }
        }
    }
}
