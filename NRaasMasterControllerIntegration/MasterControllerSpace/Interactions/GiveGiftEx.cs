using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Replacers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Interactions
{
    public class GiveGiftEx : Sim.GiveGift, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, Sim.GiveGift.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();

            if (GameUtils.IsInstalled(ProductVersion.EP4))
            {
                BooterLogger.AddError(SocialRHSReplacer.Perform<GiveGiftEx>("Give Gift", "OnGiveGiftAccept"));
                BooterLogger.AddError(SocialRHSReplacer.Perform<GiveGiftEx>("Give Gift", "OnGiveGiftReject"));
            }
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sim, Sim.GiveGift.Definition>(Singleton);
        }

        private new static void TransferGiftFromActorToTarget(Sim actor, Sim target, GameObject gift)
        {
            if (Inventories.TryToMove(gift, target.Inventory))
            {
                IGiveGiftOnGiven given = gift as IGiveGiftOnGiven;
                if (given != null)
                {
                    given.OnGiven(actor, target);
                }

                Relationship relationship = Relationship.Get(actor, target, false);
                if (relationship != null)
                {
                    relationship.UpdateBetrayedBuffIfNecessary(WaysToFixBetrayedBuff.GiveGiftsOrFlowers);
                }

                EventTracker.SendEvent(new SocialEvent(EventTypeId.kSocialInteraction, actor, target, "Give Gift", false, true, false, CommodityTypes.Undefined));
                UpdateSpoiledStatus(target);
            }
        }

        public static void OnGiveGiftAccept(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                Sim.GiveGift inst = i as Sim.GiveGift;
                GameObject gift = inst.Gift;
                DoAcceptReject(actor, target, inst);
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, interaction, e);
            }
        }

        public static void OnGiveGiftReject(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                Sim.GiveGift inst = i as Sim.GiveGift;
                DoAcceptReject(actor, target, inst);
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, interaction, e);
            }
        }

        public new static void DoAcceptReject(Sim actor, Sim target, Sim.GiveGift inst)
        {
            GameObject gift = inst.Gift;
            string message = "";
            string stingName = null;
            if (inst.RejectFromSpoiled)
            {
                if (DoesSimAlreadyOwnObject(target, gift))
                {
                    message = GiveGiftLocalizeString(target.IsFemale, "SpoiledAlreadyHave", new object[0x0]);
                }
                else
                {
                    message = GiveGiftLocalizeString(target.IsFemale, "SpoiledTooCheap", new object[0x0]);
                }
                InteractionInstance instance = Sim.GiveGiftTantrum.Singleton.CreateInstance(target, target, target.InheritedPriority(), false, true);
                target.InteractionQueue.PushAsContinuation(instance, true);
            }
            else
            {
                Relationship relationship = actor.GetRelationship(target, false);
                float liking = relationship.LTR.Liking;
                float num2 = 0f;
                List<TraitNames> randomList = new List<TraitNames>();
                TraitNames unknown = TraitNames.Unknown;
                GiveGiftTraitData data = null;
                InformationLearnedAboutSim sim = relationship.InformationAbout(target);
                foreach (Trait trait in target.TraitManager.List)
                {
                    if (sTraitToTraitDataMap.TryGetValue(trait.Guid, out data) && data.DoesTraitCareAboutObject(gift))
                    {
                        randomList.Add(trait.Guid);
                        num2 += data.Score;
                        if ((unknown == TraitNames.Unknown) && !sim.KnowsTrait(trait.Guid))
                        {
                            unknown = trait.Guid;
                        }
                    }
                }

                if (randomList.Count > 0x0)
                {
                    if (unknown == TraitNames.Unknown)
                    {
                        unknown = RandomUtil.GetRandomObjectFromList<TraitNames>(randomList);
                    }
                    else
                    {
                        sim.LearnTrait(unknown, actor.SimDescription, target.SimDescription);
                    }
                    message = sTraitToTraitDataMap[unknown].GetTNS(target);
                }

                float num3 = liking + num2;
                if (num3 >= Sim.GiveGiftTuning.kLTRToAcceptGift)
                {
                    stingName = "sting_give_gift_accept";
                    if (num3 >= Sim.GiveGiftTuning.kLTRToSuperAcceptGift)
                    {
                        message = GiveGiftLocalizeString(target.IsFemale, "UberAcceptGiftTNS", new object[0x0]);
                        stingName = "sting_give_gift_accept_super";
                    }
                    else if (liking >= Sim.GiveGiftTuning.kLTRToAcceptGift)
                    {
                        message = GiveGiftLocalizeString(target.IsFemale, "AcceptGiftTNS", new object[0x0]);
                    }
                    TransferGiftFromActorToTarget(actor, target, gift);
                }
                else
                {
                    if (liking < Sim.GiveGiftTuning.kLTRToAcceptGift)
                    {
                        message = GiveGiftLocalizeString(target.IsFemale, "RejectGiftTNS", new object[0x0]);
                    }
                    EventTracker.SendEvent(new SocialEvent(EventTypeId.kSocialInteraction, actor, target, "PillowFight", false, false, false, CommodityTypes.Undefined));
                }
            }
            if (stingName != null)
            {
                actor.ShowTNSAndPlayStingIfSelectable(message, StyledNotification.NotificationStyle.kSimTalking, target.ObjectId, gift.ObjectId, stingName);
            }
            else
            {
                actor.ShowTNSIfSelectable(message, StyledNotification.NotificationStyle.kSimTalking, target.ObjectId, gift.ObjectId);
            }
        }

        public new class Definition : Sim.GiveGift.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new GiveGiftEx();
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
