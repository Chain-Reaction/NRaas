using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Interactions
{
    public class PlasticSurgeryEx : Hospital.PlasticSurgery, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Hospital, Hospital.PlasticSurgery.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Hospital, Hospital.PlasticSurgery.Definition>(Singleton);
        }

        public static bool StartPlasticSurgeryCAS(Sim sim, Hospital.SurgeryTypes type)
        {
            CASChangeReporter.Instance.ClearChanges();

            SimDescription simDescription = sim.SimDescription;
            if (simDescription == null)
            {
                throw new Exception("EditSimInCAS:  sim doesn't have a description!");
            }

            switch (sim.CurrentOutfitCategory)
            {
                case OutfitCategories.Everyday:
                case OutfitCategories.Formalwear:
                case OutfitCategories.Sleepwear:
                case OutfitCategories.Swimwear:
                case OutfitCategories.Athletic:
                    break;
                default:
                    SimpleMessageDialog.Show(Cheats.LocalizeString("EditSimInCAS", new object[0x0]), Cheats.LocalizeString("CommonOutfitCategoryRequired", new object[0x0]), ModalDialog.PauseMode.PauseSimulator);
                    return false;
            }

            switch (type)
            {
                case Hospital.SurgeryTypes.PlasticSurgeryFaceCheap:
                case Hospital.SurgeryTypes.PlasticSurgeryFaceExpensive:
                    new Sims.Advanced.SurgeryFace().Perform(new GameHitParameters<GameObject>(sim, sim, GameObjectHit.NoHit));
                    break;

                case Hospital.SurgeryTypes.PlasticSurgeryBodyCheap:
                case Hospital.SurgeryTypes.PlasticSurgeryBodyExpensive:
                    new Sims.Advanced.SurgeryBody().Perform(new GameHitParameters<GameObject>(sim, sim, GameObjectHit.NoHit));
                    break;

                default:
                    throw new Exception("PlasticSurgery: Option not defined!");
            }

            while (GameStates.NextInWorldStateId != InWorldState.SubState.LiveMode)
            {
                SpeedTrap.Sleep();
            }

            bool flag = !CASChangeReporter.Instance.CasCancelled;
            CASChangeReporter.Instance.ClearChanges();
            return flag;
        }

        private new void ApplySurgery(Hospital.SurgeryTypes type, int cost, float failChance, string failKey, string successKey)
        {
            if (Actor.FamilyFunds > cost)
            {
                Actor.ModifyFunds(-cost);
                if (!RandomUtil.RandomChance01(failChance))
                {
                    if (StartPlasticSurgeryCAS(Actor, type))
                    {
                        Actor.ShowTNSAndPlayStingIfSelectable(Common.LocalizeEAString(Actor.IsFemale, successKey, new object[] { Actor }), StyledNotification.NotificationStyle.kGameMessagePositive, Target.ObjectId, ObjectGuid.InvalidObjectGuid, "sting_plastic_surgery_success");
                        mReaction = Reaction.Happy;
                        switch (type)
                        {
                            case Hospital.SurgeryTypes.PlasticSurgeryFaceCheap:
                            case Hospital.SurgeryTypes.PlasticSurgeryFaceExpensive:
                                Actor.SimDescription.PreSurgeryFacialBlends = null;
                                return;

                            case Hospital.SurgeryTypes.PlasticSurgeryBodyCheap:
                            case Hospital.SurgeryTypes.PlasticSurgeryBodyExpensive:
                                Actor.SimDescription.PreSurgeryBodyFitness = -1f;
                                Actor.SimDescription.PreSurgeryBodyWeight = -1f;
                                return;

                            case Hospital.SurgeryTypes.PlasticSurgeryCorrectiveFace:
                            case Hospital.SurgeryTypes.PlasticSurgeryCorrectiveBody:
                                return;
                        }
                    }
                    else
                    {
                        Actor.ModifyFunds(cost);
                        Actor.ShowTNSIfSelectable(TNSNames.PlasticSurgeryCancelledTNS, Actor, null, new object[] { Actor });
                        mReaction = Reaction.None;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case Hospital.SurgeryTypes.PlasticSurgeryFaceCheap:
                        case Hospital.SurgeryTypes.PlasticSurgeryFaceExpensive:
                            OutfitUtils.RandomizeFace(Actor);
                            break;

                        case Hospital.SurgeryTypes.PlasticSurgeryBodyCheap:
                        case Hospital.SurgeryTypes.PlasticSurgeryBodyExpensive:
                            OutfitUtils.RandomizeBody(Actor);
                            break;
                    }
                    Actor.ShowTNSAndPlayStingIfSelectable(Common.LocalizeEAString(Actor.IsFemale, failKey, new object[] { Actor }), StyledNotification.NotificationStyle.kGameMessageNegative, Target.ObjectId, ObjectGuid.InvalidObjectGuid, "sting_plastic_surgery_fail");
                    Actor.BuffManager.AddElement(BuffNames.Embarrassed, Origin.FromPlasticSurgery);
                    mReaction = Reaction.Sad;
                }
            }
        }

        private new void OnDisplayCAS()
        {
            while ((Sims3.Gameplay.Gameflow.CurrentGameSpeed == Sims3.Gameplay.Gameflow.GameSpeed.Pause) || MoveDialog.InProgress())
            {
                SpeedTrap.Sleep();
            }

            if (Actor.IsSelectable)
            {
                Definition interactionDefinition = InteractionDefinition as Definition;
                switch (interactionDefinition.GetSurgeryType())
                {
                    case Hospital.SurgeryTypes.PlasticSurgeryFaceCheap:
                        ApplySurgery(Hospital.SurgeryTypes.PlasticSurgeryFaceCheap, Hospital.kCheapSurgeryFaceCost, Hospital.kCheapSurgeryFailureChance, "Gameplay/Objects/RabbitHoles/Hospital/PlasticSurgery:PlasticSurgeryFaceFailure", "Gameplay/Objects/RabbitHoles/Hospital/PlasticSurgery:PlasticSurgeryFaceSuccess");
                        break;

                    case Hospital.SurgeryTypes.PlasticSurgeryFaceExpensive:
                        ApplySurgery(Hospital.SurgeryTypes.PlasticSurgeryFaceExpensive, Hospital.kExpensiveSurgeryFaceCost, Hospital.kExpensiveSurgeryFailureChance, "Gameplay/Objects/RabbitHoles/Hospital/PlasticSurgery:PlasticSurgeryFaceFailure", "Gameplay/Objects/RabbitHoles/Hospital/PlasticSurgery:PlasticSurgeryFaceSuccess");
                        break;

                    case Hospital.SurgeryTypes.PlasticSurgeryBodyCheap:
                        ApplySurgery(Hospital.SurgeryTypes.PlasticSurgeryBodyCheap, Hospital.kCheapSurgeryBodyCost, Hospital.kCheapSurgeryFailureChance, "Gameplay/Objects/RabbitHoles/Hospital/PlasticSurgery:PlasticSurgeryBodyFailure", "Gameplay/Objects/RabbitHoles/Hospital/PlasticSurgery:PlasticSurgeryBodySuccess");
                        break;

                    case Hospital.SurgeryTypes.PlasticSurgeryBodyExpensive:
                        ApplySurgery(Hospital.SurgeryTypes.PlasticSurgeryBodyExpensive, Hospital.kExpensiveSurgeryBodyCost, Hospital.kExpensiveSurgeryFailureChance, "Gameplay/Objects/RabbitHoles/Hospital/PlasticSurgery:PlasticSurgeryBodyFailure", "Gameplay/Objects/RabbitHoles/Hospital/PlasticSurgery:PlasticSurgeryBodySuccess");
                        break;

                    case Hospital.SurgeryTypes.PlasticSurgeryCorrectiveFace:
                        if ((Actor.SimDescription.PreSurgeryFacialBlends != null) && (Actor.FamilyFunds >= Hospital.kCorrectiveFaceSurgeryCost))
                        {
                            Actor.ModifyFunds(-Hospital.kCorrectiveFaceSurgeryCost);
                            OutfitUtils.RestoreFace(Actor);
                            Actor.ShowTNSAndPlayStingIfSelectable(Common.LocalizeEAString(Actor.IsFemale, "Gameplay/Objects/RabbitHoles/Hospital/PlasticSurgery:CorrectiveSurgerySuccess", new object[] { Actor }), StyledNotification.NotificationStyle.kGameMessagePositive, Target.ObjectId, ObjectGuid.InvalidObjectGuid, "sting_plastic_surgery_undo");
                            mReaction = Reaction.Happy;
                        }
                        break;

                    case Hospital.SurgeryTypes.PlasticSurgeryCorrectiveBody:
                        if ((Actor.SimDescription.PreSurgeryBodyFitness != -1f) && (Actor.FamilyFunds >= Hospital.kCorrectiveBodySurgeryCost))
                        {
                            Actor.ModifyFunds(-Hospital.kCorrectiveBodySurgeryCost);
                            OutfitUtils.RestoreBody(Actor);
                            Actor.ShowTNSAndPlayStingIfSelectable(Common.LocalizeEAString(Actor.IsFemale, "Gameplay/Objects/RabbitHoles/Hospital/PlasticSurgery:CorrectiveSurgerySuccess", new object[] { Actor }), StyledNotification.NotificationStyle.kGameMessagePositive, Target.ObjectId, ObjectGuid.InvalidObjectGuid, "sting_plastic_surgery_undo");
                            mReaction = Reaction.Happy;
                        }
                        break;
                }

                /*
                if (mReaction != Reaction.None)
                {
                    Actor.RecreateOccupationOutfits();
                    (Responder.Instance.HudModel as Sims3.Gameplay.UI.HudModel).NotifySimChanged(Actor.ObjectId);
                }
                */
            }
        }

        public override bool InRabbitHole()
        {
            try
            {
                BeginCommodityUpdates();
                mTookSemaphore = GameStates.WaitForInteractionStateChangeSemaphore(Actor, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                if (!mTookSemaphore)
                {
                    return false;
                }
                StartStages();
                bool succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                if (succeeded)
                {
                    OnDisplayCAS();
                }
                else
                {
                    Actor.ShowTNSIfSelectable(TNSNames.PlasticSurgeryCancelledTNS, Actor, null, new object[] { Actor });
                    mReaction = Reaction.None;
                }
                EndCommodityUpdates(succeeded);
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

        public new class Definition : Hospital.PlasticSurgery.Definition
        {
            public Definition()
            { }
            public Definition(Hospital.SurgeryTypes surgeryType, string[] path)
                : base(surgeryType, path)
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new PlasticSurgeryEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Hospital target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, Hospital target, List<InteractionObjectPair> results)
            {
                SimDescription simDescription = actor.SimDescription;
                if (simDescription.IsAlien)
                {
                    results.Add(new InteractionObjectPair(new Definition(Hospital.SurgeryTypes.PlasticSurgeryBodyCheap, new string[0x0]), target));
                }
                else
                {
                    string[] path = new string[] { Localization.LocalizeString(actor.IsFemale, "Gameplay/Objects/RabbitHoles/Hospital:PlasticSurgery", new object[0x0]) };
                    foreach (Hospital.SurgeryTypes types in Enum.GetValues(typeof(Hospital.SurgeryTypes)))
                    {
                        if (ShouldShowSurgeryOption(simDescription, types))
                        {
                            results.Add(new InteractionObjectPair(new Definition(types, path), target));
                        }
                    }
                }
            }
        }
    }
}
