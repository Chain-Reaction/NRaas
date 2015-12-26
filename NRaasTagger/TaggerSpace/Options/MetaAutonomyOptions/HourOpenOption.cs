using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TaggerSpace.Options.MetaAutonomyOptions
{
    public class HourOpenOption : IntegerSettingOption<GameObject>, IMATuningOption
    {
        Lot.MetaAutonomyType mType = Lot.MetaAutonomyType.None;
        uint mCustomType;

        public HourOpenOption(Lot.MetaAutonomyType type)
        {
            mType = type;
        }
        public HourOpenOption(uint custom)
        {
            mCustomType = custom;
        }

        protected override int Validate(int value)
        {
            return value < 0 ? -1 : value;
        }

        protected override int Value
        {
            get
            {
                return (int)Tagger.Settings.GetMASettings(mType).mHourOpen;
            }
            set
            {
                MetaAutonomySettingKey key = Tagger.Settings.GetMASettings(mType);
                key.mHourOpen = value;
                Tagger.Settings.AddOrUpdateMASettings(mType, key);                
            }
        }

        public override string GetTitlePrefix()
        {
            return "HourOpen";
        }        

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}