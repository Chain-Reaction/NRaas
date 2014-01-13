using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Pregnancies
{
    public class PregnancyOutfitScenario : SimScenario
    {
        public PregnancyOutfitScenario()
            : base ()
        { }
        protected PregnancyOutfitScenario(PregnancyOutfitScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "PregnancyOutfit";
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
            if (sim.ChildOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (sim.IsPregnant)
            {
                IncStat("Pregnant");
                return false;
            }
            else
            {
                bool found = false;
                foreach (SimDescription child in Relationships.GetChildren(sim))
                {
                    if (child.Baby)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    IncStat("No Baby");
                    return false;
                }
            }

            try
            {
                if (sim.CreatedSim.CurrentOutfitCategory != Sims3.SimIFace.CAS.OutfitCategories.Everyday)
                {
                    IncStat("Not Everyday");
                    return false;
                }
            }
            catch (Exception e)
            {
                Common.DebugException(sim, e);

                IncStat("Exception");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            ManagerSim.ChangeOutfit(Manager, Sim, OutfitCategories.Everyday);
            return true;
        }

        public override Scenario Clone()
        {
            return new PregnancyOutfitScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSim, PregnancyOutfitScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "PregnancyOutfit";
            }
        }
    }
}
