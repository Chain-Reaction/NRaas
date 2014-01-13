using NRaas.CareerSpace.Booters;
using NRaas.Gameplay.Rewards;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Rewards;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CareerSpace.SelfEmployment
{
    public class ChessPlayer : Common.IWorldLoadFinished
    {
        public void OnWorldLoadFinished()
        {
            new Common.DelayedEventListener(EventTypeId.kGameWon, OnChessWon);
            new Common.DelayedEventListener(EventTypeId.kPlayedRankedChessMatch, OnRankedChessPlayed);
        }

        public static void OnChessWon(Event e)
        {
            Sim actor = e.Actor as Sim;
            if (actor == null)
            {
                return;
            }

            if ((actor.Household == null) || (actor.Household.IsSpecialHousehold))
            {
                return;
            }

            Sim target = e.TargetObject as Sim;
            if (target == null)
            {
                return;
            }

            if (target.SimDescription.ChildOrBelow)
            {
                return;
            }

            if (actor.Household == target.Household)
            {
                return;
            }

            if (actor.InteractionQueue == null)
            {
                return;
            }

            InteractionInstance instance = actor.InteractionQueue.GetCurrentInteraction();
            if (instance == null)
            {
                return;
            }

            ChessTable.PracticeChess practice = instance as ChessTable.PracticeChess;
            if (practice == null)
            {
                return;
            }

            if (practice.Target == null)
            {
                return;
            }

            if (practice.Target.mbPlayingForChessRank)
            {
                return;
            }

            int actorCash = SkillBasedCareerBooter.GetCareerPay(actor, SkillNames.Chess);
            int targetCash = SkillBasedCareerBooter.GetCareerPay(target, SkillNames.Chess);

            if (actorCash < targetCash)
            {
                actorCash = targetCash;
            }

            if (actorCash == 0)
            {
                return;
            }

            if (actor.SkillManager == null)
            {
                return;
            }

            int diff = actor.SkillManager.GetSkillLevel(SkillNames.Chess);

            if (target.SkillManager != null)
            {
                diff -= target.SkillManager.GetSkillLevel(SkillNames.Chess);
            }

            if (diff < 0)
            {
                diff = -diff;
            }

            if (diff != 0)
            {
                actorCash /= diff;
            }

            if (actorCash > target.FamilyFunds)
            {
                actorCash = target.FamilyFunds;
                if (actorCash == 0)
                {
                    Common.Notify(Common.Localize("ChessPlayer:UnableToPay"), target.ObjectId);

                    if (actor.InteractionQueue != null)
                    {
                        actor.InteractionQueue.CancelAllInteractions();
                    }
                    return;                        
                }
            }

            actor.ModifyFunds(actorCash);

            target.ModifyFunds(-actorCash);

            SkillBasedCareerBooter.UpdateExperience(actor, SkillNames.Chess, actorCash);
        }

        public static void OnRankedChessPlayed(Event e)
        {
            Sim actor = e.Actor as Sim;
            if (actor == null)
            {
                return;
            }

            if ((actor.Household == null) || (actor.Household.IsSpecialHousehold))
            {
                return;
            }

            PlayedChessEvent pEvent = e as PlayedChessEvent;

            if (!pEvent.WonGame)
            {
                return;
            }

            int cash = SkillBasedCareerBooter.GetCareerPay(actor, SkillNames.Chess);

            cash *= 4;

            actor.ModifyFunds(cash);

            SkillBasedCareerBooter.UpdateExperience(actor, SkillNames.Chess, cash);
        }
    }
}
