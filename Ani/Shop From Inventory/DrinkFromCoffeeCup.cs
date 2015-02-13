using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Objects.Counters;
using Sims3.SimIFace;
using Sims3.Gameplay.Autonomy;
using Sims3.UI;

namespace SellFromInventory
{
    class DrinkFromCoffeeCup : Interaction<Sim, Glass>, Glass.IDrinkingInteraction
    {
        public class Definition : InteractionDefinition<Sim, Glass, DrinkFromCoffeeCup>
        {
            public override string GetInteractionName(Sim actor, Glass target, InteractionObjectPair iop)
            {
                if (target == null || target.ContainedDrink == null)
                {
                    return string.Empty;
                }
                return Glass.LocalizeString("DrinkAndReact", new object[]
			{
				target.GetDrinkName()
			});
            }
            public override bool Test(Sim a, Glass target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return a.GetObjectInRightHand() == target;
            }
        }
        public static InteractionDefinition Singleton = new DrinkFromCoffeeCup.Definition();
        public override bool Run()
        {
            try
            {

                base.StandardEntry(false);
                base.BeginCommodityUpdates();
                base.AcquireStateMachine("BarProfessional", AnimationPriority.kAPCarryRightPlus);
                base.SetActorAndEnter("x", this.Actor, "Enter");
                Bartending.SetGlassActor(this.mCurrentStateMachine, this.Target);
                Posture posture = this.Actor.Posture;
                if (posture is SimCarryingObjectPosture)
                {
                    posture = posture.PreviousPosture;
                }
                BarProfessional.SetSeatedParameter(this.Actor, this.mCurrentStateMachine);
                //string reactionNameForDrink = Bartending.GetReactionNameForDrink(this.Actor, this.Target.ContainedDrink);
                //base.AnimateSim(reactionNameForDrink);
                this.Target.TakeSip(this.Actor);
                //if (Bartending.IsReactionNaseous(reactionNameForDrink))
                //{
                //    this.Actor.BuffManager.AddElement(BuffNames.Nauseous, Origin.FromJuice);
                //    base.SetParameter("isCarryingGlass", false);
                //    CarrySystem.ExitAndKeepHolding(this.Actor);
                //    base.DestroyObject(this.Target);
                //    Glass.PopPosture(this.Actor);
                //}
                //else
                //{
                StyledNotification.Show(new StyledNotification.Format("drinking: " + base.Target.mTotalSips + " glass empty " + this.Target.IsEmpty(), StyledNotification.NotificationStyle.kGameMessagePositive));
                base.SetParameter("isCarryingGlass", true);
                if (this.Target.IsEmpty())//|| Bartending.IsReactionNegative(reactionNameForDrink))
                {
                    StyledNotification.Show(new StyledNotification.Format("glass empty, exit ", StyledNotification.NotificationStyle.kGameMessagePositive));

                    this.Target.OnDrinkFinished(this.Actor);
                    Glass.PutAwayAndPop(this.Actor);
                }
                //}
                base.AnimateSim("Exit");
                base.EndCommodityUpdates(true);
                base.StandardExit(false, false);
            }
            catch (System.Exception ex)
            {
                StyledNotification.Show(new StyledNotification.Format(ex.Message, StyledNotification.NotificationStyle.kGameMessageNegative));
            }
            return true;
        }
    }
}
