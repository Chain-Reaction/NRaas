using Sims3.Gameplay.Objects.TombObjects.ani_OFBStand;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Objects.HobbiesSkills.Inventing;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Objects.Alchemy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects.Register;

namespace ani_OFBStand
{
    public class ShoppingMethods
    {

        public static void PayLotOwner(OFBStand stand, int price)
        {
            if (stand.info.Owner != null && price > 0)
            {
                stand.info.Owner.Household.SetFamilyFunds(stand.info.Owner.Household.FamilyFunds + price);
            }
        }

        public static int CalculatePrice(int value, float multiplyer)
        {
            return (int)(value * multiplyer);
        }

        public static void PayEmployee(OFBStand stand, Sim sim, float hours)
        {            
            int wage = (int)(hours * stand.info.PayPerHour);

            //Don't pay if townie
            if (!sim.SimDescription.IsTownie)
                sim.Household.SetFamilyFunds(sim.Household.FamilyFunds + wage);

            //Reduce funds from owner
            if (stand.info.Owner != null && stand.info.PayWageFromOwnersFunds)
                stand.info.Owner.Household.SetFamilyFunds(stand.info.Owner.Household.FamilyFunds - wage);

        }

        public static void UnSpoil(OFBStand stand)
        {
            foreach (var stack in stand.Inventory.InventoryItems.Values)
            {
                foreach (var item in stack.List)
                {
                    if (item.Object.Value == 0)
                    {
                        ServingContainer single = item.Object as ServingContainer;
                        ServingContainerGroup group = item.Object as ServingContainerGroup;
                        int servingPrice = stand.info.ServingPrice;

                        if (group != null)
                        {
                            group.mPurchasedPrice = ReturnPriceByQuality(single.FoodQuality, servingPrice * group.NumServingsLeft);
                            group.RemoveSpoilageAlarm();
                        }
                        else if (single != null)
                        {
                            single.mPurchasedPrice = ReturnPriceByQuality(single.FoodQuality, servingPrice);
                            single.RemoveSpoilageAlarm();
                        }
                    }
                }

            }
        }

        private static int ReturnPriceByQuality(Quality q, int defaultPrice)
        {
            int price;
            switch (q)
            {
                case Quality.Foul:
                case Quality.Horrifying:
                case Quality.Bad:
                case Quality.Putrid:
                    price = defaultPrice;
                    break;
                case Quality.Nice:
                case Quality.VeryNice:
                    price = (int)(defaultPrice * 1.2f);
                    break;
                case Quality.Great:
                case Quality.Outstanding:
                case Quality.Excellent:
                    price = (int)(defaultPrice * 1.3f);
                    break;
                case Quality.Perfect:
                    price = (int)(defaultPrice * 1.4f);
                    break;
                default:
                    price = defaultPrice;
                    break;
            }
            return price;
        }

        public static void UpdateSkillBasedCareerEarning(SimDescription sd, GameObject soldItem)
        {
            if (sd != null && sd.Occupation != null && sd.Occupation.IsSkillBased)
            {
                if (((ulong)sd.OccupationAsSkillBasedCareer.Guid) == ((ulong)OccupationNames.SpellCrafter) && soldItem.GetType() == typeof(AlchemyPotion))
                {
                    PotionShopConsignmentRegister.RewardAlchemistForConsignmentSell((float)((long)soldItem.Value / (long)((ulong)PotionShopConsignmentRegister.GetNumSimsWithAlchemyJob())));
                    EventTracker.SendEvent(EventTypeId.kSoldConsignedObject, sd.CreatedSim);
                   // sd.OccupationAsSkillBasedCareer.UpdateXpForEarningMoneyFromSkill(Sims3.Gameplay.Skills.SkillNames.Spellcasting, soldItem.Value);
                }else 
                if (((ulong)sd.OccupationAsSkillBasedCareer.Guid) == ((ulong)OccupationNames.Inventor) && soldItem.GetType() == typeof(Invention))
                {
                    sd.OccupationAsSkillBasedCareer.UpdateXpForEarningMoneyFromSkill(Sims3.Gameplay.Skills.SkillNames.Inventing, soldItem.Value);
                }else 

                if (((ulong)sd.OccupationAsSkillBasedCareer.Guid) == ((ulong)OccupationNames.NectarMaker) && soldItem.GetType() == typeof(NectarBottle))
                {
                    sd.OccupationAsSkillBasedCareer.UpdateXpForEarningMoneyFromSkill(Sims3.Gameplay.Skills.SkillNames.Nectar, soldItem.Value);
                }else 

                if (((ulong)sd.OccupationAsSkillBasedCareer.Guid) == ((ulong)OccupationNames.Gardener) && soldItem.GetType() == typeof(Ingredient))
                {
                    sd.OccupationAsSkillBasedCareer.UpdateXpForEarningMoneyFromSkill(Sims3.Gameplay.Skills.SkillNames.Gardening, soldItem.Value);
                }else 

                if (((ulong)sd.OccupationAsSkillBasedCareer.Guid) == ((ulong)OccupationNames.Fisher) && soldItem.GetType() == typeof(Fish))
                {
                    sd.OccupationAsSkillBasedCareer.UpdateXpForEarningMoneyFromSkill(Sims3.Gameplay.Skills.SkillNames.Fishing, soldItem.Value);
                }else

                if (((ulong)sd.OccupationAsSkillBasedCareer.Guid) == ((ulong)OccupationNames.Photographer) && soldItem.GetType() == typeof(Photograph))
                {
                    sd.OccupationAsSkillBasedCareer.UpdateXpForEarningMoneyFromSkill(Sims3.Gameplay.Skills.SkillNames.Photography, soldItem.Value);
                }else 

                if (((ulong)sd.OccupationAsSkillBasedCareer.Guid) == ((ulong)OccupationNames.Sculptor) && soldItem.GetType() == typeof(Sculpture))
                {
                    sd.OccupationAsSkillBasedCareer.UpdateXpForEarningMoneyFromSkill(Sims3.Gameplay.Skills.SkillNames.Sculpting, soldItem.Value);
                } 
            }

        }

    }
}
