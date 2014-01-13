using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Sims
{
    public abstract class SimUpdateScenario : ScheduledSoloScenario
    {
        public SimUpdateScenario()
        { }
        protected SimUpdateScenario(SimUpdateScenario scenario)
            : base (scenario)
        { }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected abstract bool ContinuousUpdate
        {
            get;
        }

        protected override bool Allow(bool fullUpdate, bool initialPass)
        {
            if (initialPass) return true;

            if (ContinuousUpdate) return true;
            
            return base.Allow(fullUpdate, initialPass);
        }

        protected abstract void PrivatePerform(SimDescription sim, SimData data, ScenarioFrame frame);

        public void Perform(IEnumerable<SimDescription> sims, ScenarioFrame frame)
        {
            foreach (SimDescription sim in sims)
            {
                Perform(GetData(sim), frame);
            }
        }

        public void Perform(SimData data, ScenarioFrame frame)
        {
            if (data.SimDescription == null) return;

            IncStat(data.SimDescription.FullName, Common.DebugLevel.Logging);

            try
            {
                PrivatePerform(data.SimDescription, data, frame);
            }
            catch (Exception e)
            {
                Common.Exception(data.SimDescription, e);
            }
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Options.UpdateSimData(this, frame);
            return true;
        }
    }
}
