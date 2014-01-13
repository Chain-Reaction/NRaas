using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Lots
{
    public abstract class ArsonScenario : RelationshipScenario, IInvestigationScenario
    {
        bool mFail = false;

        static UpdateDelegate OnInvestigateScenario;

        public ArsonScenario(int delta)
            : base(delta)
        { }
        protected ArsonScenario(ArsonScenario scenario)
            : base (scenario)
        {
            mFail = scenario.mFail;
        }

        public bool Fail
        {
            get { return mFail; }
        }

        public override bool IsFriendly
        {
            get { return false; }
        }

        protected override bool TestRelationship
        {
            get { return false; }
        }

        protected virtual bool IsFail(SimDescription sim, SimDescription target)
        {
            return false;
        }

        protected override bool ShouldReport // Reported by sub scenario
        {
            get { return false; }
        }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected abstract bool AllowInjury
        {
            get;
        }

        public abstract bool AllowGoToJail
        {
            get;
        }

        protected virtual int Bail
        {
            get { return Manager.GetValue<GotArrestedScenario.BailOption, int>(); }
        }

        public virtual string InvestigateStoryName
        {
            get { return "InvestigateArson"; }
        }

        public abstract int InvestigateMinimum
        {
            get;
        }

        public abstract int InvestigateMaximum
        {
            get;
        }

        protected abstract UpdateDelegate AdditionalScenario
        {
            get;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!sim.CanStartFires)
            {
                IncStat("No Start");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (sim.LotHome == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (sim.Household == Sim.Household)
            {
                IncStat("Same Home");
                return false;
            }
            else
            {
                foreach (SimDescription member in HouseholdsEx.All(sim.Household))
                {
                    if (member.TraitManager == null) continue;

                    if (member.TraitManager.HasElement(TraitNames.FireproofLot))
                    {
                        IncStat("Fireproof");
                        return false;
                    }
                }
            }

            return base.TargetAllow(sim);
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!Sims.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }

            return (base.CommonAllow(sim));
        }

        public bool InstallInvestigation(Scenario.UpdateDelegate func)
        {
            OnInvestigateScenario += func;
            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            List<GameObject> burnables = new List<GameObject>();

            foreach (Lot lot in ManagerLot.GetOwnedLots(Target))
            {
                foreach (GameObject obj in lot.GetObjects<GameObject>())
                {
                    if (obj == null) continue;

                    if (obj.GetFireType() == FireType.DoesNotBurn) continue;

                    if (string.IsNullOrEmpty(obj.CatalogName)) continue;

                    if (obj is Sim) continue;

                    if (obj is ICrib) continue;

                    if (obj.InUse) continue;

                    if (!obj.InWorld) continue;

                    if (obj.LotCurrent == null) continue;

                    LotLocation loc = LotLocation.Invalid;
                    ulong lotLocation = World.GetLotLocation(obj.PositionOnFloor, ref loc);
                    if (!World.HasSolidFloor(obj.LotCurrent.mLotId, loc)) continue;

                    burnables.Add(obj);
                }
            }

            if (burnables.Count == 0)
            {
                IncStat("No Burnables");
                return false;
            }

            GameObject victim = RandomUtil.GetRandomObjectFromList(burnables);

            if (!Situations.PushVisit(this, Sim, Target.LotHome))
            {
                IncStat("Push Fail");
                return false;
            }

            mFail = IsFail(Sim, Target);

            if (mFail)
            {
                int cost = (int)(victim.Value * 1.5);

                Money.AdjustFunds(Sim, "Damages", -cost);

                Money.AdjustFunds(Target, "Insurance", cost);
            }

            Manager.AddAlarm(new BurnScenario(Sim, Target, victim, this, mFail));
            return true;
        }

        protected override bool Push()
        {
            // Pushed in PrivateUpdate()
            return true;
        }

        public class BurnScenario : DualSimScenario, IAlarmScenario
        {
            ArsonScenario mParentScenario;

            GameObject mVictim;

            bool mFail = false;

            public BurnScenario(SimDescription sim, SimDescription target, GameObject victim, ArsonScenario parentScenario, bool fail)
                : base (sim, target)
            {
                mVictim = victim;
                mParentScenario = parentScenario;
                mFail = fail;
            }
            protected BurnScenario(BurnScenario scenario)
                : base(scenario)
            {
                mParentScenario = scenario.mParentScenario;
                mVictim = scenario.mVictim;
                mFail = scenario.mFail;
            }

            public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
            {
                return alarms.AddAlarm(this, 1f);
            }

            public bool AllowGoToJail
            {
                get { return mParentScenario.AllowGoToJail; }
            }

            protected int Bail
            {
                get { return mParentScenario.Bail; }
            }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type == PrefixType.Pure)
                {
                    return "Burn";
                }
                else
                {
                    return mParentScenario.GetTitlePrefix(type);
                }
            }

            protected override bool CheckBusy
            {
                get { return false; }
            }

            protected override bool Progressed
            {
                get { return true; }
            }

            protected override ICollection<SimDescription> GetSims()
            {
                return null;
            }

            protected override ICollection<SimDescription> GetTargets(SimDescription sim)
            {
                return null;
            }

            protected override bool Allow()
            {
                if (mVictim == null) return false;

                if (!GetValue<AllowArsonOption, bool>()) return false;

                if (Firefighter.Instance == null)
                {
                    IncStat("No Firefighter");
                    return false;
                }

                return base.Allow();
            }

            public static bool AddFire(Common.IStatGenerator stats, GameObject obj)
            {
                LotLocation loc = LotLocation.Invalid;
                ulong lotLocation = World.GetLotLocation(obj.PositionOnFloor, ref loc);

                if (!World.HasSolidFloor(obj.LotCurrent.mLotId, loc))
                {
                    stats.IncStat("No Solid Floor");
                    return false;
                }

                if (lotLocation == 0x0L)
                {
                    stats.IncStat("No Location");
                    return false;
                }

                if (Sims3.Gameplay.Objects.Fire.CreateFire(lotLocation, loc) == null)
                {
                    stats.IncStat("CreateFire Fail");
                    return false;
                }

                return true;
            }

            public bool InstallInvestigation(Scenario.UpdateDelegate func)
            {
                OnInvestigateScenario += func;
                return true;
            }

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                if (!AddFire(this, mVictim))
                {
                    IncStat("Fire Fail");
                    return false;
                }

                Add(frame, new ExtinguishScenario(mVictim.LotCurrent), ScenarioResult.Start);

                Add(frame, new ExistingEnemyManualScenario(Sim, Target, mParentScenario.Delta, 0, GetTitlePrefix(PrefixType.Story)), ScenarioResult.Start);

                if (mParentScenario.AdditionalScenario != null)
                {
                    mParentScenario.AdditionalScenario(this, frame);
                }

                if (OnInvestigateScenario != null)
                {
                    OnInvestigateScenario(this, frame);
                }

                return true;
            }

            protected override bool Push()
            {
                SimDescription injured = Target;
                SimDescription killer = Sim;

                if (mFail)
                {
                    killer = null;

                    if (mParentScenario.AllowGoToJail)
                    {
                        Manager.AddAlarm(new GoToJailScenario(Sim, Bail));

                        injured = null;
                    }
                    else
                    {
                        injured = Sim;
                    }
                }

                if ((mParentScenario.AllowInjury) && (injured != null) && (injured.CreatedSim != null) && (injured.CreatedSim.LotCurrent == mVictim.LotCurrent))
                {
                    if (!ManagerSim.HasTrait(injured, TraitNames.ImmuneToFire))
                    {
                        ManagerSim.AddBuff(Manager, injured, BuffNames.Singed, Origin.FromFire);

                        Manager.AddAlarm(new GoToHospitalScenario(injured, killer, "InjuredArson", SimDescription.DeathType.Burn));
                    }
                }

                return true;
            }

            protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
            {
                if (parameters == null)
                {
                    parameters = new object[] { Sim, Target, mVictim.CatalogName };
                }

                if (extended == null)
                {
                    extended = new string[] { mVictim.CatalogName };
                }

                return base.PrintStory(manager, name, parameters, extended, logging);
            }

            public override Scenario Clone()
            {
                return new BurnScenario(this);
            }
        }

        public class AllowArsonOption : BooleanManagerOptionItem<ManagerLot>
        {
            public AllowArsonOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AllowArson";
            }
        }
    }
}
