using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.HybridSpace.Interactions
{
    public class WerewolfScratchObjectEx : Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<GameObject, OccultWerewolf.WerewolfScratchObject.Definition, Definition>(false);

            sOldSingleton = OccultWerewolf.WerewolfScratchObject.Singleton as InteractionDefinition;
            OccultWerewolf.WerewolfScratchObject.Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<GameObject, OccultWerewolf.WerewolfScratchObject.Definition>(OccultWerewolf.WerewolfScratchObject.Singleton as InteractionDefinition);
        }

        public class Definition : OccultWerewolf.WerewolfScratchObject.Definition
        {
            public Definition()
            { }

            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (a.SimDescription.Toddler)
                {
                    if (!a.SimDescription.HasTrait(TraitNames.LycanthropyHuman)) return false;
                }
                else
                {
                    if (!OccultTypeHelper.HasType(a, OccultTypes.Werewolf)) return false;
                }

                return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }
    }
}
