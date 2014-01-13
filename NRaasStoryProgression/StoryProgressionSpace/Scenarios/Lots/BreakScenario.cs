using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Plumbing;
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
    public abstract class BreakScenario : RelationshipScenario, IInvestigationScenario
    {
        GameObject mVictim;

        bool mFail = false;

        static UpdateDelegate OnInvestigateScenario;

        public BreakScenario() // Required for DerivativeSearch
            : base(-10)
        { }
        protected BreakScenario(BreakScenario scenario)
            : base (scenario)
        {
            mVictim = scenario.mVictim;
            mFail = scenario.mFail;
        }

        protected bool Fail
        {
            get { return mFail; }
        }

        protected virtual bool IsFail(SimDescription sim, SimDescription target)
        {
            return false;
        }

        public override bool IsFriendly
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
            get { return "InvestigateBreak"; }
        }

        public abstract int InvestigateMinimum
        {
            get;
        }

        public abstract int InvestigateMaximum
        {
            get;
        }

        protected override bool TestRelationship
        {
            get { return false; }
        }

        protected override bool Allow()
        {
            if (!GetValue<AllowBreakOption, bool>()) return false;

            return base.Allow();
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
            List<GameObject> breakables = new List<GameObject>();

            foreach (Lot lot in ManagerLot.GetOwnedLots(Target))
            {
                foreach (GameObject obj in lot.GetObjects<GameObject>())
                {
                    if (obj == null) continue;

                    if (obj.InUse) continue;

                    if (!obj.InWorld) continue;

                    if (!obj.IsRepairable) continue;

                    RepairableComponent component = obj.Repairable;
                    if (component == null) continue;

                    if (component.Broken) continue;

                    if (!component.CanBreak()) continue;

                    breakables.Add(obj);
                }
            }

            if (breakables.Count == 0) return false;

            if (!base.PrivateUpdate(frame)) return false;

            mVictim = RandomUtil.GetRandomObjectFromList(breakables);

            RepairableComponent repair = mVictim.Repairable;
            repair.BreakObject(Sim.CreatedSim, true);

            mFail = IsFail(Sim, Target);

            if (mFail)
            {
                int cost = (int)(mVictim.Value * 1.5);

                Money.AdjustFunds(Sim, "Damages", -cost);

                Money.AdjustFunds(Target, "Insurance", cost);
            }

            Add(frame, new ExistingEnemyManualScenario(Sim, Target, Delta, 0, GetTitlePrefix(PrefixType.Story)), ScenarioResult.Start);

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

                if (AllowGoToJail)
                {
                    Manager.AddAlarm(new GoToJailScenario(Sim, Bail));

                    injured = null;
                }
                else
                {
                    injured = Sim;
                }
            }
            
            if ((AllowInjury) && (injured != null) && (injured.CreatedSim != null) && (injured.CreatedSim.LotCurrent == mVictim.LotCurrent))
            {
                Handiness skill = injured.SkillManager.GetSkill<Handiness>(SkillNames.Handiness);
                if ((skill == null) || (RandomUtil.RandomChance01((skill.MaxSkillLevel - skill.SkillLevel) / (float)skill.MaxSkillLevel)))
                {
                    if ((mVictim is Bathtub) || (mVictim is Shower) || (mVictim is Toilet) || (mVictim is Sink))
                    {
                        ManagerSim.AddBuff(Manager, injured, BuffNames.Soaked, Origin.FromFailedRepair);

                        Manager.AddAlarm(new GoToHospitalScenario(injured, killer, mVictim, "InjuredDrown", SimDescription.DeathType.Drown));
                    }
                    else
                    {
                        ManagerSim.AddBuff(Manager, injured, BuffNames.SingedElectricity, Origin.FromFailedRepair);

                        Manager.AddAlarm(new GoToHospitalScenario(injured, killer, mVictim, "InjuredElectrocute", SimDescription.DeathType.Electrocution));
                    }
                }
            }

            return Situations.PushVisit(this, Sim, Target.LotHome);
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

        public class AllowBreakOption : BooleanManagerOptionItem<ManagerLot>
        {
            public AllowBreakOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AllowBreak";
            }
        }
    }
}
