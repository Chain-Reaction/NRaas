using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.HybridSpace.Helpers;
using NRaas.HybridSpace.Interfaces;
using NRaas.HybridSpace.MagicControls;
using NRaas.HybridSpace.Proxies;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Alchemy;
using Sims3.Gameplay.Objects.Fireplaces;
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
    public class PutOutFireInteractionEx : Fireplace.PutOutFireInteraction, IMagicalInteraction, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        Proxy mProxy = new Proxy();

        public void OnPreLoad()
        {
            Tunings.Inject<Fireplace, Fireplace.PutOutFireInteraction.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Fireplace, Fireplace.PutOutFireInteraction.Definition>(Singleton);
        }

        public void OnSpellEpicFailure()
        { }

        public void OnSpellSuccess()
        { }

        public void ShowEpicFailVfx(StateMachineClient sender, IEvent evt)
        { }

        public void ShowFailVfx(StateMachineClient sender, IEvent evt)
        { }

        public MagicWand Wand
        {
            set { mWand = value; }
        }

        public override bool Run()
        {
            try
            {
                if (!Target.RouteToFireplace(Actor))
                {
                    return false;
                }
                
                if (!Target.mLit)
                {
                    return false;
                }
                
                if (!Target.WaitToPutOut(Actor))
                {
                    return false;
                }

                StandardEntry();
                BeginCommodityUpdates();
                Target.EnableFootprintAndPushSims(Target.FootprintPathingHash, Actor);

                MagicControl control = MagicControl.GetBestControl(Actor, CastIceBlastEx.Singleton as IMagicalDefinition);
                if (control != null)
                {
                    mProxy.Run(this, CastIceBlastEx.Singleton as IMagicalDefinition);

                    mWand.FinishUsing(Actor);
                }
                else
                {
                    StateMachineClient stateMachine = Target.GetStateMachine(this, "Enter");
                    stateMachine.SetParameter("HaveRemote", Target.TuningFireplace.HasRemote);
                    stateMachine.SetParameter("CheapVersion", Target.UseCheapAnimations);
                    stateMachine.AddOneShotScriptEventHandler(0x65, StopFireCallback);
                    stateMachine.RequestState("x", "ExtinguishFire");
                    stateMachine.RequestState("x", "Exit");
                }

                TraitFunctions.CheckForNeuroticAnxiety(Actor, TraitFunctions.NeuroticTraitAnxietyType.Fireplace);
                EndCommodityUpdates(true);
                StandardExit();
                return true;
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

        public override void Cleanup()
        {
            base.Cleanup();

            mProxy.Cleanup();
        }

        public class Proxy : FireIceBlastProxy<PutOutFireInteractionEx, Fireplace>
        {
            protected override bool InitialPrep(PutOutFireInteractionEx ths, bool twoPerson, IMagicalDefinition definition, MagicControl control, bool spellCastingSucceeded, bool spellCastingEpiclyFailed)
            {
                return true;
            }

            protected override bool IsFailure(Sim actor, MagicControl control, IMagicalDefinition definition, out bool epicFailure)
            {
                epicFailure = false;
                return false;
            }
        }

        public new class Definition : Fireplace.PutOutFireInteraction.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new PutOutFireInteractionEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Fireplace target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
