using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public class ElderSupportScenario : SimScenario
    {
        int mTotalPayments = 0;

        public ElderSupportScenario()
        { }
        protected ElderSupportScenario(ElderSupportScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "ElderSupport";
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
            get { return true; }
        }

        protected override int PushChance
        {
            get { return 20; }
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
            List<SimDescription> parents = new List<SimDescription>();

            foreach (SimDescription parent in Relationships.GetParents(Sim))
            {
                if (SimTypes.IsDead(parent)) continue;

                if (!parent.Elder) continue;

                if (parent.Household == Sim.Household) continue;

                if (!ManagerFriendship.AreFriends(Sim, parent)) continue;

                if (GetValue<NetWorthOption,int>(parent.Household) >= GetValue<NetWorthOption,int>(Sim.Household))
                {
                    IncStat("Unnecessary");
                    continue;
                }

                parents.Add(parent);
            }

            if (parents.Count == 0) return false;

            if (AddScoring("CaresAboutChildren", GetValue<AlimonyChanceOption, int>(Sim), ScoringLookup.OptionType.Chance, Sim) <= 0) return false;

            int perParentPayment = GetValue<PaymentOption, int>();
            if (GetValue<IsRichOption,bool>(Sim.Household))
            {
                perParentPayment *= GetValue<AlimonyScenario.RichMultipleOption, int>();
            }

            foreach (SimDescription parent in parents)
            {
                if (!Money.Allow(this, parent, Managers.Manager.AllowCheck.None))
                {
                    IncStat("Money Denied Parent");
                    continue;
                }

                int payment = perParentPayment;
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
                    Money.AdjustFunds(Sim, "ElderSupport", -payment);

                    Money.AdjustFunds(parent, "ElderSupport", payment);

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
            return new ElderSupportScenario(this);
        }

        public class PaymentOption : IntegerScenarioOptionItem<ManagerMoney, ElderSupportScenario>, ManagerMoney.IFeesOption
        {
            public PaymentOption()
                : base(100)
            { }

            public override string GetTitlePrefix()
            {
                return "ElderSupportPayment";
            }
        }
    }
}
