using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
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

namespace NRaas.StoryProgressionSpace
{
    // For legacy purposes, this class must always be at this level in the namespace 

    [Persistable]
    public class HouseholdOptions : GenericOptionBase
    {
        private ulong mHouseholdId = 0;

        public HouseholdOptions() // Required for persistence
        { }
        public HouseholdOptions(Household house)
        {
            mHouseholdId = house.HouseholdId;
        }

        public override string Name
        {
            get { return "Household: " + House.Name; }
        }

        public override SimDescription SimDescription
        {
            get 
            {
                return null;
            }
        }

        public override Household House
        {
            get 
            { 
                return Household.Find(mHouseholdId); 
            }
        }

        public override Lot Lot
        {
            get
            {
                Household house = House;
                if (house == null) return null;

                return house.LotHome;
            }
        }

        public ulong HouseholdId
        {
            get { return mHouseholdId; }
        }

        public R GetValue<T, R>()
            where T : GenericOptionItem<R>, IReadHouseLevelOption, new()
        {
            return GetInternalValue<T, R>();
        }

        public bool SetValue<T, R>(R value)
            where T : GenericOptionItem<R>, IWriteHouseLevelOption, new()
        {
            return SetInternalValue<T, R>(value);
        }

        public TType AddValue<T, TType>(TType value)
            where T : OptionItem, IGenericAddOption<TType>, IWriteHouseLevelOption, new()
        {
            return AddInternalValue<T, TType>(value);
        }

        public void RemoveValue<T, TType>(TType value)
            where T : OptionItem, IGenericRemoveOption<TType>, IWriteHouseLevelOption, new()
        {
            RemoveInternalValue<T, TType>(value);
        }

        public bool HasValue<T, TType>(TType value)
            where T : OptionItem, IGenericHasOption<TType>, IReadHouseLevelOption, new()
        {
            return HasInternalValue<T, TType>(value);
        }

        public override void UpdateInheritors(IGenericLevelOption option)
        {
            base.UpdateInheritors(option);

            if (House == null) return;

            foreach (SimDescription sim in Households.All(House))
            {
                SimData data = StoryProgression.Main.GetData(sim);

                data.Uncache(option);

                if (!data.IsValidOption(option))
                {
                    data.RemoveOption(option);
                }
            }
        }

        public override bool IsValidOption(IGenericLevelOption option)
        {
            if (option is IReadHouseLevelOption) return true;

            if (option is IWriteHouseLevelOption) return true;

            if (option is IHouseLevelSimOption) return true;

            return false;
        }

        protected override void GetParentOptions(List<ParentItem> options, DefaultingLevel level)
        {
            if (Lot == null)
            {
                // Global options
                options.Add(new ParentItem(StoryProgression.Main.Options, level));
            }
            else
            {
                options.Add(new ParentItem(StoryProgression.Main.GetLotOptions(Lot), level));

                if (((level & DefaultingLevel.Castes) == DefaultingLevel.Castes) && 
                    ((level & DefaultingLevel.HeadOfFamily) == DefaultingLevel.HeadOfFamily))
                {
                    SimData sim = StoryProgression.Main.GetData(SimTypes.HeadOfFamily(House));
                    if (sim != null)
                    {
                        sim.GetAllCasteOptions(options, false, true);
                    }
                }
            }
        }

        public override string ToString()
        {
            Common.StringBuilder text = new Common.StringBuilder("<HouseholdOptions>");

            Household house = House;
            if (house != null)
            {
                text.AddXML("House", house.Name);
            }
            else
            {
                text.AddXML("House", "Null");
            }

            text += base.ToString();

            text += Common.NewLine + "</HouseholdOptions>";

            return text.ToString();
        }
    }
}

