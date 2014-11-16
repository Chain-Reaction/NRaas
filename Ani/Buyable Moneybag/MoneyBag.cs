using Sims3.Gameplay.Objects.Gardening;
using Sims3.SimIFace;
using Sims3.Gameplay.ObjectComponents;
using System;
using Sims3.Gameplay.Actors;
using System.Collections.Generic;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Utilities;
using Sims3.UI;
using System.Text;
using Sims3.Gameplay.Abstracts;

namespace Sims3.Gameplay.Objects.Gardening.PersonalBanking
{
    public class MoneyBag : HarvestMoneyBag
    {

        public override int Cost 
        {
            get
            {
                return GetMoneyValue(this.CatalogName);
            }
        }
                
        public override void OnStartup()
        {
            base.AddComponent<ItemComponent>(new object[] { ItemComponent.SimInventoryItem });
            base.AddInteraction(CashIn.Singleton);
            base.AddInventoryInteraction(CashIn.Singleton);
        }

        public int GetMoneyValue(String name)
        {
            int price;
            int sLength = name.Length;
            int sIndex = name.IndexOf('§') + 1;
            string sPrice = name.Substring(sIndex, sLength - sIndex);

            int.TryParse(sPrice, out price);

            return price;
        }

        #region CashIn
        private new sealed class CashIn : Interaction<Sim, MoneyBag>
        {

            // Fields
            private const float kMaxRouteRange = 0.75f;
            private const float kMinRouteRange = 0.25f;
            private bool mCashedIn;
            public static readonly InteractionDefinition Singleton = new Definition();
            private const string sLocalizationKey = "Gameplay/Objects/Gardening/HarvestMoneyBag/CashIn";

            // Methods
            private void AdjustFunds(int moneyDelta)
            {
                base.Actor.Household.ModifyFamilyFunds(moneyDelta);
                if (moneyDelta > 0)
                {
                    //Gardening element = base.Actor.SkillManager.GetElement(SkillNames.Gardening) as Gardening;
                    //if (element != null)
                    //{
                    //    element.MoneyEarned(moneyDelta);
                    //}
                }
            }

            private void CashInReaction()
            {
                ReactionTypes reactionType = RandomUtil.CoinFlip() ? ReactionTypes.Cheer : ReactionTypes.Excited;
                base.Actor.PlayReaction(reactionType, ReactionSpeed.Immediate);
            }

            public override void Cleanup()
            {
                if (this.mCashedIn && (base.Target != null))
                {
                    base.Target.Destroy();
                    base.Target = null;
                }
                base.Cleanup();
            }

            private void EventHandlerSwipe(StateMachineClient sender, IEvent evt)
            {
                this.mCashedIn = true;
                base.Target.FadeOut();
                this.AdjustFunds(base.Target.GetMoneyValue(base.Target.CatalogName));
            }

            private static string LocalizeString(string name, params object[] parameters)
            {
                return Localization.LocalizeString("Gameplay/Objects/Gardening/HarvestMoneyBag/CashIn:" + name, parameters);
            }

            public override bool Run()
            {
                if (!base.Actor.RouteToPointRadialRange(base.Target.Position, 0.25f, 0.75f))
                {
                    return false;
                }
                base.StandardEntry();
                base.EnterStateMachine("single_animation", "Enter", "x");
                base.mCurrentStateMachine.SetParameter("AnimationClip", "a2o_object_genericSwipe_x");
                base.mCurrentStateMachine.AddOneShotScriptEventHandler(0x65, new SacsEventHandler(this.EventHandlerSwipe));
                base.AnimateSim("Animate");
                base.AnimateSim("Enter");
                this.CashInReaction();
                base.StandardExit();
                return true;
            }

            public override bool RunFromInventory()
            {
                base.StandardEntry();
                int moneyDelta = 0;
                List<IGameObject> stackObjects = new List<IGameObject>();
                if ((base.InteractionDefinition as Definition).CashInMany)
                {
                    uint stackNumber = base.Actor.Inventory.GetStackNumber(base.Target);
                    if (stackNumber != 0)
                    {
                        stackObjects = base.Actor.Inventory.GetStackObjects(stackNumber, false);
                        foreach (GameObject obj2 in stackObjects)
                        {
                            moneyDelta += base.Target.GetMoneyValue(obj2.CatalogName);
                            base.Actor.Inventory.TryToRemove(obj2);
                        }
                    }
                }
                else
                {
                    moneyDelta += base.Target.GetMoneyValue(base.Target.CatalogName);
                    stackObjects.Add(base.Target);
                    base.Actor.Inventory.TryToRemove(base.Target);
                }
                this.AdjustFunds(moneyDelta);
                this.CashInReaction();
                base.StandardExit();
                foreach (IGameObject obj3 in stackObjects)
                {
                    if (obj3 != base.Target)
                    {
                        obj3.Destroy();
                    }
                }
                base.Target.Destroy();
                return true;
            }

            // Nested Types
            private sealed class Definition : InteractionDefinition<Sim, MoneyBag, CashIn>
            {
                // Fields
                public bool CashInMany;

                // Methods
                public Definition()
                {
                }

                private Definition(bool cashInMany)
                {
                    this.CashInMany = cashInMany;
                }

                public override void AddInteractions(InteractionObjectPair iop, Sim actor, MoneyBag target, List<InteractionObjectPair> results)
                {
                    uint stackNumber = actor.Inventory.GetStackNumber(target);
                    if ((stackNumber != 0) && (actor.Inventory.GetStackObjects(stackNumber, false).Count > 1))
                    {
                        CashIn.Definition interaction = new CashIn.Definition(true);
                        InteractionObjectPair item = new InteractionObjectPair(interaction, target);
                        results.Add(item);
                    }
                    base.AddInteractions(iop, actor, target, results);
                }

                public override string GetInteractionName(Sim a, MoneyBag target, InteractionObjectPair interaction)
                {
                    if (this.CashInMany)
                    {
                        uint stackNumber = a.Inventory.GetStackNumber(target);
                        if (stackNumber != 0)
                        {
                            List<IGameObject> stackObjects = a.Inventory.GetStackObjects(stackNumber, false);
                            if (stackObjects.Count > 1)
                            {
                                int num2 = 0;
                                foreach (GameObject obj2 in stackObjects)
                                {
                                    num2 += target.GetMoneyValue(obj2.CatalogName);
                                }
                                return CashIn.LocalizeString("CashInMany", new object[] { num2 });
                            }
                        }
                    }
                    return CashIn.LocalizeString("CashIn", new object[] { target.GetMoneyValue(target.CatalogName) });
                }

                public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (this.CashInMany != parameters.WasInventoryStackOfObjectsPicked)
                    {
                        return InteractionTestResult.Def_TestFailed;
                    }
                    return base.Test(ref parameters, ref greyedOutTooltipCallback);
                }

                public override bool Test(Sim a, MoneyBag target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    ITreasureSpawnableObject obj2 = target;
                    if ((obj2 != null) && obj2.IsOnSpawner)
                    {
                        return false;
                    }
                    return true;
                }
            }
        }

        #endregion

    }
}
