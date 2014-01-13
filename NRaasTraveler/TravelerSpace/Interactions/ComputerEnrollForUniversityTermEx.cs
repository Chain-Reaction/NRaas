using NRaas.CommonSpace.Helpers;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.TravelerSpace.Interactions
{
    public class ComputerEnrollForUniversityTermEx : Computer.EnrollForUniversityTerm, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<Computer, Computer.EnrollForUniversityTerm.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Computer, Computer.EnrollForUniversityTerm.Definition>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                StandardEntry();
                if (!Target.StartComputing(this, SurfaceHeight.Table, true))
                {
                    StandardExit();
                    return false;
                }

                Target.StartVideo(Computer.VideoType.Browse);
                AnimateSim("GenericTyping");
                bool flag = DoTimedLoop(kTimeToSpendEnrolling, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                if (flag)
                {
                    List<SimDescription> others = null;
                    int tuitionCost = 0;

                    flag = AcademicCareerEx.EnrollInAcademicCareer(Actor, Traveler.Settings.mTravelFilter, out others, out tuitionCost);
                    if (flag)
                    {
                        if (GameUtils.gGameUtils.GetCurrentWorldName() == WorldName.University)
                        {
                            if (others != null)
                            {
                                Actor.ModifyFunds(-tuitionCost);

                                AcademicCareerEx.Enroll(others);
                            }
                        }
                        else
                        {
                            TravelUtil.TriggerTravelToUniversityWorld(others, tuitionCost);
                        }
                    }
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

        public new class Definition : Computer.EnrollForUniversityTerm.Definition
        {
            public override string GetInteractionName(Sim actor, Computer target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new ComputerEnrollForUniversityTermEx();
                na.Init(ref parameters);
                return na;
            }

            public override bool Test(Sim a, Computer target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!target.IsComputerUsable(a, true, false, isAutonomous))
                {
                    return false;
                }

                if (!Helpers.TravelUtilEx.CanSimTriggerTravelToUniversityWorld(a, true, ref greyedOutTooltipCallback))
                {
                    return false;
                }

                if (a.OccupationAsAcademicCareer != null)
                {
                    return false;
                }

                return true;
            } 
        }
    }
}
