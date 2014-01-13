using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.RetunerSpace.Booters;
using NRaas.RetunerSpace.Helpers.Stores;
using NRaas.RetunerSpace.Options.Tunable;
using NRaas.RetunerSpace.Options.Tunable.Fields;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RetunerSpace
{
    public class SeasonKey : SettingsKey
    {
        Season mSeason;

        public SeasonKey()
            : this(sAllSeasons)
        { }
        public SeasonKey(Season season)
            : this(season, new Vector2(-1, 25))
        { }
        public SeasonKey(Season season, Vector2 hours)
            : base(hours)
        {
            mSeason = season;
        }

        public override bool IsActive
        {
            get
            {
                if (SeasonsManager.Enabled)
                {
                    if (mSeason != SeasonsManager.CurrentSeason) return false;
                }

                return base.IsActive;
            }
        }

        public override Season Season
        {
            get
            {
                return mSeason;
            }
        }

        public override string LocalizedName
        {
            get 
            {
                return Common.LocalizeEAString("Ui/Caption/Options:" + mSeason) + base.LocalizedName;
            }
        }

        public override bool IsEqual(SettingsKey o)
        {
            SeasonKey key = o as SeasonKey;
            if (key == null) return false;

            if (mSeason != key.mSeason) return false;

            return base.IsEqual(o);
        }

        public override void Export(Persistence.Lookup settings)
        {
            settings.Add("Season", mSeason.ToString());

            base.Export(settings);
        }

        public override void Import(Persistence.Lookup settings)
        {
            mSeason = settings.GetEnum<Season>("Season", sAllSeasons);

            base.Import(settings);
        }

        public override string ToXMLString()
        {
            string result = base.ToXMLString();

            result += Common.NewLine + "    <Season>" + mSeason + "</Season>";

            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() * (int)mSeason;
        }
    }
}
