using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class InvestInRabbithole : RabbitHole.InvestInRabbithole, Common.IPreLoad
    {
        // Fields
        public static readonly new InteractionDefinition BuyoutSingleton = new Definition(true);
        public static readonly new InteractionDefinition InvestSingleton = new Definition(false);

        public void OnPreLoad()
        {
            Tunings.Inject<RabbitHole, RabbitHole.InvestInRabbithole.Definition, Definition>(false);
        }

        public static void ReimburseDeeds(RealEstateManager newOwner, RabbitHole rabbitHole)
        {
            foreach (Household house in Household.GetHouseholdsLivingInWorld())
            {
                if (house.RealEstateManager == null) continue;

                if (house.RealEstateManager == newOwner) continue;

                PropertyData data = house.RealEstateManager.FindProperty(rabbitHole);
                if (data == null) continue;

                int totaValue = data.TotalValue;

                using (ManagerMoney.SetAccountingKey setKey = new ManagerMoney.SetAccountingKey(house, "PropertySold"))
                {
                    house.RealEstateManager.SellProperty(data, true);
                }

                if (house != Household.ActiveHousehold)
                {
                    Common.Notify(Common.Localize("Deeds:Reimbursement", false, new object[] { house.Name, totaValue }));
                }
            }
        }

        public static IPropertyData PurchaceorUpgradeRabbitHole(RealEstateManager ths, RabbitHole rabbitHole)
        {
            IPropertyData data = null;
            PropertyData data2 = ths.FindProperty(rabbitHole);
            if (data2 == null)
            {
                if (ths.mOwningHousehold.FamilyFunds >= rabbitHole.RabbitHoleTuning.kInvestCost)
                {
                    NRaas.StoryProgression.Main.Money.AdjustFunds(ths.mOwningHousehold, "PropertyBought", -rabbitHole.RabbitHoleTuning.kInvestCost);
                    data2 = new PropertyData.RabbitHole(rabbitHole, ths);
                    ths.AddToProperties(data2);
                }
            }
            else if (ths.mOwningHousehold.FamilyFunds >= rabbitHole.RabbitHoleTuning.kBuyoutCost)
            {
                ReimburseDeeds(ths, rabbitHole);

                NRaas.StoryProgression.Main.Money.AdjustFunds(ths.mOwningHousehold, "PropertyBought", -rabbitHole.RabbitHoleTuning.kBuyoutCost);

                data2.UpgradeToFullOwnership();
                ths.UpdateProperty(data2);
            }
            data = data2;
            if (data != null)
            {
                Tutorialette.TriggerLesson(Lessons.RealEstate, null);
            }
            return data;
        }

        public override bool InRabbitHole()
        {
            try
            {
                Definition interactionDefinition = InteractionDefinition as Definition;
                if (WasPropertyScrewedWithDuringInteraction(Target, interactionDefinition.IsBuyout))
                {
                    return false;
                }
                bool succeeded = false;
                StartStages();
                BeginCommodityUpdates();

                try
                {
                    succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                }
                finally
                {
                    EndCommodityUpdates(succeeded);
                }
                if (succeeded)
                {
                    int num = interactionDefinition.IsBuyout ? Target.RabbitHoleTuning.kBuyoutCost : Target.RabbitHoleTuning.kInvestCost;
                    if (Actor.FamilyFunds < num)
                    {
                        Actor.ShowTNSIfSelectable(LocalizeString("NotEnoughMoneyTNS", new object[0x0]), StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid);
                        return false;
                    }

                    if (WasPropertyScrewedWithDuringInteraction(Target, interactionDefinition.IsBuyout))
                    {
                        return false;
                    }

                    succeeded = PurchaceorUpgradeRabbitHole(Actor.Household.RealEstateManager, Target) != null;
                    if (!succeeded)
                    {
                        return succeeded;
                    }

                    if (interactionDefinition.IsBuyout)
                    {
                        Actor.ShowTNSIfSelectable(Common.LocalizeEAString(Actor.IsFemale, "Gameplay/Abstracts/RabbitHole/InvestInRabbithole:OwnerTNS", new object[] { Actor, Target.CatalogName }), StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, Actor.ObjectId);
                    }
                    else
                    {
                        Actor.ShowTNSIfSelectable(Common.LocalizeEAString(Actor.IsFemale, "Gameplay/Abstracts/RabbitHole/InvestInRabbithole:PartnerTNS", new object[] { Actor, Target.CatalogName }), StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, Actor.ObjectId);
                    }
                    if (Actor.IsSelectable)
                    {
                        Audio.StartSound("sting_career_positive");
                    }
                }
                return succeeded;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        // Nested Types
        public new class Definition : InteractionDefinition<Sim, RabbitHole, InvestInRabbithole>
        {
            // Fields
            public bool IsBuyout;

            // Methods
            private Definition()
            {
            }

            public Definition(bool isBuyout)
            {
                this.IsBuyout = isBuyout;
            }

            public override string GetInteractionName(Sim a, RabbitHole target, InteractionObjectPair interaction)
            {
                if (this.IsBuyout)
                {
                    return InvestInRabbithole.LocalizeString("BuyOutInteractionName", new object[] { target.RabbitHoleTuning.kBuyoutCost });
                }
                return InvestInRabbithole.LocalizeString("InvestInteractionName", new object[] { target.RabbitHoleTuning.kInvestCost });
            }

            public override bool Test(Sim a, RabbitHole target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if ((!target.RabbitHoleTuning.kCanInvestHere || (a.SimDescription.DeathStyle != SimDescription.DeathType.None)) || !a.Household.RealEstateManager.HasBeenFixedupInHomeworld)
                {
                    return false;
                }
                PropertyData data = a.Household.RealEstateManager.FindProperty(target);
                if (this.IsBuyout)
                {
                    return (((data != null) && !data.IsFullOwner) && (a.FamilyFunds >= target.RabbitHoleTuning.kBuyoutCost));
                }
                return ((data == null) && (a.FamilyFunds >= target.RabbitHoleTuning.kInvestCost));
            }
        }
    }
}

