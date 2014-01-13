using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.ShelvesStorage;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class WardrobeWoohooAEx : Wardrobe.WooHooInWardrobeA, Common.IPreLoad
    {
        static InteractionDefinition sOldSingleton;

        public WardrobeWoohooAEx()
        { }

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, Wardrobe.WooHooInWardrobeA.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public override bool Run()
        {
            try
            {
                if ((mWardrobe == null) || (mOtherWooHoo == null))
                {
                    return false;
                }
                if (!mWardrobe.SimLine.WaitForTurn(this, SimQueue.WaitBehavior.Default, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), 10f))
                {
                    return false;
                }

                mWardrobe.SimLine.RemoveFromQueue(Actor);
                OutfitCategories currentOutfitCategory = Actor.CurrentOutfitCategory;
                int currentOutfitIndex = Actor.CurrentOutfitIndex;
                Actor.RouteToObjectRadius(mWardrobe, 1.5f);

                while (!mOtherWooHoo.mIsInWardrobe)
                {
                    if (((Target.InteractionQueue.GetCurrentInteraction() != mOtherWooHoo) || Target.HasExitReason(ExitReason.Canceled)) || Actor.HasExitReason())
                    {
                        return false;
                    }
                    SpeedTrap.Sleep(0xa);
                }

                Route r = Actor.CreateRoute();
                r.AddObjectToIgnoreForRoute(Target.ObjectId);
                r.PlanToSlot(mWardrobe, Slot.RoutingSlot_0);
                if (!Actor.DoRoute(r))
                {
                    return false;
                }

                mWardrobe.UpdateFootprint(true, Actor, new Sim[] { Target });
                mWardrobe.AddToUseList(Actor);
                StandardEntry();
                BeginCommodityUpdates();
                mCurrentStateMachine = mOtherWooHoo.mCurrentStateMachine;
                SetActorAndEnter("x", Actor, "Enter");
                Animate("x", "GetInX");
                mIsInWardrobe = true;

                if (((Actor.OccultManager != null) && !Actor.OccultManager.DisallowClothesChange()) && !Actor.DoesSimHaveTransformationBuff())
                {
                    List<OutfitCategories> randomList = new List<OutfitCategories>(Wardrobe.kPossibleOutfitsAfterWooHoo);
                    randomList.Remove(Actor.CurrentOutfitCategory);

                    if (Woohooer.Settings.mNakedOutfitWardrobe)
                    {
                        randomList.Add(OutfitCategories.Naked);
                    }

                    Actor.SwitchToOutfitWithSpin(Sim.ClothesChangeReason.Force, RandomUtil.GetRandomObjectFromList(randomList));
                }

                while (mOtherWooHoo.mIsInWardrobe)
                {
                    if (Target.InteractionQueue.GetCurrentInteraction() != mOtherWooHoo)
                    {
                        Actor.SetOpacity(1f, 0f);
                        mWardrobe.RemoveFromUseList(Actor);
                        EndCommodityUpdates(true);
                        StandardExit();
                        mWardrobe.UpdateFootprint(false, Actor, new Sim[0x0]);
                        return false;
                    }
                    SpeedTrap.Sleep(0xa);
                }

                IWooHooDefinition otherDefinition = mOtherWooHoo.InteractionDefinition as IWooHooDefinition;

                EventTracker.SendEvent(EventTypeId.kWoohooInWardrobe, Actor, Target);
                CommonWoohoo.RunPostWoohoo(Actor, Target, mWardrobe, otherDefinition.GetStyle(this), otherDefinition.GetLocation(mWardrobe), true);

                mWardrobe.UpdateFootprint(false, Actor, new Sim[0x0]);
                mWardrobe.RemoveFromUseList(Actor);
                EndCommodityUpdates(true);
                StandardExit();
                return true;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception exception)
            {
                Common.Exception(Actor, Target, exception);
                return false;
            }
        }

        public new class Definition : Wardrobe.WooHooInWardrobeA.Definition
        {
            public Definition()
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new WardrobeWoohooAEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
