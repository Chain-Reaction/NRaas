using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
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

namespace NRaas.HybridSpace.Proxies
{
    public abstract class InteractionProxy<T, TARGET> : IInteractionProxy
        where T : InteractionInstance<Sim, TARGET>, IMagicalSubInteraction
        where TARGET : class, IGameObject
    {
        protected bool mSucceeded = true;

        MagicWand mWand;
        bool mWandCreated = false;

        protected abstract bool InitialPrep(T ths, bool twoPerson, IMagicalDefinition definition, MagicControl control, bool spellCastingSucceeded, bool spellCastingEpiclyFailed);

        public MagicWand Wand
        {
            get { return mWand; }
        }

        protected virtual bool EpicFailureAllowed(T ths)
        {
            return true;
        }

        protected virtual bool SeparateCalls
        {
            get { return true; }
        }

        public void Cleanup()
        {
            if ((mWandCreated) && (mWand != null))
            {
                mWand.Destroy();
            }
        }

        protected virtual bool SetupAnimation(T ths, MagicControl control, bool twoPerson)
        {
            if (mWand is MagicHands)
            {
                ths.SetParameter("noWand", true);
            }
            else
            {
                ths.SetParameter("noWand", false);
            }

            ths.SetActor("wand", mWand);
            ths.SetParameter("isSkilled", control.GetSkillLevel(ths.Actor.SimDescription) >= MagicWand.kExpertLevel);

            IMagicalInteraction interaction = ths as IMagicalInteraction;
            if (interaction != null)
            {
                ths.AddOneShotScriptEventHandler(0x65, interaction.ShowSuccessVfx);
                ths.AddOneShotScriptEventHandler(0x66, interaction.ShowFailVfx);
                ths.AddOneShotScriptEventHandler(0x67, interaction.ShowEpicFailVfx);
            }

            return true;
        }

        protected virtual bool PerformResults(T ths, string epicJazzName, IMagicalDefinition definition, MagicControl control, bool spellCastingSucceeded, bool spellCastingEpiclyFailed)
        {
            if (spellCastingSucceeded)
            {
                if (epicJazzName != "Epic") // CastSpell
                {
                    ths.AnimateSim("SuccessIdle");
                }
                ths.AnimateSim("Success");

                if (SeparateCalls)
                {
                    (ths as IMagicalInteraction).OnSpellSuccess();
                }
                return true;
            }
            else if (spellCastingEpiclyFailed)
            {
                ths.AnimateSim(epicJazzName);

                if (SeparateCalls)
                {
                    (ths as IMagicalInteraction).OnSpellEpicFailure();
                }
                return false;
            }
            else
            {
                ths.AnimateSim("Fail");
                return false;
            }
        }

        protected virtual bool IsFailure(Sim actor, MagicControl control, IMagicalDefinition definition, out bool epicFailure)
        {
            return control.IsFailure(actor, this, definition, out epicFailure);
        }

        public bool Run(T ths)
        {
            IMagicalDefinition definition = ths.InteractionDefinition as IMagicalDefinition;
            if (definition == null) return false;

            return Run(ths, definition);
        }
        public bool Run(T ths, IMagicalDefinition definition)
        {
            MagicControl control = MagicControl.GetBestControl(ths.Actor, definition);
            if (control == null) return false;

            mWand = control.InitialPrep(ths.Actor, definition, out mWandCreated);
            if (mWand == null) return false;

            mWand.PrepareForUse(ths.Actor);

            ths.Wand = mWand;

            bool twoPerson = (ths.Actor == ths.Target);
            if (!Hybrid.Settings.mEnforceTwoPersonAnimation)
            {
                twoPerson = false;
            }

            bool spellCastingEpiclyFailed;
            bool spellCastingSucceeded = !IsFailure(ths.Actor, control, definition, out spellCastingEpiclyFailed);

            if (!EpicFailureAllowed(ths))
            {
                spellCastingEpiclyFailed = false;
            }

            if (!InitialPrep(ths, twoPerson, definition, control, spellCastingSucceeded, spellCastingEpiclyFailed)) return false;

            ths.StandardEntry();
            ths.BeginCommodityUpdates();

            if (!SetupAnimation(ths, control, twoPerson)) return false;

            control.ConsumeMana(ths.Actor, this, definition);

            if (!PerformResults(ths, "EpicFail", definition, control, spellCastingSucceeded, spellCastingEpiclyFailed))
            {
                mSucceeded = false;
            }

            EventTracker.SendEvent(EventTypeId.kCastSpell, ths.Actor);
            ths.EndCommodityUpdates(mSucceeded);
            ths.StandardExit();
            return mSucceeded;
        }
    }
}
