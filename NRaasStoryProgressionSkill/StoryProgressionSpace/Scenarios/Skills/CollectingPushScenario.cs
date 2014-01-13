using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interactions;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Money;
using NRaas.CommonSpace.Scoring;
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
    public class CollectingPushScenario : SimSingleProcessScenario, IHasSkill
    {
        List<GameObject> mCollectables = null;

        public CollectingPushScenario()
        { }
        protected CollectingPushScenario(CollectingPushScenario scenario)
            : base (scenario)
        {
            mCollectables = scenario.mCollectables;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "Collecting";
        }

        public SkillNames[] CheckSkills
        {
            get
            {
                return new SkillNames[] { SkillNames.Collecting };
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
            return new SimScoringList(this, "Collecting", Sims.All, false).GetBestByMinScore(1);
        }

        protected override Scenario.GatherResult Gather(List<Scenario> list, ref int continueChance, ref int maximum, ref bool random)
        {
            if (mCollectables == null)
            {
                mCollectables = new List<GameObject>();
                foreach (GameObject obj in Sims3.Gameplay.Queries.GetObjects<GameObject>())
                {
                    if (obj.InInventory) continue;

                    if (!obj.InWorld) continue;

                    if (obj.LotCurrent == null) continue;

                    RockGemMetalBase rock = obj as RockGemMetalBase;
                    if (rock != null)
                    {
                        if (rock.mCollected) continue;

                        if (rock.IsOnSpawner) continue;

                        mCollectables.Add(obj);
                    }
                    else if (obj is InsectJig)
                    {
                        mCollectables.Add(obj);
                    }
                    else if (obj is MinorPet)
                    {
                        MinorPet pet = obj as MinorPet;

                        if (pet.IsUnconscious) continue;

                        if (pet.Captured)
                        {
                            if (!pet.Escaped) continue;
                        }

                        mCollectables.Add(obj);
                    }
                    else
                    {
                        DigSite site = obj as DigSite;
                        if ((site != null) && (site.NumTreasuresRemaining > 0))
                        {
                            mCollectables.Add(obj);
                        }
                    }
                }

                if (mCollectables.Count == 0)
                {
                    return GatherResult.Failure;
                }
            }

            return base.Gather(list, ref continueChance, ref maximum, ref random);
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.ToddlerOrBelow)
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
            else if (!Sims.AllowInventory(this, sim, Managers.Manager.AllowCheck.None))
            {
                IncStat("Inventory Denied");
                return false;
            }
            else if (!Situations.Allow(this, sim))
            {
                IncStat("Situations Denied");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            base.PrivateUpdate(frame);

            bool success = false;
            while (mCollectables.Count > 0)
            {
                GameObject choice = RandomUtil.GetRandomObjectFromList(mCollectables);
                mCollectables.Remove(choice);

                InteractionDefinition interaction = null;
                if (choice is RockGemMetalBase)
                {
                    interaction = CollectEx.Singleton;

                    IncStat("Collect");
                }
                else if (choice is MinorPet)
                {
                    interaction = CaptureEx.Singleton;

                    IncStat("Capture");
                }
                else if (choice is InsectJig)
                {
                    interaction = CatchInsectEx.Singleton;

                    IncStat("CatchInsect");
                }
                else if ((choice is DigSite) && (Sim.TeenOrAbove))
                {
                    interaction = Excavate.Singleton;

                    IncStat("Excavate");
                }
                else
                {
                    continue;
                }

                if (Situations.PushInteraction(this, Sim, choice, interaction))
                {
                    IncStat("Pushed");
                    success = true;
                }
                else
                {
                    break;
                }
            }

            return success;
        }

        protected override bool Push()
        {
            return true;
        }

        public override Scenario Clone()
        {
            return new CollectingPushScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSkill, CollectingPushScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "CollectingPush";
            }
        }
    }
}
