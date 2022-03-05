using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Helpers;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using System;
using System.Collections.Generic;

namespace NRaas.CareerSpace.Interactions
{
    public class ComputerSignUpForAfterschoolClassEx : Computer.SignUpForAfterschoolClass, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Computer, Computer.SignUpForAfterschoolClass.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Computer>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                AfterschoolActivityType chosenActivity = (InteractionDefinition as Definition).ChosenActivity;
                if (AfterschoolActivityEx.IsChildActivity(chosenActivity) && AfterschoolActivityEx.AlreadyHasChildActivity(Actor.SimDescription))
                {
                    return false;
                }

                StandardEntry();
                if (!Target.StartComputing(this, SurfaceHeight.Table, true))
                {
                    StandardExit();
                    return false;
                }

                Target.StartVideo(Computer.VideoType.Browse);
                AnimateSim("GenericTyping");
                bool flag = DoTimedLoop(kTimeToSpendSigningUp, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));

                // Custom
                if (flag && AfterschoolActivityEx.AddNewActivity(Actor.SimDescription, chosenActivity))
                {
                    EventTracker.SendEvent(new AfterschoolActivityEvent(EventTypeId.kSignedUpForAfterschoolActivity, Actor, Target, chosenActivity));
                }

                Target.StopComputing(this, Computer.StopComputingAction.TurnOff, false);
                StandardExit();
                return flag;
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

        public new class Definition : Computer.SignUpForAfterschoolClass.Definition
        {
            public Definition()
            { }
            public Definition(AfterschoolActivityType type)
                : base(type)
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new ComputerSignUpForAfterschoolClassEx();
                na.Init(ref parameters);
                return na;
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, Computer target, List<InteractionObjectPair> results)
            {
                if (AfterschoolActivityEx.AlreadyHasChildActivity(actor.SimDescription)) return;

                foreach (AfterschoolActivityData data in AfterschoolActivityBooter.Activities.Values)
                {
                    if (!data.IsValidFor(actor.SimDescription)) continue;

                    results.Add(new InteractionObjectPair(new Definition(data.mActivity.CurrentActivityType), iop.Target));
                }
            }

            public override string GetInteractionName(Sim a, Computer target, InteractionObjectPair interaction)
            {
                return base.GetInteractionName(a, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, Computer target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!target.IsComputerUsable(a, true, false, isAutonomous))
                    {
                        return false;
                    }
                    else if (!AfterschoolActivityEx.MeetsCommonAfterschoolActivityRequirements(a.SimDescription, ChosenActivity, ref greyedOutTooltipCallback))
                    {
                        return false;
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Common.Exception(a, target, e);
                    return false;
                }
            }
        }
    }
}
