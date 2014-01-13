using NRaas.RelationshipPanelSpace.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RelationshipPanelSpace
{
    public class MiniSimDescriptionEx
    {
        public static InteractionDefinition GetCallInviteOverForeignVisitorsFromRelationPanelDefinition(IMiniSimDescription simToCall)
        {
            return new CallInviteOverForeignVisitorsFromRelationPanelEx.Definition(simToCall);
        }

        public static void OnPickFromPanel(MiniSimDescription ths, UIMouseEventArgs eventArgs, GameObjectHit gameObjectHit)
        {
            try
            {
                Sims3.Gameplay.UI.PieMenu.ClearInteractions();
                Sims3.Gameplay.UI.PieMenu.HidePieMenuSimHead = true;
                Sims3.UI.Hud.PieMenu.sIncrementalButtonIndexing = true;

                Sim activeActor = Sim.ActiveActor;
                if (activeActor != null)
                {
                    if (activeActor.InteractionQueue.CanPlayerQueue())
                    {
                        List<InteractionObjectPair> interactions = new List<InteractionObjectPair>();

                        if (GameUtils.IsInstalled(ProductVersion.EP8))
                        {
                            InteractionDefinition interaction = new Mailbox.WriteLoveLetter.Definition(ths.SimDescriptionId);
                            interactions.Add(new InteractionObjectPair(interaction, activeActor));
                        }

                        IPhone targetObject = activeActor.Inventory.Find<IPhone>();
                        if (targetObject != null)
                        {
                            interactions.Add(new InteractionObjectPair(GetCallInviteOverForeignVisitorsFromRelationPanelDefinition(ths), targetObject));
                            interactions.Add(new InteractionObjectPair(targetObject.GetCallChatFromRelationPanelDefinition(ths), targetObject));
                            interactions.Add(new InteractionObjectPair(targetObject.GetCallInviteToAttendGraduationFromRelationPanelDefinition(ths), targetObject));
                        }
                        if (GameUtils.IsInstalled(ProductVersion.EP9))
                        {
                            interactions.Add(new InteractionObjectPair(targetObject.GetSendChatTextFromRelationPanelDefinition(ths), targetObject));
                            interactions.Add(new InteractionObjectPair(targetObject.GetSendInsultTextFromRelationPanelDefinition(ths), targetObject));
                            interactions.Add(new InteractionObjectPair(targetObject.GetSendPictureTextFromRelationPanelDefinition(ths), targetObject));
                            interactions.Add(new InteractionObjectPair(targetObject.GetSendSecretAdmirerTextFromRelationPanelDefinition(ths), targetObject));
                            interactions.Add(new InteractionObjectPair(targetObject.GetSendBreakUpTextFromRelationPanelDefinition(ths), targetObject));
                            interactions.Add(new InteractionObjectPair(targetObject.GetSendWooHootyTextFromRelationPanelDefinition(ths), targetObject));
                        }

                        if (CelebrityManager.CanSocialize(activeActor.SimDescription, ths))
                        {
                            Sims3.Gameplay.UI.PieMenu.TestAndBringUpPieMenu(null, eventArgs, gameObjectHit, interactions, InteractionMenuTypes.Normal);
                        }
                        else
                        {
                            GreyedOutTooltipCallback greyedOutTooltipCallbackForced = delegate
                            {
                                return Common.LocalizeEAString("Gameplay/CelebritySystem/CelebrityManager:NotAbleToSocialize");
                            };
                            Sims3.Gameplay.UI.PieMenu.TestAndBringUpPieMenu(null, eventArgs, gameObjectHit, interactions, InteractionMenuTypes.Normal, "Gameplay/Abstracts/GameObject:NoInteractions", false, greyedOutTooltipCallbackForced);
                        }
                    }
                    else
                    {
                        Vector2 mousePosition;
                        if (eventArgs.DestinationWindow != null)
                        {
                            mousePosition = eventArgs.DestinationWindow.WindowToScreen(eventArgs.MousePosition);
                        }
                        else
                        {
                            mousePosition = eventArgs.MousePosition;
                        }
                        Sims3.Gameplay.UI.PieMenu.ShowGreyedOutTooltip(Common.LocalizeEAString("Gameplay/Abstracts/GameObject:TooManyInteractions"), mousePosition);
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(ths.FullName, e);
            }
        }
    }
}
