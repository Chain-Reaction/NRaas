using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public class ActiveCareerToneScenario : CareerToneScenario
    {
        public ActiveCareerToneScenario(SimDescription sim)
            : base (sim)
        { }
        protected ActiveCareerToneScenario(ActiveCareerToneScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Story) return null;

            return "ActiveTones";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected bool ChatOnly
        {
            get { return GetValue<ChatOnlyOption,bool>(); }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool ConfigureInteraction(RabbitHole.RabbitHoleInteraction<Sim, RabbitHole> wk, List<ITone> allTones)
        {
            if (ChatOnly)
            {
                // Search for the Meet new People tone
                foreach (ITone displaytone in allTones)
                {
                    MeetCoworkersTone careertone = displaytone as MeetCoworkersTone;
                    if (careertone == null) continue;

                    if (ManagerCareer.VerifyTone(careertone))
                    {
                        try
                        {
                            wk.CurrentITone = careertone;
                            return true;
                        }
                        catch (Exception e)
                        {
                            Common.DebugException(careertone.Name(), e);

                            wk.CurrentITone = null;
                        }
                    }
                    break;
                }

                // Search for the Hang with Coworkers tone
                foreach (ITone displaytone in allTones)
                {
                    HangWithCoworkersTone careertone = displaytone as HangWithCoworkersTone;
                    if (careertone == null) continue;

                    if (ManagerCareer.VerifyTone(careertone))
                    {
                        try
                        {
                            wk.CurrentITone = careertone;
                            return true;
                        }
                        catch (Exception e)
                        {
                            Common.DebugException(careertone.Name(), e);

                            wk.CurrentITone = null;
                        }
                    }
                    break;
                }
            }

            return base.ConfigureInteraction(wk, allTones);
        }

        protected override ICollection<SimDescription> GetSims()
        {
            if (Household.ActiveHousehold == null) return null;

            return Household.ActiveHousehold.AllSimDescriptions;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!base.Allow(sim)) return false;

            if (!sim.CreatedSim.IsSelectable) return false;

            return true;
        }

        public override Scenario Clone()
        {
            return new ActiveCareerToneScenario(this);
        }

        public class Option : BooleanManagerOptionItem<ManagerCareer>
        {
            public Option()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "AutoTonesforActive";
            }
        }

        public class ChatOnlyOption : BooleanManagerOptionItem<ManagerCareer>
        {
            public ChatOnlyOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "OnlyChatTonesforActive";
            }
        }
    }
}
