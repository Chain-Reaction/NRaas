using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios
{
    public abstract class SimScenario : Scenario
    {
        SimDescription mSim;

        SimDescription mNotSim;

        SimScenarioFilter mFilter;

        protected SimScenario()
        { }
        protected SimScenario(SimDescription sim)
        {
            mSim = sim;
        }
        protected SimScenario(SimScenario scenario)
            : base (scenario)
        {
            mSim = scenario.Sim;
            mNotSim = scenario.mNotSim;
            mFilter = scenario.mFilter;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "Sim=" + mSim;
            text += Common.NewLine + "NotSim=" + mNotSim;
            text += Common.NewLine + "Filter" + Common.NewLine + mFilter;

            return text;
        }

        public SimDescription Sim
        {
            get { return mSim; }
            set { mSim = value; }
        }

        public SimDescription NotSim
        {
            get { return mNotSim; }
            set { mNotSim = value; }
        }

        protected override object ActorObjectId
        {
            get { return Sim; }
        }

        public override IAlarmOwner Owner
        {
            get { return Sim; }
        }

        public override string GetIDName()
        {
            string text = base.GetIDName();

            if (Sim != null)
            {
                text += Common.NewLine + "S: " + Sim.FullName;
            }

            return text;
        }

        protected abstract ICollection<SimDescription> GetSims();

        protected SimScenarioFilter Filter
        {
            get { return mFilter; }
            set { mFilter = value; }
        }

        protected virtual ICollection<SimDescription> GetFilteredSims(SimDescription sim)
        {
            return mFilter.Filter(new SimScenarioFilter.Parameters(this, true), "Actors", sim);
        }

        protected virtual bool AllowActive
        {
            get { return false; }
        }

        protected abstract bool CheckBusy
        { get; }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mFilter = new SimScenarioFilter();

            if (!mFilter.Parse(row, Manager, this, "Actor", true, ref error))
            {
                return false;
            }

            return base.Parse(row, ref error);
        }

        protected override bool Allow()
        {
            if (!base.Allow()) return false;

            if (Sim == null)
            {
                return true;
            } 
            else 
            {
                return Test(Sim);
            }
        }

        protected virtual bool AllowSpecies(SimDescription sim)
        {
            return sim.IsHuman;
        }

        protected bool Test(SimDescription sim)
        {
            using (Common.TestSpan span = new Common.TestSpan(Scenarios, "AllowSim " + UnlocalizedName))
            {
                return Allow(sim);
            }
        }

        protected virtual bool Allow (SimDescription sim)
        {
            if (SimTypes.IsPassporter(sim))
            {
                IncStat("Simport");
                return false;
            }
            else if (sim == mNotSim)
            {
                IncStat("Not Sim");
                return false;
            }
            else if (!AllowSpecies(sim))
            {
                IncStat("Species Denied");
                return false;
            }

            if (!AllowActive)
            {
                if (SimTypes.IsSelectable(sim))
                {
                    IncStat("Active");
                    return false;
                }
            }

            if (CheckBusy)
            {
                if (Situations.IsBusy(this, sim, true))
                {
                    IncStat("Busy");
                    return false;
                }
            }

            if (!sim.IsHuman)
            {
                IncStat("Species " + sim.Species);
            }
            return true;
        }

        protected override bool Matches(Scenario scenario)
        {
            if (!base.Matches(scenario)) return false;

            SimScenario simScenario = scenario as SimScenario;
            if (simScenario == null) return false;

            return (Sim == simScenario.Sim);
        }

        protected override bool UsesSim(ulong sim)
        {
            if (Sim != null)
            {
                return (Sim.SimDescriptionId == sim);
            }
            else
            {
                return false;
            }
        }

        public override bool ManualSetup(StoryProgressionObject manager)
        {
            if (!base.ManualSetup(manager)) return false;

            int originalID = Scenarios.TrackingID;
            Scenarios.TrackingID = ID;

            SimSelection sim = new SimSelection(this);
            try
            {
                if (!sim.Perform())
                {
                    if (!AcceptCancelDialog.Show(Common.Localize("FireScenario:ApplyAll")))
                    {
                        return false;
                    }
                }

                Scenarios.WriteTrack(false);
            }
            finally
            {
                Scenarios.TrackingID = originalID;
            }

            Sim = sim.SimDescription;
            return true;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (parameters == null)
            {
                if (Sim == null) return null;

                parameters = new object[] { Sim };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        protected override ManagerStory.Story PrintDebuggingStory(object[] parameters)
        {
            if (parameters == null)
            {
                if (Sim != null)
                {
                    parameters = new object[] { Sim };
                }
            }

            return base.PrintDebuggingStory(parameters);
        }

        protected override ManagerStory.Story PrintFormattedStory(StoryProgressionObject manager, string text, string summaryKey, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (parameters == null)
            {
                if (Sim == null) return null;

                parameters = new object[] { Sim };
            }

            return base.PrintFormattedStory(manager, text, summaryKey, parameters, extended, logging);
        }

        protected virtual bool Sort(ref List<SimDescription> sims)
        {
            return false;
        }

        public List<SimDescription> GetAllowedSims()
        {
            List<SimDescription> results = new List<SimDescription>();

            if (Sim != null)
            {
                if (Test(Sim))
                {
                    results.Add(Sim);
                }
            }
            else
            {
                ICollection<SimDescription> sims = null;
                using (Common.TestSpan span = new Common.TestSpan(Scenarios, "GetSims " + UnlocalizedName))
                {
                    sims = GetSims();
                }

                if (sims != null)
                {
                    AddStat("Sim Potentials", sims.Count);

                    foreach (SimDescription sim in new List<SimDescription> (sims))
                    {
                        Sim = sim;
                        if (Test(sim))
                        {
                            IncStat("Included");

                            results.Add(sim);
                        }

                        Main.Sleep("SimScenario:GetAllowedSims");
                    }

                    AddStat("Sim Allowed", results.Count);
                }
            }

            return results;
        }

        protected override GatherResult Gather(List<Scenario> results, ref int continueChance, ref int maximum, ref bool random)
        {
            if (Sim != null)
            {
                if (Test(Sim))
                {
                    return GatherResult.Update;
                }
            }
            else
            {
                List<SimDescription> allowed = GetAllowedSims();

                Sim = null;
                random = !Sort(ref allowed);

                AddStat("After Sort", allowed.Count);

                foreach (SimDescription sim in allowed)
                {
                    Sim = sim;

                    Scenario scenario = Clone();
                    if (scenario != null)
                    {
                        results.Add(scenario);
                    }
                }

                Sim = null;

                if (results.Count > 0)
                {
                    return GatherResult.Success;
                }
            }

            return GatherResult.Failure;
        }

        protected class SimSelection : SimIDOption
        {
            SimScenario mScenario = null;

            public SimSelection(SimScenario scenario)
            {
                mScenario = scenario;
            }

            public override string GetTitlePrefix()
            {
                return null;
            }

            protected override bool Allow(SimDescription me, SimScenarioFilter scoring)
            {
                mScenario.Scenarios.Track(me.FullName);

                if (!base.Allow(me, scoring)) return false;

                if (scoring != null)
                {
                    if (!scoring.Test(new SimScenarioFilter.Parameters(mScenario), "SimSelection", null, me)) return false;
                }

                mScenario.Sim = me;

                return mScenario.Test(me);
            }

            protected override SimScenarioFilter GetScoring()
            {
                return null;
            }
        }
    }
}
