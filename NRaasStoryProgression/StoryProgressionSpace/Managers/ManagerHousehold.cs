using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scenarios.Pregnancies;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NRaas.StoryProgressionSpace.Managers
{
    public class ManagerHousehold : Manager
    {
        float mAverageHouseholdHumanSize;

        int mHouseholdCount;

        Household mActiveHousehold;

        public ManagerHousehold(Main manager)
            : base (manager)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Households";
        }

        public float AverageHouseholdHumanSize
        {
            get { return mAverageHouseholdHumanSize; }
        }

        public bool FamiliesPacked
        {
            get { return (AverageHouseholdHumanSize >= 5f); }
        }

        public Household ActiveHousehold
        {
            get { return mActiveHousehold; }
            set { mActiveHousehold = value; }
        }

        public override void InstallOptions(bool initial)
        {
            new Installer<ManagerHousehold>(this).Perform(initial);
        }

        protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
        {
            if (initialPass)
            {
                mActiveHousehold = Household.ActiveHousehold;
            }

            if ((ProgressionEnabled) && (fullUpdate))
            {
                mHouseholdCount = 0;
                mAverageHouseholdHumanSize = 0f;

                foreach (Household house in Household.GetHouseholdsLivingInWorld())
                {
                    if (HouseholdsEx.NumSims(house) == 0) continue;

                    mHouseholdCount++;
                    mAverageHouseholdHumanSize += HouseholdsEx.NumHumansIncludingPregnancy(house);
                }

                if (mHouseholdCount == 0)
                {
                    mAverageHouseholdHumanSize = 0f;
                }
                else
                {
                    mAverageHouseholdHumanSize /= mHouseholdCount;
                }
            }

            base.PrivateUpdate(fullUpdate, initialPass);
        }

        protected override string IsOnActiveLot(SimDescription sim, bool testViewLot)
        {
            return null;
        }

        public bool Allow(IScoringGenerator stats, SimDescription actor, SimDescription target, AllowCheck check)
        {
            if ((check & AllowCheck.UserDirected) == AllowCheck.None)
            {
                if (Personalities.IsOpposing(stats, actor, target, true))
                {
                    stats.IncStat("Opposing Clan");
                    return false;
                }
            }

            SimData actorData = GetData(actor);
            SimData targetData = GetData(target);

            if (!actorData.Allowed<AllowCasteMoveOption>(targetData, true))
            {
                stats.IncStat("Caste Denied");
                return false;
            }
            else if (actorData.Disallowed<DisallowCasteMoveOption>(targetData, true))
            {
                stats.IncStat("Caste Denied");
                return false;
            }
            else if (!DualAllow(stats, actorData, targetData, check))
            {
                return false;
            }

            return true;
        }
        public bool Allow(IScoringGenerator stats, Sim sim)
        {
            return PrivateAllow(stats, sim);
        }
        public bool Allow(IScoringGenerator stats, Sim sim, AllowCheck check)
        {
            return PrivateAllow(stats, sim, check);
        }
        public bool Allow(IScoringGenerator stats, SimDescription sim)
        {
            return PrivateAllow(stats, sim);
        }
        public bool Allow(IScoringGenerator stats, SimDescription sim, AllowCheck check)
        {
            return PrivateAllow(stats, sim, check);
        }

        protected override bool PrivateAllow(IScoringGenerator stats, SimDescription sim, SimData settings, AllowCheck check)
        {
            if (sim.Household != null)
            {
                if (SimTypes.IsTourist(sim))
                {
                    stats.IncStat("Allow: Tourist");
                    return false;
                }

                if (sim.Household.IsTravelHousehold)
                {
                    stats.IncStat("Allow: Travel Household");
                    return false;
                }
            }

            if (!settings.GetValue<AllowMoveFamilyOption, bool>())
            {
                stats.IncStat("Allow: Move Denied");
                return false;
            }

            return true;
        }

        protected override bool PrivateAllow(IScoringGenerator stats, SimDescription sim, AllowCheck check)
        {
            if (sim == null) return false;

            return PrivateAllow(stats, sim, GetData(sim), check);
        }

        public bool Allow(IScoringGenerator stats, Household house, int moveCooldown)
        {
            return Allow(stats, SimTypes.HeadOfFamily(house), moveCooldown);
        }
        public bool Allow(IScoringGenerator stats, SimDescription sim, int moveCooldown)
        {
            if (!Allow(stats, sim, AllowCheck.Active)) return false;

            if (sim.LotHome == null) return true;

            if (stats.AddScoring("Allow: Move Cooldown", GetElapsedTime<DayOfLastMoveOption>(sim) - moveCooldown) < 0)
            {
                stats.IncStat("Allow: Cooldown Denied");
                return false;
            }

            return true;
        }

        public int Assets(SimDescription sim)
        {
            return Assets(sim.Household);
        }
        public int Assets(Household house)
        {
            int assets = 0;

            if (house == null)
            {
                return 0;
            }
            else 
            {
                assets = house.FamilyFunds + Lots.GetLotCost(house.LotHome);
            }

            if (house.RealEstateManager != null)
            {
                foreach (PropertyData data in house.RealEstateManager.AllProperties)
                {
                    Lot lot = LotManager.GetLot(data.LotId);
                    if (lot == null)
                    {
                        assets += data.TotalValue;
                    }
                    else if (lot.ResidentialLotSubType == ResidentialLotSubType.kEP1_PlayerOwnable)
                    {
                        assets += data.TotalValue;
                    }
                    else
                    {
                        assets += ManagerLot.GetUnfurnishedLotCost(lot, 0);
                    }
                }
            }

            return assets;
        }

        public bool MoveSim (SimDescription sim, Household moveTo)
        {
            if (sim.Household == moveTo) return true;

            if (sim.Household != null)
            {
                if (sim.CreatedSim != null)
                {
                    try
                    {
                        if ((sim.CreatedSim.Autonomy != null) && (sim.CreatedSim.Autonomy.Motives == null))
                        {
                            sim.CreatedSim.Autonomy.RecreateAllMotives();

                            IncStat("Motives Recreated");
                        }

                        if (sim.CreatedSim.LotCurrent == null)
                        {
                            sim.CreatedSim.InternalOnSetLotCurrent(null);
                        }
                    }
                    catch (Exception e)
                    {
                        Common.DebugException(sim, e);

                        Sims.Reset(sim);
                    }
                }

                Household oldHouse = sim.Household;

                ProcessAbandonHouse(oldHouse, moveTo);

                sim.Household.Remove(sim);

                foreach (SimDescription member in HouseholdsEx.All(oldHouse))
                {
                    GetData(member).InvalidateCache();
                }
            }

            moveTo.Add(sim);

            foreach (SimDescription member in HouseholdsEx.All(moveTo))
            {
                GetData(member).InvalidateCache();
            }

            ManagerSim.ForceRecount();

            SetValue<InspectedOption,bool>(moveTo, false);

            Sims.Instantiate(sim, moveTo.LotHome, true);

            return (sim.Household == moveTo);
        }

        public void ProcessAbandonHouse(Household oldHouse, Household newHouse)
        {
            if (HouseholdsEx.NumHumans(oldHouse) != 1) return;

            Lots.ProcessAbandonLot(oldHouse.LotHome);

            foreach (PropertyData data in oldHouse.RealEstateManager.AllProperties)
            {
                ManagerMoney.TransferProperty(oldHouse, newHouse, data);
            }
        }

        public static List<Household> GetResidentHouseholds()
        {
            List<Household> houses = new List<Household>();
            foreach (Household house in Household.GetHouseholdsLivingInWorld())
            {
                if (HouseholdsEx.NumSims(house) == 0) continue;

                houses.Add(house);
            }

            return houses;
        }

        public bool EliminateHousehold(string storyName, Household me)
        {
            if (SimTypes.IsService(me)) return false;

            if (!GameStates.IsLiveState)
            {
                IncStat("Mode Save");
                return false;
            }

            if (AlarmManager.Global == null)
            {
                IncStat("No Alarm Manager");
                return false;
            }

            if (Deaths == null)
            {
                IncStat("No Death Manager");
                return false;
            }

            foreach (SimDescription sim in new List<SimDescription>(me.AllSimDescriptions))
            {
                Deaths.CleansingKill(sim, false);
            }

            me.Destroy();

            try
            {
                me.Dispose();
            }
            catch (Exception e)
            {
                Common.DebugException(me.Name, e);

                Household.sHouseholdList.Remove(me);
            }

            AddSuccess("Household Eliminated");

            IncStat("Household Eliminated: " + me.Name + Common.NewLine + "Story: " + storyName, Common.DebugLevel.High);

            ManagerSim.ForceRecount();
            return true;
        }

        public bool AllowGuardian(SimDescription sim)
        {
            if (sim == null) return false;

            if (!sim.IsHuman) return false;

            if (sim.IsEP11Bot && (sim.TraitManager != null && sim.TraitChipManager != null))
            {
                if (!sim.TraitManager.HasElement(TraitNames.RoboNannyChip) && !sim.TraitManager.HasElement(TraitNames.HumanEmotionChip))
                {
                    return false;
                }
            }

            if (sim.YoungAdultOrAbove) return true;

            return AllowSoloMove(sim);
        }

        public bool AllowSoloMove(SimDescription sim)
        {
            if (sim == null) return false;

            if (!sim.IsHuman) return false;

            if (sim.ChildOrBelow) return false;

            return GetValue<AllowMoveSoloOption, bool>(sim);
        }

        public class Updates : AlertLevelOption<ManagerHousehold>
        {
            public Updates()
                : base(AlertLevel.All)
            { }

            public override string GetTitlePrefix()
            {
                return "Stories";
            }
        }

        public class DebugOption : DebugLevelOption<ManagerHousehold>
        {
            public DebugOption()
                : base(Common.DebugLevel.Quiet)
            { }
        }

        public class SpeedOption : SpeedBaseOption<ManagerHousehold>
        {
            public SpeedOption()
                : base(1000, false)
            { }
        }

        public class HomelessInspectionOption : BooleanManagerOptionItem<ManagerHousehold>
        {
            public HomelessInspectionOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "HomelessRequireInspection";
            }
            
            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<ScheduledHomelessScenario.OptionV2,bool>()) return false;

                if (!Manager.GetValue<NewcomerGoneScenario.AllowHomelessMoveInOptionV2, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class TicksPassedOption : TicksPassedBaseOption<ManagerHousehold>
        {
            public TicksPassedOption()
            { }
        }

        protected class DumpScoringOption : DumpScoringBaseOption<ManagerHousehold>
        {
            public DumpScoringOption()
            { }
        }

        public class MinTimeBetweenMovesOption : CooldownOptionItem<ManagerHousehold>
        {
            public MinTimeBetweenMovesOption()
                : base(5)
            { }

            public override string GetTitlePrefix()
            {
                return "CooldownBetweenMoves";
            }
        }
    }
}

