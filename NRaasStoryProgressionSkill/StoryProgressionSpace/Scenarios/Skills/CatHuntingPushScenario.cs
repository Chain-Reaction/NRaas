using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interactions;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios.Money;
using NRaas.StoryProgressionSpace.SimDataElement;
using NRaas.StoryProgressionSpace.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Insect;
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
    public class CatHuntingPushScenario : HuntingPushScenario
    {
        static List<Pair<Lot, GameObjectHit>> sFishingPoints; 

        public CatHuntingPushScenario()
        { }
        protected CatHuntingPushScenario(CatHuntingPushScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "CatHunting";
        }

        public override SkillNames[] CheckSkills
        {
            get
            {
                return new SkillNames[] { SkillNames.CatHunting };
            }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if ((SimTypes.IsSelectable(sim)) && (sim.SkillManager.GetSkillLevel(SkillNames.CatHunting) < 1))
            {
                IncStat("Active Skill Denied");
                return false;
            }
            else if (InteractionsEx.HasInteraction<Terrain.CatFishHere.Definition>(sim))
            {
                IncStat("Has Interaction");
                return false;
            }
            else if (InteractionsEx.HasInteraction<CatHuntingComponent.StalkForPrey.Definition>(sim))
            {
                IncStat("Has Interaction");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return sim.IsCat;
        }

        public static void PopulateFishingPoints()
        {
            if (sFishingPoints == null)
            {
                sFishingPoints = new List<Pair<Lot, GameObjectHit>>();

                foreach (Lot lot in LotManager.AllLots)
                {
                    Vector3[] vectorArray;
                    if (((lot != null) && !lot.IsWorldLot) && (World.FindPondRepresentativePositions(lot.LotId, out vectorArray) && (vectorArray.Length != 0x0)))
                    {
                        foreach (Vector3 vector in vectorArray)
                        {
                            GameObjectHit b = InteractionInstance.CreateFakeGameObjectHit(vector);
                            if ((b.mType == GameObjectHitType.WaterSea) || (b.mType == GameObjectHitType.WaterPond))
                            {
                                sFishingPoints.Add(new Pair<Lot, GameObjectHit>(lot, b));
                            }
                        }
                    }
                }
            }
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            base.PrivateUpdate(frame);

            CatHuntingSkill skill = Sim.SkillManager.AddElement(SkillNames.CatHunting) as CatHuntingSkill;
            if (skill == null)
            {
                IncStat("Skill Fail");
                return false;
            }

            if ((skill.SkillLevel < 1) || (!skill.CanCatchAnything()) || (RandomUtil.RandomChance(25)))
            {
                PopulateFishingPoints();

                List<GameObjectHit> fishingSpots = new List<GameObjectHit>();
                foreach (Pair<Lot, GameObjectHit> pair in sFishingPoints)
                {
                    if ((pair.First.IsCommunityLot) || (pair.First.CanSimTreatAsHome(Sim.CreatedSim)))
                    {
                        fishingSpots.Add(pair.Second);
                    }
                }

                if (fishingSpots.Count != 0x0)
                {
                    Terrain.CatFishHere fishing = Terrain.CatFishHere.Singleton.CreateInstance(Terrain.Singleton, Sim.CreatedSim, Sim.CreatedSim.InheritedPriority(), true, true) as Terrain.CatFishHere;
                    if (fishing == null)
                    {
                        IncStat("Fish Creation Fail");
                    }
                    else
                    {
                        fishing.Hit = RandomUtil.GetRandomObjectFromList<GameObjectHit>(fishingSpots);

                        if (Situations.PushInteraction(this, Sim, Managers.Manager.AllowCheck.Active, fishing))
                        {
                            return true;
                        }

                        IncStat("Fish Fail");
                    }
                }
                else
                {
                    IncStat("No Fishing Spots");
                }
            }

            List<SimDescription> choices = new List<SimDescription>();

            foreach (SimDescription sim in new SimScoringList(this, "LikesCats", HouseholdsEx.Humans(Sim.Household), false).GetBestByMinScore(1))
            {
                if (sim.ChildOrBelow) continue;

                choices.Add(sim);
            }

            if (choices.Count == 0)
            {
                IncStat("No Master");
                return false;
            }

            CatHuntingComponent.StalkForPrey interaction = CatHuntingComponent.StalkForPrey.Singleton.CreateInstance(Sim.CreatedSim, Sim.CreatedSim, Sim.CreatedSim.InheritedPriority(), true, true) as CatHuntingComponent.StalkForPrey;
            if (interaction == null)
            {
                IncStat("Catch Creation Fail");
                return false;
            }

            interaction.PushedByGoCatch = true;
            interaction.PickAnyPrey = true;
            interaction.PresentToID = RandomUtil.GetRandomObjectFromList(choices).SimDescriptionId;
            interaction.ForceCatchFailureObject = false;

            if (Situations.PushInteraction(this, Sim, Managers.Manager.AllowCheck.Active, interaction))
            {
                return true;
            }

            IncStat("Catch Prey Fail");
            return false;
        }

        public override Scenario Clone()
        {
            return new CatHuntingPushScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSkill, CatHuntingPushScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "CatHuntingPush";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP5);
            }

            public override bool Install(ManagerSkill main, bool initial)
            {
                sFishingPoints = null;

                return base.Install(main, initial);
            }
        }
    }
}
