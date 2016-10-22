using NRaas.CareerSpace.Interfaces;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Interactions
{
    public class SolicitGigEx : Sim.SolicitGig, Common.IPreLoad, Common.IAddInteraction
    {
        public static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, Sim.SolicitGig.Definition, Definition>(false);

            sOldSingleton = Sim.SolicitGig.Singleton;
            Sim.SolicitGig.Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sim, Sim.SolicitGig.Definition>(Sim.SolicitGig.Singleton);
        }

        public new class Definition : Sim.SolicitGig.Definition
        {
            public Definition()
            {
                base.MenuText = string.Empty;
            }

            public Definition(string text, string[] path, bool accessingNeeds)
            {
                base.MenuText = text;
                base.MenuPath = path;
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, Sim target, List<InteractionObjectPair> results)
            {
                PerformanceCareer occupationAsPerformanceCareer = actor.OccupationAsPerformanceCareer;
                if (((occupationAsPerformanceCareer != null) && target.SimDescription.HasActiveRole) && (target.SimDescription.AssignedRole.Type == Role.RoleType.Proprietor))
                {
                    results.Add(new InteractionObjectPair(new SolicitGigEx.Definition(occupationAsPerformanceCareer.SocializationTextSolicitJob, new string[0], false), iop.Target));
                }
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override string[] GetPath(bool isFemale)
            {
                return base.MenuPath;
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (((a == target) || !target.SimDescription.TeenOrAbove) || target.IsSleeping)
                {
                    return false;
                }
                if ((target.Posture is SwimmingInPool) || (a.Posture is SwimmingInPool))
                {
                    return false;
                }
                PerformanceCareer occupationAsPerformanceCareer = a.CareerManager.OccupationAsPerformanceCareer;
                if (occupationAsPerformanceCareer == null)
                {
                    return false;
                }
                if (!occupationAsPerformanceCareer.CanSolicitGig() || !target.LotCurrent.CanScheduleAnotherGigOnLot())
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Sim.LocalizeString("FullyBooked", new object[0]));
                    return false;
                }
                if (occupationAsPerformanceCareer.HasGigScheduledAtVenue(target.LotCurrent) || (occupationAsPerformanceCareer.TryToGetSteadyGigOwnedBySim(target) != null))
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Sim.LocalizeString("AlreadyHaveGig", new object[0]));
                    return false;
                }
                if (!occupationAsPerformanceCareer.DoesClientAcceptSolicitation(target))
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Sim.LocalizeString("SolicitedTooRecently", new object[0]));
                    return false;
                }
                if (target.SimDescription.HasActiveRole)
                {
                    Proprietor assignedRole = target.SimDescription.AssignedRole as Proprietor;
                    if ((assignedRole != null) && !assignedRole.HasArrivedAtTargetLot())
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Sim.LocalizeString("ProprietorHasNotArrived", new object[0]));
                        return false;
                    }
                }
                if (!occupationAsPerformanceCareer.CanSimBeSolicited(target))
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Sim.LocalizeString("LevelTooLowToSolicit", new object[0]));
                    return false;
                }
                /*
                if (a.SimDescription.IsVisuallyPregnant)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Sim.LocalizeString("CantDoGigsWhilePregnant", new object[0]));
                    return false;
                }
                 */
                if (target.SimDescription.HasActiveRole)
                {
                    Proprietor proprietor2 = target.SimDescription.AssignedRole as Proprietor;
                    if ((proprietor2 != null) && proprietor2.IsInSimFest())
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Sim.LocalizeString("ProprietorBusy", new object[0]));
                        return false;
                    }
                }
                if (!IUtil.IsPass(SocialInteractionA.Definition.CanSocializeWith("Solicit Gig", a, target, isAutonomous, ref greyedOutTooltipCallback)))
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Sim.LocalizeString("LTRTooLowToSolicit", new object[0]));
                    return false;
                }
                return true;
            }
        }
    }
}
