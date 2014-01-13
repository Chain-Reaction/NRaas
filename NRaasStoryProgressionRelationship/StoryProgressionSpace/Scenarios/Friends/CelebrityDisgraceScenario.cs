using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Friends
{
    public class CelebrityDisgraceScenario : SimEventScenario<DisgracefulActionEvent>
    {
        List<SimDescription> mWitnesses;

        public CelebrityDisgraceScenario()
        { }
        protected CelebrityDisgraceScenario(CelebrityDisgraceScenario scenario)
            : base (scenario)
        {
            if (scenario.mWitnesses != null)
            {
                mWitnesses = new List<SimDescription>(scenario.mWitnesses);
            }
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "CelebrityDisgrace";
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kSimCommittedDisgracefulAction);
        }

        protected override Scenario Handle(Event e, ref ListenerAction result)
        {
            mWitnesses = CheckForWitnesses(e.Actor as Sim);

            return base.Handle(e, ref result);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!GetValue<CelebrityDisgraceOption,bool>(sim))
            {
                IncStat("Age Denied");
                return false;
            }
            else if (!Friends.AllowCelebrity(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            else if (sim.CelebrityManager == null)
            {
                IncStat("No Manager");
                return false;
            }
            else if (sim.CelebrityManager.OwnerSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (sim.CelebrityManager.CantBeDisgraced())
            {
                IncStat("No Disgrace");
                return false;
            }

            return base.Allow(sim);
        }

        public static bool CloseEnough(Sim actor, Sim witness)
        {
            if (witness.RoomId == actor.RoomId)
            {
                return true;
            }
            else if (witness.IsOutside != actor.IsOutside)
            {
                return false;
            }
            else
            {
                int diff = witness.Level - actor.Level;
                if (diff < 0)
                {
                    diff = -diff;
                }

                if ((actor.Level < 0) && (diff > 0))
                {
                    return false;
                }
                else if ((actor.Level >= 0) && (diff > 1))
                {
                    return false;
                }
                else if (witness.CurrentInteraction is ISleeping)
                {
                    return witness.TraitManager.HasElement(TraitNames.LightSleeper);
                }
                else
                {
                    return true;
                }
            }
        }

        public static List<SimDescription> CheckForWitnesses(Sim sim)
        {
            if (sim == null) return null;

            if (sim.LotCurrent == null) return null;

            if (sim.LotCurrent.IsWorldLot) return null;

            List<SimDescription> sims = new List<SimDescription>();
            foreach (Sim witness in sim.LotCurrent.GetSims())
            {
                if (witness.SimDescription.ToddlerOrBelow) continue;

                if (CloseEnough(sim, witness))
                {
                    sims.Add(witness.SimDescription);
                }
            }

            return sims;
        }

        protected bool Cares(SimDescription witness, DisgracefulActionType type, int reportChance, out bool testFriendship)
        {
            if (witness.AssignedRole is RolePaparazzi)
            {
                testFriendship = false;
                return true;
            }

            testFriendship = true;
            switch (type)
            {
                case DisgracefulActionType.BiteSomeoneInPublic:
                    if (witness.IsVampire) return false;
                    break;
                case DisgracefulActionType.Cheating:
                case DisgracefulActionType.WooHooInPublic:
                    if (ManagerFlirt.AreRomantic(Sim, witness))
                    {
                        testFriendship = false;
                    }
                    break;
                case DisgracefulActionType.WooHooWithOccult:
                    if (witness.IsPlayableGhost) return false;

                    if (witness.OccultManager.HasAnyOccultType()) return false;
                    break;
            }

            if (testFriendship)
            {
                if (AddScoring("DisgraceWitness", reportChance, ScoringLookup.OptionType.Chance, witness) >= 0) return false;
            }

            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            CelebrityDisgracefulActionStaticData data;

            DisgracefulActionType disgracefulActionType = Event.DisgracefulActionType;
            if (!CelebrityUtil.sCelebrityDisgracefulActionData.TryGetValue(disgracefulActionType, out data))
            {
                IncStat("No Disgrace Data");
                return false;
            }

            if (data.DisgracedOrigin != Origin.FromFalselyAccused)
            {
                if (HasValue<DisallowByTypeOption, DisgracefulActionType>(Event.DisgracefulActionType))
                {
                    IncStat("User Disallow");
                    return false;
                }

                switch (Event.DisgracefulActionType)
                {
                    case DisgracefulActionType.Cheating:
                    case DisgracefulActionType.ChildOutOfMarriage:
                    case DisgracefulActionType.Divorce:
                    case DisgracefulActionType.WooHooInPublic:
                    case DisgracefulActionType.WooHooWithOccult:
                        if (Sim.HasTrait(TraitNames.AboveReproach))
                        {
                            IncStat("Above Reproach");
                            return false;
                        }
                        break;
                };

                switch (Event.DisgracefulActionType)
                {
                    case DisgracefulActionType.Arrested:
                    case DisgracefulActionType.CaughtSneakingIntoClub:
                    case DisgracefulActionType.Divorce:
                    case DisgracefulActionType.ChildTaken:
                    case DisgracefulActionType.ChildOutOfMarriage:
                    case DisgracefulActionType.JobSetback:
                        // No witness necessary
                        break;
                    default:
                        if ((mWitnesses == null) || (mWitnesses.Count == 0))
                        {
                            IncStat("No Witnesses");
                            return false;
                        }

                        SimDescription otherNaughty = ManagerSim.Find(Event.TargetId);

                        bool valid = false;
                        foreach (SimDescription witness in mWitnesses)
                        {
                            if (witness == Sim) continue;

                            if (witness == otherNaughty) continue;

                            bool testFriendship = true;
                            if (!Cares(witness, Event.DisgracefulActionType, GetValue<ReportChanceOption, int>(), out testFriendship)) continue;

                            if (testFriendship)
                            {
                                if (ManagerFriendship.AreFriends(Sim, witness)) continue;
                            }

                            valid = true;
                            break;
                        }

                        if (!valid)
                        {
                            IncStat("No One Cares");
                            return false;
                        }
                        break;
                }
            }
            else
            {
                if (!GetValue<AllowFalseOption, bool>())
                {
                    IncStat("False Denied");
                    return false;
                }
            }

            if (Sim.CelebrityManager.mDisgracefulActionQueue != null)
            {
                foreach (DisgracefulActionEvent e in Sim.CelebrityManager.mDisgracefulActionQueue)
                {
                    if ((e.TargetId == Event.TargetId) && (e.DisgracefulActionType == Event.DisgracefulActionType))
                    {
                        IncStat("Already Queued");
                        return false;
                    }
                }
            }

            if (!RandomUtil.RandomChance01(data.Chance))
            {
                IncStat("Chance Fail");
                return false;
            }

            if (Sim.CelebrityManager.mDisgracefulActionQueue == null)
            {
                Sim.CelebrityManager.mDisgracefulActionQueue = new List<DisgracefulActionEvent>();
            }

            Sim.CelebrityManager.mDisgracefulActionQueue.Add(Event);

            new TriggerDisgraceTask(this);
            return true;
        }

        public override Scenario Clone()
        {
            return new CelebrityDisgraceScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerFriendship, CelebrityDisgraceScenario>, ManagerFriendship.ICelebrityOption, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "CelebrityDisgrace";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP3);
            }

            public override bool Install(ManagerFriendship main, bool initial)
            {
                if (initial)
                {
                    ManagerCaste.OnInitializeCastes += OnInitialize;
                }

                return base.Install(main, initial);
            }

            protected static void OnInitialize()
            {
                bool created;
                CasteOptions options = StoryProgression.Main.Options.GetNewCasteOptions("DisgraceByAge", Common.Localize("Caste:DisgraceByAge"), out created);
                if (created)
                {
                    options.SetValue<CasteAutoOption, bool>(true);
                    options.SetValue<CasteAgeOption, List<CASAgeGenderFlags>>(null);
                    options.AddValue<CasteAgeOption, CASAgeGenderFlags>(CASAgeGenderFlags.Teen);
                    options.AddValue<CasteAgeOption, CASAgeGenderFlags>(CASAgeGenderFlags.YoungAdult);
                    options.AddValue<CasteAgeOption, CASAgeGenderFlags>(CASAgeGenderFlags.Adult);
                    options.AddValue<CasteAgeOption, CASAgeGenderFlags>(CASAgeGenderFlags.Elder);

                    options.SetValue<CelebrityDisgraceOption, bool>(true);
                }
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }

        public class AllowFalseOption : BooleanManagerOptionItem<ManagerFriendship>, ManagerFriendship.ICelebrityOption
        {
            public AllowFalseOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "CelebrityAllowFalse";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP3);
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class RelationshipChangeOption : IntegerManagerOptionItem<ManagerFriendship>, ManagerFriendship.ICelebrityOption
        {
            public RelationshipChangeOption()
                : base(-(int)CelebrityManager.kLTRLostFromDisgrace)
            { }

            public override string GetTitlePrefix()
            {
                return "DisgraceRelationshipChange";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP3);
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class ReportChanceOption : IntegerManagerOptionItem<ManagerFriendship>, ManagerFriendship.ICelebrityOption
        {
            public ReportChanceOption()
                : base(50)
            { }

            public override string GetTitlePrefix()
            {
                return "DisgraceReportChance";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP3);
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class RelationDropChanceOption : IntegerManagerOptionItem<ManagerFriendship>, ManagerFriendship.ICelebrityOption
        {
            public RelationDropChanceOption()
                : base(25)
            { }

            public override string GetTitlePrefix()
            {
                return "DisgraceRelationDropChance";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP3);
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class DisallowByTypeOption : MultiEnumManagerOptionItem<ManagerFriendship, DisgracefulActionType>, ManagerFriendship.ICelebrityOption
        {
            public DisallowByTypeOption()
                : base(new List<DisgracefulActionType>())
            { }

            public override string GetTitlePrefix()
            {
                return "CelebrityDisallowByType";
            }

            protected override string GetLocalizationValueKey()
            {
                return "DisgraceType";
            }

            public override string ValuePrefix
            {
                get { return "Disallowed"; }
            }

            protected override bool Allow(DisgracefulActionType value)
            {
                if (value == DisgracefulActionType.Invalid) return false;

                return base.Allow(value);
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class TriggerDisgraceTask : Common.AlarmTask
        {
            CelebrityDisgraceScenario mScenario;

            public TriggerDisgraceTask(CelebrityDisgraceScenario scenario)
                : base (60, TimeUnit.Minutes)
            {
                mScenario = scenario;
            }

            protected override void OnPerform()
            {
                try
                {
                    mScenario.IncStat("TriggerDisgraceTask");

                    CelebrityManager manager = mScenario.Sim.CelebrityManager;

                    if ((manager.mDisgracefulActionQueue == null) || (manager.mDisgracefulActionQueue.Count == 0x0))
                    {
                        mScenario.IncStat("DisgraceTask: No Task Event");
                        return;
                    }

                    CelebrityDisgracefulActionStaticData data;
                    DisgracefulActionEvent item = manager.mDisgracefulActionQueue[0x0];
                    manager.mDisgracefulActionQueue.Remove(item);

                    DisgracefulActionType disgracefulActionType = item.DisgracefulActionType;

                    if (manager.CantBeDisgraced())
                    {
                        mScenario.IncStat("DisgraceTask: Not Disgracable");
                        return;
                    }

                    if (!CelebrityUtil.sCelebrityDisgracefulActionData.TryGetValue(disgracefulActionType, out data))
                    {
                        mScenario.IncStat("DisgraceTask: No Task Data");
                        return;
                    }

                    BuffManager buffManager = null;

                    if (mScenario.Sim.CreatedSim != null)
                    {
                        buffManager = mScenario.Sim.CreatedSim.BuffManager;
                    }

                    if (buffManager != null)
                    {
                        buffManager.RemoveElement(BuffNames.PubliclyDisgraced);
                    }

                    SimDescription otherNaughty = ManagerSim.Find(item.TargetId);

                    bool flag = (disgracefulActionType == DisgracefulActionType.Cheating) && !mScenario.Sim.TraitManager.HasElement(TraitNames.NoJealousy);

                    int change = NRaas.StoryProgression.Main.GetValue<RelationshipChangeOption, int>();
                    if (change > 0)
                    {
                        change = 0;
                    }

                    foreach (Relationship relationship in Relationship.Get(mScenario.Sim))
                    {
                        SimDescription other = relationship.GetOtherSimDescription(mScenario.Sim);

                        if (other == otherNaughty) continue;

                        if (other.ToddlerOrBelow) continue;

                        bool testFriendship = true;
                        if (!mScenario.Cares(other, disgracefulActionType, mScenario.GetValue<RelationDropChanceOption, int>(), out testFriendship))
                        {
                            mScenario.IncStat("DisgraceTask: Sim Doesn't Care");
                            continue;
                        }

                        if (flag && relationship.AreRomantic())
                        {
                            Sim createdSim = other.CreatedSim;
                            if (createdSim != null)
                            {
                                SocialComponent.OnIWasCheatedOn(createdSim, mScenario.Sim, otherNaughty, JealousyLevel.Medium);
                            }
                        }

                        if (relationship.AreFriends())
                        {
                            mScenario.AddScoring("DisgraceTask: Relation Change", change);

                            relationship.LTR.UpdateLiking(change);
                        }
                    }

                    if (buffManager != null)
                    {
                        buffManager.AddElement(BuffNames.PubliclyDisgraced, data.DisgracedOrigin);
                    }

                    if (data.DisgracedOrigin == Origin.FromFalselyAccused)
                    {
                        manager.IncrementFalselyAccused();
                    }
                    else
                    {
                        manager.IncrementPubliclyDisgraced();
                    }

                    Sim sim = mScenario.Sim.CreatedSim;
                    if (sim != null)
                    {
                        mScenario.IncStat("DisgraceTask: Display Message");

                        sim.ShowTNSAndPlayStingIfSelectable("sting_generic_tragic", data.TnsId, null, sim, null, null, new bool[] { mScenario.Sim.IsFemale }, mScenario.Sim.IsFemale, new object[] { sim });
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(mScenario.Sim, e);
                }
            }
        }
    }
}
