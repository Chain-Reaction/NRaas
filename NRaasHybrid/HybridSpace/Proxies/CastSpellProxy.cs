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
    public class CastSpellProxy<T> : InteractionProxy<T, Sim>
        where T : MagicWand.CastSpell, IMagicalInteraction
    {
        MagicWand.BeTargetOfSpell mEntry = null;

        protected override bool SetupAnimation(T ths, MagicControl control, bool twoPerson)
        {
            if (twoPerson)
            {
                ths.EnterStateMachine("SpellcastingSolo", "Enter", "x");
            }
            else
            {
                ths.EnterStateMachine("SpellcastingSocial", "Enter", "x");
                ths.SetActor("y", ths.Target);
            }

            return base.SetupAnimation(ths, control, twoPerson);
        }

        protected override bool EpicFailureAllowed(T ths)
        {
            return ths.EpicFailureAllowed();
        }

        protected override bool InitialPrep(T ths, bool twoPerson, IMagicalDefinition definition, MagicControl control, bool spellCastingSucceeded, bool spellCastingEpiclyFailed)
        {
            ths.mSpellCastingSucceeded = spellCastingSucceeded;
            ths.mSpellCastingEpiclyFailed = spellCastingEpiclyFailed;

            if (twoPerson)
            {
                ths.mJig = GlobalFunctions.CreateObjectOutOfWorld("practiceSpell_jig", ProductVersion.EP7) as SocialJig;
                if (ths.mJig == null)
                {
                    return false;
                }
                ths.mJig.RegisterParticipants(ths.Actor, null);
            }
            else if (!ths.Actor.HasExitReason())
            {
                mEntry = MagicWand.BeTargetOfSpell.Singleton.CreateInstance(ths.Actor, ths.Target, ths.GetPriority(), false, false) as MagicWand.BeTargetOfSpell;
                mEntry.LinkedInteractionInstance = ths as InteractionInstance;
                mEntry.Caster = ths.Actor;
                ths.Target.InteractionQueue.AddNext(mEntry);
                if ((ths.Actor.GetDistanceToObjectSquared(ths.Target) > (MagicWand.CastSpell.kCloseEnoughToPlaceJig * MagicWand.CastSpell.kCloseEnoughToPlaceJig)) && !ths.Actor.RouteToPointRadius(ths.Target.Position, MagicWand.CastSpell.kCloseEnoughToPlaceJig))
                {
                    return false;
                }
                ths.mJig = GlobalFunctions.CreateObjectOutOfWorld("castSpell_jig", ProductVersion.EP7) as SocialJig;
                ths.mJig.RegisterParticipants(ths.Actor, ths.Target);
            }
            else
            {
                return false;
            }

            ths.mJig.SetOpacity(0f, 0f);
            Vector3 position = ths.Actor.Position;
            Vector3 forwardVector = ths.Actor.ForwardVector;
            if (!GlobalFunctions.FindGoodLocationNearby(ths.mJig, ref position, ref forwardVector))
            {
                return false;
            }

            ths.mJig.SetPosition(position);
            ths.mJig.SetForward(forwardVector);
            ths.mJig.AddToWorld();
            if (!twoPerson)
            {
                long currentTicks = SimClock.CurrentTicks;

                mEntry.Jig = ths.mJig as SocialJigTwoPerson;
                while (!ths.mJig.RouteComplete && !ths.Actor.HasExitReason(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached)))
                {
                    if ((SimClock.ConvertFromTicks(SimClock.CurrentTicks - currentTicks, TimeUnit.Minutes) > 15f) && (ths.Actor.GetDistanceToObject(ths.mJig) < MagicWand.CastSpell.kDistanceToForceRouteAway))
                    {
                        ths.Actor.RouteToObjectRadialRange(ths.mJig, MagicWand.CastSpell.kDistanceToForceRouteAway, MagicWand.CastSpell.kDistanceToForceRouteAway);
                    }
                    SpeedTrap.Sleep(0x0);
                }
            }

            if (!ths.Actor.DoRoute(ths.mJig.RouteToJigA(ths.Actor)))
            {
                return false;
            }

            mSucceeded = ths.AdditionalInteractionPreparation();
            if (!mSucceeded)
            {
                return false;
            }

            return true;
        }

        protected override bool PerformResults(T ths, string epicJazzName, IMagicalDefinition definition, MagicControl control, bool spellCastingSucceeded, bool spellCastingEpiclyFailed)
        {
            ths.AnimateSim(ths.JazzStateName);

            return base.PerformResults(ths, "Epic", definition, control, spellCastingSucceeded, spellCastingEpiclyFailed);
        }
    }
}
