using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    /* Causes a game-engine crash for an unknown reason
    public class CleanupJigs : LoadupOption
    {
        public CleanupJigs()
        { }

        public override string GetTitlePrefix()
        {
            return "CleanupJigs";
        }

        protected static bool IsValidSocialJig(SocialJig jig, Sim sim)
        {
            if (sim == null) return true;

            if (sim.HasBeenDestroyed) return false;

            SocialInteraction interaction = sim.CurrentInteraction as SocialInteraction;
            if (interaction != null)
            {
                if (interaction.SocialJig == jig) return true;
            }

            if (sim.Posture != null)
            {
                if (sim.Posture.Container == jig) return true;
            }

            return false;
        }

        public override void OnWorldLoadFinished()
        {
            Overwatch.Log(GetTitlePrefix());

            List<SocialJig> cleanup = new List<SocialJig>();

            foreach (SocialJig jig in Sims3.Gameplay.Queries.GetObjects<SocialJig>())
            {
                if ((jig.SimA == null) && (jig.SimB == null))
                {
                    cleanup.Add(jig);
                }
                else if (!IsValidSocialJig(jig, jig.SimA))
                {               
                    cleanup.Add(jig);
                }
                else if (!IsValidSocialJig(jig, jig.SimB))
                {
                    cleanup.Add(jig);
                }
            }

            foreach (SocialJig jig in cleanup)
            {
                try
                {
                    string result = "  Destroyed Social Jig : " + jig.GetType().ToString();

                    if (jig.SimA != null)
                    {
                        result += Common.NewLine + "    SimA: " + jig.SimA.Name;
                    }
                    if (jig.SimB != null)
                    {
                        result += Common.NewLine + "    SimB: " + jig.SimB.Name;
                    }

                    jig.Destroy();

                    Overwatch.Log(result);
                }
                catch (Exception e)
                {
                    Common.DebugException(jig, e);
                }
            }
        }
    }
    */
}
