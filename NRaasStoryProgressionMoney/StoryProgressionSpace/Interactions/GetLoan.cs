using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Money;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class GetLoan : RabbitHole.RabbitHoleInteraction<Sim, RabbitHole>, Common.IAddInteraction
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<BusinessAndJournalismRabbitHole>(Singleton);
        }

        public override void ConfigureInteraction()
        {
            base.ConfigureInteraction();
            TimedStage stage = new TimedStage(GetInteractionName(), RabbitHole.InvestInRabbithole.kTimeToSpendInside, false, false, true);
            Stages = new List<Stage>(new Stage[] { stage });
            ActiveStage = stage;
        }

        public override bool InRabbitHole()
        {
            try
            {
                StartStages();
                BeginCommodityUpdates();
                bool succeeded = false;

                try
                {
                    succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                }
                finally
                {
                    EndCommodityUpdates(succeeded);
                }
                
                if (!succeeded)
                {
                    return succeeded;
                }

                if (StoryProgression.Main.GetValue<NetWorthOption,int>(Actor.Household) <= 0)
                {
                    Common.Notify(Actor, Common.Localize("GetLoan:Fail", Actor.IsFemale, new object[] { Actor }));
                    return false;
                }

                float interest = StoryProgression.Main.GetValue<LoanInterestOption, int>();
                if (interest < 0)
                {
                    interest = 0;
                }

                string text = StringInputDialog.Show(Common.Localize("GetLoan:MenuName", Actor.IsFemale), Common.Localize("GetLoan:Prompt", Actor.IsFemale, new object[] { Actor, interest }), "10000");
                if (string.IsNullOrEmpty(text)) return false;

                int funds;
                if (!int.TryParse(text, out funds)) return false;

                int debt = (int)(funds * (1 + interest / 100f));

                if (StoryProgression.Main.GetValue<NetWorthOption,int>(Actor.Household) <= debt)
                {
                    interest = StoryProgression.Main.GetValue<LoanInterestHighRiskOption, int>();
                    if (interest < 0)
                    {
                        interest = 0;
                    }

                    if (!AcceptCancelDialog.Show(Common.Localize("GetLoan:HighRisk", Actor.IsFemale, new object[] { Actor, interest })))
                    {
                        return false;
                    }

                    debt = (int)(funds * (1 + interest / 100f));
                }

                StoryProgression.Main.AddValue<DebtOption, int>(Actor.Household, debt);

                StoryProgression.Main.Money.AdjustFunds(Actor.SimDescription, "Loan", funds);

                Common.Notify(Actor, Common.Localize("GetLoan:Success", Actor.IsFemale, new object[] { Actor, funds, debt }));
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }
            return true;
        }

        // Nested Types
        public sealed class Definition : InteractionDefinition<Sim, RabbitHole, GetLoan>
        {
            // Methods
            public Definition()
            { }

            public override string GetInteractionName(Sim a, RabbitHole target, InteractionObjectPair interaction)
            {
                return Common.Localize("GetLoan:MenuName", a.IsFemale);
            }

            public override bool Test(Sim a, RabbitHole target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!StoryProgression.Main.GetValue<UnifiedBillingScenario.UnifiedBillingOption, bool>()) return false;

                return true;
            }
        }

        public class LoanInterestOption : IntegerManagerOptionItem<ManagerMoney>, ManagerMoney.IDebtOption
        {
            public LoanInterestOption()
                : base(10)
            { }

            public override string GetTitlePrefix()
            {
                return "LoanInterest";
            }
        }

        public class LoanInterestHighRiskOption : IntegerManagerOptionItem<ManagerMoney>, ManagerMoney.IDebtOption
        {
            public LoanInterestHighRiskOption()
                : base(25)
            { }

            public override string GetTitlePrefix()
            {
                return "LoanInterestHighRisk";
            }
        }
    }
}

