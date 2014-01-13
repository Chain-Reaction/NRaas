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
    public class FlyAroundEx : Jetpack.FlyAround, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, Jetpack.FlyAround.Definition, Definition>(false);

            sOldSingleton = Singleton as InteractionDefinition;
            Singleton = new Definition();

            // Note that skinny dip interactions are replaced by [Woohooer]
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sim, Jetpack.FlyAround.Definition>(Singleton as InteractionDefinition);
        }

        private new class Definition : Jetpack.FlyAround.Definition
        {
            public Definition()
            { }
            
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new FlyAroundEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!Jetpack.IsAllowedToUseJetpack(a))
                {
                    return false;
                }
                if (isAutonomous)
                {
                    if (!a.Inventory.ContainsType(typeof(Jetpack), 0x1))
                    {
                        return false;
                    }
                }
                else if (a.GetActiveJetpack() == null)
                {
                    return false;
                }

                /*
                if (a.SimDescription.IsVisuallyPregnant)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString(a.SimDescription.IsFemale, "Gameplay/ActorSystems/OccultImaginaryFriend:ImaginaryFriendModeDisabledPregnancy", new object[] { a.SimDescription }));
                    return false;
                }
                */

                SocialJig socialjig = null;
                if (!Jetpack.CheckSpaceForFlyAroundJig(a, target, ref socialjig, true, true))
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString("Gameplay/Objects/EP11/Jetpack:NotEnoughSpace", new object[] { target }));
                    return false;
                }
                return true;
            }
        }
    }
}
