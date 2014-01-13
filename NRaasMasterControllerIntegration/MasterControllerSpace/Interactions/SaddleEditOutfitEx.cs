using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Interactions
{
    public class SaddleEditOutfitEx : Saddle.EditOutfit, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, Saddle.EditOutfit.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sim, Saddle.EditOutfit.Definition>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                if (!IntroTutorial.IsRunning || IntroTutorial.AreYouExitingTutorial())
                {
                    SocialJig = GlobalFunctions.CreateObjectOutOfWorld("horseMountDismountJig", ProductVersion.EP5) as SocialJig;
                    SocialJig.RegisterParticipants(Actor, Target);
                    bool succeeded = false;
                    if (!BeginSocialInteraction(new SocialInteractionB.Definition(null, Saddle.LocalizeString(Target.IsFemale, "HaveOutfitEdited", new object[0x0]), false), true, false))
                    {
                        return succeeded;
                    }
                    StandardEntry();
                    if (Responder.Instance.OptionsModel.SaveGameInProgress)
                    {
                        BeginCommodityUpdates();
                        EndCommodityUpdates(true);
                        StandardExit();
                        return true;
                    }
                    mTookSemaphore = GameStates.WaitForInteractionStateChangeSemaphore(Target, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                    if (!mTookSemaphore)
                    {
                        StandardExit();
                        return false;
                    }
                    if (Target.IsSelectable)
                    {
                        while ((Sims3.Gameplay.Gameflow.CurrentGameSpeed == Sims3.Gameplay.Gameflow.GameSpeed.Pause) || MoveDialog.InProgress())
                        {
                            SpeedTrap.Sleep();
                        }

                        BeginCommodityUpdates();
                        SimDescription simDescription = Target.SimDescription;
                        if (simDescription == null)
                        {
                            throw new Exception("ChangeOutfit:  sim doesn't have a description!");
                        }
                        CASChangeReporter.Instance.ClearChanges();

                        new Sims.Dresser().Perform(new GameHitParameters<GameObject>(Target, Target, GameObjectHit.NoHit));

                        while (GameStates.NextInWorldStateId != InWorldState.SubState.LiveMode)
                        {
                            SpeedTrap.Sleep();
                        }

                        EndCommodityUpdates(true);

                        /*
                        if (Target.CurrentOutfitIndex > simDescription.GetOutfitCount(Target.CurrentOutfitCategory))
                        {
                            Target.UpdateOutfitInfo();
                        }

                        (Responder.Instance.HudModel as Sims3.Gameplay.UI.HudModel).NotifySimChanged(Target.ObjectId);
                         */

                        StandardExit();
                        return true;
                    }
                    StandardExit();
                }
                return false;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }

            return false;
        }

        public new class Definition : Saddle.EditOutfit.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new SaddleEditOutfitEx();
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
