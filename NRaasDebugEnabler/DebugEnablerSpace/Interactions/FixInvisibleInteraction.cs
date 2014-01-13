using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Tasks;
using NRaas.DebugEnablerSpace.Interfaces;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.DebugEnablerSpace.Interactions
{
    public class FixInvisibleInteraction : DebugEnablerInteraction<GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override void AddPair(GameObject obj, List<InteractionObjectPair> list)
        {
            list.Add(new InteractionObjectPair(Singleton, obj));
        }

        private static bool Perform(SimDescription sim, bool force)
        {
            try
            {
                FixInvisibleTask.Approach approach = FixInvisibleTask.Perform(sim, force);
                if (approach != FixInvisibleTask.Approach.None)
                {
                    DebugEnabler.Notify(sim.CreatedSim, Common.Localize("FixInvisibleSims:Success", sim.IsFemale, new object[] { Common.Localize("FixInvisibleSims:" + approach), sim }));
                }
                return true;
            }
            catch(Exception e)
            {
                Common.Exception(sim, e);
                return false;
            }
        }

        public override bool Run()
        {
            try
            {
                if (Target is Sim)
                {
                    Perform((Target as Sim).SimDescription, true);
                }
                else
                {
                    foreach (SimDescription sim in SimListing.GetResidents(false).Values)
                    {
                        if (SimTypes.InServicePool(sim)) continue;

                        Perform(sim, false);
                    }
                }
            }
            catch (Exception exception)
            {
                Common.Exception(Actor, Target, exception);
            }
            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : DebugEnablerDefinition<FixInvisibleInteraction>
        {
            public override string GetInteractionName(IActor a, GameObject target, InteractionObjectPair interaction)
            {
                return Common.Localize("FixInvisibleSims:MenuName");
            }
        }
    }
}