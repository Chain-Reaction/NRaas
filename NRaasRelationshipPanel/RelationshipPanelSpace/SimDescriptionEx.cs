using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RelationshipPanelSpace
{
    public class SimDescriptionEx
    {
        public static void OnPickFromPanel(SimDescription ths, UIMouseEventArgs eventArgs, GameObjectHit gameObjectHit)
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
                        bool success = false;
                        try
                        {
                            IPhone activePhone = null;
                            if (activeActor.Inventory != null)
                            {
                                activePhone = activeActor.Inventory.Find<IPhone>();
                            }

                            List<InteractionObjectPair> interactions = new List<InteractionObjectPair>();

                            bool isServiceAlien = (ths.Household != null) && ths.Household.IsAlienHousehold;
                            if (GameUtils.IsInstalled(ProductVersion.EP8) && !isServiceAlien)
                            {
                                interactions.Add(new InteractionObjectPair(new Mailbox.WriteLoveLetter.Definition(ths.SimDescriptionId), activeActor));
                            }

                            if (GameUtils.IsInstalled(ProductVersion.EP10))
                            {
                                interactions.Add(new InteractionObjectPair(new OccultMermaid.SignalMermaid.Definition(ths), activeActor));
                            }

                            if (ths.CreatedSim != null)
                            {
                                List<InteractionObjectPair> others = ths.CreatedSim.GetAllInteractionsForActor(activeActor);
                                if (others != null)
                                {
                                    interactions.AddRange(others);
                                }
                            }

                            if (ths.IsHuman)
                            {
                                if (ths.CreatedSim != null)
                                {
                                    interactions.Add(new InteractionObjectPair(CallOver.Singleton, ths.CreatedSim));
                                }

                                if (activePhone != null)
                                {
                                    interactions.Add(new InteractionObjectPair(activePhone.GetCallChatFromRelationPanelDefinition(ths), activePhone));

                                    if (GameUtils.IsInstalled(ProductVersion.EP4))
                                    {
                                        interactions.Add(new InteractionObjectPair(activePhone.GetCallPrank(ths), activePhone));
                                    }

                                    if (GameUtils.IsInstalled(ProductVersion.EP9))
                                    {
                                        interactions.Add(new InteractionObjectPair(activePhone.GetSendChatTextFromRelationPanelDefinition(ths), activePhone));
                                        interactions.Add(new InteractionObjectPair(activePhone.GetSendInsultTextFromRelationPanelDefinition(ths), activePhone));
                                        interactions.Add(new InteractionObjectPair(activePhone.GetSendPictureTextFromRelationPanelDefinition(ths), activePhone));
                                        interactions.Add(new InteractionObjectPair(activePhone.GetSendSecretAdmirerTextFromRelationPanelDefinition(ths), activePhone));
                                        interactions.Add(new InteractionObjectPair(activePhone.GetSendBreakUpTextFromRelationPanelDefinition(ths), activePhone));
                                        interactions.Add(new InteractionObjectPair(activePhone.GetSendWooHootyTextFromRelationPanelDefinition(ths), activePhone));
                                    }

                                    if ((!ths.IsEnrolledInBoardingSchool() && !ths.IsDroppingOut) && !GameStates.IsEarlyDepartureSim(ths.SimDescriptionId))
                                    {
                                        interactions.Add(new InteractionObjectPair(activePhone.GetCallInviteOverFromRelationPanelDefinition(ths, true), activePhone));
                                        interactions.Add(new InteractionObjectPair(activePhone.GetCallInviteToLotFromRelationPanelDefintion(ths), activePhone));
                                        interactions.Add(new InteractionObjectPair(activePhone.GetCallAskOutOnDateFromRelationPanelDefintion(ths), activePhone));
                                        if (!isServiceAlien)
                                        {
                                            interactions.Add(new InteractionObjectPair(activePhone.GetCallInviteHouseholdOverFromRelationshipPanelDefiniton(ths), activePhone));
                                        }
                                        interactions.Add(new InteractionObjectPair(activePhone.GetCallInviteToAttendGraduationFromRelationPanelDefinition(ths), activePhone));
                                    }
                                    else
                                    {
                                        interactions.Add(new InteractionObjectPair(activePhone.GetRemoveFromBoardingSchool(ths), activePhone));
                                    }
                                }
                            }
                            else if (ths.IsPet)
                            {
                                if (ths.CreatedSim != null)
                                {
                                    interactions.Add(new InteractionObjectPair(Sim.CallPet.Singleton, ths.CreatedSim));
                                }

                                if (activePhone != null)
                                {
                                    interactions.Add(new InteractionObjectPair(activePhone.GetCallBringPetOverFromRelationshipPanelDefinition(ths), activePhone));
                                }
                            }

                            if (CelebrityManager.CanSocialize(activeActor.SimDescription, ths))
                            {
                                Sims3.Gameplay.UI.PieMenu.TestAndBringUpPieMenu(ths.CreatedSim, eventArgs, gameObjectHit, interactions, InteractionMenuTypes.Normal);
                                success = true;
                            }
                        }
                        catch (Exception e)
                        {
                            Common.Exception(activeActor.SimDescription, ths, e);
                        }

                        if ((!success) && (ths.CreatedSim != null))
                        {
                            Sims3.Gameplay.UI.PieMenu.TestAndBringUpPieMenu(ths.CreatedSim, eventArgs, gameObjectHit, InteractionsEx.GetImmediateInteractions(ths.CreatedSim), InteractionMenuTypes.Normal);
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
                Common.Exception(ths, e);
            }
        }
    }
}
