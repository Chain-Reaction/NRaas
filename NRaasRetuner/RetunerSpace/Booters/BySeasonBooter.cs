using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.RetunerSpace.Helpers.Stores;
using NRaas.RetunerSpace.Options.Tunable;
using NRaas.RetunerSpace.Options.Tunable.Fields;
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

namespace NRaas.RetunerSpace.Booters
{
    public abstract class BySeasonBooter<T> : BooterHelper.ByRowListingBooter
    {
        static Dictionary<SettingsKey, List<T>> sSettings = new Dictionary<SettingsKey, List<T>>();

        protected BySeasonBooter(string table, string listing, string field)
            : base(table, listing, field, VersionStamp.sNamespace + ".Tuning", true)
        {}

        public static IEnumerable<SettingsKey> Keys
        {
            get
            {
                return sSettings.Keys;
            }
        }

        public static List<T> GetSettings(SettingsKey key)
        {
            List<T> list;
            if (!sSettings.TryGetValue(key, out list))
            {
                list = new List<T>();
                sSettings.Add(key, list);
            }

            return list;
        }

        protected static SettingsKey ParseKey(XmlDbRow row)
        {
            Season season = row.GetEnum<Season>("Season", SettingsKey.sAllSeasons);

            Vector2 hours = new Vector2();

            hours.x = row.GetFloat("StartHour", -1);
            hours.y = row.GetFloat("EndHour", 25);

            if (season == SettingsKey.sAllSeasons)
            {
                return new GeneralKey(hours);
            }
            else
            {
                return new SeasonKey(season, hours);
            }
        }

        protected static void Add(SettingsKey key, T obj)
        {
            GetSettings(key).Add(obj);
        }
    }
}
