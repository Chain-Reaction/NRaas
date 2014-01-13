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
    public class GoHereEx : GoHereBase
    {
        public static InteractionDefinition sOldSameLotSingleton;

        static InteractionDefinition sOldOtherLotSingleton;
        static InteractionDefinition sOldOtherLotWithCarriedChildSingleton;

        public override void OnPreLoad()
        {
            Tunings.Inject<Terrain, Terrain.GoHere.OtherLotDefinition, OtherLotDefinition>(false);
            Tunings.Inject<Terrain, Terrain.GoHere.OtherLotWithCarriedChildDefinition, OtherLotWithCarriedChildDefinition>(false);
            Tunings.Inject<Terrain, Terrain.GoHere.SameLotDefinition, SameLotDefinition>(false);

            sOldOtherLotSingleton = OtherLotSingleton;
            OtherLotSingleton = new OtherLotDefinition();

            sOldOtherLotWithCarriedChildSingleton = OtherLotWithCarriedChildSingleton;
            OtherLotWithCarriedChildSingleton = new OtherLotWithCarriedChildDefinition();

            sOldSameLotSingleton = SameLotSingleton;
            SameLotSingleton = new SameLotDefinition();
        }

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Terrain, Terrain.GoHere.OtherLotDefinition>(OtherLotSingleton);
            interactions.Replace<Terrain, Terrain.GoHere.OtherLotWithCarriedChildDefinition>(OtherLotWithCarriedChildSingleton);
            interactions.Replace<Terrain, Terrain.GoHere.SameLotDefinition>(SameLotSingleton);
        }

        private new class OtherLotDefinition : Terrain.GoHere.OtherLotDefinition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new GoHereEx();
                na.Init(ref parameters);
                return na;
            }
        }

        private new class OtherLotWithCarriedChildDefinition : Terrain.GoHere.OtherLotWithCarriedChildDefinition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new GoHereEx();
                na.Init(ref parameters);
                return na;
            }
        }

        public new class SameLotDefinition : Terrain.GoHere.SameLotDefinition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new GoHereEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(IActor actor, Terrain target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSameLotSingleton, target));
            }
        }
    }
}
