using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interactions;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Money;
using NRaas.CommonSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Skills
{
    public class SculptingPushScenario : SimSingleProcessScenario, IHasSkill
    {
        public SculptingPushScenario()
        { }
        protected SculptingPushScenario(SculptingPushScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Sculpting";
        }

        public SkillNames[] CheckSkills
        {
            get
            {
                return new SkillNames[] { SkillNames.Sculpting };
            }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return new SimScoringList(this, "Sculpting", Sims.TeensAndAdults, false).GetBestByMinScore(1);
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
            else if (sim.LotHome == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (!Skills.Allow(this, sim))
            {
                IncStat("Skill Denied");
                return false;
            }
            else if (!Situations.Allow(this, sim))
            {
                IncStat("Situations Denied");
                return false;
            }
            else if ((sim.IsEP11Bot) && (!sim.HasTrait(TraitNames.ArtisticAlgorithmsChip)))
            {
                IncStat("Chip Denied");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            base.PrivateUpdate(frame);

            List<SculptingStation> stations = new List<SculptingStation>();
            List<SculptingStation> empties = new List<SculptingStation>();

            foreach (Lot lot in ManagerLot.GetOwnedLots(Sim))
            {
                foreach (SculptingStation station in lot.GetObjects<SculptingStation>())
                {
                    if (station.HasFinishedSculpture())
                    {
                        GameObject sculpture = station.GetCurrentSculpture();

                        station.OnHandToolChildUnslottedBase(sculpture, Slot.ContainmentSlot_0);

                        sculpture.UnParent();

                        if (!Sim.Household.SharedFamilyInventory.Inventory.TryToMove(sculpture))
                        {
                            int value = Money.Sell(Sim, sculpture);

                            AddStat("Sculpture Sold", value);
                        }
                        else
                        {
                            IncStat("Sculpture Stored");
                        }
                    }

                    if (station.HasUnfinishedSculpture())
                    {
                        GameObject currentSculpture = station.GetCurrentSculpture();
                        if ((currentSculpture != null) && (currentSculpture.SculptureComponent != null))
                        {
                            if (currentSculpture.SculptureComponent.Artist == Sim)
                            {
                                stations.Add(station);
                            }
                        }
                    }
                    else
                    {
                        empties.Add(station);
                    }
                }
            }

            if (stations.Count == 0)
            {
                stations.AddRange(empties);
            }

            if (stations.Count == 0)
            {
                IncStat("No Station");
                return false;
            }

            SculptingStation choice = RandomUtil.GetRandomObjectFromList(stations);
            if (choice.HasUnfinishedSculpture())
            {
                IncStat("Sculpture Continued");

                return Situations.PushInteraction(this, Sim, choice, ContinueSculptureEx.Singleton);
            }
            else
            {
                List<SculptureComponent.SculptureMaterial> materials = new List<SculptureComponent.SculptureMaterial>();

                int skillLevel = Sim.SkillManager.GetSkillLevel(SkillNames.Sculpting);

                skillLevel = Math.Max(0, skillLevel);

                foreach (SculptureComponent.SculptureMaterial material in Enum.GetValues(typeof(SculptureComponent.SculptureMaterial)))
                {
                    if (material == SculptureComponent.SculptureMaterial.None) continue;

                    if (material == SculptureComponent.SculptureMaterial.Metal) continue;

                    if (!SculptingSkill.HasLevelForMaterial(skillLevel, material)) continue;

                    if (!SculptingSkill.CanAfford(Sim.CreatedSim, material)) continue;

                    materials.Add(material);
                }

                if (materials.Count == 0)
                {
                    IncStat("No Material");
                    return false;
                }

                SculptureComponent.SculptureMaterial materialChoice = RandomUtil.GetRandomObjectFromList(materials);

                List<SculptingSkill.SkillSculptureData> sculptures = SculptingSkill.ValidRandomSculpturesForLevelAndMaterial(skillLevel, materialChoice, Sim.CreatedSim);
                if (sculptures.Count == 0)
                {
                    IncStat("No Sculptures");
                    return false;
                }

                SculptingSkill.SkillSculptureData sculptureChoice = RandomUtil.GetWeightedRandomObjectFromList(sculptures.ToArray()) as SculptingSkill.SkillSculptureData;

                IncStat("Sculpture Started");

                return Situations.PushInteraction(this, Sim, choice, new CreateSculptureEx.Definition(null, sculptureChoice, materialChoice, new string[0]));
            }
        }

        protected override bool Push()
        {
            return true;
        }

        public override Scenario Clone()
        {
            return new SculptingPushScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSkill, SculptingPushScenario>
        {
            public Option()
                : base(true)
            { }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP2);
            }

            public override string GetTitlePrefix()
            {
                return "SculptingPush";
            }
        }
    }
}
