using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Replacers;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.ShelvesStorage;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Interactions
{
    public class FinishMakeoverFreestyle : Common.IPreLoad
    {
        public void OnPreLoad()
        {
            if (GameUtils.IsInstalled(ProductVersion.EP2))
            {
                BooterLogger.AddError(SocialRHSReplacer.Perform<FinishMakeoverFreestyle>("Makeover Freestyle", "OnFinishMakeoverFreestyle"));
            }
        }

        public static void OnFinishMakeoverFreestyle(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                bool tookSemaphore = false;
                Styling.MakeoverOutcome makeoverOutcome = Styling.GetMakeoverOutcome(target, actor, true);
                bool forceFailureOutfit = makeoverOutcome == Styling.MakeoverOutcome.EpicFailure;
                bool flag3 = false;
                try
                {
                    if (forceFailureOutfit)
                    {
                        Styling.LoadMakeoverEpicFailureOutfitForCasOverride(target);
                    }
                    flag3 = GetMakeoverEx.DisplayCAS(target, actor, ref tookSemaphore, forceFailureOutfit);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (tookSemaphore)
                    {
                        GameStates.ReleaseInteractionStateChangeSemaphore();
                    }
                }

                if (CASChangeReporter.Instance.GetPropertyChanged(CASChangeReporter.ChangeFlags.Any))
                {
                    SkillLevel customerReactionType = Styling.GetCustomerReactionType(target, actor, makeoverOutcome, false);
                    SkillLevel stylerReactionType = Styling.GetStylerReactionType(customerReactionType);
                    StateMachineClient client = StateMachineClient.Acquire(actor, "StylistActiveCareer");
                    client.SetActor("x", target);
                    client.SetActor("y", actor);
                    client.SetParameter("doClothesSpin", !flag3);
                    client.SetParameter("customerReactionType", customerReactionType);
                    client.SetParameter("stylistReactionType", stylerReactionType);
                    client.EnterState("x", "Enter");
                    client.EnterState("y", "Enter");
                    actor.LoopIdle();
                    client.RequestState("x", "Customer Reaction");
                    Styling.PostMakeover(target, actor, makeoverOutcome, false, customerReactionType, true, true, new Styling.OnMakeoverCompletedCallback(SocialCallback.OnMakeoverFreestyleCompleted));
                    client.RequestState(false, "x", "Exit");
                    client.RequestState("y", "Stylist Reaction");
                    client.RequestState("y", "Exit");
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
