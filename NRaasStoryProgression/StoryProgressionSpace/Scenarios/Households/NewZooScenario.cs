using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Scenarios.Pregnancies;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Households
{
    public class NewZooScenario : PetAdoptionBaseScenario
    {
        public NewZooScenario()
        { }
        protected NewZooScenario(NewZooScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "NewZoo";
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (mNewSim == null)
            {
                bool allowAnother = RandomUtil.RandomChance(GetValue<PetAdoptionBaseScenario.ChanceOfAnotherOptionV2, int>());

                List<SimDescription> choices = new List<SimDescription>();

                foreach (SimDescription sim in Sims.All)
                {
                    if (sim.Species != Species) continue;

                    if (!sim.ChildOrBelow) continue;

                    if (SimTypes.IsSelectable(sim)) continue;

                    // Don't take sims that are not living with their parents
                    bool found = false;
                    foreach (SimDescription parent in Relationships.GetParents(sim))
                    {
                        if (parent.Household == sim.Household)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        IncStat("Target Not With Parents");
                        continue;
                    }

                    if (!GetValue<AllowAdoptionOption, bool>(sim))
                    {
                        IncStat("Target Adoption Denied");
                        continue;
                    }

                    SimDescription head = SimTypes.HeadOfFamily(sim.Household);
                    if (head == null) continue;

                    // Test whether the sim's existing household is too large and should shed an animal
                    if (!HouseholdsEx.IsFull(this, sim.Household, sim.Species, 0, true, true)) continue;

                    // Test whether the sim's new household is too small and can gain an animal
                    if (HouseholdsEx.IsFull(this, Sim.Household, sim.Species, 0, true, allowAnother)) continue;

                    choices.Add(sim);
                }

                if (choices.Count == 0)
                {
                    IncStat("No Choice");
                    return false;
                }

                mNewSim = RandomUtil.GetRandomObjectFromList(choices);
            }

            return base.PrivateUpdate(frame);
        }

        public override Scenario Clone()
        {
            return new NewZooScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerHousehold, NewZooScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "NewZoo";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP5);
            }
        }
    }
}
