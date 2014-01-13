using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Scoring;
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
    public class WooHooSocialInteraction : SocialInteractionA
    {
        public void PushWooHoo(Sim actor, Sim target)
        {
            try
            {
                ProxyDefinition definition = InteractionDefinition as ProxyDefinition;

                definition.PushWooHoo(actor, target);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception exception)
            {
                Common.Exception(actor, target, exception);
            }
       }

        [DoesntRequireTuning]
        public class ProxyDefinition : SocialInteractionA.Definition
        {
            IGameObject mObject;

            IWooHooProxyDefinition mDefinition;

            public ProxyDefinition(IWooHooProxyDefinition definition, Sim actor, IGameObject obj)
                : base(CommonWoohoo.GetSocialName(definition.GetStyle(null), actor), new string[0x0], null, false)
            {
                mDefinition = definition;

                mObject = obj;

                ChecksToSkip |= ActionData.ChecksToSkip.ProceduralTests;
            }

            public void PushWooHoo(Sim actor, Sim target)
            {
                mDefinition.PushWooHoo(actor, target, mObject);
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance instance = new WooHooSocialInteraction();
                instance.Init(ref parameters);
                return instance;
            }
        }
    }
}
