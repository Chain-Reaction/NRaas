using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Money;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Skills
{
    public class RepairedScenario : SimEventScenario<Event>
    {
        GameObject mRepaired;

        Household mHousehold;

        public RepairedScenario()
        { }
        protected RepairedScenario(RepairedScenario scenario)
            : base (scenario)
        {
            mRepaired = scenario.mRepaired;
            mHousehold = scenario.mHousehold;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "RepairedEvent";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kRepairedObject);
        }

        protected override Scenario Handle(Event e, ref ListenerAction result)
        {
            mRepaired = e.TargetObject as GameObject;
            if (mRepaired == null) return null;

            Sim sim = e.Actor as Sim;
            if (sim == null) return null;

            if (mRepaired.LotCurrent == null) return null;

            if (mRepaired.LotCurrent.CanSimTreatAsHome(sim)) return null;

            mHousehold = mRepaired.LotCurrent.Household;

            return base.Handle(e, ref result);
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.ChildOrBelow)
            {
                IncStat("Too Young");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (mHousehold != null)
            {
                SimDescription head = SimTypes.HeadOfFamily(mHousehold);
                if (head != null)
                {
                    Add(frame, new GotPaidScenario(Sim, head, mRepaired), ScenarioResult.Start);
                    return false;
                }
            }

            int basePay = GotPaidScenario.GetBasePay(Sim.OccupationAsSkillBasedCareer);

            int funds = RandomUtil.GetInt(basePay / 2, basePay);

            Money.AdjustFunds(Sim, "Repaired", funds);

            GotPaidScenario.UpdateExperience(Sim, funds);
            return true;
        }

        public override Scenario Clone()
        {
            return new RepairedScenario(this);
        }

        public class GotPaidScenario : MoneyTransferScenario
        {
            GameObject mRepaired;

            public GotPaidScenario(SimDescription sim, SimDescription target, GameObject obj)
                : base(sim, target, 10)
            {
                mRepaired = obj;
            }
            protected GotPaidScenario(GotPaidScenario scenario)
                :base(scenario)
            {
                mRepaired = scenario.mRepaired;
            }

            public override string GetTitlePrefix(PrefixType type)
            {
                return "Repaired";
            }

            protected override string AccountingKey
            {
                get { return "Repaired"; }
            }

            protected override bool CheckBusy
            {
                get { return false; }
            }

            protected override bool AllowActive
            {
                get { return true; }
            }

            protected override int ContinueReportChance
            {
                get { return 10; }
            }

            protected override bool AllowDebt
            {
                get { return true; }
            }

            public override bool IsFriendly
            {
                get { return true; }
            }

            protected override int Minimum
            {
                get 
                {
                    return GetBasePay(Sim.OccupationAsSkillBasedCareer) / 2; 
                }
            }

            protected override int Maximum
            {
                get 
                {
                    return GetBasePay(Sim.OccupationAsSkillBasedCareer); 
                }
            }

            public static void UpdateExperience(SimDescription sim, int funds)
            {
                Skill skill = sim.SkillManager.GetElement(SkillNames.Handiness);
                if (skill != null)
                {
                    skill.UpdateXpForEarningMoney(funds);

                    if (sim.CareerManager != null)
                    {
                        sim.CareerManager.UpdateCareerUI();
                    }
                }
            }

            public static int GetBasePay(SkillBasedCareer career)
            {
                float basePay = Sims3.Gameplay.Services.Repairman.kServiceTuning.kCost;

                if (career != null)
                {
                    SkillBasedCareerStaticData skillData = career.GetOccupationStaticDataForSkillBasedCareer();
                    if ((skillData != null) && (skillData.CorrespondingSkillName == SkillNames.Handiness))
                    {
                        for (int i = 0; i < career.CareerLevel; i++)
                        {
                            basePay *= 1.2f;
                        }
                    }
                }

                return (int)basePay;
            }

            protected override ICollection<SimDescription> GetSims()
            {
                return null;
            }

            protected override ICollection<SimDescription> GetTargets(SimDescription sim)
            {
                return null;
            }

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                if (!base.PrivateUpdate(frame)) return false;

                UpdateExperience(Sim, Funds);
                return true;
            }

            protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
            {
                if (parameters == null)
                {
                    parameters = new object[] { Sim, Target, Funds, mRepaired.CatalogName };
                }

                return base.PrintStory(manager, name, parameters, extended, logging);
            }

            public override Scenario Clone()
            {
                return new GotPaidScenario(this);
            }
        }

        public class Installer : ExpansionInstaller<ManagerSkill>
        {
            protected override bool PrivateInstall(ManagerSkill manager, bool initial)
            {
                manager.AddListener(new RepairedScenario());

                return true;
            }
        }
    }
}
