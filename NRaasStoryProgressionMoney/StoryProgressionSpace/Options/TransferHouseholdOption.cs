using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Options
{
    public class TransferHouseholdOption : GenericOptionBase.GenericOption<ulong>, IReadCasteLevelOption, IWriteCasteLevelOption, IReadHouseLevelOption, IWriteHouseLevelOption
    {
        public TransferHouseholdOption()
            : base(0, 0)
        { }

        public override string GetTitlePrefix()
        {
            return "TransferHousehold";
        }

        protected override bool PrivatePerform()
        {
            List<HouseholdItem> allOptions = new List<HouseholdItem>();
            foreach (Household house in Household.GetHouseholdsLivingInWorld())
            {
                allOptions.Add(new HouseholdItem(house));
            }

            SetValue (0);

            bool okayed = false;
            List<HouseholdItem> choices = OptionItem.ListOptions(allOptions, Name, 1, out okayed);
            if ((choices != null) && (choices.Count > 0))
            {
                SetValue (choices[0].Value.HouseholdId);
            }

            return okayed;
        }

        protected Household House
        {
            get
            {
                return Household.Find(Value);
            }
        }

        public override string GetUIValue(bool pure)
        {
            Household house = House;
            if (house == null) return null;

            return house.Name;
        }

        protected override string GetLocalizationValueKey()
        {
            return null;
        }

        public override object PersistValue
        {
            get
            {
                return Value;
            }
            set
            {
                if (value is string)
                {
                    ulong newValue;
                    if (!ulong.TryParse(value as string, out newValue)) return;

                    SetValue (newValue);
                }
                else
                {
                    SetValue ((ulong)value);
                }
            }
        }

        protected class HouseholdItem : GenericOptionItem<Household>
        {
            public HouseholdItem(Household house)
                : base(house, null)
            { }

            public override string Name
            {
                get
                {
                    string name = Value.Name;

                    name += " - ";

                    SimDescription sim = SimTypes.HeadOfFamily(Value);
                    if (sim != null)
                    {
                        name += sim.FullName;
                    }

                    return name;
                }
            }

            public override string GetTitlePrefix()
            {
                return null;
            }

            protected override string GetLocalizationValueKey()
            {
                return null;
            }

            public override string GetUIValue(bool pure)
            {
                return null;
            }

            public override object PersistValue
            {
                get
                {
                    return Value;
                }
                set
                {
                    return;
                }
            }

            protected override bool PrivatePerform()
            {
                return true;
            }

            public override bool ShouldDisplay()
            {
                return true;
            }
        }
    }
}

