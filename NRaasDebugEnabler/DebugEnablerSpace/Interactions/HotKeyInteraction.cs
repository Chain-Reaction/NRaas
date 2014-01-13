using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using NRaas.DebugEnablerSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.DebugEnablerSpace.Interactions
{
    public class HotKeyInteraction : BaseInteraction<GameObject>
    {
        public static InteractionDefinition Singleton = new Definition();

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<GameObject>(Singleton);
        }

        protected override bool Test(IActor a, GameObject target, GameObjectHit hit, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            return true;
        }

        public override bool Run()
        {
            try
            {
                List<InteractionObjectPair> interactions = DebugMenu.GetInteractions(Actor, Target, Hit);
                if (interactions == null) return false;

                for (int i = interactions.Count - 1; i >= 0; i--)
                {
                    if (!DebugEnabler.Settings.mInteractions.ContainsKey(interactions[i].InteractionDefinition.GetType()))
                    {
                        interactions.RemoveAt(i);
                    }
                }

                if (interactions.Count == 0)
                {
                    Common.Notify(Common.Localize("HotKeys:Failure"));
                    return true;
                }

                InteractionDefinitionOptionList.Perform(Actor, Hit, interactions);
                return true;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        public class Definition : BaseDefinition<HotKeyInteraction>
        {
            public Definition()
            { }

            public override string[] GetPath(bool isFemale)
            {
                return new string[] { "NRaas" };
            }

            public override string GetInteractionName(IActor actor, GameObject target, InteractionObjectPair iop)
            {
                return Common.Localize("HotKeys:MenuName");
            }

            public override bool Test(IActor actor, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (DebugEnabler.Settings.mInteractions.Count == 0) return false;

                return DebugEnabler.Settings.mEnabled;
            }
        }
    }
}
