using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Flirts;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using NRaas.StoryProgressionSpace.Situations;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Managers
{
    public class ManagerFlirt : Manager
    {
        private List<SimDescription> mFlirtySims = new List<SimDescription>();
        private List<SimDescription> mFlirtPool = new List<SimDescription>();

        protected SimID mPreviousLoveLoss = null;

        static TraitNames[] sFlirtChips = new TraitNames[] { TraitNames.CapacityToLoveChip, TraitNames.HumanEmotionChip, TraitNames.SentienceChip, TraitNames.MoodAdjusterChip };

        public ManagerFlirt(Main manager)
            : base (manager)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Flirt";
        }

        public SimID PreviousLoveLoss
        {
            get { return mPreviousLoveLoss; }
            set { mPreviousLoveLoss = value; }
        }

        public ICollection<SimDescription> FlirtySims
        {
            get { return mFlirtySims; }
        }

        public ICollection<SimDescription> FlirtPool
        {
            get { return mFlirtPool; }
        }

        public override void InstallOptions(bool initial)
        {
            new Installer<ManagerFlirt>(this).Perform(initial);
        }

        public static bool FirstAction(GoToLotSituation parent, GoToLotSituation.MeetUp meetUp)
        {
            try
            {
                if (parent.mSimA.IsHuman)
                {
                    meetUp.ForceSituationSpecificInteraction(parent.mSimB, parent.mSimA, new SocialInteractionA.Definition("Flirt", null, null, false), null, meetUp.OnSocialSucceeded, meetUp.OnSocialFailed);
                }
                else
                {
                    meetUp.ForceSituationSpecificInteraction(parent.mSimB, parent.mSimA, new SocialInteractionA.Definition("Nuzzle", null, null, false), null, meetUp.OnSocialSucceeded, meetUp.OnSocialFailed);
                }
                return true;
            }
            catch (Exception e)
            {
                Common.DebugException(parent.mSimA, parent.mSimB, e);
                return false;
            }
        }

        public void CalculateGayRatio(ref float gaySims, ref float straightSims)
        {
            foreach (SimDescription sim in Sims.All)
            {
                if (sim.TeenOrAbove)
                {
                    if (sim.Household == null) continue;

                    DetermineGayRatio(sim, ref gaySims, ref straightSims);
                }
            }
        }

        protected void DetermineGayRatio(SimDescription sim, ref float gaySims, ref float straightSims)
        {
            if (sim.Gender == CASAgeGenderFlags.Male)
            {
                if (sim.mGenderPreferenceMale > 0)
                {
                    gaySims++;
                }
                else if (sim.mGenderPreferenceFemale > 0)
                {
                    straightSims++;
                }
            }
            else
            {
                if (sim.mGenderPreferenceFemale > 0)
                {
                    gaySims++;
                }
                else if (sim.mGenderPreferenceMale > 0)
                {
                    straightSims++;
                }
            }
        }

        public void SetGenderPreference(SimDescription sim, bool allowGay)
        {
            if ((sim.mGenderPreferenceMale == 0) &&
                (sim.mGenderPreferenceFemale == 0))
            {
                if (sim.Partner != null)
                {
                    if (sim.Partner.IsMale)
                    {
                        sim.InternalIncreaseGenderPreferenceMale();
                    }
                    else
                    {
                        sim.InternalIncreaseGenderPreferenceFemale();
                    }
                }
                else if ((allowGay) && (RandomUtil.RandomChance(GetValue<ChanceOfGaySim, int>())))
                {
                    if (sim.IsMale)
                    {
                        sim.InternalIncreaseGenderPreferenceMale();
                    }
                    else
                    {
                        sim.InternalIncreaseGenderPreferenceFemale();
                    }
                }
                else if ((allowGay) && (RandomUtil.RandomChance(GetValue<ChanceOfBisexualOption, int>())))
                {
                    if (RandomUtil.CoinFlip())
                    {
                        sim.mGenderPreferenceMale += 100000;
                        sim.mGenderPreferenceFemale += 50000;
                    }
                    else
                    {
                        sim.mGenderPreferenceMale += 50000;
                        sim.mGenderPreferenceFemale += 100000;
                    }

                    if (sim.CreatedSim != null)
                    {
                        EventTracker.SendEvent(EventTypeId.kGenderPreferencesChanged, sim.CreatedSim);
                    }
                }
                else
                {
                    if (sim.IsMale)
                    {
                        sim.InternalIncreaseGenderPreferenceFemale();
                    }
                    else
                    {
                        sim.InternalIncreaseGenderPreferenceMale();
                    }
                }
            }
        }

        protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
        {
            if (initialPass)
            {
                UpdateEAFlag();
            }

            if ((ProgressionEnabled) && (fullUpdate))
            {
                mFlirtPool.Clear();
                mFlirtySims.Clear();

                float gaySims = 0;
                float straightSims = 0;

                CalculateGayRatio(ref gaySims, ref straightSims);

                int maximumGayRatio = GetValue<MaximumGayRatioOption, int>();

                foreach (SimDescription sim in Sims.All)
                {
                    if (sim.TeenOrAbove)
                    {
                        if (sim.Household == null) continue;

                        bool allowGay = ((gaySims / straightSims) * 100) < maximumGayRatio;

                        SetGenderPreference(sim, allowGay);

                        DetermineGayRatio(sim, ref gaySims, ref straightSims);

                        if (!Allow(this, sim)) continue;

                        if (!sim.Marryable) continue;

                        int score = 0;
                        if (Romances.AllowAdultery(this, sim, AllowCheck.None))
                        {
                            score = AddScoring("FlirtyPartner", sim);
                        }

                        bool bAddedToPool = false;

                        if ((AddScoring("FlirtySingle", sim) > 0) &&
                            ((sim.Partner == null) || (score > 0)))
                        {
                            mFlirtySims.Add(sim);

                            mFlirtPool.Add(sim);
                            bAddedToPool = true;
                        }

                        if ((sim.Partner == null) && (!bAddedToPool))
                        {
                            mFlirtPool.Add(sim);
                        }
                    }
                }

                AddStat("Flirty Sims", mFlirtySims.Count);
                AddStat("Flirt Pool", mFlirtPool.Count);

                mPreviousLoveLoss = null;
            }

            base.PrivateUpdate(fullUpdate, initialPass);
        }

        public static bool AreRomantic(SimDescription a, SimDescription b)
        {
            Relationship relation = ManagerSim.GetRelationship(a, b);
            if (relation == null) return false;

            return relation.AreRomantic();
        }

        public bool Allow(IScoringGenerator stats, SimDescription actor, SimDescription target)
        {
            return Allow(stats, actor, target, AllowCheck.Active);
        }
        public bool Allow(IScoringGenerator stats, SimDescription actor, SimDescription target, AllowCheck check)
        {
            if (!Allow(stats, actor, check)) return false;

            if (!Allow(stats, target, check)) return false;

            if (!CanHaveAutonomousRomance(stats, actor, target, (check & AllowCheck.Active) == AllowCheck.Active))
            {
                return false;
            }

            SimData actorData = GetData(actor);
            SimData targetData = GetData(target);

            if (!actorData.Allowed<AllowCasteFlirtOption>(targetData, true))
            {
                stats.IncStat("Caste Flirt Denied");
                return false;
            }
            else if (actorData.Disallowed<DisallowCasteFlirtOption>(targetData, true))
            {
                stats.IncStat("Caste Flirt Denied");
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
            bool testRomance = true;

            if ((sim.Household != null) && ((check & AllowCheck.UserDirected) == AllowCheck.None))
            {
                if (SimTypes.InServicePool(sim, ServiceType.GrimReaper))
                {
                    stats.IncStat("Allow: Reaper");
                    return false;
                }

                if (sim.LotHome == null)
                {
                    if (!GetValue<NewcomerGoneScenario.AllowHomelessMoveInOptionV2, bool>())
                    {
                        stats.IncStat("Allow: Homeless Move In Denied");
                        return false;
                    }
                }
            }
            else
            {
                if (SimTypes.IsService(sim))
                {
                    testRomance = false;
                }
            }

            if (sim.IsEP11Bot)
            {
                if (!sim.TraitManager.HasAnyElement(sFlirtChips))
                {
                    stats.IncStat("Allow: Missing Chips");
                    return false;
                }
            }
            
            if (testRomance)
            {
                if (!settings.GetValue<AllowRomanceOption, bool>())
                {
                    stats.IncStat("Allow: Romance Denied");
                    return false;
                }
            }

            return true;
        }

        public override void RemoveSim(ulong sim)
        {
            base.RemoveSim(sim);

            RemoveSim(mFlirtPool, sim);
            RemoveSim(mFlirtySims, sim);
            
            if (SimID.Matches (mPreviousLoveLoss, sim))
            {
                mPreviousLoveLoss = null;
            }
        }

        public static bool WasOldLover(SimDescription a, SimDescription b)
        {
            Relationship relation = ManagerSim.GetRelationship(a, b);
            if (relation == null) return false;

            if (relation.LTR.HasInteractionBit(LongTermRelationship.InteractionBits.BreakUp)) return true;

            if (relation.LTR.HasInteractionBit(LongTermRelationship.InteractionBits.Divorce)) return true;

            return false;
        }

        public bool CanHaveAutonomousRomance(Common.IStatGenerator stats, SimDescription sim, SimDescription other)
        {
            return CanHaveAutonomousRomance(stats, sim, other, true);
        }
        public bool CanHaveAutonomousRomance(Common.IStatGenerator stats, SimDescription sim, SimDescription other, bool fullTest)
        {
            if ((sim.Household == null) || (other.Household == null))
            {
                stats.IncStat("No Home");
                return false;
            }
            else if ((sim.Genealogy == null) || (other.Genealogy == null))
            {
                stats.IncStat("No Genealogy");
                return false;
            }
            else if ((fullTest) && ((SimTypes.IsSelectable(sim)) || (SimTypes.IsSelectable(other))))
            {
                stats.IncStat("Active");
                return false;
            }
            else if ((fullTest) && (!sim.CheckAutonomousGenderPreference(other)))
            {
                stats.IncStat("Gender Pref");
                return false;
            }
            else
            {
                bool testRelation = fullTest;
                if ((sim.Partner == other) || (other.Partner == sim))
                {
                    testRelation = false;
                }

                if (!Relationships.CanHaveRomanceWith(stats.IncStat, sim, other, true, true, testRelation, GetValue<ManagerFlirt.CompleteAncestorCheckOption, bool>()))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsActiveInvolved(SimDescription sim)
        {
            if (GetValue<AllowFlirtActiveInvolvedOption, bool>()) return false;

            foreach (Relationship relation in Relationship.Get(sim))
            {
                if (!relation.AreRomantic()) continue;

                SimDescription other = relation.GetOtherSimDescription(sim);
                if (other == null) continue;

                if (other.Household == Household.ActiveHousehold) return true;
            }

            return false;
        }

        protected bool IsValidLover(IScoringGenerator stats, SimDescription sim, SimDescription potential, bool allowAffair, bool force)
        {
            if (sim == potential)
            {
                //stats.IncStat("Myself");
                return false;
            }
            else if (!GetValue<AllowRomanceOption, bool>(potential))
            {
                stats.IncStat("User Denied");
                return false;
            }
            else if ((!allowAffair) && (potential.Partner != null))
            {
                stats.IncStat("Affair Denied");
                return false;
            }
            else if (sim.Partner == potential)
            {
                stats.IncStat("Partner");
                return false;
            }
            else if ((!GetValue<AllowOldLoverOption, bool>()) && (WasOldLover(sim, potential)))
            {
                stats.IncStat("Old Lover");
                return false;
            }
            else if (!Allow(stats, sim, potential))
            {
                //stats.IncStat("Mixed Aged Denied");
                return false;
            }
            else if (potential.Household == null)
            {
                stats.IncStat("Invalid House");
                return false;
            }
            else if ((SimTypes.IsSpecial(sim)) && (SimTypes.IsSpecial(potential)))
            {
                stats.IncStat("Both Special");
                return false;
            }
            else if (Situations.IsBusy(stats, potential, true))
            {
                stats.IncStat("Other Busy");
                return false;
            }
            else if (IsActiveInvolved(potential))
            {
                stats.IncStat("Other Active Involved");
                return false;
            }

            return true;
        }

        public List<SimDescription> FindAnyFor(IScoringGenerator stats, SimDescription sim, bool allowAffair, bool force)
        {
            return FindAnyFor(stats, sim, allowAffair, force, mFlirtPool);
        }
        public List<SimDescription> FindAnyFor(IScoringGenerator stats, SimDescription sim, bool allowAffair, bool force, ICollection<SimDescription> pool)
        {
            if ((sim == null) || (sim.Household == null))
            {
                stats.IncStat("Invalid Sim");
                return new List<SimDescription>();
            }

            stats.AddStat("Pool", pool.Count);

            SimScoringList scoring = new SimScoringList("NewFlirt");

            foreach (SimDescription potential in pool)
            {
                stats.IncStat("FindAnyFor " + potential.FullName, Common.DebugLevel.Logging);

                if (IsValidLover(stats, sim, potential, allowAffair, force))
                {
                    scoring.Add(stats, "Single", potential, sim);
                }

                Main.Sleep("ManagerFlirt:FindAnyFor");
            }

            if (force)
            {
                return scoring.GetBestByPercent(50f);
            }
            else
            {
                return scoring.GetBestByMinScore(0);
            }
        }

        public List<SimDescription> FindExistingFor(IScoringGenerator stats, SimDescription sim, bool disallowPartner)
        {
            if ((sim == null) || (sim.Household == null))
            {
                stats.IncStat("Invalid Sim");
                return new List<SimDescription>();
            }

            List<Relationship> relations = new List<Relationship>(Relationship.GetRelationships(sim));
            if (relations.Count == 0)
            {
                stats.IncStat("None");
                return new List<SimDescription>();
            }

            List<SimDescription> list = new List<SimDescription>();

            stats.AddStat("Relations", relations.Count);

            foreach (Relationship relation in relations)
            {
                SimDescription other = relation.GetOtherSimDescription(sim);
                if (other == null)
                {
                    stats.IncStat("Bad Link");
                    continue;
                }

                stats.IncStat(other.FullName, Common.DebugLevel.Logging);

                if (relation.LTR.Liking <= 0)
                {
                    stats.AddStat("Unliked", relation.LTR.Liking);
                }
                else if (!relation.AreRomantic())
                {
                    stats.IncStat("No Romantic");
                }
                else if ((disallowPartner) && (sim.Partner == other))
                {
                    stats.IncStat("Partner");
                }
                else if (SimTypes.IsDead(other))
                {
                    stats.IncStat("Dead");
                }
                else if (Situations.IsBusy(stats, other, true))
                {
                    stats.IncStat("Other Busy");
                }
                else if (!mFlirtPool.Contains(other))
                {
                    stats.IncStat("Not Pool");
                }
                else if (!Allow(stats, sim, other))
                {
                    stats.IncStat("User Denied");
                }
                else if (other.Household == null)
                {
                    stats.IncStat("Invalid Sim");
                }
                else if ((SimTypes.IsSpecial(sim)) && (SimTypes.IsSpecial(other)))
                {
                    stats.IncStat("Both Special");
                }
                else if (IsActiveInvolved(other))
                {
                    stats.IncStat("Other Active Involved");
                }
                else
                {
                    list.Add(other);
                }

                Main.Sleep("ManagerFlirt:FindExistingFor");
            }

            return list;
        }

        public override Scenario GetImmigrantRequirement(ImmigrationRequirement requirement)
        {
            return new ImmigrantRequirementScenario(requirement);
        }

        public void UpdateEAFlag()
        {
            if (GetValue<ChanceOfGaySim, int>() + GetValue<ChanceOfBisexualOption, int>() > 0)
            {
                SimDescription.sNumUserDirectedSameSexGenderPreferences = int.MaxValue / 2;
            }
            else
            {
                SimDescription.sNumUserDirectedSameSexGenderPreferences = 0;
            }
        }

        public bool IsCloselyRelated(SimDescription a, SimDescription b)
        {
            return Relationships.IsCloselyRelated(a, b, GetValue<CompleteAncestorCheckOption, bool>());
        }

        public class Updates : AlertLevelOption<ManagerFlirt>
        {
            public Updates()
                : base(AlertLevel.All)
            { }

            public override string GetTitlePrefix()
            {
                return "Stories";
            }
        }

        public class DebugOption : DebugLevelOption<ManagerFlirt>
        {
            public DebugOption()
                : base(Common.DebugLevel.Quiet)
            { }
        }

        public class AllowOldLoverOption : BooleanManagerOptionItem<ManagerFlirt>
        {
            public AllowOldLoverOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AllowOldLover";
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<ScheduledFlirtScenario.AllowFlirtsOption, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class ChanceOfGaySim : IntegerManagerOptionItem<ManagerFlirt>
        {
            public ChanceOfGaySim()
                : base(1)
            { }

            public override string GetTitlePrefix()
            {
                return "ChanceofGaySim";
            }

            public override void SetValue(int value, bool persist)
            {
                base.SetValue(value, persist);

                if (Manager != null)
                {
                    Manager.UpdateEAFlag();
                }
            }
        }

        public class ChanceOfBisexualOption : IntegerManagerOptionItem<ManagerFlirt>
        {
            public ChanceOfBisexualOption()
                : base(5)
            { }

            public override string GetTitlePrefix()
            {
                return "ChanceofBisexual";
            }

            public override void SetValue(int value, bool persist)
            {
                base.SetValue(value, persist);

                if (Manager != null)
                {
                    Manager.UpdateEAFlag();
                }
            }
        }

        public class MaximumGayRatioOption : IntegerManagerOptionItem<ManagerFlirt>
        {
            public MaximumGayRatioOption()
                : base(100)
            { }

            public override string GetTitlePrefix()
            {
                return "MaximumGayRatio";
            }
        }

        public class SpeedOption : SpeedBaseOption<ManagerFlirt>
        {
            public SpeedOption()
                : base(300, true)
            { }
        }

        public class DumpStatsOption : DumpStatsBaseOption<ManagerFlirt>
        {
            public DumpStatsOption()
                : base(5)
            { }
        }

        protected class DumpScoringOption : DumpScoringBaseOption<ManagerFlirt>
        {
            public DumpScoringOption()
            { }
        }

        public class TicksPassedOption : TicksPassedBaseOption<ManagerFlirt>
        {
            public TicksPassedOption()
                : base()
            { }
        }

        public class AllowFlirtActiveInvolvedOption : BooleanManagerOptionItem<ManagerFlirt>
        {
            public AllowFlirtActiveInvolvedOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AllowFlirtActiveInvolved";
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<ScheduledFlirtScenario.AllowFlirtsOption, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class CompleteAncestorCheckOption : BooleanManagerOptionItem<ManagerFlirt>
        {
            public CompleteAncestorCheckOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "CompleteAncestorCheck";
            }
        }
    }
}

