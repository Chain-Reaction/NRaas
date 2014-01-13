using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.InteractionsShared;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Objects.Seating;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.Store.Objects;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.WoohooerSpace.Interactions
{
    public class TakeShowerOutdoorEx : TakeShowerEx
    {
        static InteractionDefinition sOldSingleton;

        public override void OnPreLoad()
        {
            Tunings.Inject<ShowerOutdoor, ShowerOutdoor.TakeShower.Definition, Definition>(false);

            sOldSingleton = ShowerOutdoor.TakeShower.Singleton;
            ShowerOutdoor.TakeShower.Singleton = new Definition();
        }

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<ShowerOutdoor, ShowerOutdoor.TakeShower.Definition>(ShowerOutdoor.TakeShower.Singleton);

            // The other interactions are added by ShowerWoohoo itself
            interactions.Add<IShowerable>(ShowerWoohoo.SafeSingleton);
        }

        protected override Sim.ClothesChangeReason OutfitChoice
        {
            get
            {
                if (HavingWooHoo)
                {
                    return Sim.ClothesChangeReason.GoingToBathe;
                }
                else
                {
                    return Sim.ClothesChangeReason.GoingToSwim;
                }
            }
        }

        protected override bool EnforcePrivacy
        {
            get { return false; }
        }

        public new class Definition : TakeShowerEx.Definition
        {
            public Definition()
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new TakeShowerOutdoorEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, IShowerable target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, IShowerable target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return (!isAutonomous || ((a.Autonomy.Motives.GetValue(CommodityKind.Hygiene) < 100f) && !a.SimDescription.IsFrankenstein));
            }
        }
    }
}


