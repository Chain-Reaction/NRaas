using NRaas.CommonSpace.Helpers;
using NRaas.MoverSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Moving;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.MoverSpace.Interactions
{
    public class CallToMoveEx : Phone.CallToMove, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<Phone, Phone.CallToMove.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }

            sOldSingleton = Singleton;
            Singleton = new Definition ();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Phone, Phone.CallToMove.Definition>(Singleton);
        }

        public override Phone.Call.ConversationBehavior OnCallConnected()
        {
            try
            {
                if (UIUtils.IsOkayToStartModalDialog())
                {
                    string failReason = null;
                    if (((Actor.Household == Household.ActiveHousehold) && !MovingSituation.MovingInProgress) && InWorldSubState.IsEditTownValid(Actor.LotHome, ref failReason))
                    {
                        Definition interactionDefinition = InteractionDefinition as Definition;
                        if (interactionDefinition.LocalMove)
                        {
                            if (!Household.ActiveHousehold.LotHome.IsApartmentLot && (Household.ActiveHousehold.GetNumberOfRoommates() > 0))
                            {
                                if (!TwoButtonDialog.Show(Localization.LocalizeString("Ui/Caption/Roommates:MovingDismissConfirmation", new object[0]), LocalizationHelper.Yes, LocalizationHelper.No))
                                {
                                    return Phone.Call.ConversationBehavior.JustHangUp;
                                }

                                Household.RoommateManager.StopAcceptingRoommates(true);
                            }

                            MovingDialogEx.Show(new GameplayMovingModelEx(Actor));
                        }
                        else
                        {
                            MovingWorldsModel model = new MovingWorldsModel(Actor);
                            MovingWorldDialog.Show(model);
                            if ((model.WorldName != null) && MovingWorldUtil.VerifyWorldMove())
                            {
                                Common.FunctionTask.Perform(model.TriggerSaveAndTravel);
                            }
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
                Common.Exception(Actor, Target, e);
            }
            return Phone.Call.ConversationBehavior.JustHangUp;
        }

        public new class Definition : Phone.CallToMove.Definition
        {
            public Definition()
            { }
            public Definition(bool localMove, string menuText)
                : base(localMove, menuText)
            { }

            public override string GetInteractionName(Sim actor, Phone target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, Phone target, List<InteractionObjectPair> results)
            {
                results.Add(new InteractionObjectPair(new Definition(true, "MoveLocal"), target));
                results.Add(new InteractionObjectPair(new Definition(false, "MoveGlobal"), target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CallToMoveEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}
