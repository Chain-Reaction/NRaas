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
    public class UseJetpackProxyEx : Jetpack.UseJetpackProxy, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, Jetpack.UseJetpackProxy.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();

            // Note that skinny dip interactions are replaced by [Woohooer]
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sim, Jetpack.UseJetpackProxy.Definition>(Singleton);
        }

        private new class Definition : Jetpack.UseJetpackProxy.Definition
        {
            public Definition()
            { }
            
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new UseJetpackProxyEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim Actor, Sim Target, List<InteractionObjectPair> results)
            {
                if (Actor == Target)
                {
                    Jetpack activeJetpack = Actor.GetActiveJetpack();
                    if (activeJetpack != null)
                    {
                        // Custom
                        results.Add(new InteractionObjectPair(new FlyToLotEx.Definition(true, true), activeJetpack));
                        results.Add(new InteractionObjectPair(new FlyToLotEx.Definition(false, true), activeJetpack));
                    }
                    if ((activeJetpack == null) && (Actor.Inventory != null))
                    {
                        activeJetpack = Actor.Inventory.Find<Jetpack>();
                    }
                    if (activeJetpack != null)
                    {
                        results.Add(new InteractionObjectPair(new Jetpack.UseJetpack.Definition(true), activeJetpack));
                    }
                }
            }
        }
    }
}
