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
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Interactions
{
    public class RideHereEx : Terrain.RideHere, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;
        static InteractionDefinition sOldSkipGoHereTestSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Terrain, Terrain.RideHere.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();

            sOldSkipGoHereTestSingleton = SkipGoHereTestsSingleton;
            SkipGoHereTestsSingleton = new Definition(false);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Terrain, Terrain.RideHere.Definition>(Singleton);
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

        private new class Definition : Terrain.RideHere.Definition
        {
            public Definition()
            { }
            public Definition(bool useGoHereTests)
                : base(useGoHereTests)
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new RideHereEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}
