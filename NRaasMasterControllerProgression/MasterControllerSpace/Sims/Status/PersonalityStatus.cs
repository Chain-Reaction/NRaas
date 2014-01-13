extern alias SP;

using SimPersonality = SP::NRaas.StoryProgressionSpace.Personalities.SimPersonality;

using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.SelectionCriteria;
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

namespace NRaas.MasterControllerSpace.Sims.Status
{
    public class PersonalityStatus : SimFromList, IStatusOption
    {
        protected override OptionResult RunResult
        {
            get { return OptionResult.SuccessRetain; }
        }

        public override string GetTitlePrefix()
        {
            return "PersonalityStatus";
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

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            return Perform(me);
        }

        public string GetDetails(IMiniSimDescription me)
        {
            string msg = null;

            try
            {
                msg = PersonalStatus.GetHeader(me);

                SimDescription simDesc = me as SimDescription;

                foreach (SimPersonality personality in SP::NRaas.StoryProgression.Main.Personalities.GetClanMembership(simDesc, true))
                {
                    msg += Common.NewLine + personality.GetLocalizedName();

                    if (personality.Me != null)
                    {
                        msg += Common.NewLine + Common.Localize("Personalities:Leader", personality.IsFemaleLocalization(), new object[] { personality.Me });
                    }

                    int memberCount = personality.GetClanMembers(false).Count;
                    if (memberCount > 0)
                    {
                        msg += Common.Localize("Personalities:Members", personality.IsFemaleLocalization(), new object[] { memberCount });
                    }

                    Dictionary<SimDescription, bool> opponents = new Dictionary<SimDescription, bool>();

                    foreach (SimPersonality opponent in SP::NRaas.StoryProgression.Main.Personalities.AllPersonalities)
                    {
                        if (opponent.IsOpposing(personality))
                        {
                            int count = 0;

                            foreach (SimDescription sim in opponent.GetClanMembers(true))
                            {
                                if (opponents.ContainsKey(sim)) continue;

                                opponents.Add(sim, true);
                                count++;
                            }

                            if (count > 0)
                            {
                                msg += Common.NewLine + Common.Localize(GetTitlePrefix() + ":Opposing", opponent.IsFemaleLocalization(), new object[] { opponent.GetLocalizedName(), count });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(me.FullName, e);

                msg += Common.NewLine + "END OF LINE";
            }

            return msg;
        }

        protected bool Perform(IMiniSimDescription me)
        {
            SimpleMessageDialog.Show(Name, GetDetails(me));
            return true;
        }
    }
}
