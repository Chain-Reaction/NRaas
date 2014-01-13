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
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Interactions
{
    public class BoatHereEx : BoatHere, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Terrain, BoatHere.Definition, Definition>(false);

            sOldSingleton = BoatSingleton;
            BoatSingleton = new Definition();

            // Note that skinny dip interactions are replaced by [Woohooer]
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Terrain, BoatHere.Definition>(BoatSingleton);
        }

        public override void Init(ref InteractionInstanceParameters parameters)
        {
            base.Init(ref parameters);

            if (GoHere.Settings.mAllowGoHereStack)
            {
                if (mPriority.Value < 0)
                {
                    RaisePriority();
                }
            }
        }

        public override bool ShouldReplace(InteractionInstance interaction)
        {
            if (GoHere.Settings.mAllowGoHereStack)
            {
                return false;
            }
            else
            {
                return base.ShouldReplace(interaction);
            }
        }

        private new class Definition : BoatHere.Definition
        {
            public Definition()
            { }
            
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new BoatHereEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Terrain target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
