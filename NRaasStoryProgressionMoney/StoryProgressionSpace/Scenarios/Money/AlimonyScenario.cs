using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Propagation;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public class AlimonyScenario : SimScenario
    {
        int mTotalPayments = 0;

        public AlimonyScenario()
        { }
        protected AlimonyScenario(AlimonyScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Alimony";
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override int PushChance
        {
            get
            {
                if (GetValue<ShowAllOption, bool>())
                {
                    return 100;
                }
                else
                {
                    return 20;
                }
            }
        }

        protected override bool Allow()
        {
            if (GetValue<PaymentOption, int>() <= 0) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.Adults;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (Deaths.IsDying(sim))
            {
                IncStat("Dying");
                return false;
            }
            else if (sim.Household == null)
            {
                IncStat("No Home");
                return false;
            }
            else if (!Households.AllowGuardian(sim))
            {
                IncStat("Too Young");
                return false;
            }
            else if (sim.Genealogy == null)
            {
                IncStat("No Gene");
                return false;
            }
            else if (!Money.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            else if (sim.TraitManager.HasElement(TraitNames.NoBillsEver))
            {
                IncStat("No Bills");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            List<SimDescription> children = new List<SimDescription>();

            int chance = GetValue<AlimonyChanceOption, int>(Sim);

            foreach (SimDescription child in Relationships.GetChildren(Sim))
            {
                if (GetValue<ForTeensOption, bool>())
                {
                    if (child.YoungAdultOrAbove) continue;
                }
                else
                {
                    if (child.TeenOrAbove) continue;
                }

                if (SimTypes.IsDead(child)) continue;

                if (child.IsMarried) continue;

                if (child.Household == Sim.Household) continue;

                if (GetValue<NetWorthOption,int>(child.Household) >= GetValue<NetWorthOption,int>(Sim.Household))
                {
                    IncStat("Unnecessary");
                    continue;
                }

                if (!GetValue<ForAdoptedOption, bool>())
                {
                    List<SimDescription> parents = Relationships.GetParents(child);

                    bool found = false;
                    foreach (SimDescription member in CommonSpace.Helpers.Households.All(child.Household))
                    {
                        if (parents.Contains(member))
                        {
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        IncStat("Child Adopted");
                        continue;
                    }
                }

                children.Add(child);
            }

            if (children.Count == 0) return false;

            int perChildPayment = GetValue<PaymentOption, int>();
            if (GetValue<IsRichOption,bool>(Sim.Household))
            {
                perChildPayment *= GetValue<RichMultipleOption, int>();
            }

            if (AddScoring("CaresAboutChildren", chance, ScoringLookup.OptionType.Chance, Sim) <= 0)
            {
                foreach (SimDescription child in children)
                {
                    if (RandomUtil.RandomChance(GetValue<PropagationChanceOption, int>()))
                    {
                        Add(frame, new PropagateAlimonyFailScenario(child, Sim), ScenarioResult.Start);
                    }
                }

                AddValue<AlimonyLapsesOption, int>(Sim, 1);

                if ((GetValue<AlimonyJailOption,bool>()) && (RandomUtil.RandomChance(GetValue<AlimonyLapsesOption, int>(Sim) * 10)))
                {
                    Add(frame, new GoToJailScenario(Sim, "AlimonyJail", perChildPayment), ScenarioResult.Start);
                
                    SetValue<AlimonyLapsesOption,int>(Sim, 0);
                }

                Add(frame, new SuccessScenario(), ScenarioResult.Start);
                return true;
            }
            else
            {
                SetValue<AlimonyLapsesOption,int>(Sim, 0);
            }

            foreach (SimDescription child in children)
            {
                if (!Money.Allow(this, child, Managers.Manager.AllowCheck.None))
                {
                    IncStat("Money Denied Child");
                    continue;
                }

                int payment = perChildPayment;
                if (Sim.FamilyFunds < payment)
                {
                    payment = Sim.FamilyFunds;
                }

                mTotalPayments += payment;

                if (payment == 0)
                {
                    IncStat("Insufficient");
                }
                else
                {
                    Money.AdjustFunds(Sim, "ChildSupport", -payment);

                    Money.AdjustFunds(child, "ChildSupport", payment);

                    AddStat("Paid", payment);
                }
            }

            return (mTotalPayments > 0);
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Money;
            }

            if (parameters == null)
            {
                parameters = new object[] { Sim, mTotalPayments };
            }

            if (mTotalPayments == 0)
            {
                name += "Fail";
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new AlimonyScenario(this);
        }

        public class PaymentOption : IntegerScenarioOptionItem<ManagerMoney, AlimonyScenario>, ManagerMoney.IFeesOption
        {
            public PaymentOption()
                : base(100)
            { }

            public override string GetTitlePrefix()
            {
                return "AlimonyPayment";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }

        public class ForTeensOption : BooleanManagerOptionItem<ManagerMoney>, ManagerMoney.IFeesOption
        {
            public ForTeensOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AlimonyforTeenageChildren";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<PaymentOption,int>() <= 0) return false;

                return base.ShouldDisplay();
            }
        }

        public class ShowAllOption : BooleanManagerOptionItem<ManagerMoney>, ManagerMoney.IFeesOption
        {
            public ShowAllOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ShowAllAlimonyStories";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<PaymentOption,int>() <= 0) return false;

                return base.ShouldDisplay();
            }
        }

        public class ForAdoptedOption : BooleanManagerOptionItem<ManagerMoney>, ManagerMoney.IFeesOption
        {
            public ForAdoptedOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AlimonyForAdopted";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<PaymentOption, int>() <= 0) return false;

                return base.ShouldDisplay();
            }
        }

        public class AlimonyJailOption : BooleanManagerOptionItem<ManagerMoney>, ManagerMoney.IFeesOption, IDebuggingOption
        {
            public AlimonyJailOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AlimonyJail";
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<PaymentOption, int>() <= 0) return false;

                return base.ShouldDisplay();
            }
        }

        public class RichMultipleOption : IntegerManagerOptionItem<ManagerMoney>, ManagerMoney.IFeesOption
        {
            public RichMultipleOption()
                : base(5)
            { }

            public override string GetTitlePrefix()
            {
                return "RichAlimonyMultiple";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<PaymentOption, int>() <= 0) return false;

                return base.ShouldDisplay();
            }
        }

        public class PropagationChanceOption : IntegerManagerOptionItem<ManagerMoney>, ManagerMoney.IFeesOption, IDebuggingOption
        {
            public PropagationChanceOption()
                : base(20)
            { }

            public override string GetTitlePrefix()
            {
                return "AlimonyFailPropagationChance";
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<PaymentOption, int>() <= 0) return false;

                return base.ShouldDisplay();
            }
        }
    }
}
