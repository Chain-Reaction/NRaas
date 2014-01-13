using NRaas.CommonSpace.Helpers;
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

namespace NRaas.GoHereSpace.Interactions
{
    public class TeleportEx : Common.IPreLoad, Common.IAddInteraction
    {
        public static InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<Terrain, Terrain.TeleportMeHere.Definition, Definition>(false);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Terrain>(Singleton);
        }

        public class Definition : Terrain.TeleportMeHere.Definition
        {
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
                if ((actor != null) && (actor.LotCurrent == actor.LotHome) && (Sims3.Gameplay.Queries.CountObjects<ITeleporter>(actor.LotHome) > 0x0))
                {
                    return false;
                }

                if (GoHere.Settings.mTeleportForAll) return true;

                if (GoHere.Settings.mVampireTeleport)
                {
                    if (actor.SimDescription.IsVampire) return true;

                    if (actor.SimDescription.IsPlayableGhost) return true;

                    if (actor.SimDescription.IsUnicorn) return true;

                    if (actor.SimDescription.IsGenie) return true;

                    if (actor.SimDescription.IsFairy) return true;

                    if (actor.SimDescription.IsWitch) return true;
                }

                return false;
            }
        }
    }
}
