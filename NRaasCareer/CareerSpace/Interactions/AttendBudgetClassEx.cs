using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Interfaces;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Interactions
{
    public class AttendBudgetClassEx : Common.IPreLoad, Common.IAddInteraction
    {
        public static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<RabbitHole, CollegeOfBusiness.AttendBudgetClass.Definition, Definition>(false);

            sOldSingleton = CollegeOfBusiness.AttendBudgetClass.Singleton;
            CollegeOfBusiness.AttendBudgetClass.Singleton = new Definition ();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<RabbitHole, CollegeOfBusiness.AttendBudgetClass.Definition>(CollegeOfBusiness.AttendBudgetClass.Singleton);
        }

        public class Definition : CollegeOfBusiness.AttendBudgetClass.Definition
        {
            public Definition()
            { }

            public override string GetInteractionName(Sim actor, RabbitHole target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, RabbitHole target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (a.FamilyFunds < CollegeOfBusiness.kCostOfBudgetClass)
                    {
                        return false;
                    }
                    if (!SimClock.IsTimeBetweenTimes(CollegeOfBusiness.AttendBudgetClass.kStartAvailibilityTime, CollegeOfBusiness.AttendBudgetClass.kEndAvailibilityTime))
                    {
                        return false;
                    }
                    /*
                    if (!GameUtils.IsUniversityWorld())
                    {
                        return false;
                    }
                    */
                    float num = 0f;
                    if (CollegeOfBusiness.AttendBudgetClass.sCooldownDict.TryGetValue(a.SimDescription.SimDescriptionId, out num))
                    {
                        return ((SimClock.ElapsedTime(TimeUnit.Hours) - num) > CollegeOfBusiness.AttendBudgetClass.kInteractionCooldown);
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
