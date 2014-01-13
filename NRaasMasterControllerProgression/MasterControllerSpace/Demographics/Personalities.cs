extern alias SP;

using SimPersonality = SP::NRaas.StoryProgressionSpace.Personalities.SimPersonality;

using NRaas.CommonSpace.Selection;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Demographics
{
    public class Personalities : OptionItem, IDemographicOption
    {
        public override string GetTitlePrefix()
        {
            return "Personalities";
        }

        public class Item : ValueSettingOption<SimPersonality>
        {
            public Item(SimPersonality personality)
                : base(personality, personality.GetLocalizedName(), 0)
            { }
        }

        protected string GetDetails()
        {
            List<Item> choices = new List<Item>();

            IEnumerable<SimPersonality> personalities = SP::NRaas.StoryProgression.Main.Personalities.AllPersonalities;
            foreach (SimPersonality personality in personalities)
            {
                if (personality.IsLeaderless) continue;

                choices.Add(new Item(personality));
            }

            CommonSelection<Item>.Results results = new CommonSelection<Item>(Name, choices).SelectMultiple();

            string msg = null;

            foreach (Item choice in results)
            {
                SimPersonality personality = choice.Value;

                msg += Common.NewLine + personality.GetLocalizedName();

                if (personality.Me != null)
                {
                    msg += Common.Localize(GetTitlePrefix() + ":Leader", personality.Me.IsFemale, personality.IsFemaleLocalization(), new object[] { personality.Me });
                }
                else
                {
                    msg += Common.Localize(GetTitlePrefix() + ":NoLeader", personality.IsFemaleLocalization());
                }

                int memberCount = personality.GetClanMembers(false).Count;
                if (memberCount > 0)
                {
                    msg += Common.Localize(GetTitlePrefix() + ":Members", personality.IsFemaleLocalization(), new object[] { memberCount });
                }

                Dictionary<SimDescription,bool> opponents = new Dictionary<SimDescription,bool>();

                foreach(SimPersonality opponent in personalities)
                {
                    if (opponent.IsOpposing(personality))
                    {
                        foreach(SimDescription sim in opponent.GetClanMembers(true))
                        {
                            if (opponents.ContainsKey(sim)) continue;

                            opponents.Add(sim, true);
                        }
                    }
                }

                if (opponents.Count > 0)
                {
                    msg += Common.Localize(GetTitlePrefix() + ":Opposing", personality.IsFemaleLocalization(), new object[] { opponents.Count });
                }

                msg += Common.NewLine;
            }

            return msg;
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Sims3.UI.SimpleMessageDialog.Show(Common.Localize("Personalities:Header"), GetDetails());
            return OptionResult.SuccessRetain;
        }
    }
}
