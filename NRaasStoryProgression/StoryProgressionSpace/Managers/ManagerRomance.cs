using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NRaas.StoryProgressionSpace.Managers
{
    public class ManagerRomance : Manager
    {
        public enum AffairStory : int
        {
            None = 0x00,
            Partner = 0x01,
            Actor = 0x02,
            Target = 0x04,
            Duo = 0x08,
            Suppress = 0x10,
            All = Partner | Actor | Target | Duo,
        }

        private List<SimDescription> mPartneredSims = new List<SimDescription>();

        static Common.MethodStore sWoohooAddNotch = new Common.MethodStore("NRaasWoohooer", "NRaas.WoohooerSpace.Skills.KamaSimtra", "AddNotches", new Type[] { typeof(SimDescription), typeof(SimDescription), typeof(bool), typeof(bool) });

        public ManagerRomance(Main manager)
            : base (manager)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Romance";
        }

        public List<SimDescription> Partnered
        {
            get { return mPartneredSims; }
        }

        public override void InstallOptions(bool initial)
        {
            new Installer<ManagerRomance>(this).Perform(initial);
        }

        protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
        {
            if ((ProgressionEnabled) && (fullUpdate))
            {
                mPartneredSims.Clear();

                foreach (SimDescription sim in Sims.All)
                {
                    if (!sim.TeenOrAbove) continue;

                    if (SimTypes.IsDead(sim)) continue;

                    if (!Romances.Allow(this, sim)) continue;

                    if (!sim.Marryable) continue;

                    if (sim.Household == null) continue;

                    if (sim.Partner == null) continue;

                    mPartneredSims.Add(sim);
                }
            }

            base.PrivateUpdate(fullUpdate, initialPass);
        }

        public override void RemoveSim(ulong sim)
        {
            base.RemoveSim(sim);

            RemoveSim(mPartneredSims, sim);
        }

        public static event OnAllow OnAllowAdultery;

        public bool AllowAdultery(IScoringGenerator stats, SimDescription sim, AllowCheck check)
        {
            if (sim.Partner == null) return true;

            if (OnAllowAdultery == null) return false;

            SimData settings = GetData(sim);

            foreach (OnAllow del in OnAllowAdultery.GetInvocationList())
            {
                if (!del(stats, settings, check)) return false;
            }

            return true;
        }

        public static event OnAllow OnAllowLiaison;

        public bool AllowLiaison(IScoringGenerator stats, SimDescription sim, AllowCheck check)
        {
            if (OnAllowLiaison == null) return false;

            SimData settings = GetData(sim);

            foreach (OnAllow del in OnAllowLiaison.GetInvocationList())
            {
                if (!del(stats, settings, check)) return false;
            }

            return true;
        }

        public static event OnDualAllowFunc OnAllowAffair;

        public bool AllowAffair(IScoringGenerator stats, SimDescription actor, SimDescription target, AllowCheck check)
        {
            if (!IsAffair(actor, target)) return true;

            if (OnAllowAffair == null) return false;

            SimData actorData = GetData(actor);
            SimData targetData = GetData(target);

            foreach (OnDualAllowFunc del in OnAllowAffair.GetInvocationList())
            {
                if (!del(stats, actorData, targetData, check)) return false;
            }

            return true;
        }

        public bool Allow(IScoringGenerator stats, SimDescription actor, SimDescription target)
        {
            return Allow(stats, actor, target, AllowCheck.Active);
        }
        public bool Allow(IScoringGenerator stats, SimDescription actor, SimDescription target, AllowCheck check)
        {
            if (Personalities.IsOpposing(stats, actor, target, true))
            {
                stats.IncStat("Opposing Clan");
                return false;
            }
            else if (!Flirts.CanHaveAutonomousRomance(stats, actor, target, ((check & AllowCheck.Active) == AllowCheck.Active)))
            {
                return false;
            }

            return AllowPartner(stats, actor, target, check);
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

        protected override bool PrivateAllow(IScoringGenerator stats, SimDescription sim, AllowCheck check)
        {
            return Flirts.Allow(stats, sim, check);
        }

        public bool AllowPartner(IScoringGenerator stats, SimDescription actor, SimDescription target, AllowCheck check)
        {
            if (!Allow(stats, actor, check)) return false;
            if (!Allow(stats, target, check)) return false;

            SimData actorData = GetData(actor);
            SimData targetData = GetData(target);

            if (!actorData.Allowed<AllowCasteRomanceOption>(targetData, true))
            {
                stats.IncStat("Caste Romance Denied");
                return false;
            }
            else if (actorData.Disallowed<DisallowCasteRomanceOption>(targetData, true))
            {
                stats.IncStat("Caste Romance Denied");
                return false;
            }
            else if (!DualAllow(stats, actorData, targetData, check))
            {
                return false;
            }

            return true;
        }

        public bool AllowMarriage(IScoringGenerator stats, SimDescription actor, SimDescription target, AllowCheck check)
        {
            if (!AllowMarriage(stats, actor, check)) return false;
            if (!AllowMarriage(stats, target, check)) return false;

            return AllowPartner(stats, actor, target, check);
        }
        public bool AllowMarriage(IScoringGenerator stats, SimDescription sim, AllowCheck check)
        {
            if (!Allow(stats, sim, check)) return false;

            if (!GetValue<AllowMarriageOption, bool>(sim))
            {
                stats.IncStat("Allow: Marriage Denied");
                return false;
            }

            return true;
        }

        public event OnAllow OnAllowBreakup;

        public bool AllowBreakup(IScoringGenerator stats, SimDescription sim, AllowCheck check)
        {
            if (sim.Partner == null) return true;

            if (OnAllowBreakup == null) return false;

            SimData simData = GetData(sim);

            foreach (OnAllow allow in OnAllowBreakup.GetInvocationList())
            {
                if (!allow(stats, simData, check)) return false;
            }

            return true;
        }

        public List<SimDescription> FindPartneredFor(SimDescription sim)
        {
            if ((sim == null) || (sim.Household == null))
            {
                IncStat("Taken: Invalid Sim");
                return null;
            }

            List<SimDescription> choices = new List<SimDescription>();

            foreach (SimDescription potential in Sims.All)
            {
                if (sim == potential)
                {
                    //IncStat("Taken: Myself");
                }
                else if (!Romances.Allow(this, sim, potential))
                {
                    IncStat("Taken: User Denied");
                }
                else if (Situations.IsBusy(this, potential, true))
                {
                    IncStat("Taken: Busy");
                }
                else if (!potential.Marryable)
                {
                    IncStat("Taken: Not Marryable");
                }
                else if (potential.Partner == null)
                {
                    IncStat("Taken: No Partner");
                }
                else if (sim.Teen != potential.Teen)
                {
                    IncStat("Taken: Wrong Age");
                }
                else if (potential.Household == null)
                {
                    IncStat("Taken: Invalid Sim");
                }
                else
                {
                    choices.Add(potential);
                }

                Main.Sleep("ManagerRomance:FindPartneredFor");
            }

            return choices;
        }

        public void AddWoohooerNotches(SimDescription a, SimDescription b, bool risky, bool tryForBaby)
        {
            if (sWoohooAddNotch.Valid)
            {
                bool allow = false;

                if ((Households.AllowGuardian(a)) || (Pregnancies.Allow(this, a)))
                {
                    allow = true;
                }
                else if ((Households.AllowGuardian(b)) || (Pregnancies.Allow(this, b)))
                {
                    allow = true;
                }

                if (!allow) return;

                sWoohooAddNotch.Invoke<object>(new object[] { a, b, risky, tryForBaby });
            }
        }

        public void HandleMarriageName(SimDescription a, SimDescription b, bool actorOnEither)
        {
            string lastName = null;

            bool wasEither = false;

            SimDescription left = a;
            SimDescription right = b;

            if (a.Gender == b.Gender)
            {
                if (actorOnEither)
                {
                    switch (GetValue<SameSexMarriageNameOption, NameTakeType>(right))
                    {
                        case NameTakeType.User:
                        case NameTakeType.Either:
                            left = b;
                            right = a;
                            break;
                    }
                }
                else
                {
                    if (GetValue<SameSexMarriageNameOption, NameTakeType>(right) == NameTakeType.User)
                    {
                        left = b;
                        right = a;
                    }
                }

                lastName = GetData(left).HandleName<SameSexMarriageNameOption>(Sims, right, out wasEither);
            }
            else
            {
                if (actorOnEither)
                {
                    switch (GetValue<MarriageNameOption, NameTakeType>(right))
                    {
                        case NameTakeType.User:
                        case NameTakeType.Either:
                            left = b;
                            right = a;
                            break;
                    }
                }
                else
                {
                    if (GetValue<MarriageNameOption, NameTakeType>(right) == NameTakeType.User)
                    {
                        left = b;
                        right = a;
                    }
                }

                lastName = GetData(left).HandleName<MarriageNameOption>(Sims, right, out wasEither);
            }

            if ((actorOnEither) && (wasEither))
            {
                lastName = a.LastName;
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                a.LastName = lastName;
                b.LastName = lastName;

                if (GetValue<MarriageScenario.RenameChildrenOption, bool>())
                {
                    bool onlyMutual = GetValue<MarriageScenario.RenameOnlyMutualOption,bool>();

                    foreach (SimDescription child in Relationships.GetChildren(a))
                    {
                        if (a.Household != child.Household) continue;

                        if (Households.AllowGuardian(child)) continue;

                        if (onlyMutual)
                        {
                            if (!Relationships.GetParents(child).Contains(b)) continue;
                        }

                        child.LastName = a.LastName;
                    }

                    foreach (SimDescription child in Relationships.GetChildren(b))
                    {
                        if (b.Household != child.Household) continue;

                        if (Households.AllowGuardian(child)) continue;

                        if (onlyMutual)
                        {
                            if (!Relationships.GetParents(child).Contains(a)) continue;
                        }

                        child.LastName = b.LastName;
                    }
                }
            }
        }

        public static bool AreUnPartnered (SimDescription a, SimDescription b)
        {
            if ((a.Partner == null) && (b.Partner == null)) return true;

            return (a.Partner == b);
        }

        public static bool IsAffair(SimDescription sim, SimDescription potential)
        {
            if (((sim.Partner != null) || (potential.Partner != null)) && (sim.Partner != potential))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected static void ForceChangeState(Relationship relation, LongTermRelationshipTypes state)
        {
            LongTermRelationship.InteractionBits bits = relation.LTR.LTRInteractionBits & (LongTermRelationship.InteractionBits.HaveBeenBestFriends | LongTermRelationship.InteractionBits.HaveBeenFriends | LongTermRelationship.InteractionBits.HaveBeenPartners);

            relation.LTR.ForceChangeState(state);
            relation.LTR.AddInteractionBit(bits);
        }

        public bool BumpToLowerState(Common.IStatGenerator stats, SimDescription a, SimDescription b)
        {
            return BumpToLowerState(stats, a, b, true);
        }
        public bool BumpToLowerState(Common.IStatGenerator stats, SimDescription a, SimDescription b, bool story)
        {
            if (a.Partner == b)
            {
                if ((!AllowBreakup(this, a, Managers.Manager.AllowCheck.None)) || (!AllowBreakup(this, b, Managers.Manager.AllowCheck.None)))
                {
                    IncStat("BumpDown: Breakup: User Denied");
                    stats.IncStat("BumpDown: Breakup: User Denied");
                    return false;
                }

                SetElapsedTime<DayOfLastPartnerOption>(a);
                SetElapsedTime<DayOfLastPartnerOption>(b);

                try
                {
                    a.Partner = null;
                }
                catch (Exception e)
                {
                    Common.DebugException(a, e);
                }

                try
                {
                    b.Partner = null;
                }
                catch (Exception e)
                {
                    Common.DebugException(b, e);
                }

                if ((a.CreatedSim != null) && (a.CreatedSim.BuffManager != null))
                {
                    a.CreatedSim.BuffManager.RemoveElement(BuffNames.NewlyEngaged);
                    a.CreatedSim.BuffManager.RemoveElement(BuffNames.FirstKiss);
                    a.CreatedSim.BuffManager.RemoveElement(BuffNames.FirstRomance);
                    a.CreatedSim.BuffManager.RemoveElement(BuffNames.JustMarried);
                    a.CreatedSim.BuffManager.RemoveElement(BuffNames.WeddingDay);
                }

                if ((b.CreatedSim != null) && (b.CreatedSim.BuffManager != null))
                {
                    b.CreatedSim.BuffManager.RemoveElement(BuffNames.NewlyEngaged);
                    b.CreatedSim.BuffManager.RemoveElement(BuffNames.FirstKiss);
                    b.CreatedSim.BuffManager.RemoveElement(BuffNames.FirstRomance);
                    b.CreatedSim.BuffManager.RemoveElement(BuffNames.JustMarried);
                    b.CreatedSim.BuffManager.RemoveElement(BuffNames.WeddingDay);
                }

                if (story)
                {
                    Stories.PrintStory(this, "Breakup", new object[] { a, b }, null);
                }
            }

            Relationship relation = ManagerSim.GetRelationship(a, b);
            if (relation != null)
            {
                LongTermRelationshipTypes currentState = relation.LTR.CurrentLTR;

                LongTermRelationshipTypes nextState = ChangeRelationship.NextNegativeRomanceState(currentState, relation.IsPetToPetRelationship);
                if (nextState != LongTermRelationshipTypes.Undefined)
                {
                    float liking = relation.LTR.Liking;

                    AddStat("BumpDown: Pre", liking);
                    stats.AddStat("BumpDown: Pre", liking);

                    ForceChangeState(relation, nextState);

                    int score = AddScoring("BumpDown: Hate Loss", ScoringLookup.GetScore ("HateRomanceLoss", b));
                    if ((SimTypes.IsSelectable(a)) || (SimTypes.IsSelectable(b)) || (stats.AddScoring("BumpDown: Hate Loss", score) <= 0))
                    {
                        if (liking > relation.LTR.Liking)
                        {
                            try
                            {
                                relation.LTR.SetLiking(liking);
                            }
                            catch (Exception e)
                            {
                                Common.DebugException(a, b, e);

                                IncStat("BumpDown: Reset Liking Fail");
                                stats.IncStat("BumpDown: Reset Liking Fail");
                            }
                        }
                    }

                    AddStat("BumpDown: Post", relation.LTR.Liking);
                    stats.AddStat("BumpDown: Post", relation.LTR.Liking);

                    IncStat("BumpDown " + relation.LTR.CurrentLTR);
                    stats.IncStat("BumpDown " + relation.LTR.CurrentLTR);
                }
            }
            return true;
        }

        public bool DumpOtherRomances(Common.IStatGenerator stats, SimDescription sim, SimDescription retain)
        {
            if ((sim.Partner != null) && (sim.Partner != retain))
            {
                if (!BumpToLowerState(stats, sim, sim.Partner)) return false;
            }

            int baseDropChance = GetValue<BaseChanceToDropRomanceOption, int>();

            List<Relationship> relations = new List<Relationship>(Relationship.GetRelationships(sim));
            foreach (Relationship relation in relations)
            {
                SimDescription other = relation.GetOtherSimDescription(sim);
                if (other == null) continue;

                if (other == retain) continue;

                if (relation.AreRomantic())
                {
                    if (((AddScoring("DropFlirt", baseDropChance, ScoringLookup.OptionType.Bounded, sim)) > 0) ||
                        ((AddScoring("DropFlirt", baseDropChance, ScoringLookup.OptionType.Bounded, other)) > 0))
                    {
                        if (!BumpToLowerState(stats, sim, other, false)) return false;
                    }
                }
            }

            return true;
        }

        public bool BumpToEngagement(Common.IStatGenerator stats, SimDescription a, SimDescription b)
        {
            if (a.Partner != b)
            {
                IncStat("BumpUp: Not Partner");
                stats.IncStat("BumpUp: Not Partner");
                return false;
            }

            Relationship relation = ManagerSim.GetRelationship(a, b);

            while (relation.CurrentLTR != LongTermRelationshipTypes.Fiancee)
            {
                if (!BumpToHigherState(stats, a, b)) break;
            }

            if (relation.CurrentLTR != LongTermRelationshipTypes.Fiancee)
            {
                IncStat("BumpUp: Not Engaged");
                stats.IncStat("BumpUp: Not Engaged");
                return false;
            }

            if (a.CreatedSim != null)
            {
                a.CreatedSim.BuffManager.AddElement(BuffNames.NewlyEngaged, Origin.FromSocialization);
            }

            if (b.CreatedSim != null)
            {
                b.CreatedSim.BuffManager.AddElement(BuffNames.NewlyEngaged, Origin.FromSocialization);
            }

            return true;
        }

        public bool BumpToHighestState(Common.IStatGenerator stats, SimDescription a, SimDescription b)
        {
            while (BumpToHigherState (stats, a, b));

            if ((!a.IsMarried) && (!b.IsMarried))
            {
                IncStat("BumpUp: Not Married");
                stats.IncStat("BumpUp: Not Married");
                return false;
            }

            if (a.CreatedSim != null)
            {
                a.CreatedSim.BuffManager.AddElement(BuffNames.JustMarried, Origin.FromSocialization);
                a.CreatedSim.BuffManager.RemoveElement(BuffNames.NewlyEngaged);
                ActiveTopic.AddToSim(a.CreatedSim, "Wedding");
            }

            if (b.CreatedSim != null)
            {
                b.CreatedSim.BuffManager.AddElement(BuffNames.JustMarried, Origin.FromSocialization);
                b.CreatedSim.BuffManager.RemoveElement(BuffNames.NewlyEngaged);
                ActiveTopic.AddToSim(b.CreatedSim, "Wedding");
            }

            if ((a.CreatedSim != null) && (b.CreatedSim != null))
            {
                SocialCallback.GiveDaysOffIfRequired(a.CreatedSim, b.CreatedSim);
            }

            a.Genealogy.Marry(b.Genealogy);

            Relationship relationship = ManagerSim.GetRelationship(a, b);
            if (relationship != null)
            {
                MidlifeCrisisManager.OnBecameMarried(a, b);

                relationship.LTR.RemoveInteractionBit(LongTermRelationship.InteractionBits.Divorce);
                relationship.LTR.AddInteractionBit(LongTermRelationship.InteractionBits.Marry);
 
                relationship.SetMarriedInGame ();

                if (SeasonsManager.Enabled)
                {
                    relationship.WeddingAnniversary = new WeddingAnniversary(SeasonsManager.CurrentSeason, (int)SeasonsManager.DaysElapsed);
                    relationship.WeddingAnniversary.SimA = relationship.SimDescriptionA;
                    relationship.WeddingAnniversary.SimB = relationship.SimDescriptionB;
                    relationship.WeddingAnniversary.CreateAlarm();
                }

                return true;
            }
            else
            {
                IncStat("BumpUp: Relation Fail");
                stats.IncStat("BumpUp: Relation Fail");
                return false;
            }
        }

        public bool BumpToHigherState(Common.IStatGenerator stats, SimDescription a, SimDescription b)
        {
            Relationship relation = ManagerSim.GetRelationship(a, b);
            if (relation == null)
            {
                IncStat("BumpUp: Bad Relation");
                stats.IncStat("BumpUp: Bad Relation");
                return false;
            }

            LongTermRelationshipTypes currentState = relation.LTR.CurrentLTR;

            LongTermRelationshipTypes nextState = ChangeRelationship.NextPositiveRomanceState(currentState, relation.IsPetToPetRelationship);
            if (nextState == LongTermRelationshipTypes.Undefined)
            {
                IncStat("BumpUp: No Next Level");
                stats.IncStat("BumpUp: No Next Level");
                return false;
            }

            if (currentState == LongTermRelationshipTypes.RomanticInterest)
            {
                if (Flirts.IsCloselyRelated(a, b))
                {
                    IncStat("BumpUp: Related");
                    stats.IncStat("BumpUp: Related");
                    return false;
                }

                if ((!GetValue<AllowSteadyOption, bool>(a)) || (!GetValue<AllowSteadyOption, bool>(b)))
                {
                    IncStat("BumpUp: Steady Denied");
                    stats.IncStat("BumpUp: Steady Denied");
                    return false;
                }

                if (!DumpOtherRomances(stats, a, b)) return false;
                if (!DumpOtherRomances(stats, b, a)) return false;

                if ((a.Partner != b) || (b.Partner != a))
                {
                    try
                    {
                        a.SetPartner(b);

                        SetElapsedTime<DayOfLastPartnerOption>(a);
                        SetElapsedTime<DayOfLastPartnerOption>(b);

                        SetElapsedTime<DayOfLastRomanceOption>(a);
                        SetElapsedTime<DayOfLastRomanceOption>(b);
                    }
                    catch (Exception e)
                    {
                        Common.DebugException(a, b, e);
                    }
                }
            }
            else if (currentState == LongTermRelationshipTypes.Partner)
            {
                if ((a.TeenOrBelow) || (b.TeenOrBelow))
                {
                    if ((!GetValue<AllowMarriageOption, bool>(a)) || (!GetValue<AllowMarriageOption, bool>(b)))
                    {
                        IncStat("BumpUp: Marriage Denied");
                        stats.IncStat("BumpUp: Marriage Denied");
                        return false;
                    }
                    else if (GetValue<MarriageScenario.AllowTeenOnlyOnPregnancyOption, bool>())
                    {
                        if ((!a.IsPregnant) && (!b.IsPregnant))
                        {
                            IncStat("BumpUp: Non-Pregnant Marriage Denied");
                            stats.IncStat("BumpUp: Non-Pregnant Marriage Denied");
                            return false;
                        }
                    }
                }

                relation.ProposerDesc = a;
            }

            float liking = relation.LTR.Liking;

            ForceChangeState(relation, nextState);

            if (liking > relation.LTR.Liking)
            {
                relation.LTR.SetLiking(liking);
            }

            if (currentState == relation.LTR.CurrentLTR)
            {
                IncStat("Invalid: " + currentState);
                stats.IncStat("Invalid: " + currentState);
                return false;
            }

            IncStat("BumpUp: " + currentState + " to " + relation.LTR.CurrentLTR);
            stats.IncStat("BumpUp: " + currentState + " to " + relation.LTR.CurrentLTR);
            return true;
        }

        public class Updates : AlertLevelOption<ManagerRomance>
        {
            public Updates()
                : base(AlertLevel.All)
            { }

            public override string GetTitlePrefix()
            {
                return "Stories";
            }
        }

        public class DebugOption : DebugLevelOption<ManagerRomance>
        {
            public DebugOption()
                : base(Common.DebugLevel.Quiet)
            { }
        }

        public class SpeedOption : SpeedBaseOption<ManagerRomance>
        {
            public SpeedOption()
                : base(300, true)
            { }
        }

        public class DumpStatsOption : DumpStatsBaseOption<ManagerRomance>
        {
            public DumpStatsOption()
                : base(5)
            { }
        }

        protected class DumpScoringOption : DumpScoringBaseOption<ManagerRomance>
        {
            public DumpScoringOption()
            { }
        }

        public class TicksPassedOption : TicksPassedBaseOption<ManagerRomance>
        {
            public TicksPassedOption()
            { }
        }

        public class BaseChanceToDropRomanceOption : IntegerManagerOptionItem<ManagerRomance>
        {
            public BaseChanceToDropRomanceOption()
                : base(0)
            { }

            public override string GetTitlePrefix()
            {
                return "BaseChanceToDropRomance";
            }
        }
    }
}

