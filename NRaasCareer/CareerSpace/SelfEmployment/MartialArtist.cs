using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Interactions;
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
    public class MartialArtist : Common.IWorldLoadFinished
    {
        static Dictionary<ulong, Dictionary<ulong, float>> sAccrued = new Dictionary<ulong, Dictionary<ulong, float>>();

        public void OnWorldLoadFinished()
        {
            sAccrued.Clear();

            new Common.DelayedEventListener(EventTypeId.kSparred, OnSparred);
            new Common.DelayedEventListener(EventTypeId.kSimWonSparTournamentMatch, OnWonTournament);

            new Common.DelayedEventListener(EventTypeId.kSkillLevelUp, OnLeveled);
            new Common.DelayedEventListener(EventTypeId.kTrainedSim, OnTrained);
            new Common.DelayedEventListener(EventTypeId.kMeditated, OnMeditated);
        }

        public static float GetPay(Sim sim)
        {
            float cashPerHour = SkillBasedCareerBooter.GetCareerPay(sim, SkillNames.MartialArts);

            if ((cashPerHour == 0) && (!sim.IsSelectable))
            {
                cashPerHour = 25;
            }

            cashPerHour /= 5f; // Value normally used for tournament pay

            cashPerHour *= SkillBasedCareerBooter.GetLevelFactor(sim, SkillNames.MartialArts);

            return cashPerHour;
        }

        protected static void AddAccrued(Sim actor, Sim target, float cash)
        {
            if (actor.Household == target.Household) return;

            Dictionary<ulong,float> funds;
            if (!sAccrued.TryGetValue(actor.SimDescription.SimDescriptionId, out funds))
            {
                funds = new Dictionary<ulong, float>();
                sAccrued.Add(actor.SimDescription.SimDescriptionId, funds);
            }

            if (funds.ContainsKey(target.SimDescription.SimDescriptionId))
            {
                funds[target.SimDescription.SimDescriptionId] += cash;
            }
            else
            {
                funds.Add(target.SimDescription.SimDescriptionId, cash);
            }
        }

        protected static bool CollectAccrued(Sim sim, Sim target)
        {
            Dictionary<ulong, float> funds;
            if (!sAccrued.TryGetValue(sim.SimDescription.SimDescriptionId, out funds))
            {
                return true;
            }

            float cash = 0;

            bool success = true;

            Dictionary<ulong,float> replace = new Dictionary<ulong,float>();
            foreach (KeyValuePair<ulong, float> value in funds)
            {
                if (value.Value >= 1)
                {
                    Household house = Household.Find(value.Key);
                    if (house == null)
                    {
                        replace.Add(value.Key, 0);
                    }
                    else
                    {
                        int remains = (int)value.Value;
                        if (remains > house.FamilyFunds)
                        {
                            remains = house.FamilyFunds;

                            replace.Add(value.Key, value.Value - remains);

                            if ((target != null) && (target.Household == house))
                            {
                                success = true;
                            }
                        }

                        cash += remains;

                        house.ModifyFamilyFunds(-remains);
                    }
                }
            }

            foreach (KeyValuePair<ulong, float> value in replace)
            {
                funds.Remove(value.Key);

                if (value.Value > 0)
                {
                    funds.Add(value.Key, value.Value);
                }
            }


            sim.ModifyFunds((int)cash);

            SkillBasedCareerBooter.UpdateExperience(sim, SkillNames.MartialArts, (int)cash);

            return success;
        }

        public static void OnMeditated(Event e)
        {
            Sim actor = e.Actor as Sim;
            if (actor == null)
            {
                return;
            }

            if (SkillBasedCareerBooter.GetSkillBasedCareer(actor, SkillNames.MartialArts) == null)
            {
                return;
            }

            TimePassedEvent tEvent = e as TimePassedEvent;

            float perSimValue = 0;
            perSimValue *= tEvent.Increment;

            foreach (Sim sim in actor.LotCurrent.GetSims())
            {
                if (sim == actor) continue;

                if (sim.RoomId != actor.RoomId) continue;

                if (sim.FamilyFunds >= perSimValue)
                {
                    sim.Motives.SetMax(CommodityKind.VampireThirst);
                    sim.Motives.SetMax(CommodityKind.Hunger);
                    sim.Motives.SetMax(CommodityKind.Energy);

                    AddAccrued(actor, sim, perSimValue);
                }
            }

            CollectAccrued(actor, null);
        }

        public static void OnLeveled(Event e)
        {
            HasGuidEvent<SkillNames> skillEvent = e as HasGuidEvent<SkillNames>;
            if (skillEvent == null)
            {
                return;
            }

            if (skillEvent.Guid != SkillNames.MartialArts)
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

            TrainingDummy gameobj = interaction.Target as TrainingDummy;
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

            Common.Notify("Bonus: " + cash);

            AddAccrued(actor, trainer, cash);
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

            if (!(interaction.Target is TrainingDummy))
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

            AddAccrued (actor, target, GetPay(actor) * pEvent.mIncrement);

            if (!CollectAccrued(actor, target))
            {
                if (actor.InteractionQueue != null)
                {
                    actor.InteractionQueue.CancelAllInteractions();
                }

                Common.Notify(Common.Localize("Trainer:UnableToPay"), actor.ObjectId);
            }
        }

        public static void OnSparred(Event e)
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

            MartialArts.SparEvent sEvent = e as MartialArts.SparEvent;

            if (sEvent.IsRanked)
            {
                return;
            }

            Sim target = e.TargetObject as Sim;
            if (target == null)
            {
                return;
            }

            if (actor.Household == target.Household)
            {
                return;
            }

            int actorCash = SkillBasedCareerBooter.GetCareerPay(actor, SkillNames.MartialArts);
            if (actorCash == 0)
            {
                return;
            }

            int targetCash = SkillBasedCareerBooter.GetCareerPay(target, SkillNames.MartialArts);

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

            int diff = actor.SkillManager.GetSkillLevel(SkillNames.MartialArts);

            if (target.SkillManager != null)
            {
                diff -= target.SkillManager.GetSkillLevel(SkillNames.MartialArts);
            }

            if (diff < 0)
            {
                diff = -diff;
            }

            if (diff != 0)
            {
                actorCash /= diff;
            }

            Sim winner = actor;
            Sim loser = target;
            if (!sEvent.HasWon)
            {
                winner = target;
                loser = actor;
            }

            if (actorCash > loser.FamilyFunds)
            {
                actorCash = loser.FamilyFunds;
                if (actorCash == 0)
                {
                    if ((winner.Household == Household.ActiveHousehold) ||
                        (loser.Household == Household.ActiveHousehold))
                    {
                        Common.Notify(Common.Localize("MartialArts:UnableToPay"), loser.ObjectId);
                        return;
                    }
                }
            }

            winner.ModifyFunds(actorCash);

            loser.ModifyFunds(-actorCash);

            SkillBasedCareerBooter.UpdateExperience(winner, SkillNames.MartialArts, actorCash);
        }

        public static void OnWonTournament(Event e)
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

            int cash = SkillBasedCareerBooter.GetCareerPay(actor, SkillNames.MartialArts);

            cash *= 4;

            actor.ModifyFunds(cash);

            SkillBasedCareerBooter.UpdateExperience(actor, SkillNames.MartialArts, cash);

            if (GameUtils.GetCurrentWorld() != WorldName.China)
            {
                RollNewSparOpponent.Perform(actor);
            }
        }
    }
}
