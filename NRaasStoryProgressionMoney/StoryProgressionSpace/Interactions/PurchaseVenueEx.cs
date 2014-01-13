using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class PurchaseVenueEx : PurchaseVenue, Common.IPreLoad
    {
        public static readonly new InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<Lot, PurchaseVenue.Definition, Definition>(false);
        }

        public static void ReimburseDeeds(RealEstateManager newOwner, Lot lot)
        {
            foreach (Household house in Household.GetHouseholdsLivingInWorld())
            {
                if (house.RealEstateManager == null) continue;

                if (house.RealEstateManager == newOwner) continue;

                PropertyData data = house.RealEstateManager.FindProperty(lot);
                if (data == null) continue;

                int totaValue = data.TotalValue;

                using (ManagerMoney.SetAccountingKey setKey = new ManagerMoney.SetAccountingKey(house, "PropertySold"))
                {
                    house.RealEstateManager.SellProperty(data, true);
                }

                if (house != Household.ActiveHousehold)
                {
                    Common.Notify(Common.Localize("Deeds:Reimbursement", false, new object[] { house.Name, totaValue }));
                }
            }
        }

        public override bool Run()
        {
            try
            {
                RabbitHole target = null;

                foreach (RabbitHole hole in Sims3.Gameplay.Queries.GetObjects<RabbitHole>())
                {
                    if (hole.Guid == RabbitHoleType.CityHall)
                    {
                        target = hole;
                        break;
                    }
                }

                ReimburseDeeds(Actor.Household.RealEstateManager, Target);

                if (target != null)
                {
                    RabbitHole.PurchaseVenue.Definition definition = new RabbitHole.PurchaseVenue.Definition(Target);
                    return Actor.InteractionQueue.PushAsContinuation(definition.CreateInstance(target, Actor, GetPriority(), Autonomous, true), false);
                }

                StandardEntry();
                BeginCommodityUpdates();

                bool flag = false;
                try
                {
                    using (ManagerMoney.SetAccountingKey setKey = new ManagerMoney.SetAccountingKey(Actor.Household, "PropertyBought"))
                    {
                        if (Target.IsCommunityLot)
                        {
                            flag = Actor.Household.RealEstateManager.PurchaseVenue(Target) != null;
                        }
                        else
                        {
                            flag = Actor.Household.RealEstateManager.PurchasePrivateLot(Target) != null;
                        }
                    }
                }
                finally
                {
                    EndCommodityUpdates(true);
                    StandardExit();
                }
                return flag;
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

        public new class Definition : PurchaseVenue.Definition
        {
            public Definition()
            { }
            private Definition(bool isFree)
                : base(isFree)
            { }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, Lot target, List<InteractionObjectPair> results)
            {
                results.Add(new InteractionObjectPair(new Definition(RealEstateData.GetPurchaseCost(target) == 0), target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new PurchaseVenueEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}

