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
    public class Trainer : Common.IWorldLoadFinished
    {
        static Dictionary<ulong, float> sAccrued = new Dictionary<ulong, float>();

        public void OnWorldLoadFinished()
        {
            sAccrued.Clear();

            new Common.DelayedEventListener(EventTypeId.kSkillLevelUp, OnLeveled);
            new Common.DelayedEventListener(EventTypeId.kTrainedSim, OnTrained);
        }

        public static float GetPay(Sim sim)
        {
            float cashPerHour = SkillBasedCareerBooter.GetCareerPay(sim, SkillNames.Athletic);

            if ((cashPerHour == 0) && (!sim.IsSelectable))
            {
                cashPerHour = 10;
            }

            cashPerHour *= SkillBasedCareerBooter.GetLevelFactor(sim, SkillNames.Athletic);

            return cashPerHour;
        }

        public static void OnLeveled(Event e)
        {
            HasGuidEvent<SkillNames> skillEvent = e as HasGuidEvent<SkillNames>;
            if (skillEvent == null)
            {
                return;
            }

            if (skillEvent.Guid != SkillNames.Athletic)
            {
                return;
            }

            Sim actor = e.Actor as Sim;
            if (actor == null)
            {
                return;
            }

            if (actor.InteractionQueue == null)
            {
                return;
            }

            InteractionInstance interaction = actor.InteractionQueue.GetCurrentInteraction();
            if (interaction == null)
            {
                return;
            }

            AthleticGameObject gameobj = interaction.Target as AthleticGameObject;
            if (gameobj == null)
            {
                return;
            }

            Sim trainer = gameobj.OtherActor(actor);
            if (trainer == null)
            {
                return;
            }

            if (trainer.Household == actor.Household)
            {
                return;
            }

            float cash = GetPay(trainer);
            if (cash == 0)
            {
                return;
            }

            cash *= 2;

            if (sAccrued.ContainsKey(actor.SimDescription.SimDescriptionId))
            {
                sAccrued[actor.SimDescription.SimDescriptionId] += cash;
            }
            else
            {
                sAccrued.Add(actor.SimDescription.SimDescriptionId, cash);
            }
        }

        public static void OnTrained(Event e)
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

            if (actor.InteractionQueue == null)
            {
                return;
            }

            InteractionInstance interaction = actor.InteractionQueue.GetCurrentInteraction();
            if (interaction == null)
            {
                return;
            }

            if (!(interaction.Target is AthleticGameObject))
            {
                return;
            }

            TimePassedEvent pEvent = e as TimePassedEvent;

            Sim target = e.TargetObject as Sim;
            if (target == null)
            {
                return;
            }

            if (actor.Household == target.Household)
            {
                return;
            }

            float cash = GetPay(actor) * pEvent.mIncrement;

            if (sAccrued.ContainsKey(actor.SimDescription.SimDescriptionId))
            {
                cash += sAccrued[actor.SimDescription.SimDescriptionId];
            }

            if (cash >= target.FamilyFunds)
            {
                cash = target.FamilyFunds;
                if (cash == 0)
                {
                    if (actor.InteractionQueue != null)
                    {
                        actor.InteractionQueue.CancelAllInteractions();
                    }

                    if (actor.IsSelectable)
                    {
                        Common.Notify(Common.Localize("Trainer:UnableToPay"), actor.ObjectId);
                    }
                    return;
                }
            }

            sAccrued.Remove(actor.SimDescription.SimDescriptionId);

            if (cash < 1)
            {
                sAccrued.Add(actor.SimDescription.SimDescriptionId, cash);
            }
            else
            {
                target.ModifyFunds(-(int)cash);

                actor.ModifyFunds((int)cash);

                SkillBasedCareerBooter.UpdateExperience(actor, SkillNames.Athletic, (int)cash);
            }
        }
    }
}
