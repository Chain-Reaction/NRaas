using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Interactions;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RelationshipPanelSpace.Interactions
{
    public class CallInviteOverForeignVisitorsFromRelationPanelEx : Phone.CallInviteOverForeignVisitorsFromRelationPanel, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Phone, Phone.CallInviteOverForeignVisitorsFromRelationPanel.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition ();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Phone, Phone.CallInviteOverForeignVisitorsFromRelationPanel.Definition>(Singleton);
        }

        public bool PushCallInviteOverForeignVisitors(Phone ths, Sim caller, IMiniSimDescription receiver)
        {
            caller.InteractionQueue.PushAsContinuation(new CallInviteOverForeignVisitorsEx.Definition(receiver), ths, false);
            return true;
        }

        public override bool Run()
        {
            try
            {
                return (((InteractionDefinition as Definition).mSimToCall != null) && PushCallInviteOverForeignVisitors(Target, Actor, (InteractionDefinition as Definition).mSimToCall));
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        public new class Definition : Phone.CallInviteOverForeignVisitorsFromRelationPanel.Definition
        {
            public Definition()
            { }
            public Definition(IMiniSimDescription simToCall)
                : base(simToCall)
            { }

            public override string GetInteractionName(Sim actor, Phone target, InteractionObjectPair iop)
            {
                bool isFemale = false;
                if (mSimToCall != null)
                {
                    isFemale = mSimToCall.IsFemale;
                }

                return Common.LocalizeEAString(isFemale, "Gameplay/Objects/Electronics/Phone/CallInviteOverForeignVisitors:InteractionName");
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CallInviteOverForeignVisitorsFromRelationPanelEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}
