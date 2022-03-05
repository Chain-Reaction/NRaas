using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Scuba;
using Sims3.Gameplay.Tasks;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.SelectorSpace.Tasks
{
    public class GameObjectEx
    {
        private static bool IsImmediateInteraction(InteractionObjectPair pair, bool isFemale)
        {
            if (pair.InteractionDefinition is IImmediateInteractionDefinition)
            {
                Type type = pair.InteractionDefinition.GetType();

                if (!type.Namespace.StartsWith("Sims3."))
                {
                    foreach (object attribute in type.GetCustomAttributes(true))
                    {
                        if (attribute is DoesntRequireTuningAttribute) return true;
                    }
                }
            }

            return false;
        }

        private static bool TestInteractions(Sim activeActor, GameObjectHit gameObjectHit, List<InteractionObjectPair> interactions)
        {
            bool found = false;

            for (int i = interactions.Count - 1; i >= 0; i--)
            {
                InteractionObjectPair interaction = interactions[i];

                string name = interaction.InteractionDefinition.GetType().ToString();

                bool testSuccess = false, nameSuccess = false;
                try
                {
                    InteractionInstanceParameters userDirected = new InteractionInstanceParameters(interaction, activeActor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true);
                    userDirected.mGameObjectHit = gameObjectHit;

                    try
                    {
                        name = interaction.InteractionDefinition.GetInteractionName(ref userDirected);
                        nameSuccess = true;
                    }
                    catch
                    {
                        name = interaction.InteractionDefinition.GetType().ToString();
                    }

                    GreyedOutTooltipCallback callback = null;
                    if (IUtil.IsPass(interaction.InteractionDefinition.Test(ref userDirected, ref callback)) || (callback != null))
                    {
                        found = true;
                    }

                    testSuccess = true;
                }
                catch (Exception e)
                {
                    Common.Exception(activeActor, interaction.Target, name, e);
                }

                if ((!testSuccess) || (!nameSuccess))
                {
                    interactions.RemoveAt(i);
                }
            }

            return found;
        }

        public static bool OnPick(GameObject ths, UIMouseEventArgs eventArgs, GameObjectHit gameObjectHit)
        {
            Sim activeActor = Sim.ActiveActor;

            try
            {
                if (!GameStates.IsPlayFlowState)
                {
                    if (GameStates.IsBuildBuyLikeState) return false;

                    bool isFemale = false;
                    if (activeActor != null)
                    {
                        isFemale = activeActor.IsFemale;
                    }

                    InteractionMenuTypes menuType = InteractionMenuTypes.Normal;

                    List<InteractionObjectPair> interactions = new List<InteractionObjectPair>();
                    if (((eventArgs.Modifiers & Modifiers.kModifierMaskShift) != Modifiers.kModifierMaskNone) && Cheats.sTestingCheatsEnabled)
                    {
                        interactions.AddRange(ths.GetAllCheatInteractionsForActor(activeActor));

                        if (Selector.Settings.mMoveInteractionsToShift)
                        {
                            foreach (InteractionObjectPair pair in ths.GetAllInteractionsForActor(activeActor))
                            {
                                if (IsImmediateInteraction(pair, isFemale))
                                {
                                    interactions.Add(pair);
                                }
                            }
                        }

                        menuType = InteractionMenuTypes.Debug;

                        GameObject.ValidateInteractionList(activeActor, interactions);
                    }
                    else if ((activeActor != null) && activeActor.InteractionQueue.CanPlayerQueue())
                    {
                        List<InteractionObjectPair> allInteractions = null;                        

                        if (ths.Charred)
                        {
                            allInteractions = ths.GetCharredInteractions(activeActor);
                        }
                        else
                        {
                            allInteractions = ths.GetAllInteractionsForActor(activeActor);
                        }

                        if (Selector.Settings.mMoveInteractionsToShift)
                        {
                            foreach (InteractionObjectPair pair in allInteractions)
                            {
                                if (!IsImmediateInteraction(pair, isFemale))
                                {
                                    interactions.Add(pair);
                                }
                            }
                        }
                        else
                        {
                            interactions = allInteractions;
                        }

                        GameObject.ValidateInteractionList(activeActor, interactions);

                        Lot targetObject = LotManager.GetLot(gameObjectHit.mId);
                        if (ths.IsOnDivingFloor && ((activeActor.Level != ths.Level) || (activeActor.LotCurrent != ths.LotCurrent)))
                        {
                            interactions.Add(new InteractionObjectPair(ProxyGoToTargetsLotAndScubaDive.Singleton, ths.LotCurrent));
                        }
                        else if ((((targetObject != null) && targetObject.IsDivingLot) && (gameObjectHit.mType == GameObjectHitType.LotTerrain)) && ((activeActor.Level != 0) || (activeActor.LotCurrent != targetObject)))
                        {
                            interactions.Add(new InteractionObjectPair(ProxyGoToTargetsLotAndScubaDive.Singleton, targetObject));
                        }
                    }
                    else if ((activeActor != null) && !activeActor.InteractionQueue.CanPlayerQueue())
                    {
                        Sims3.Gameplay.UI.PieMenu.ShowGreyedOutTooltip(Localization.LocalizeString("Gameplay/Abstracts/GameObject:TooManyInteractions", new object[0x0]), eventArgs.MousePosition);
                        return true;
                    }
                    else
                    {
                        if (Selector.Settings.mShowNoInteractions)
                        {
                            Sims3.Gameplay.UI.PieMenu.ShowGreyedOutTooltip(Localization.LocalizeString("Gameplay/Abstracts/GameObject:NoInteractions", new object[0x0]), eventArgs.MousePosition);
                        }
                        return true;
                    }

                    if (!TestInteractions(activeActor, gameObjectHit, interactions))
                    {
                        if (!Selector.Settings.mShowNoInteractions) return true;
                    }

                    bool success = false;

                    try
                    {
                        Sims3.Gameplay.UI.PieMenu.TestAndBringUpPieMenu(ths, eventArgs, gameObjectHit, interactions, menuType);
                        success = true;
                    }
                    catch (Exception e)
                    {
                        Common.Exception(activeActor, ths, e);
                    }

                    if (!success)
                    {
                        List<InteractionObjectPair> immediateInteractions = new List<InteractionObjectPair>();

                        foreach (InteractionObjectPair interaction in interactions)
                        {
                            if (interaction.InteractionDefinition is IImmediateInteractionDefinition)
                            {
                                immediateInteractions.Add(interaction);
                            }
                        }

                        immediateInteractions = ths.BuildInteractions(activeActor, immediateInteractions);

                        Sims3.Gameplay.UI.PieMenu.TestAndBringUpPieMenu(ths, eventArgs, gameObjectHit, immediateInteractions, menuType);
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                Common.Exception(activeActor, ths, e);
            }

            return false;
        }
    }
}
