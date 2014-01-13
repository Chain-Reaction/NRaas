using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Replacers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Interactions
{
    public class CallThrowPartyEx : Phone.CallThrowParty, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Phone, Phone.CallThrowParty.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Phone, Phone.CallThrowParty.Definition>(Singleton);
        }

        public override Phone.Call.ConversationBehavior OnCallConnected()
        {
            try
            {
                if (Actor.LotHome == null)
                {
                    return Phone.Call.ConversationBehavior.ShakeHead;
                }

                using (Sims.Basic.Party.ThrowHome.SuppressCriteria suppress = new Sims.Basic.Party.ThrowHome.SuppressCriteria())
                {
                    if (new Sims.Basic.Party.ThrowHome().Perform(new GameHitParameters<SimDescriptionObject>(Actor, null, GameObjectHit.NoHit)) != CommonSpace.Options.OptionResult.Failure)
                    {
                        return ConversationBehavior.TalkBriefly;
                    }
                    else
                    {
                        return ConversationBehavior.JustHangUp;
                    }
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return ConversationBehavior.JustHangUp;
            }
        }

        public new class Definition : Phone.CallThrowParty.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CallThrowPartyEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Phone target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
