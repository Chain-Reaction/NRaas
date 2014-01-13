using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Replacers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Moving;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MoverSpace.Helpers
{
    public class CommonSocials : Common.IPreLoad
    {
        static Common.MethodStore sStoryProgressionHandleMarriageName = new Common.MethodStore("NRaasStoryProgression", "NRaas.StoryProgression", "HandleMarriageName", new Type[] { typeof(SimDescription), typeof(SimDescription) });

        public void OnPreLoad()
        {
            BooterLogger.AddError(SocialRHSReplacer.Perform<CommonSocials>("Propose to Move in With", "OnMoveInWith"));
            BooterLogger.AddError(SocialRHSReplacer.Perform<CommonSocials>("Ask to Move In", "OnAskToMoveInAccepted"));
            BooterLogger.AddError(SocialRHSReplacer.Perform<CommonSocials>("Have Private Wedding", "OnMarried"));
            BooterLogger.AddError(SocialRHSReplacer.Perform<CommonSocials>("Exchange Rings", "OnMarried"));

            foreach (ActionData data in ActionData.sData.Values)
            {
                if (!data.IsRomantic)
                {
                    switch (data.Key)
                    {
                        case "Propose to Move in With":
                        case "Ask to Move In":
                            data.mActorAgeAllowed |= CASAGSAvailabilityFlags.HumanTeen;
                            data.mTargetAgeAllowed |= CASAGSAvailabilityFlags.HumanTeen;
                            break;
                    }

                    continue;
                }
            }
        }

        public static void OnMarried(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                string failReason = null;
                if ((actor.Household == target.Household) || (!MovingSituation.MovingInProgress && ((Household.ActiveHousehold == null) || InWorldSubState.IsEditTownValid(Household.ActiveHousehold.LotHome, ref failReason))))
                {
                    Sim actorSim;
                    Sim targetSim;
                    BuffManager actorBuffManager = actor.BuffManager;
                    BuffManager targetBuffManager = target.BuffManager;

                    actorBuffManager.AddElement(BuffNames.JustMarried, Origin.FromSocialization);
                    targetBuffManager.AddElement(BuffNames.JustMarried, Origin.FromSocialization);

                    BuffJustMarried.BuffInstanceJustMarried element = actorBuffManager.GetElement(BuffNames.JustMarried) as BuffJustMarried.BuffInstanceJustMarried;
                    if (element != null)
                    {
                        element.CreateGiftAlarm(actorBuffManager);
                    }

                    actorBuffManager.RemoveElement(BuffNames.NewlyEngaged);
                    actorBuffManager.RemoveElement(BuffNames.ParentsBlessing);
                    targetBuffManager.RemoveElement(BuffNames.NewlyEngaged);
                    targetBuffManager.RemoveElement(BuffNames.ParentsBlessing);

                    ActiveTopic.AddToSim(actor, "Wedding");
                    ActiveTopic.AddToSim(target, "Wedding");
                    Relationship relationship = Relationship.Get(actor, target, true);
                    relationship.LTR.RemoveInteractionBit(LongTermRelationship.InteractionBits.Divorce);
                    relationship.LTR.AddInteractionBit(LongTermRelationship.InteractionBits.Marry);
                    if (actor.IsNPC && !target.IsNPC)
                    {
                        actorSim = target;
                        targetSim = actor;
                    }
                    else if (!actor.IsNPC && target.IsNPC)
                    {
                        actorSim = actor;
                        targetSim = target;
                    }
                    else
                    {
                        SimDescription proposerDesc = relationship.ProposerDesc;
                        actorSim = (proposerDesc == null) ? null : proposerDesc.CreatedSim;
                        if (actorSim == null)
                        {
                            if (RandomUtil.CoinFlip())
                            {
                                actorSim = actor;
                            }
                            else
                            {
                                actorSim = target;
                            }
                        }

                        targetSim = target;
                        if (actorSim == target)
                        {
                            targetSim = actor;
                        }
                    }

                    SocialCallback.GiveDaysOffIfRequired(actorSim, targetSim);
                    MidlifeCrisisManager.OnBecameMarried(actor.SimDescription, target.SimDescription);

                    relationship.SetMarriedInGame();

                    if (SeasonsManager.Enabled)
                    {
                        relationship.WeddingAnniversary = new WeddingAnniversary(SeasonsManager.CurrentSeason, (int)SeasonsManager.DaysElapsed);
                        relationship.WeddingAnniversary.SimA = relationship.SimDescriptionA;
                        relationship.WeddingAnniversary.SimB = relationship.SimDescriptionB;
                        relationship.WeddingAnniversary.CreateAlarm();
                    }

                    if (sStoryProgressionHandleMarriageName.Valid)
                    {
                        sStoryProgressionHandleMarriageName.Invoke<bool>(new object[] { actorSim.SimDescription, targetSim.SimDescription });
                    }
                    else
                    {
                        targetSim.SimDescription.LastName = actorSim.SimDescription.LastName;
                        foreach (Genealogy genealogy in targetSim.Genealogy.Children)
                        {
                            SimDescription simDescription = genealogy.SimDescription;
                            if (((simDescription != null) && simDescription.TeenOrBelow) && (simDescription.CreatedSim != null))
                            {
                                simDescription.LastName = actorSim.SimDescription.LastName;
                            }
                        }
                    }

                    actor.Genealogy.Marry(target.Genealogy);
                    OnMoveInWith(actor, target, interaction, topic, i);
                    SocialCallback.EndServiceForActor(actor);
                    SocialCallback.EndServiceForActor(target);

                    actorBuffManager.RemoveElement(BuffNames.MissedWedding);
                    targetBuffManager.RemoveElement(BuffNames.MissedWedding);
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }

        public static void OnMoveInWith(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                bool isFemale = actor.IsFemale && target.IsFemale;
                if (actor.Household == target.Household)
                {
                    if (actor.IsSelectable && Household.RoommateManager.IsNPCRoommate(target))
                    {
                        Household.RoommateManager.MakeRoommateSelectable(target.SimDescription);
                    }

                    if (target.IsSelectable && Household.RoommateManager.IsNPCRoommate(actor))
                    {
                        Household.RoommateManager.MakeRoommateSelectable(actor.SimDescription);
                    }

                    actor.ShowTNSIfSelectable(SocialCallback.LocalizeString(isFemale, "MarriedSameHousehold", new object[] { actor, target }), StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, actor.ObjectId);
                }
                else
                {
                    int value = RandomUtil.GetInt(MovingSituation.kMarriageGiftAndTaxBenefits[0x0], MovingSituation.kMarriageGiftAndTaxBenefits[0x1]);
                    
                    if (actor.IsSelectable || target.IsSelectable)
                    {
                        MovingDialogEx.Show(new GameplayMovingModelEx(actor, target));
                    }

                    actor.ModifyFunds(value);
                    actor.ShowTNSIfSelectable(SocialCallback.LocalizeString(isFemale, "MarriedDifferentHouseholds", new object[] { value, actor, target }), StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, actor.ObjectId);
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }

        public static void OnAskToMoveInAccepted(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                if (!MovingSituation.MovingInProgress)
                {
                    using (GameplayMovingModelEx.ProtectFunds protect = new GameplayMovingModelEx.ProtectFunds(target.Household))
                    {
                        MovingDialogEx.Show(new GameplayMovingModelEx(target, actor.Household));
                    }
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }
    }
}
