using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Interactions;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class SendWooHootyTextFromRelationPanelEx : Phone.SendWooHootyTextFromRelationPanel, Common.IAddInteraction
    {
        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Phone, Phone.SendWooHootyTextFromRelationPanel.Definition>(new Definition());
        }

        [DoesntRequireTuning]
        public new class Definition : Phone.SendWooHootyTextFromRelationPanel.Definition
        {
            public Definition()
            { }
            public Definition(IMiniSimDescription simToText)
                : base(simToText)
            { } 

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new SendWooHootyTextFromRelationPanelEx();
                na.Init(ref parameters);
                return na;
            }

            public override bool CanTextWithEachOther(Sim actor, IMiniSimDescription other, bool foreignText)
            {                
                if (!Phone.SendTextBase.SimCanTextWithActor(actor, other, foreignText))
                {                    
                    return false;
                }
                return SendWooHootyTextEx.SimCanTextWithActorEx(actor, other, foreignText);
            }

            public override InteractionDefinition MainDefinition
            {
                get
                {
                    return SendWooHootyTextEx.Singleton;
                }
            }
        }
    }
}

