using NRaas.CareerSpace.Booters;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Skills;
using Sims3.SimIFace;

namespace NRaas.CareerSpace.SelfEmployment
{
    public class Repairman : Common.IWorldLoadFinished
    {
        public void OnWorldLoadFinished()
        {
            new Common.DelayedEventListener(EventTypeId.kMopped, OnMopped);

            if (!Common.AssemblyCheck.IsInstalled("NRaasStoryProgressionExpanded"))
            {
                new Common.DelayedEventListener(EventTypeId.kRepairedObject, OnRepaired);
            }

            new Common.DelayedEventListener(EventTypeId.kUpgradedObject, OnUpgraded);
        }

        public static int GetBasePay(SkillBasedCareer career)
        {
            int level = 0;
            if (career != null)
            {
                SkillBasedCareerStaticData skillData = career.GetOccupationStaticDataForSkillBasedCareer();
                if ((skillData != null) && (skillData.CorrespondingSkillName == SkillNames.Handiness))
                {
                    level = career.CareerLevel;
                }
            }
            return GetBasePay(level);
        }
        public static int GetBasePay(int level)
        {
            float basePay = Sims3.Gameplay.Services.Repairman.kServiceTuning.kCost;

            for (int i = 0; i < level; i++)
            {
                basePay *= 1.2f;
            }

            return (int)basePay;
        }

        public static void OnMopped(Event e)
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

            if (actor.LotCurrent == actor.LotHome)
            {
                return;
            }

            foreach (GameObject obj in actor.LotCurrent.GetObjects<GameObject>())
            {
                if (obj.RoomId != actor.RoomId) continue;

                if ((obj.IsRepairable) && (obj is Sims3.Gameplay.Scenarios.IFloodWhenBroken))
                {
                    RepairableComponent repairable = obj.Repairable;
                    if ((repairable != null) && repairable.Broken)
                    {
                        return;
                    }
                }
            }

            GetPaid(actor, actor.LotCurrent, GetBasePay(actor.OccupationAsSkillBasedCareer) / 10, false);
        }

        public static void OnUpgraded(Event e)
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

            GameObject obj = e.TargetObject as GameObject;
            if (obj == null)
            {
                return;
            }

            if (obj.LotCurrent == actor.LotHome)
            {
                return;
            }

            int level = 1;

            if (actor.InteractionQueue == null)
            {
                return;
            }

            InteractionInstance interaction = actor.InteractionQueue.GetCurrentInteraction();
            if (interaction != null)
            {
                Availability availability = interaction.InteractionObjectPair.Tuning.Availability;
                if (availability.SkillThresholdType == SkillNames.Handiness)
                {
                    level = availability.SkillThresholdValue;
                }
            }

            int basePay = GetBasePay(level);

            GetPaid(actor, obj.LotCurrent, RandomUtil.GetInt(basePay * 2, basePay * 4), true);
        }

        public static void OnRepaired(Event e)
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

            GameObject obj = e.TargetObject as GameObject;
            if (obj == null)
            {
                return;
            }

            if (obj.LotCurrent == actor.LotHome)
            {
                return;
            }

            int basePay = GetBasePay(actor.OccupationAsSkillBasedCareer);

            GetPaid(actor, obj.LotCurrent, RandomUtil.GetInt(basePay / 2, basePay), true);
        }

        public static void GetPaid(Sim actor, Lot lot, int payment, bool notify)
        {
            if (payment > 0)
            {
                Household house = lot.Household;
                if (house != null)
                {
                    if (payment > house.FamilyFunds)
                    {
                        payment = house.FamilyFunds;
                        if ((payment == 0) && (notify))
                        {
                            if (actor.Household == Household.ActiveHousehold)
                            {
                                ObjectGuid guid = ObjectGuid.InvalidObjectGuid;
                                if (house.Sims.Count > 0)
                                {
                                    guid = house.Sims[0].ObjectId;
                                }

                                Common.Notify(Common.Localize("Repairman:UnableToPay"), guid);
                            }
                        }
                    }

                    house.ModifyFamilyFunds(-payment);
                }

                actor.ModifyFunds(payment);

                SkillBasedCareerBooter.UpdateExperience(actor, SkillNames.Handiness, payment);
            }
        }
    }
}
