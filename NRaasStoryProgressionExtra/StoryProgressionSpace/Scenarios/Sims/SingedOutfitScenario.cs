using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Careers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Sims
{
    public class SingedOutfitScenario : SimScenario
    {
        public SingedOutfitScenario()
            : base ()
        { }
        protected SingedOutfitScenario(SingedOutfitScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "SingedOutfit";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            bool singedOutfit = false;

            try
            {
                if (Sim.CreatedSim.CurrentOutfitCategory == OutfitCategories.Singed)
                {
                    singedOutfit = true;
                }
            }
            catch (Exception e)
            {
                Common.DebugException(Sim, e);

                IncStat("Exception");
            }

            if (Sim.CreatedSim != null)
            {
                Sim.CreatedSim.BuffManager.RemoveElement(BuffNames.Singed);
                Sim.CreatedSim.BuffManager.RemoveElement(BuffNames.SingedElectricity);
            }

            if (singedOutfit)
            {
                ManagerSim.ChangeOutfit(Manager, Sim, OutfitCategories.Everyday);

                ManagerCareer.PerformStylistHelp(this, frame);
            }
            return true;
        }

        public override Scenario Clone()
        {
            return new SingedOutfitScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSim, SingedOutfitScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "SingedOutfit";
            }
        }
    }
}
