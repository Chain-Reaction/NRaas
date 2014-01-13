using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
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

namespace NRaas.StoryProgressionSpace.Scenarios
{
    public abstract class DualSimScenario : SimScenario 
    {
        SimDescription mTarget = null;

        SimDescription mNotTarget = null;

        SimScenarioFilter mTargetFilter = null;
        SimScenarioFilter mMutualFilter = null;

        SimDescription mMutualSelection = null;

        int mTargetContinueChance = 0;

        int mTargetMaximumCount = 0;

        bool mTargetGatheringFailure = false;

        public DualSimScenario()
        { }
        protected DualSimScenario(SimDescription sim)
            : base(sim)
        { }
        protected DualSimScenario(SimDescription sim, SimDescription target)
            :  base(sim)
        {
            mTarget = target;
        }
        protected DualSimScenario(DualSimScenario scenario)
            : base (scenario)
        {
            mTarget = scenario.Target;
            mNotTarget = scenario.NotTarget;
            mTargetFilter = scenario.mTargetFilter;
            mMutualFilter = scenario.mMutualFilter;
            mTargetContinueChance = scenario.mTargetContinueChance;
            mTargetMaximumCount = scenario.mTargetMaximumCount;
            mMutualSelection = scenario.mMutualSelection;
            //mTargetGatheringFailure = scenario.mTargetGatheringFailure;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "Target=" + mTarget;
            text += Common.NewLine + "NotTarget=" + mNotTarget;
            text += Common.NewLine + "TargetFilter" + Common.NewLine + mTargetFilter;
            text += Common.NewLine + "MutualFilter" + Common.NewLine + mMutualFilter;
            text += Common.NewLine + "TargetContinueChance=" + mTargetContinueChance;
            text += Common.NewLine + "TargetMaximumCount=" + mTargetMaximumCount;

            return text;
        }

        protected SimDescription Mutual
        {
            get { return mMutualSelection; }
        }

        public SimDescription Target
        {
            get { return mTarget; }
            set { mTarget = value; }
        }

        public SimDescription NotTarget
        {
            get { return mNotTarget; }
            set { mNotTarget = value; }
        }

        protected override object TargetObjectId
        {
            get { return Target; }
        }

        protected SimScenarioFilter TargetFilter
        {
            get { return mTargetFilter; }
            set { mTargetFilter = value; }
        }

        protected virtual bool TargetAllowActive
        {
            get { return AllowActive; }
        }

        protected virtual bool TargetCheckBusy
        {
            get { return CheckBusy; }
        }

        protected override int ContinueChance
        {
            get { return 0; }
        }

        protected virtual int TargetContinueChance
        {
            get { return mTargetContinueChance; }
        }

        protected virtual int TargetMaximum
        {
            get { return mTargetMaximumCount; }
        }

        protected virtual bool TestOpposing
        {
            get { return false; }
        }

        public override StoryProgressionObject Manager
        {
            set
            {
                base.Manager = value;

                if (mMutualFilter != null)
                {
                    mMutualFilter.UpdateManager(value);
                }

                if (mTargetFilter != null)
                {
                    mTargetFilter.UpdateManager(value);
                }
            }
        }

        protected void ExchangeSims()
        {
            SimDescription sim = Sim;
            Sim = Target;
            Target = sim;
        }

        public override string GetIDName()
        {
            string text = base.GetIDName();

            if (mMutualSelection != null)
            {
                text += Common.NewLine + "M: " + mMutualSelection.FullName;
            }

            if (Target != null)
            {
                text += Common.NewLine + "T: " + Target.FullName;
            }

            return text;
        }

        protected override bool Matches(Scenario scenario)
        {
            if (!base.Matches(scenario)) return false;

            DualSimScenario simScenario = scenario as DualSimScenario;
            if (simScenario == null) return false;

            return (Target == simScenario.Target);
        }

        protected override GatherResult Gather(List<Scenario> list, ref int continueChance, ref int maximum, ref bool random)
        {
            if ((mMutualSelection == null) && (mMutualFilter != null) && (mMutualFilter.Enabled))
            {
                ICollection<SimDescription> mutuals = mMutualFilter.Filter(new SimScenarioFilter.Parameters(this, false), "Mutual", null);
                if ((mutuals == null) || (mutuals.Count == 0))
                {
                    IncStat("No Mutual");

                    return GatherResult.Failure;
                }

                mMutualSelection = RandomUtil.GetRandomObjectFromList(new List<SimDescription>(mutuals));

                IncStat("Mutual Selected");
            }

            if (Sim != null)
            {
                if (!Allow(Sim)) return GatherResult.Failure;
            }
            else 
            {
                return base.Gather(list, ref continueChance, ref maximum, ref random);
            }

            continueChance = TargetContinueChance;
            maximum = TargetMaximum;

            return TargetGather(list, ref random);
        }

        protected override ICollection<SimDescription> GetFilteredSims(SimDescription sim)
        {
            if (mMutualSelection != null)
            {
                return base.GetFilteredSims(mMutualSelection);
            }
            else
            {
                return base.GetFilteredSims(sim);
            }
        }

        protected abstract ICollection<SimDescription> GetTargets(SimDescription sim);

        protected ICollection<SimDescription> GetFilteredTargets(SimDescription sim)
        {
            return GetFilteredTargets(sim, null);
        }
        protected ICollection<SimDescription> GetFilteredTargets(SimDescription sim, ICollection<SimDescription> potentials)
        {
            if (mMutualSelection != null)
            {
                return mTargetFilter.Filter(new SimScenarioFilter.Parameters(this, true), "Targets", mMutualSelection, potentials);
            }
            else
            {
                return mTargetFilter.Filter(new SimScenarioFilter.Parameters(this, true), "Targets", sim, potentials);
            }
        }

        protected virtual void PerformTargetGatheringFailure(SimDescription sim)
        { }

        public List<SimDescription> GetAllowedTargets(SimDescription sim)
        {
            List<SimDescription> results = new List<SimDescription>();

            if (Target != null)
            {
                if (TargetTest(Target))
                {
                    results.Add(Target);
                }
            }
            else
            {
                ICollection<SimDescription> sims = null;
                using (Common.TestSpan span = new Common.TestSpan(Scenarios, "GetTargets " + UnlocalizedName))
                {
                    sims = GetTargets(sim);
                }
                if (sims != null)
                {
                    AddStat("Target Potentials", sims.Count);

                    foreach (SimDescription target in new List<SimDescription> (sims))
                    {
                        Target = target;
                        if (TargetTest(target))
                        {
                            IncStat("Included");

                            results.Add(target);
                        }

                        Main.Sleep("DualSimScenario:GetAllowedTargets");
                    }

                    AddStat("Target Allowed", results.Count);
                }
                else
                {
                    IncStat("Empty Target List");
                }
            }

            return results;
        }

        protected virtual GatherResult TargetGather(List<Scenario> results, ref bool random)
        {
            if (Target != null)
            {
                if (TargetTest(Target))
                {
                    return GatherResult.Update;
                }
            }
            else
            {
                List<SimDescription> allowed = GetAllowedTargets(Sim);

                Target = null;
                random = !TargetSort(Sim, ref allowed);

                AddStat("After Sort", allowed.Count);

                foreach (SimDescription sim in allowed)
                {
                    Target = sim;
                    Scenario scenario = Clone();
                    if (scenario != null)
                    {
                        results.Add(scenario);
                    }
                }

                Target = null;

                if (results.Count > 0)
                {
                    return GatherResult.Success;
                }

                try
                {
                    mTargetGatheringFailure = true;

                    ICollection<SimDescription> sims = GetTargets(Sim);
                    if ((sims == null) || (sims.Count == 0))
                    {
                        PerformTargetGatheringFailure(Sim);
                    }
                }
                finally
                {
                    mTargetGatheringFailure = false;
                }
            }

            return GatherResult.Failure;
        }

        protected override bool UsesSim(ulong sim)
        {
            if (Target != null)
            {
                if (Target.SimDescriptionId == sim) return true;
            }

            return base.UsesSim(sim);
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mTargetContinueChance = row.GetInt("TargetContinueChance", mTargetContinueChance);

            mTargetMaximumCount = row.GetInt("TargetMaximumCount", mTargetMaximumCount);

            mMutualFilter = new SimScenarioFilter();
            if (!mMutualFilter.Parse(row, Manager, this, "Mutual", false, ref error))
            {
                return false;
            }

            mTargetFilter = new SimScenarioFilter();
            if (!mTargetFilter.Parse(row, Manager, this, "Target", true, ref error))
            {
                return false;
            }

            return base.Parse(row, ref error);
        }

        protected override bool Allow()
        {
            if (!base.Allow()) return false;

            if ((Sim != null) && (Target != null))
            {
                return TargetTest(Target);
            }
            else
            {
                return true;
            }
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!base.Allow(sim)) return false;

            return CommonAllow(sim);
        }

        protected virtual bool AllowSpecies(SimDescription sim, SimDescription target)
        {
            return SimTypes.IsEquivalentSpecies(sim, target);
        }

        protected virtual bool AllowStorySpecies(SimDescription sim, SimDescription target)
        {
            if ((sim.IsHuman) || (target.IsHuman))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected virtual bool TargetSort(SimDescription sim, ref List<SimDescription> targets)
        {
            return false;
        }

        protected bool TargetTest(SimDescription sim)
        {
            using (Common.TestSpan span = new Common.TestSpan(Scenarios, "AllowDualSim " + UnlocalizedName))
            {
                return TargetAllow(sim);
            }
        }

        protected virtual bool TargetAllow(SimDescription target)
        {
            if (SimTypes.IsPassporter(target))
            {
                IncStat("Simport");
                return false;
            }
            else if (Sim == target) 
            {
                IncStat("Is Actor");
                return false;
            }
            else if (target == mNotTarget)
            {
                IncStat("Not Target");
                return false;
            }
            else if (!AllowSpecies(Sim, target))
            {
                IncStat("Species Match Denied");
                return false;
            }

            if (TestOpposing)
            {
                if (Personalities.IsOpposing(this, Sim, target, false))
                {
                    IncStat("Opposing");
                    return false;
                }
            }

            if (!TargetAllowActive)
            {
                if (SimTypes.IsSelectable(target))
                {
                    IncStat("Active");
                    return false;
                }
            }

            if ((!mTargetGatheringFailure) && (TargetCheckBusy))
            {
                if (Situations.IsBusy(this, target, true))
                {
                    IncStat("Busy");
                    return false;
                }
            }

            if (!CommonAllow(target)) return false;

            if ((!Sim.IsHuman) || (!target.IsHuman))
            {
                IncStat("Species Match " + Sim.Species + " - " + target.Species);
            }
            return true;
        }

        protected virtual bool CommonAllow(SimDescription sim)
        {
            return true;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (parameters == null)
            {
                if (Sim == null) return null;

                if (Target == null) return null;

                if (!AllowStorySpecies(Sim, Target))
                {
                    return null;
                }

                parameters = new object[] { Sim, Target };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        protected override ManagerStory.Story PrintDebuggingStory(object[] parameters)
        {
            if (parameters == null)
            {
                if ((Sim != null) && (Target != null))
                {
                    parameters = new object[] { Sim, Target };
                }
            }

            return base.PrintDebuggingStory(parameters);
        }

        protected override ManagerStory.Story PrintFormattedStory(StoryProgressionObject manager, string text, string summaryKey, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (parameters == null)
            {
                if (Sim == null) return null;

                if (Target == null) return null;

                parameters = new object[] { Sim, Target };
            }

            return base.PrintFormattedStory(manager, text, summaryKey, parameters, extended, logging);
        }
    }
}
