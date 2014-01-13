using NRaas.CareerSpace.Helpers;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Interactions
{
    public class EnrollInUniversityEx : SchoolRabbitHole.EnrollinUniversity, Common.IPreLoad, Common.IAddInteraction
    {
        public new static readonly InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<RabbitHole, SchoolRabbitHole.EnrollinUniversity.Definition, Definition>(true);
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
                tuning.Availability.WorldRestrictionType = WorldRestrictionType.None;
            }
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<AdminstrationCenter>(Singleton);
        }

        public override bool InRabbitHole()
        {
            try
            {
                List<SimDescription> others = null;
                int tuitionCost = 0;

                try
                {
                    List<SimDescription> choices = new List<SimDescription>();
                    foreach (SimDescription sim in Households.Humans(Actor.Household))
                    {
                        if (sim.TeenOrAbove)
                        {
                            choices.Add(sim);
                        }
                    }

                    if (AcademicCareerEx.EnrollInAcademicCareer(Actor, choices, out others, out tuitionCost))
                    {
                        if (others != null)
                        {
                            Actor.ModifyFunds(-tuitionCost);

                            foreach (SimDescription sim in others)
                            {
                                AcademicHelper.AddAcademic(sim, AcademicCareer.GlobalTermLength);
                            }

                            AcademicCareerEx.Enroll(others);
                        }
                    }
                }
                finally
                {
                    TravelUtil.PlayerMadeTravelRequest = false;
                }

                return true;
            }
            catch (Exception exception)
            {
                Common.Exception(Actor, Target, exception);
            }
            return true;
        }

        private new class Definition : InteractionDefinition<Sim, RabbitHole, EnrollInUniversityEx>
        {
            public override string GetInteractionName(Sim actor, RabbitHole target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(SchoolRabbitHole.EnrollinUniversity.Singleton, target));
            }

            public override bool Test(Sim actor, RabbitHole target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (actor.OccupationAsAcademicCareer != null)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Already Enrolled");
                    return false;   
                }

                if (!GameUtils.IsInstalled(ProductVersion.EP9))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("EP9 Fail");
                    return false;
                }

                return true;
            }
        }
    }
}
