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
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Alchemy;
using Sims3.Gameplay.Objects.Decorations.Mimics;
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
    public class CastFireBlastOnStatueEx : MagicWand.CastFireBlastOnStatue, IMagicalInteraction, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        CastSpellOnStatueProxy<CastFireBlastOnStatueEx> mProxy = new CastSpellOnStatueProxy<CastFireBlastOnStatueEx>();

        public void OnPreLoad()
        {
            Tunings.Inject<SculptureFrozenSim, MagicWand.CastFireBlastOnStatue.Definition, Definition>(false);

            sOldSingleton = MagicWand.CastFireBlastOnStatue.Singleton;
            MagicWand.CastFireBlastOnStatue.Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<SculptureFrozenSim, MagicWand.CastFireBlastOnStatue.Definition>(MagicWand.CastFireBlastOnStatue.Singleton);
        }

        public void ShowEpicFailVfx(StateMachineClient sender, IEvent evt)
        {
            ShowEpicFailVfxCB(sender, evt);
        }

        public void ShowFailVfx(StateMachineClient sender, IEvent evt)
        {
            ShowFailVfxCB(sender, evt);
        }

        public void ShowSuccessVfx(StateMachineClient sender, IEvent evt)
        {
            ShowSuccessVfxCB(sender, evt);
        }

        public MagicWand Wand
        {
            set { mWand = value; }
        }

        public override bool Run()
        {
            try
            {
                return mProxy.Run(this);
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

        public new class Definition : MagicWand.CastFireBlastOnStatue.Definition, IMagicalDefinition
        {
            static PersistedSettings.SpellSettings sSettings;

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CastFireBlastOnStatueEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, ISculptureFrozenSim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, ISculptureFrozenSim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                MagicControl control = MagicControl.GetBestControl(a, this);
                if (control == null)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("No Control");
                    return false;
                }

                return CastSpellOnStatueProxy<CastFireBlastOnStatueEx>.CommonSpellOnTest(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }

            public MagicControl IntendedControl
            {
                get { return WitchControl.sControl; }
            }

            public PersistedSettings.SpellSettings DefaultSettings
            {
                get
                {
                    return new PersistedSettings.SpellSettings(OccultTypes.Witch, MagicWand.kSpellLevels[0x4], CastFireBlastEx.kMotiveDrain, 0);
                }
            }

            public PersistedSettings.SpellSettings SpellSettings
            {
                get
                {
                    return sSettings;
                }
                set
                {
                    sSettings = value;
                }
            }
        }
    }
}
