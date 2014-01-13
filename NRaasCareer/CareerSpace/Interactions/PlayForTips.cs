using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Interfaces;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Interactions
{
    public class PlayForTips : Common.IPreLoad, Common.IAddInteraction
    {
        public static InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<Guitar, Guitar.PlayForTips.Definition, Definition>(false);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Guitar>(Singleton);
        }

        protected class Definition : Guitar.PlayForTips.Definition
        {
            public override string GetInteractionName(Sim actor, Guitar target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(Guitar.PlayForTips.Singleton, target));
            }

            public override bool Test(Sim a, Guitar target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback)) return false;

                if (SkillBasedCareerBooter.GetSkillBasedCareer(a, Sims3.Gameplay.Skills.SkillNames.Guitar) == null)
                {
                    return false;
                }

                if (a.SkillManager.GetSkillLevel(Sims3.Gameplay.Skills.SkillNames.Guitar) >= 5)
                {
                    return false;
                }

                return true;
            }
        }
    }
}
