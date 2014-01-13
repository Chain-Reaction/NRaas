using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.TempestSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace NRaas.TempestSpace.Booters
{
    public abstract class BySeasonBooter : BooterHelper.ByRowListingBooter
    {
        protected BySeasonBooter(string table, string listing, string field)
            : base(table, listing, field, VersionStamp.sNamespace + ".Tuning", true)
        {}

        protected WeatherSettings GetSeason(XmlDbRow row)
        {
            Season season = row.GetEnum<Season>("Season", Season.Spring | Season.Summer | Season.Fall | Season.Winter);
            if (season == (Season.Spring | Season.Summer | Season.Fall | Season.Winter))
            {
                BooterLogger.AddError("Unknown Season: " + row.GetString("Season"));
                return null;
            }

            return Tempest.Settings.GetWeather(season, false);
        }

        protected WeatherProfile GetProfile(XmlDbRow row)
        {
            WeatherSettings settings = GetSeason(row);
            if (settings == null) return null;

            string name = row.GetString("Name");
            if (string.IsNullOrEmpty(name))
            {
                BooterLogger.AddError("Missing Name");
                return null;
            }

            return settings.AddProfile(name);
        }
    }
}
