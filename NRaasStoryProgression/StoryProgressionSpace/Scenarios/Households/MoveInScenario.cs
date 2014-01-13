using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Services;
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
    public abstract class MoveInScenario : HouseholdScenario, IFormattedStoryScenario
    {
        List<SimDescription> mMovers = null;

        SimDescription mMoveInWith = null;

        protected MoveInScenario(SimDescription mover, SimDescription moveInWith)
            : base(moveInWith.Household)
        {
            mMoveInWith = moveInWith;

            if (mover != null)
            {
                mMovers = new List<SimDescription>();
                mMovers.Add(mover);
            }
        }
        protected MoveInScenario(List<SimDescription> movers, SimDescription moveInWith)
            : base(moveInWith.Household)
        {
            mMoveInWith = moveInWith;
            mMovers = movers;
        }
        protected MoveInScenario(SimDescription mover)
        {
            if (mover != null)
            {
                mMovers = new List<SimDescription>();
                mMovers.Add(mover);
            }
        }
        protected MoveInScenario(List<SimDescription> movers)
        {
            mMovers = movers;
        }
        protected MoveInScenario(MoveInScenario scenario)
            : base (scenario)
        {
            mMovers = new List<SimDescription>(scenario.mMovers);
        }

        public virtual Manager GetFormattedStoryManager()
        {
            return StoryProgression.Main.Households;
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override int ContinueChance
        {
            get { return 0; }
        }

        protected abstract ManagerLot.FindLotFlags Inspect
        {
            get;
        }

        protected virtual bool TestAllow
        {
            get { return true; }
        }

        protected List<SimDescription> Movers
        {
            get { return mMovers; }
        }

        protected override bool Allow()
        {
            if ((mMovers == null) || (mMovers.Count == 0))
            {
                return false;
            }

            foreach (SimDescription sim in mMovers)
            {
                if ((sim.Household != null) && (TestAllow) && (!Households.Allow(this, sim)))
                {
                    IncStat("User Denied");
                    return false;
                }
                else if (SimTypes.IsDead(sim))
                {
                    IncStat("Dead");
                    return false;
                }
                else if (!sim.Marryable)
                {
                    IncStat("Not Marryable");
                    return false;
                }
                else if (SimTypes.InServicePool(sim, ServiceType.GrimReaper))
                {
                    IncStat("Reaper Denied");
                    return false;
                }
            }

            return base.Allow();
        }

        protected List<SimDescription> GetMovers(Household house, bool packup)
        {
            List<SimDescription> newMembers = new List<SimDescription>();
            foreach (SimDescription sim in mMovers)
            {
                if (sim.Household == house) continue;

                newMembers.Add(sim);

                if (packup)
                {
                    Lots.PackupVehicles(sim.CreatedSim, true);
                }
            }

            return newMembers;
        }

        protected override bool Allow(Household house)
        {
            if (house.LotHome == null)
            {
                IncStat("No Home");
                return false;
            }
            else if (SimTypes.IsSpecial(house)) 
            {
                IncStat("Special");
                return false;
            }
            else if (HouseholdsEx.NumSims(house) == 0)
            {
                IncStat("No Members");
                return false;
            }
            else 
            {
                List<SimDescription> movers = GetMovers(house, false);
                if (movers.Count == 0)
                {
                    IncStat("No Movers");
                    return false;
                }

                int humanMovers = 0, petMovers = 0;
                HouseholdsEx.NumSimsIncludingPregnancy(movers, ref humanMovers, ref petMovers);

                int humanCount = 0, petCount = 0;
                HouseholdsEx.NumSimsIncludingPregnancy(house, ref humanCount, ref petCount);

                int maximumHumans = GetValue<MaximumSizeOption, int>(house);
                int maximumPets = GetValue<MaximumPetSizeOption, int>(house);

                if (((humanCount + humanMovers) > maximumHumans) && (humanCount <= maximumHumans) && (humanMovers <= maximumHumans))
                {
                    AddStat("Too Many Human Movers", humanMovers);
                    AddStat("Too Many Human Existing", humanCount);
                    AddStat("Too Many Human Total", humanCount + humanMovers);
                    return false;
                }
                else if (((petCount + petMovers) > maximumPets) && (petCount <= maximumPets) && (petMovers <= maximumPets))
                {
                    AddStat("Too Many Pet Movers", petMovers);
                    AddStat("Too Many Pet Existing", petCount);
                    AddStat("Too Many Pet Total", petCount + petMovers);
                    return false;
                }
                else if ((TestAllow) && (!Households.Allow(this, house, 0)))
                {
                    IncStat("User Denied");
                    return false;
                }
                else if (!Lots.PassesHomeInspection(this, house.LotHome, movers, HouseholdsEx.All(house), Inspect | ManagerLot.FindLotFlags.InspectCareerItems))
                {
                    IncStat("Inspection Fail");
                    return false;
                }
                else if (GetValue<IsAncestralOption, bool>(house))
                {
                    SimDescription head = SimTypes.HeadOfFamily(house);

                    // Ensure that none of the sims moving into the home are older than the current Head of the Family
                    foreach (SimDescription newSim in movers)
                    {
                        if (SimTypes.IsOlderThan(newSim, head))
                        {
                            IncStat("Ancestral Age Denied");
                            return false;
                        }
                    }
                }

                foreach (SimDescription mover in movers)
                {
                    foreach (SimDescription member in HouseholdsEx.All(house))
                    {
                        if (!Households.Allow(this, mover, member, Managers.Manager.AllowCheck.None))
                        {
                            return false;
                        }
                    }
                }
            }

            return base.Allow(house);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if ((mMoveInWith == null) || (!House.Contains (mMoveInWith)))
            {
                mMoveInWith = SimTypes.HeadOfFamily(House);
            }

            List<SimDescription> movers = GetMovers(House, true);

            AddStat("Movers", movers.Count);

            foreach (SimDescription sim in movers)
            {
                int funds = 0, debt = 0;

                if ((!SimTypes.IsSpecial(sim)) && (!Household.RoommateManager.IsNPCRoommate(sim)))
                {
                    int count = HouseholdsEx.NumHumans(sim.Household);
                    if (count > 0)
                    {
                        funds = sim.FamilyFunds / count;

                        Money.AdjustFunds(sim, "MoveOut", -funds);
                    }

                    if (count == 1)
                    {
                        debt += GetValue<DebtOption, int>(sim.Household);

                        SetValue<DebtOption, int>(sim.Household, 0);

                        funds += Households.Assets(sim);
                    }
                }

                AddStat("New Funds", funds);

                Money.AdjustFunds(House, "MoveIn", funds);

                AddValue<DebtOption, int>(House, debt);

                Households.MoveSim(sim, House);
            }

            mMovers = movers;
            return true;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (mMovers.Count != 1) return null;

            if (manager == null)
            {
                if (Manager is SimPersonality)
                {
                    manager = Households;
                }
            }

            return base.PrintStory(manager, name, new object[] { mMovers[0], mMoveInWith }, extended, logging);
        }

        protected override ManagerStory.Story PrintFormattedStory(StoryProgressionObject manager, string text, string summaryKey, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (mMovers.Count == 1) return null;

            if (manager == null)
            {
                if (Manager is SimPersonality)
                {
                    manager = Households;
                }
            }

            if (text == null)
            {
                text = GetTitlePrefix(PrefixType.Summary);
            }

            return Stories.PrintFormattedSummary(Households, text, summaryKey, mMoveInWith, mMovers, extended, logging);
        }
    }
}
