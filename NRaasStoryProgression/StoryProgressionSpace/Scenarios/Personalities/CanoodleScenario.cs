using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Flirts;
using NRaas.StoryProgressionSpace.Scenarios.Propagation;
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
    public class CanoodleScenario : FlirtScenario, IHasPersonality
    {
        enum TypeOfStory
        {
            None, // SteadyDumped, MarriedDumped
            Standard,  // Canoodle, SteadyDumped, MarriedDumped
            Segregated, // EarlyFlirt, NewFlirt, OldFlirt, SteadyDumped, MarriedDumped
            Both // Canoodle, EarlyFlirt, NewFlirt, OldFlirt, SteadyDumped, MarriedDumped
        }

        /*
         * Affair Stories
         * 
         * SteadyTryst, SteadyTrystSecond, SteadyTrystDuo, MarriedTryst, MarriedTrystSecond, MarriedTrystDuo
         * 
         */

        WeightOption.NameOption mName = null;

        bool mFail = false;

        bool mAllowPartner = false;

        WeightScenarioHelper mSuccess = null;
        WeightScenarioHelper mFailure = null;

        string mAcceptanceScoring = null;

        TypeOfStory mTypeOfStory = TypeOfStory.None;

        ManagerRomance.AffairStory mAffairStory = ManagerRomance.AffairStory.All;

        IntegerOption.OptionValue mChanceOfPregnancy = null;

        public CanoodleScenario()
            : base(10)
        { }
        protected CanoodleScenario(CanoodleScenario scenario)
            : base (scenario)
        {
            mName = scenario.mName;
            //mFail = scenario.mFail;
            mAllowPartner = scenario.mAllowPartner;
            mSuccess = scenario.mSuccess;
            mFailure = scenario.mFailure;
            mAcceptanceScoring = scenario.mAcceptanceScoring;
            mTypeOfStory = scenario.mTypeOfStory;
            mAffairStory = scenario.mAffairStory;
            mChanceOfPregnancy = scenario.mChanceOfPregnancy;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "ChanceOfPregnancy=" + mChanceOfPregnancy;
            text += Common.NewLine + "AllowPartner=" + mAllowPartner;
            text += Common.NewLine + "AcceptanceScoring=" + mAcceptanceScoring;
            text += Common.NewLine + "TypeOfStory=" + mTypeOfStory;
            text += Common.NewLine + "AffairStory=" + mAffairStory;
            text += Common.NewLine + "Success" + Common.NewLine + mSuccess;
            text += Common.NewLine + "Failure" + Common.NewLine + mFailure;

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
                if (mFail)
                {
                    return mName + "Fail";
                }
                else
                {
                    return mName.ToString();
                }
            }
        }

        protected override bool CheckBusy
        {
            get { return base.ShouldPush; }
        }

        protected override bool TestFlirtCooldown
        {
            get { return false; }
        }

        protected override int InitialReportChance
        {
            get
            {
                if (mTypeOfStory == TypeOfStory.Both)
                {
                    return 100;
                }
                else
                {
                    return base.InitialReportChance;
                }
            }
        }

        protected override int ContinueReportChance
        {
            get
            {
                if (mTypeOfStory == TypeOfStory.Both)
                {
                    return 0;
                }
                else
                {
                    return base.ContinueReportChance;
                }
            }
        }

        protected override bool TestAffair(SimDescription sim, SimDescription target)
        {
            if (!Filter.AllowAffair) return false;

            if (!TargetFilter.AllowAffair) return false;

            Managers.Manager.AllowCheck check = Managers.Manager.AllowCheck.None;

            if (!Romances.AllowAdultery(this, sim, check)) return false;

            if (target.Partner != null)
            {
                if (!Romances.AllowLiaison(this, sim, check)) return false;
            }

            if (!Romances.AllowAdultery(this, target, check)) return false;

            if (sim.Partner != null)
            {
                if (!Romances.AllowLiaison(this, target, check)) return false;
            }

            return true;
        }

        protected override ManagerRomance.AffairStory AffairStory
        {
            get
            {
                return mAffairStory;
            }
        }

        protected override int ReportSegregatedChance
        {
            get 
            {
                switch (mTypeOfStory)
                {
                    case TypeOfStory.None:
                    case TypeOfStory.Standard:
                        return 0;
                }

                return base.ReportSegregatedChance;
            }
        }

        protected override bool ShouldReport
        {
            get
            {
                switch (mTypeOfStory)
                {
                    case TypeOfStory.None:
                    case TypeOfStory.Segregated:
                        return false;
                }

                return base.ShouldReport;
            }
        }

        protected override int PregnancyChance
        {
            get 
            { 
                return mChanceOfPregnancy.Value; 
            }
        }

        protected override bool TestRelationship
        {
            get { return false; }
        }

        public override bool ShouldPush
        {
            get
            {
                if (mFail)
                {
                    return mFailure.ShouldPush(base.ShouldPush);
                }
                else
                {
                    return mSuccess.ShouldPush(base.ShouldPush);
                }
            }
        }

        public override StoryProgressionObject Manager
        {
            set
            {
                base.Manager = value;

                if (mSuccess != null)
                {
                    mSuccess.UpdateManager(value);
                }

                if (mFailure != null)
                {
                    mFailure.UpdateManager(value);
                }

                if (mChanceOfPregnancy != null)
                {
                    mChanceOfPregnancy.UpdateManager(value);
                }
            }
        }

        public SimPersonality Personality
        {
            get { return Manager as SimPersonality; }
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mName = new WeightOption.NameOption(row);

            mChanceOfPregnancy = new IntegerOption.OptionValue(-1);

            if (row.Exists("ChanceOfPregnancy"))
            {
                if (!mChanceOfPregnancy.Parse(row, "ChanceOfPregnancy", Manager, this, ref error))
                {
                    return false;
                }
            }

            mAllowPartner = row.GetBool("AllowPartner");

            if (!row.Exists("TypeOfStory"))
            {
                error = "TypeOfStory missing";
                return false;
            }
            else if (!ParserFunctions.TryParseEnum<TypeOfStory>(row.GetString("TypeOfStory"), out mTypeOfStory, TypeOfStory.None))
            {
                error = "TypeOfStory unknown";
                return false;
            }

            mSuccess = new WeightScenarioHelper(Origin.FromRomanticBetrayal);
            if (!mSuccess.Parse(row, Manager, this, "Success", ref error))
            {
                return false;
            }

            mFailure = new WeightScenarioHelper(Origin.FromRomanticBetrayal);
            if (!mFailure.Parse(row, Manager, this, "Failure", ref error))
            {
                return false;
            }

            if (!base.Parse(row, ref error))
            {
                return false;
            }

            if (!row.Exists("AffairStory"))
            {
                error = "AffairStory missing";
                return false;
            }

            if (!ParserFunctions.TryParseEnum<ManagerRomance.AffairStory>(row.GetString("AffairStory"), out mAffairStory, ManagerRomance.AffairStory.None))
            {
                error = "AffairStory unknown";
                return false;
            }

            if (mAffairStory != ManagerRomance.AffairStory.None)
            {
                if ((!Filter.AllowAffair) && (!TargetFilter.AllowAffair))
                {
                    error = "ActorAllowAffair or TargetAllowAffair must be True";
                    return false;
                }
            }
            else
            {
                if ((Filter.AllowAffair) || (TargetFilter.AllowAffair))
                {
                    error = "ActorAllowAffair and TargetAllowAffair must be False";
                    return false;
                }
            }

            mAcceptanceScoring = row.GetString("AcceptanceScoring");

            return true;
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return GetFilteredSims(Personalities.GetClanLeader(Manager));
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return GetFilteredTargets(sim);
        }

        protected override bool Allow()
        {
            if (Manager.GetValue<WeightOption, int>(mName.WeightName) <= 0) return false;

            return base.Allow();
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!Personalities.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected override bool TargetAllow(SimDescription target)
        {
            if ((!mAllowPartner) && (Sim.Partner == target))
            {
                IncStat("Own Partner");
                return false;
            }

            // For the Sim, we could not test Affair until both sims were available
            if ((Sim.Partner != null) && (Sim.Partner != target))
            {
                if (!Filter.AllowAffair)
                {
                    IncStat("Actor Affair Denied");
                    return false;
                }
                else if (AddScoring("FlirtyPartner", Sim) < 0)
                {
                    IncStat("Not Flirty");
                    return false;
                }
            }

            return base.TargetAllow(target);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!mSuccess.TestBeforehand(Manager, Sim, Target))
            {
                IncStat("Success TestBeforehand Fail");
                return false;
            }

            if (!mFailure.TestBeforehand(Manager, Sim, Target))
            {
                IncStat("Failure TestBeforehand Fail");
                return false;
            }

            mFail = ((!string.IsNullOrEmpty(mAcceptanceScoring)) && (AddScoring("Acceptance", ScoringLookup.GetScore(mAcceptanceScoring, Target, Sim)) <= 0));
                
            if (!mFail)
            {
                if (!base.PrivateUpdate(frame)) return false;

                Add(frame, new PropagateClanDelightScenario(Sim, Manager, Origin.FromSocialization), ScenarioResult.Start);

                mSuccess.Perform(this, frame, "Success", Sim, Target);
                return true;
            }
            else
            {
                mFailure.Perform(this, frame, "Failure", Sim, Target);
                return false;
            }
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new CanoodleScenario(this);
        }
    }
}
