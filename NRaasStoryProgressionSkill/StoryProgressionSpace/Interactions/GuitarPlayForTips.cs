using NRaas.StoryProgressionSpace.Scenarios.Skills;
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

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class GuitarPlayForTips
    {
        public static InteractionDefinition Singleton = new Definition();

        protected class Definition : Guitar.PlayForTips.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                return StoryProgression.Main.Skills.ApplyTimeLimit(base.CreateInstance(ref parameters), StoryProgression.Main.GetValue<BuskerPushScenario.TimeLimitOption,int>());
            }

            public override string GetInteractionName(Sim actor, Guitar target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(Guitar.PlayForTips.Singleton, target));
            }

            public override bool Test(Sim a, Guitar target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }
        }
    }
}
