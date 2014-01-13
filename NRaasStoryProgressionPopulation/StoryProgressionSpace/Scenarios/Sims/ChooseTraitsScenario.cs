using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Sims
{
    public class ChooseTraitsScenario : AgeUpBaseScenario
    {
        public ChooseTraitsScenario()
        { }
        public ChooseTraitsScenario(SimDescription sim)
            : base (sim)
        { }
        protected ChooseTraitsScenario(ChooseTraitsScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ChooseTraits";
        }

        protected override int MaximumReschedules
        {
	        get { return 4; }
        }

        protected override int Rescheduling
        {
            get { return 60; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.LotHome == null)
            {
                IncStat("Not Resident");
                return false;
            }
            else if (sim.AdultOrAbove)
            {
                IncStat("Too Old");
                return false;
            }
            else if (sim.TraitManager == null)
            {
                IncStat("No Manager");
                return false;
            }
            else if (!Sims.MatchesAlertLevel(sim))
            {
                IncStat("No Match");
                return false;
            }

            return base.Allow(sim);
        }

        public bool Run(Manager manager)
        {
            if (Sim == null) return false;

            Manager = manager;

            return PrivatePerform();
        }

        protected bool PrivatePerform()
        {
            try
            {
                if (Sim.CreatedSim == null)
                {
                    IncStat("Hibernating");
                    return false;
                }

                HudModel.ShowTraitsPickerDialog(0, Sim.CreatedSim);
            }
            catch (Exception e)
            {
                Common.DebugException(Sim, e);
            }
            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            base.PrivateUpdate(frame);

            return PrivatePerform ();
        }

        public override Scenario Clone()
        {
            return new ChooseTraitsScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerSim,ChooseTraitsScenario>, ManagerSim.ITraitsLTWOption
        {
            public Option()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "ChooseTraitsOnAgeUp";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }
    }
}
