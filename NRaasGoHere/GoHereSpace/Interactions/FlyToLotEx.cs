using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Interactions
{
    public class FlyToLotEx : Jetpack.FlyToLot, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Jetpack, Jetpack.FlyToLot.Definition, Definition>(false);

            sOldSingleton = Singleton as InteractionDefinition;
            Singleton = new Definition();

            // Note that skinny dip interactions are replaced by [Woohooer]
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Jetpack, Jetpack.FlyToLot.Definition>(Singleton as InteractionDefinition);
        }

        private new class Definition : Jetpack.FlyToLot.Definition
        {
            public Definition()
            { }
            public Definition(bool flyToCommunityLot)
                : base(flyToCommunityLot)
            { }
            public Definition(bool flyToCommunityLot, bool isInJetpackPieMenu)
                : base(flyToCommunityLot, isInJetpackPieMenu)
            { }
         
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new FlyToLotEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Jetpack target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, Jetpack target, List<InteractionObjectPair> results)
            {
                results.Add(new InteractionObjectPair(new Definition(false), target));
                results.Add(new InteractionObjectPair(new Definition(true), target));
            }

            public override bool Test(Sim actor, Jetpack target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!Jetpack.IsAllowedToUseJetpack(actor))
                {
                    return false;
                }
                if (actor.GetActiveJetpack() == null)
                {
                    return false;
                }
                if (actor.GetActiveJetpack() != target)
                {
                    return false;
                }

                /*
                if (actor.SimDescription.IsPregnant)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString(actor.IsFemale, "Gameplay/Actors/Sim:PregnantFailure", new object[0x0]));
                    return false;
                }
                */
                return true;

            }
        }
    }
}
