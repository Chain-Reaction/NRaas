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
    public class PayLoan : RabbitHole.RabbitHoleInteraction<Sim, RabbitHole>, Common.IAddInteraction
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

                int debt = StoryProgression.Main.GetValue<DebtOption, int>(Actor.Household);

                int promptValue = debt;
                if (promptValue > Actor.FamilyFunds)
                {
                    promptValue = Actor.FamilyFunds;
                }

                string text = StringInputDialog.Show(Common.Localize("PayLoan:MenuName", Actor.IsFemale), Common.Localize("PayLoan:Prompt", Actor.IsFemale, new object[] { Actor, debt, Actor.FamilyFunds }), promptValue.ToString());
                if (string.IsNullOrEmpty(text)) return false;

                int funds;
                if (!int.TryParse(text, out funds)) return false;

                if (debt <= 0) return false;

                if (funds > debt)
                {
                    funds = debt;
                }

                StoryProgression.Main.AddValue<DebtOption, int>(Actor.Household, -funds);

                StoryProgression.Main.Money.AdjustFunds(Actor.SimDescription, "PayLoan", -funds);

                if (funds == debt)
                {
                    Common.Notify(Actor, Common.Localize("PayLoan:Complete", Actor.IsFemale, new object[] { Actor }));
                }
                else
                {
                    Common.Notify(Actor, Common.Localize("PayLoan:Success", Actor.IsFemale, new object[] { Actor, funds, debt }));
                }
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
        public sealed class Definition : InteractionDefinition<Sim, RabbitHole, PayLoan>
        {
            // Methods
            public Definition()
            { }

            public override string GetInteractionName(Sim a, RabbitHole target, InteractionObjectPair interaction)
            {
                return Common.Localize("PayLoan:MenuName", a.IsFemale);
            }

            public override bool Test(Sim a, RabbitHole target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!StoryProgression.Main.GetValue<UnifiedBillingScenario.UnifiedBillingOption, bool>()) return false;

                    if (a.FamilyFunds <= 0) return false;

                    if (StoryProgression.Main.GetValue<DebtOption, int>(a.Household) <= 0) return false;

                    return true;
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Common.Exception(a, target, e);
                    return false;
                }
            }
        }
    }
}

