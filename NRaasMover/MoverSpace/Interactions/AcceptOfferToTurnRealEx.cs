using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using Sims3.UI;
using System;

namespace NRaas.MoverSpace.Interactions
{
    public class AcceptOfferToTurnRealEx : OccultImaginaryFriend.AcceptOfferToTurnReal, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            if (!GameUtils.IsInstalled(ProductVersion.EP4)) return;

            interactions.Replace<Sim, OccultImaginaryFriend.AcceptOfferToTurnReal.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, OccultImaginaryFriend.AcceptOfferToTurnReal.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public override bool Run()
        {
            try
            {
                if (mJig == null)
                {
                    return false;
                }

                Route r = mJig.RouteToJigB(Actor);
                RequestWalkStyle(Sim.WalkStyle.Run);
                bool flag = Actor.DoRoute(r);
                UnrequestWalkStyle(Sim.WalkStyle.Run);
                if (!flag)
                {
                    return false;
                }
                CurrentStatus = Status.Started;
                while (CurrentStatus != Status.CanTransform)
                {
                    if (CurrentStatus == Status.Cancelled)
                    {
                        return false;
                    }
                    SpeedTrap.Sleep();
                }

                /* Removed
                if (Households.NumHumansIncludingPregnancy(Household.ActiveHousehold) >= 0x8)
                {
                    return false;
                }
                */

                OccultImaginaryFriend occult = null;
                if (!OccultImaginaryFriend.TryGetOccultFromSim(Actor, out occult))
                {
                    return false;
                }

                Actor.InteractionQueue.CancelAllInteractions();
                occult.AddToActiveHousehold(Actor);
                occult.MakeReal();
                occult.RebuildAppearance();
                SwitchToEverydayOutfit();

                EventTracker.SendEvent(EventTypeId.kTurnImaginaryFriendReal, Target);
                string message = OccultImaginaryFriend.LocalizeString(new bool[] { Actor.IsFemale, Target.IsFemale }, "ImaginaryFriendTurnedRealTNS", new object[] { Actor, Target });
                Actor.ShowTNSAndPlayStingIfSelectable(message, StyledNotification.NotificationStyle.kGameMessagePositive, Actor.ObjectId, Target.ObjectId, "sting_imaginary_real");

                {
                    Target.BuffManager.AddElement(BuffNames.ImaginaryFriendTurnedReal, Origin.FromHavingImaginaryFriend);
                    BuffImaginaryFriendTurnedReal.BuffInstanceImaginaryFriendTurnedReal buff = Target.BuffManager.GetElement(BuffNames.ImaginaryFriendTurnedReal) as BuffImaginaryFriendTurnedReal.BuffInstanceImaginaryFriendTurnedReal;
                    if (buff != null)
                    {
                        buff.SetImaginaryFriend(Actor);
                    }
                }

                {
                    Actor.BuffManager.AddElement(BuffNames.ImaginaryFriendIAmReal, Origin.FromImaginaryFriendFirstTime);
                    BuffImaginaryFriendIAmReal.BuffInstanceImaginaryFriendIAmReal buff2 = Actor.BuffManager.GetElement(BuffNames.ImaginaryFriendIAmReal) as BuffImaginaryFriendIAmReal.BuffInstanceImaginaryFriendIAmReal;
                    if (buff2 != null)
                    {
                        buff2.SetOwner(Target);
                    }
                }

                CurrentStatus = Status.Done;
                return true;
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

        public new class Definition : OccultImaginaryFriend.AcceptOfferToTurnReal.Definition
        {
             public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new AcceptOfferToTurnRealEx();
                result.Init(ref parameters);
                return result;
            }
        }
    }
}
