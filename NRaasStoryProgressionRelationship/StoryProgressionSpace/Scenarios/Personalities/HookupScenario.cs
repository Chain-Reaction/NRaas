using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Personalities
{
    public class HookupScenario : PartneringScenario, IHasPersonality
    {
        WeightOption.NameOption mName = null;

        bool mSeperateAdultStory;

        WeightScenarioHelper mSuccess = null;

        public HookupScenario()
        { }
        protected HookupScenario(HookupScenario scenario)
            : base (scenario)
        {
            mName = scenario.mName;
            mSeperateAdultStory = scenario.mSeperateAdultStory;
            mSuccess = scenario.mSuccess;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "SeperateAdultStory=" + mSeperateAdultStory;
            text += Common.NewLine + "Success" + Common.NewLine + mSuccess;

            return text;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return mName.WeightName;
            }
            else
            {
                return mName.ToString();
            }
        }

        protected override bool AllowAffair
        {
            get { return (Filter.AllowAffair && TargetFilter.AllowAffair); }
        }

        protected override bool HandleAdultSeperately
        {
            get { return mSeperateAdultStory; }
        }

        protected override bool CheckBusy
        {
            get { return base.ShouldPush; }
        }

        public override bool ShouldPush
        {
            get
            {
                return mSuccess.ShouldPush(base.ShouldPush);
            }
        }

        public SimPersonality Personality
        {
            get { return Manager as SimPersonality; }
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mName = new WeightOption.NameOption(row);

            mSeperateAdultStory = row.GetBool("SeperateAdultStory");

            mSuccess = new WeightScenarioHelper(Origin.FromSocialization);
            if (!mSuccess.Parse(row, Manager, this, "Success", ref error))
            {
                return false;
            }

            return base.Parse(row, ref error);
        }

        protected override bool Allow()
        {
            if (Manager.GetValue<WeightOption, int>(mName.WeightName) <= 0) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return GetFilteredSims(Personalities.GetClanLeader(Manager));
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return GetFilteredTargets(sim);
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if ((sim.Partner != null) && (AddScoring("Breakup Cooldown", GetElapsedTime<DayOfLastRomanceOption>(sim) - GetValue<BreakupScenario.MinTimeFromRomanceToBreakupOption, int>()) < 0))
            {
                AddStat("Too Early", GetElapsedTime<DayOfLastRomanceOption>(sim));
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (!Personalities.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!mSuccess.TestBeforehand(Manager, Sim, Target))
            {
                IncStat("Success TestBeforehand Fail");
                return false;
            }

            if (!base.PrivateUpdate(frame)) return false;

            mSuccess.Perform(this, frame, "Success", Sim, Target);

            return true;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Manager;
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new HookupScenario(this);
        }
    }
}
