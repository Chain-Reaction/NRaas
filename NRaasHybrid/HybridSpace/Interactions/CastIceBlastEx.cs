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
    public class CastIceBlastEx : MagicWand.CastIceBlast, IMagicalInteraction, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        CastSpellProxy<CastIceBlastEx> mProxy = new CastSpellProxy<CastIceBlastEx>();

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, MagicWand.CastIceBlast.Definition, Definition>(false);

            sOldSingleton = MagicWand.CastIceBlast.Singleton;
            MagicWand.CastIceBlast.Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sim, MagicWand.CastIceBlast.Definition>(MagicWand.CastIceBlast.Singleton);
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

        public new class Definition : MagicWand.CastIceBlast.Definition, IMagicalDefinition
        {
            static PersistedSettings.SpellSettings sSettings;

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CastIceBlastEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (a == target)
                {
                    return false;
                }

                MagicControl control = MagicControl.GetBestControl(a, this);
                if (control == null)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("No Control");
                    return false;
                }

                if (target.BuffManager.HasElement(BuffNames.SoCold))
                {
                    return false;
                }
                /*
                if (target.SimDescription.IsPregnant)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString(target.IsFemale, "Gameplay/Actors/Sim:PregnantFailure", new object[0x0]));
                    return false;
                }*/
                return CastSpellEx.CommonSpellTests(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }

            public MagicControl IntendedControl
            {
                get { return WitchControl.sControl; }
            }

            public PersistedSettings.SpellSettings DefaultSettings
            {
                get
                {
                    return new PersistedSettings.SpellSettings(OccultTypes.Witch, MagicWand.kSpellLevels[0x5], CastIceBlastEx.kMotiveDrain, 0);
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
