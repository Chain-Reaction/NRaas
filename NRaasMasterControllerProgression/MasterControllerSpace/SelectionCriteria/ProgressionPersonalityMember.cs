extern alias SP;

using SimPersonality = SP::NRaas.StoryProgressionSpace.Personalities.SimPersonality;

using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class ProgressionPersonalityMember : SelectionTestableOptionList<ProgressionPersonalityMember.Item, SimPersonality, string>, IDoesNotNeedSpeciesFilter
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.PersonalityMember";
        }

        public class Item : TestableOption<SimPersonality, string>
        {
            public Item()
            { }
            public Item(SimPersonality personality, int count)
                : base(GetName(personality), personality.GetLocalizedName(), count)
            { }

            public override void SetValue(SimPersonality value, string storeType)
            {
                mValue = storeType;

                mName = value.GetLocalizedName();
            }

            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<string, SimPersonality> results)
            {
                foreach (SimPersonality personality in SP::NRaas.StoryProgression.Main.Personalities.GetClanMembership(me, true))
                {
                    results[GetName(personality)] = personality;
                }

                return true;
            }

            public static string GetName(SimPersonality personality)
            {
                return personality.GetTitlePrefix(SP.NRaas.StoryProgressionSpace.ManagerProgressionBase.PrefixType.Pure);
            }

            public override string DisplayKey
            {
                get { return "Has"; }
            }
        }
    }
}
