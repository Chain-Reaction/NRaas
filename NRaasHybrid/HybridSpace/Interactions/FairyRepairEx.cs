using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.HybridSpace.Interfaces;
using NRaas.HybridSpace.MagicControls;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
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
    public class FairyRepairEx : RepairableComponent.FairyRepair, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<IGameObject, RepairableComponent.FairyRepair.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.RequiredTraits.Remove(TraitNames.FairyHiddenTrait);
            }

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<IGameObject, RepairableComponent.FairyRepair.Definition>(Singleton);
        }

        public class Definition : RepairableComponent.FairyRepair.Definition, IMagicalDefinition
        {
            static PersistedSettings.SpellSettings sSettings;

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new FairyRepairEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, IGameObject target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, IGameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                IFairyRepairable repairable = target as IFairyRepairable;
                if (isAutonomous)
                {
                    return false;
                }

                if ((target.Repairable == null) || (!target.Repairable.Broken))
                {
                    return false;
                }

                MagicControl control = MagicControl.GetBestControl(a, this);
                if (control == null)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("No Control");
                    return false;
                }

                return ((repairable != null) && repairable.repairCheck(a, target, ref greyedOutTooltipCallback));
            }

            public MagicControl IntendedControl
            {
                get { return FairyControl.sControl; }
            }

            public PersistedSettings.SpellSettings DefaultSettings
            {
                get
                {
                    return new PersistedSettings.SpellSettings(OccultTypes.Fairy, kRequiredFairySkillLevel, 50, 0);
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
