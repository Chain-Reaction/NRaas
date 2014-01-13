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
    public class PhoneCallEnrollForUniversityTermEx : Phone.CallEnrollForUniversityTerm, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<Phone, Phone.CallEnrollForUniversityTerm.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Phone, Phone.CallEnrollForUniversityTerm.Definition>(Singleton);
        }

        public override Phone.Call.ConversationBehavior OnCallConnected()
        {
            try
            {
                if (AcademicCareerEx.EnrollInAcademicCareer(Actor, Traveler.Settings.mTravelFilter, out mTravelers, out mTuitionCost))
                {
                    mIsEnrolled = true;
                    return Phone.Call.ConversationBehavior.ExpressSatisfaction;
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

            return Phone.Call.ConversationBehavior.ExpressDisappointment;
        }

        public override void OnPhoneFinished()
        {
            try
            {
                if (GameUtils.gGameUtils.GetCurrentWorldName() == WorldName.University)
                {
                    if (mIsEnrolled)
                    {
                        if (mTravelers != null)
                        {
                            Actor.ModifyFunds(-mTuitionCost);

                            AcademicCareerEx.Enroll(mTravelers);
                        }
                    }
                }
                else
                {
                    base.OnPhoneFinished();
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
        }

        public new class Definition : Phone.Call.CallDefinition<Phone.CallEnrollForUniversityTerm>
        {
            public override string GetInteractionName(Sim actor, Phone target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new PhoneCallEnrollForUniversityTermEx();
                na.Init(ref parameters);
                return na;
            }

            public override bool Test(Sim a, Phone target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback))
                {
                    return false;
                }

                if (!CommonSpace.Helpers.TravelUtilEx.CanSimTriggerTravelToUniversityWorld(a, Traveler.Settings.mTravelFilter, true, ref greyedOutTooltipCallback))
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
