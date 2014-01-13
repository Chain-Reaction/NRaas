using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class SnorkelSwimHere : SwimHere, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<Terrain, SwimHere.SnorkelDefinition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.SetFlags(Availability.FlagField.DisallowedIfPregnant, false);
            }

            tuning = Tunings.GetTuning<Terrain, SwimHere.Definition>();
            if (tuning != null)
            {
                tuning.Availability.SetFlags(Availability.FlagField.DisallowedIfPregnant, false);
            }

            sOldSingleton = SnorkelSingleton;
            SnorkelSingleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Terrain, SwimHere.SnorkelDefinition>(SnorkelSingleton);
        }

        public override void Init(ref InteractionInstanceParameters parameters)
        {
            base.Init(ref parameters);

            if (mPriority.Value < 0)
            {
                RaisePriority();
            }
        }

        public override bool ShouldReplace(InteractionInstance interaction)
        {
            // Allows stacking
            return false;
        }

        public new class Definition : SwimHere.SnorkelDefinition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new SnorkelSwimHere();
                result.Init(ref parameters);
                return result;
            }
            
            public override string GetInteractionName(Sim actor, Terrain target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                Sim actor = parameters.Actor as Sim;

                if (actor.MoodManager.StressInteractionTest(ref greyedOutTooltipCallback) || actor.Motives.StressInteractionTest(ref greyedOutTooltipCallback, null))
                {
                    return InteractionTestResult.GenericFail;
                }

                SwimHere.SwimHereType type = mSwimHereType;
                try
                {
                    // Bypass some world restrictions encoded into the snorkel check
                    mSwimHereType = SwimHere.SwimHereType.None;

                    return base.Test(ref parameters, ref greyedOutTooltipCallback);
                }
                finally
                {
                    mSwimHereType = type;
                }
            }
        }
    }
}
