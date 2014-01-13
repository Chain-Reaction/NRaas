using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class TownFamily : SelectionTestableOptionList<TownFamily.Item,Household,ulong>, IDoesNotNeedSpeciesFilter
    {
        [Persistable(false)]
        Household mHouse = null;

        public TownFamily()
        {}
        public TownFamily(Household house)
        {
            mHouse = house;
        }

        public override string GetTitlePrefix()
        {
            return "Criteria.TownFamily";
        }

        public override void GetOptions(List<IMiniSimDescription> actors, List<IMiniSimDescription> allSims, List<Item> items)
        {
            if (mHouse != null)
            {
                items.Add(new Item(mHouse));
            }
            else
            {
                base.GetOptions(actors, allSims, items);
            }
        }

        protected static bool IsOlderThan(SimDescription description, SimDescription test)
        {
            return (AgingManager.Singleton.GetCurrentAgeInDays(description) > AgingManager.Singleton.GetCurrentAgeInDays(test));
        }

        public static string GetQualifiedName(Household house)
        {
            string name = house.Name;
            if (house.IsServiceNpcHousehold)
            {
                return Common.Localize("SimType:Service");
            }
            else if (house.IsPetHousehold)
            {
                return Common.Localize("SimType:Pet");
            }
            else if (house.IsTouristHousehold)
            {
                return Common.LocalizeEAString("Gameplay/Roles/RoleTourist:Tourist");
            }
            else
            {
                SimDescription sim = SimTypes.HeadOfFamily(house);
                if (sim != null)
                {
                    name += " - " + sim.FirstName;
                    if (sim.LastName != house.Name)
                    {
                        name += " " + sim.LastName;
                    }
                }
            }
            return name;
        }

        public class Item : TestableOption<Household, ulong>
        {
            public Item()
            { }
            public Item(Household house)
                : base(house.HouseholdId, house.Name, 0)
            { }

            public override void SetValue(Household value, ulong storeType)
            {
                if (value == null)
                {
                    mValue = 0;
                }
                else
                {
                    mValue = value.HouseholdId;

                    mName = GetQualifiedName(value);

                    mCount = CommonSpace.Helpers.Households.NumSimsIncludingPregnancy(value);
                }
            }

            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<ulong, Household> results)
            {
                if (me.Household == null)
                {
                    results[0] = null;
                }
                else
                {
                    results[me.Household.HouseholdId] = me.Household;
                }

                return true;
            }

            public Household House
            {
                get { return Household.Find(Value); }
            }
        }
    }
}
