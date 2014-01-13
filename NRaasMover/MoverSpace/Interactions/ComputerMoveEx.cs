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
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.MoverSpace.Interactions
{
    public class ComputerMoveEx : Computer.Move, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<Computer, Computer.Move.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Computer, Computer.Move.Definition>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                if (!Target.StartComputing(this, SurfaceHeight.Table, true))
                {
                    return false;
                }

                if (!UIUtils.IsOkayToStartModalDialog())
                {
                    return false;
                }

                Target.StartVideo(Computer.VideoType.Browse);
                AnimateSim("GenericTyping");

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
                                Target.StopComputing(this, Computer.StopComputingAction.TurnOff, false);
                                return true;
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

                Target.StopComputing(this, Computer.StopComputingAction.TurnOff, false);
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

        public new class Definition : Computer.Move.Definition
        {
            public Definition()
            { }
            public Definition(bool localMove, string menuText)
                : base(localMove, menuText)
            { }

            public override string GetInteractionName(Sim actor, Computer target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, Computer target, List<InteractionObjectPair> results)
            {
                results.Add(new InteractionObjectPair(new Definition(true, "MoveLocal"), target));
                results.Add(new InteractionObjectPair(new Definition(false, "MoveGlobal"), target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new ComputerMoveEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}
