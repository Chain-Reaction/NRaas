using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TravelerSpace.Options
{
    public class TravelFilterSetting : ListedSettingOption<TravelUtilEx.Type, GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "TravelFilter";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override Proxy GetList()
        {
            return new EnumProxy();
        }

        public override bool ConvertFromString(string value, out TravelUtilEx.Type newValue)
        {
            return ParserFunctions.TryParseEnum<TravelUtilEx.Type>(value, out newValue, TravelUtilEx.Type.None);
        }

        public override string ConvertToString(TravelUtilEx.Type value)
        {
            return value.ToString();
        }

        protected override bool Allow(TravelUtilEx.Type value)
        {
            if (value == TravelUtilEx.Type.None) return false;

            return base.Allow(value);
        }

        public override string GetExportValue()
        {
            return Traveler.Settings.mTravelFilter.ToString();
        }

        public class EnumProxy : Proxy
        {
            public EnumProxy()
            { }

            public override string GetDisplayValue(ListedSettingOption<TravelUtilEx.Type, GameObject> option)
            {
                return "";
            }

            public override string GetExportValue(ListedSettingOption<TravelUtilEx.Type, GameObject> option)
            {
                return Traveler.Settings.mTravelFilter.ToString();
            }

            public override void Clear()
            {
                Traveler.Settings.mTravelFilter = TravelUtilEx.Type.None;
            }

            public override bool Contains(TravelUtilEx.Type value)
            {
                return ((Traveler.Settings.mTravelFilter & value) != TravelUtilEx.Type.None);
            }

            public override void Add(TravelUtilEx.Type value)
            {
                Traveler.Settings.mTravelFilter |= value;
            }

            public override void Remove(TravelUtilEx.Type value)
            {
                Traveler.Settings.mTravelFilter &= ~value;
            }
        }
    }
}
