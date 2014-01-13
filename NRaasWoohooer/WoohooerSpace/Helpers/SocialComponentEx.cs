using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Interactions;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Helpers
{
    public class SocialComponentEx
    {
        public static void ReactToJealousEventMedium(Sim s, ReactionBroadcaster rb)
        {
            JealousyLevel level = JealousyLevel.Medium;
            if (level > Woohooer.Settings.mRomanceJealousyLevel)
            {
                level = Woohooer.Settings.mRomanceJealousyLevel;
            }

            ReactToJealousEvent(s, rb, level, false);
        }

        public static void ReactToJealousEventHigh(Sim s, ReactionBroadcaster rb)
        {
            ReactToJealousEvent(s, rb, Woohooer.Settings.mWoohooJealousyLevel, true);
        }

        public static void ReactToJealousEvent(Sim s, ReactionBroadcaster rb, JealousyLevel level, bool woohoo)
        {
            try
            {
                if (!WoohooScoring.ReactsToJealousy(s)) return;

                Sim broadcastingObject = rb.BroadcastingObject as Sim;
                if (broadcastingObject == null) return;

                if (broadcastingObject.CurrentInteraction == null)
                {
                    return;
                }

                Sim target = null;

                IWooHooDefinition definition = broadcastingObject.CurrentInteraction.InteractionDefinition as IWooHooDefinition;
                if (definition != null)
                {
                    target = definition.ITarget(broadcastingObject.CurrentInteraction);
                }

                if (target == null)
                {
                    target = broadcastingObject.SynchronizationTarget;
                }

                if (target == null)
                {
                    Woohooer.DebugNotify("Bad Target");
                    return;
                }

                if (CommonSocials.CaresAboutJealousy(broadcastingObject, target, s, level, woohoo))
                {
                    if ((s != broadcastingObject) && (s != target))
                    {
                        if ((target != null) && (!SocialComponentEx.CheckCheating(s, broadcastingObject, target, level)) && (!broadcastingObject.CurrentInteraction.IsRejected))
                        {
                            SocialComponentEx.CheckCheating(s, target, broadcastingObject, level);
                        }
                    }
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(s, e);
            }
        }

        public static bool CheckCheating(Sim observer, Sim actor, Sim target, JealousyLevel jealousyLevel)
        {
            if (CommonSocials.IsPolyamorous(actor.SimDescription, target.SimDescription, observer.SimDescription)) return false;

            if (target.HasTrait(TraitNames.NoJealousy)) return false;

            if (!actor.HasTrait(TraitNames.NoJealousy) && (jealousyLevel != JealousyLevel.None))
            {
                foreach (Situation situation in actor.Autonomy.SituationComponent.Situations)
                {
                    if (situation.DoesSituationRuleOutJealousy(observer, actor, target, jealousyLevel))
                    {
                        return false;
                    }
                }

                Relationship relationship = Relationship.Get(observer, actor, false);
                if (relationship != null)
                {
                    if (!LTRData.Get(relationship.LTR.CurrentLTR).IsRomantic)
                    {
                        if ((actor.Partner != null) && (actor.Partner != target.SimDescription))
                        {
                            bool flag = observer.Genealogy.IsBloodRelated(actor.Partner.Genealogy);
                            Relationship relationship2 = Relationship.Get(observer.SimDescription, actor.Partner, false);
                            if (relationship2 != null)
                            {
                                LTRData data2 = LTRData.Get(relationship2.LTR.CurrentLTR);
                                flag |= data2.IsFriendly;
                            }

                            if (flag)
                            {
                                SocialComponent.OnSomeoneICareAboutWasCheatedOn(observer, actor.Partner, actor.SimDescription, target.SimDescription, jealousyLevel);
                                return true;
                            }
                        }
                    }
                    else
                    {
                        // Custom
                        switch (jealousyLevel)
                        {
                            case JealousyLevel.Medium:
                            case JealousyLevel.High:
                                GreyedOutTooltipCallback greyedOutTooltipCallback = null;
                                if (CommonSocials.TestAllowBreakup(actor, true, ref greyedOutTooltipCallback))
                                {
                                    RomanceVisibilityState.PushAccuseSimOfBetrayal(observer, actor);
                                }
                                return true;
                        }

                        LongTermRelationshipTypes longTermRelationship = Relationship.GetLongTermRelationship(observer, actor);
                        SocialComponent.PlayReactionAndUpdateRelationshipOnJealousy(observer, actor.SimDescription, target.SimDescription, jealousyLevel);
                        LongTermRelationshipTypes currentLTR = Relationship.GetLongTermRelationship(observer, actor);
                        SocialComponent.SetSocialFeedback(CommodityTypes.Insulting, observer, false, 0x0, longTermRelationship, currentLTR);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
