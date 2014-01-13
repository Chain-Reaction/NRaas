using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Scenarios.Skills;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Situations
{
    public class PushLaundryScenario : SimSingleProcessScenario
    {
        class LaundryState
        {
            public bool mDryer;
            public bool mWasher;
        }

        Dictionary<Household, LaundryState> mPushed = new Dictionary<Household, LaundryState>();

        public PushLaundryScenario()
        { }
        protected PushLaundryScenario(PushLaundryScenario scenario)
            : base (scenario)
        {
            mPushed = scenario.mPushed;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "PushLaundry";
        }

        protected override int ContinueChance
        {
            get { return 100; }
        }

        protected override int MaximumReschedules
        {
            get 
            { 
                mPushed.Clear();
                return 4; 
            }
        }

        protected override bool AlwaysReschedule
        {
            get { return true; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Lots.GetSimsWith<WashingMachine>();
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
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
            else if (!Inventories.VerifyInventory(sim))
            {
                IncStat("No Inventory");
                return false;
            }
            else
            {
                LaundryState state;
                if (mPushed.TryGetValue(sim.Household, out state))
                {
                    if ((state.mDryer) && (state.mWasher))
                    {
                        IncStat("Already Pushed");
                        return false;
                    }
                }

                if (AddScoring("Neat", sim) < 0)
                {
                    int count = 0;
                    foreach (ClothingPileDry pile in sim.LotHome.GetObjects<ClothingPileDry>())
                    {
                        count += pile.Count;
                    }

                    if (count < ClothingPileDry.kCapacity * 2)
                    {
                        IncStat("Scoring Fail");
                        return false;
                    }
                }

                bool hasNeed = false;
                if (sim.CreatedSim.Inventory.ContainsType(typeof(ClothingPileDry), 1))
                {
                    hasNeed = true;
                }
                else if (sim.LotHome.CountObjects<ClothingPileDry>() > 0)
                {
                    hasNeed = true;
                }
                else
                {
                    foreach (Hamper hamper in sim.LotHome.GetObjects<Hamper>())
                    {
                        if (hamper.HasClothingPiles())
                        {
                            hasNeed = true;
                            break;
                        }
                    }
                }

                if (!hasNeed)
                {
                    IncStat("No Need");
                    return false;
                }
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            base.PrivateUpdate(frame);

            LaundryState state;
            if (!mPushed.TryGetValue(Sim.Household, out state))
            {
                state = new LaundryState();
                mPushed.Add(Sim.Household, state);
            }

            List<GameObject> broken = new List<GameObject>();

            if (!state.mDryer)
            {
                List<ClothingPileWet> wetPiles = new List<ClothingPileWet>(Sim.LotHome.GetObjects<ClothingPileWet>());

                foreach(ICanDryClothes obj in Sim.LotHome.GetObjects<ICanDryClothes>())
                {
                    if (obj.IsAvailible()) continue;

                    Clothesline line = obj as Clothesline;
                    if (line != null)
                    {
                        IncStat("Try Empty Line");

                        if (Situations.PushInteraction(this, Sim, line, Clothesline.GetCleanLaundry.Singleton))
                        {
                            state.mDryer = true;

                            IncStat("Push Empty Line");
                            return true;
                        }

                        if (wetPiles.Count > 0)
                        {
                            ClothingPileWet pile = RandomUtil.GetRandomObjectFromList(wetPiles);
                            wetPiles.Remove(pile);

                            if (Situations.PushInteraction(this, Sim, pile, ClothingPileWet.DryClothesOnClothesline.Singleton))
                            {
                                state.mDryer = true;

                                IncStat("Hang Wet Pile");
                                return true;
                            }
                        }
                    }
                    else
                    {
                        Dryer dryer = obj as Dryer;
                        if (dryer != null)
                        {
                            IncStat("Try Empty Dryer");

                            if (Situations.PushInteraction(this, Sim, dryer, Dryer.GetCleanLaundry.Singleton))
                            {
                                state.mDryer = true;

                                IncStat("Push Empty Dryer");
                                return true;
                            }

                            if (wetPiles.Count > 0)
                            {
                                ClothingPileWet pile = RandomUtil.GetRandomObjectFromList(wetPiles);
                                wetPiles.Remove(pile);

                                if (Situations.PushInteraction(this, Sim, pile, ClothingPileWet.DryClothesInDryer.Singleton))
                                {
                                    state.mDryer = true;

                                    IncStat("Dry Wet Pile");
                                    return true;
                                }
                            }

                            if ((dryer.Repairable != null) && (dryer.Repairable.Broken))
                            {
                                broken.Add(dryer);
                            }
                        }
                    }
                }
            }

            if (!state.mWasher)
            {
                foreach (WashingMachine machine in Sim.LotHome.GetObjects<WashingMachine>())
                {
                    if (machine.mWashState == WashingMachine.WashState.HasCleanLaundry)
                    {
                        if (WashingMachine.DryClothesBase.DoesClotheslineExist(Sim.LotHome))
                        {
                            IncStat("Try Clothesline");

                            if (Situations.PushInteraction(this, Sim, machine, WashingMachine.DryClothesOnClothesline.Singleton))
                            {
                                state.mWasher = true;

                                IncStat("Push Clothesline");
                                return true;
                            }
                        }
                        else
                        {
                            IncStat("Try Dryer");

                            if (Situations.PushInteraction(this, Sim, machine, WashingMachine.DryClothesInDryer.Singleton))
                            {
                                state.mWasher = true;

                                IncStat("Push Dryer");
                                return true;
                            }
                        }
                    }
                    else if (machine.CanDoLaundry())
                    {
                        IncStat("Try Washer");

                        if (Situations.PushInteraction(this, Sim, machine, WashingMachine.DoLaundry.Singleton))
                        {
                            state.mWasher = true;

                            IncStat("Push Washer");
                            return true;
                        }
                    }

                    if ((machine.Repairable != null) && (machine.Repairable.Broken))
                    {
                        broken.Add(machine);
                    }
                }
            }

            if (broken.Count > 0)
            {
                IncStat("Repair Push");

                Add(frame, new ScheduledRepairScenario(Sim, RandomUtil.GetRandomObjectFromList(broken)), ScenarioResult.Start);
                return true;
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new PushLaundryScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSituation, PushLaundryScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override bool Install(ManagerSituation main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                CleanupLotScenario.sExpandedInstalled = true;
                return true;
            }

            public override string GetTitlePrefix()
            {
                return "PushLaundry";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP2);
            }
        }
    }
}
