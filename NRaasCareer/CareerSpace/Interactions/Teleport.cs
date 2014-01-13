using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Interfaces;
using NRaas.CareerSpace.Skills;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Interactions
{
    public class Teleport : Common.IAddInteraction
    {
        public static InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Terrain>(Singleton);
        }

        public class Definition : Terrain.TeleportMeHere.Definition
        {
            // Methods
            public Definition()
                : base(false)
            { }

            public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!Test(parameters.Actor as Sim, parameters.Target as Terrain, parameters.Autonomous, ref greyedOutTooltipCallback))
                {
                    return InteractionTestResult.Def_TestFailed;
                }

                return base.Test(ref parameters, ref greyedOutTooltipCallback);
            }

            public override bool Test(Sim actor, Terrain target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (actor.SkillManager == null) return false;

                Assassination skill = actor.SkillManager.GetSkill<Assassination>(Assassination.StaticGuid);
                if (skill == null) return false;

                if ((actor != null) && (actor.LotCurrent == actor.LotHome) && (Sims3.Gameplay.Queries.CountObjects<ITeleporter>(actor.LotHome) > 0x0))
                {
                    return false;
                }

                return skill.IsNinja();
            }
        }
    }
}
