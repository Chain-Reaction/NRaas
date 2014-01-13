using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scoring;
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
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.HobbiesSkills.Inventing;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Lots
{
    public abstract class RepairScenario : SimScenario, IHasSkill
    {
        Dictionary<Household,List<GameObject>> mRepairs = null;

        bool mTownRepair = false;

        Household mHouse = null;

        public RepairScenario()
        { }
        protected RepairScenario(SimDescription sim, GameObject obj)
            : base(sim)
        {
            List<GameObject> list = new List<GameObject>();
            list.Add(obj);

            mRepairs = new Dictionary<Household, List<GameObject>>();
            mRepairs.Add(sim.Household, list);
        }
        protected RepairScenario(RepairScenario scenario)
            : base (scenario)
        {
            mRepairs = scenario.mRepairs;
            mTownRepair = scenario.mTownRepair;
            mHouse = scenario.mHouse;
        }

        protected bool TownRepair
        {
            get { return mTownRepair; }
        }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected Household House
        {
            get { return mHouse; }
        }

        public SkillNames[] CheckSkills
        {
            get
            {
                return new SkillNames[] { SkillNames.Handiness };
            }
        }

        protected abstract bool AllowHouse(Household house);

        public static void GetRepairs(StoryProgressionObject manager, GameObject[] choices, Dictionary<Household,List<GameObject>> allRepairs)
        {
            foreach (GameObject obj in choices)
            {
                if (obj == null) continue;

                if (obj is Sim)
                {
                    Sim sim = obj as Sim;

                    if (!sim.SimDescription.IsFrankenstein) continue;

                    if (!sim.BuffManager.HasElement(BuffNames.ShortOut)) continue;
                }
                else
                {
                    if (obj.InUse) continue;

                    if (!obj.InWorld) continue;

                    if (!obj.IsRepairable) continue;

                    RepairableComponent component = obj.Repairable;
                    if (component == null) continue;

                    if (!component.Broken) continue;
                }

                if (obj.LotCurrent == null) continue;

                Household household = obj.LotCurrent.Household;
                if (household == null) continue;

                if (household == Household.ActiveHousehold) continue;

                foreach (SimDescription member in HouseholdsEx.All(household))
                {
                    if (manager.GetValue<InStasisOption, bool>(member)) continue;
                }

                List<GameObject> repairs;
                if (!allRepairs.TryGetValue(household, out repairs))
                {
                    repairs = new List<GameObject>();
                    allRepairs.Add(household, repairs);
                }

                repairs.Add(obj);
            }
        }

        public static bool PushInteractions(StoryProgressionObject manager, SimDescription sim, List<GameObject> repair)
        {
            if (sim.CreatedSim == null) return false;

            if (sim.Household == null) return false;

            List<Pair<GameObject, InteractionDefinition>> electronics = new List<Pair<GameObject, InteractionDefinition>>();

            bool bSuccess = false;

            foreach (GameObject obj in repair)
            {
                InteractionDefinition interaction = null;

                bool electric = false;
                if (obj is Dishwasher)
                {
                    interaction = Dishwasher.RepairDishwasher.Singleton;
                    electric = true;
                }
                else if (obj is Computer)
                {
                    interaction = Computer.RepairComputer.Singleton;
                    electric = true;
                }
                else if (obj is Stereo)
                {
                    interaction = Stereo.RepairStereo.Singleton;
                    electric = true;
                }
                else if (obj is TV)
                {
                    interaction = TV.RepairTV.Singleton;
                    electric = true;
                }
                else if (obj is TrashCompactor)
                {
                    interaction = TrashCompactor.Repair.Singleton;
                    electric = true;
                }
                else if (obj is Bathtub)
                {
                    interaction = Bathtub.RepairBathtub.Singleton;
                }
                else if (obj is Shower)
                {
                    interaction = Shower.RepairShower.Singleton;
                }
                else if (obj is Sink)
                {
                    interaction = Sink.RepairSink.Singleton;
                }
                else if (obj is Toilet)
                {
                    interaction = Toilet.Repair.Singleton;
                }
                else if (obj is NectarMaker)
                {
                    interaction = NectarMaker.Repair.Singleton;
                    electric = true;
                }
                else if (obj is WashingMachine)
                {
                    interaction = WashingMachine.Repair.Singleton;
                    electric = true;
                }
                else if (obj is Sim)
                {
                    interaction = OccultFrankenstein.RepairFrankenstein.Singleton;
                }
                else if (obj is HotTubBase)
                {
                    interaction = HotTubBase.RepairHotTub.Singleton;
                }
                else if (obj is TimeMachine)
                {
                    interaction = TimeMachine.Repair.Singleton;
                    electric = true;
                }

                if (interaction == null) continue;

                if (electric)
                {
                    electronics.Add(new Pair<GameObject, InteractionDefinition>(obj, interaction));
                }
                else
                {
                    sim.Household.AddGreetedLotToHousehold(obj.LotCurrent, sim.CreatedSim.ObjectId);

                    if (!manager.Situations.PushInteraction(manager, sim, obj, interaction))
                    {
                        break;
                    }
                    else
                    {
                        bSuccess = true;
                    }
                }
            }

            if (electronics.Count > 0)
            {
                Pair<GameObject, InteractionDefinition> item = RandomUtil.GetRandomObjectFromList(electronics);

                sim.Household.AddGreetedLotToHousehold(item.First.LotCurrent, sim.CreatedSim.ObjectId);

                if (manager.Situations.PushInteraction(manager, sim, item.First, item.Second))
                {
                    bSuccess = true;
                }
            }

            return bSuccess;
        }

        protected override Scenario.GatherResult Gather(List<Scenario> list, ref int continueChance, ref int maximum, ref bool random)
        {
            if (mRepairs == null)
            {
                mRepairs = new Dictionary<Household, List<GameObject>>();

                GetRepairs(Manager, Sims3.Gameplay.Queries.GetObjects<GameObject>(), mRepairs);

                if (mRepairs.Count == 0) return GatherResult.Failure;
            }

            return base.Gather(list, ref continueChance, ref maximum, ref random);
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.ChildOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (sim.LotHome == null)
            {
                IncStat("Not Resident");
                return false;
            }
            else if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (!Skills.Allow(this, sim))
            {
                IncStat("Skills Denied");
                return false;
            }
            else if (!Situations.Allow(this, sim))
            {
                IncStat("Push Denied");
                return false;
            }
            else if (sim.CreatedSim.BuffManager == null)
            {
                IncStat("No Manager");
                return false;
            }
            else if ((sim.CreatedSim.BuffManager.HasElement(BuffNames.Singed)) ||
                     (sim.CreatedSim.BuffManager.HasElement(BuffNames.SingedElectricity)))
            {
                IncStat("Singed");
                return false;
            }
            else if ((sim.IsEP11Bot) && (!sim.HasTrait(TraitNames.HandiBotChip)))
            {
                IncStat("Chip Denied");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (mRepairs.Count == 0) return false;

            List<GameObject> repair = new List<GameObject>();

            if (mRepairs.ContainsKey(Sim.Household))
            {
                repair.AddRange(mRepairs[Sim.Household]);
                mRepairs.Remove(Sim.Household);

                mHouse = Sim.Household;
            }

            if (repair.Count == 0)
            {
                List<Household> houses = new List<Household>(mRepairs.Keys);

                while (houses.Count > 0)
                {
                    mHouse = RandomUtil.GetRandomObjectFromList(houses);
                    houses.Remove(mHouse);

                    if (AllowHouse(mHouse))
                    {
                        repair.AddRange(mRepairs[mHouse]);
                        mRepairs.Remove(mHouse);
                        break;
                    }
                    else
                    {
                        IncStat("House Denied");
                    }
                }

                mTownRepair = true;

                if (repair.Count > 0)
                {
                    IncStat("Around Town");
                }
                else
                {
                    IncStat("No Choice");
                }
            }
            else
            {
                IncStat("Home Work");
            }

            return PushInteractions(Manager, Sim, repair);
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Skills;
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }
    }
}
