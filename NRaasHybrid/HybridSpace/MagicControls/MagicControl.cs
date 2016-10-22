using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using NRaas.HybridSpace.Interfaces;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Alchemy;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.HybridSpace.MagicControls
{
    public abstract class MagicControl
    {
        static List<MagicControl> sControls = new List<MagicControl>();

        static MagicControl()
        {
            // Written in a specific order
            if (GameUtils.IsInstalled(ProductVersion.EP6))
            {
                sControls.Add(new GenieControl());
            }

            if (GameUtils.IsInstalled(ProductVersion.EP7))
            {
                sControls.Add(new FairyControl());
                sControls.Add(new WitchControl());
            }

            if (GameUtils.IsInstalled(ProductVersion.EP5))
            {
                sControls.Add(new UnicornControl());
            }
        }

        protected abstract OccultTypes Occult
        {
            get;
        }

        public abstract int GetSkillLevel(SimDescription sim);

        public abstract float GetMana(Sim sim);

        protected virtual bool IsAvailable(Sim sim, IMagicalDefinition definition)
        {            
            if ((Occult & definition.SpellSettings.mValidTypes) == OccultTypes.None) return false;

            if (!OccultTypeHelper.HasType(sim, Occult)) return false;
            
            if (GetSkillLevel(sim.SimDescription) < GetMinSkillLevel(definition)) return false;
            
            if (GetMana(sim) + definition.SpellSettings.mMinMana >= 100) return false;                      

            return true;
        }

        public static MagicControl GetBestControl(Sim sim, IMagicalDefinition definition)
        {
            MagicControl intendedControl = definition.IntendedControl;
            if (intendedControl != null)
            {
                if (intendedControl.IsAvailable(sim, definition)) return intendedControl;
            }

            foreach (MagicControl control2 in sControls)
            {
                if (control2.IsAvailable(sim, definition)) return control2;
            }

            return null;
        }

        public int GetMinSkillLevel(IMagicalDefinition definition)
        {
            return definition.SpellSettings.GetMinSkillLevel (definition.IntendedControl.Occult == Occult);
        }

        public abstract bool IsFailure(Sim actor, IInteractionProxy proxy, IMagicalDefinition definition, out bool epicFailure);

        public abstract void ConsumeMana(Sim actor, IInteractionProxy proxy, IMagicalDefinition definition);

        public virtual MagicWand InitialPrep(Sim actor, IMagicalDefinition definition, out bool wandCreated)
        {
            IGameObject toAdd = GlobalFunctions.CreateObject("magicHandsLHR", ProductVersion.EP7, Vector3.OutOfWorld, 0x0, Vector3.UnitZ, null, null);

            MagicWand wand = toAdd as MagicWand;
            if (wand == null)
            {
                toAdd.Destroy();
                wandCreated = false;
            }
            else
            {
                wandCreated = true;
            }

            return wand;
        }
    }
}
