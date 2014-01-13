using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;

namespace NRaas.MoverSpace.Interactions
{
    public class OfferToMakeRealEx : OccultImaginaryFriend.OfferToTurnReal, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            if (!GameUtils.IsInstalled(ProductVersion.EP4)) return;

            interactions.Replace<Sim, OccultImaginaryFriend.OfferToTurnReal.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, OccultImaginaryFriend.OfferToTurnReal.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public new class Definition : OccultImaginaryFriend.OfferToTurnReal.Definition
        {
            public Definition()
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new OfferToMakeRealEx();
                result.Init(ref parameters);
                return result;
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                OccultImaginaryFriend friend;
                if (!OccultImaginaryFriend.TryGetOccultFromSim(target, out friend) || friend.IsReal)
                {
                    return false;
                }
                
                if (friend.OwnerSimDescriptionId != a.SimDescription.SimDescriptionId)
                {
                    return false;
                }
                
                Relationship relationship = Relationship.Get(a, target, false);
                bool flag = (relationship != null) && (relationship.CurrentLTRLiking >= OccultImaginaryFriend.kRelationshipThresholdBeforeCanTurnFriendReal);
                
                if (a.Inventory.Find<IImaginaryFriendPotion>(true) == null)
                {
                    if (flag)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(OccultImaginaryFriend.LocalizeString(a.IsFemale, "NeedImaginaryFriendPotion", new object[] { a }));
                    }
                    return false;
                }
                
                if (!flag)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(OccultImaginaryFriend.LocalizeString(new bool[] { a.IsFemale, target.IsFemale }, "NeedRelToUseImaginaryFriendPotion", new object[] { a, target }));
                    return false;
                }

                return true;
            }
        }
    }
}
