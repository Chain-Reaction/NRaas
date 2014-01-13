using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.DreamerSpace.Helpers
{
    public class ActiveDreamNodeEx
    {
        public static bool OnCompletion(ActiveDreamNode ths)
        {
            DreamsAndPromisesManager manager = null;
            if (ths.Owner != null)
            {
                manager = ths.Owner.DreamsAndPromisesManager;
            }

            if (manager == null) return false;

            // Changed
            if ((ths.IsPromised) ||
                ((manager.mDisplayedDreamNodes != null) && (manager.mDisplayedDreamNodes.Contains(ths))))
            {
                bool wasMajorDream = false;
                try
                {
                    if (ths.IsMajorWish && !ths.mOwner.SimDescription.HasCompletedLifetimeWish)
                    {
                        wasMajorDream = true;
                        ths.mOwner.SimDescription.HasCompletedLifetimeWish = true;
                        ths.mOwner.BuffManager.AddElement(BuffNames.Fulfilled, DreamsAndPromisesManager.kVeryFulfilledMoodValue, DreamsAndPromisesManager.kVeryFulfilledDuration, Origin.FromLifetimeWish);
                        (ths.mOwner.BuffManager.GetElement(BuffNames.Fulfilled) as BuffFulfilled.BuffInstanceFulfilled).SetVeryFulfilledText();
                    }
                    else
                    {
                        BuffFulfilled.BuffInstanceFulfilled element = ths.mOwner.BuffManager.GetElement(BuffNames.Fulfilled) as BuffFulfilled.BuffInstanceFulfilled;
                        if (element != null)
                        {
                            float num = element.mBuff.EffectValue + (BuffFulfilled.kFulfilledBuffStackTimes * BuffFulfilled.kFulfilledBuffStackMoodValue);
                            if (element.mEffectValue < num)
                            {
                                ths.mOwner.BuffManager.AddElement(BuffNames.Fulfilled, (int)(element.mEffectValue + BuffFulfilled.kFulfilledBuffStackMoodValue), Origin.FromCompletingPromisedWish);
                            }
                        }
                        else
                        {
                            ths.mOwner.BuffManager.AddElement(BuffNames.Fulfilled, Origin.FromCompletingPromisedWish);
                        }
                    }
                }
                catch(Exception e)
                {
                    Common.DebugException(ths.Owner, e);
                }

                // Changed
                manager.RemoveDisplayedPromiseNode(ths);
                if (DreamsAndPromisesManager.IsMLCrisisDream(ths))
                {
                    MidlifeCrisisManager.MidlifeCrisisPromiseCompleted(ths.Owner.SimDescription);
                    MidlifeCrisisManager.UpdateCrisisType(ths.Owner.SimDescription);
                }

                if (ths.IsPromised)
                {
                    if ((manager == DreamsAndPromisesManager.ActiveDreamsAndPromisesManager) && (DreamsAndPromisesModel.Singleton != null))
                    {
                        DreamsAndPromisesModel.Singleton.FireFulfillPromise(ths);
                    }
                }

                float achievementPoints = ths.AchievementPoints;
                if (ths.mOwner.BuffManager.HasElement(BuffNames.WishMaster))
                {
                    achievementPoints *= DreamsAndPromisesManager.kWishMasterMultiple;
                }

                SimDescriptionEx.IncrementLifetimeHappiness(ths.Owner.SimDescription, achievementPoints);

                if (!Responder.Instance.EditTownModel.IsLoading)
                {
                    manager.PopRewardEffect(achievementPoints, wasMajorDream);
                }
                if (wasMajorDream)
                {
                    if (ths.Owner.IsEP11Bot)
                    {
                        if ((ths.Owner.SimDescription.TraitChipManager != null) && ths.Owner.SimDescription.TraitChipManager.HasElement(11L))
                        {
                            ths.Owner.ShowTNSIfSelectable(Localization.LocalizeString(ths.Owner.IsFemale, "Gameplay/DreamsAndPromises/DreamsAndPromisesManager:LifetimeWishSuccessTNS", new object[] { ths.Owner }), StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, ths.Owner.ObjectId);
                        }
                    }
                    else
                    {
                        ths.Owner.ShowTNSIfSelectable(Localization.LocalizeString(ths.Owner.IsFemale, "Gameplay/DreamsAndPromises/DreamsAndPromisesManager:LifetimeWishSuccessTNS", new object[] { ths.Owner }), StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, ths.Owner.ObjectId);
                    }
                }

                EventTracker.SendEvent(new DreamEvent(EventTypeId.kPromisedDreamCompleted, ths, true));
            }

            return true;
        }
    }
}
