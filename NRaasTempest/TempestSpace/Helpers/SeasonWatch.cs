using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Insect;
using Sims3.Gameplay.Objects.Spawners;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.SimIFace.Weather;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.TempestSpace.Helpers
{
    public class SeasonWatch : Common.IPreLoad, Common.IWorldLoadFinished
    {
        static Dictionary<ObjectSpawner.ObjectSpawnerTuning, float> sOriginalSpawnChance = new Dictionary<ObjectSpawner.ObjectSpawnerTuning, float>();

        public void OnPreLoad()
        {
            XmlDbData data = XmlDbData.ReadData("Insects");
            ParseSpawnerData(data, false);
        
            data = XmlDbData.ReadData(Insect.kKeyInsectsSpawners, false);
            if (data != null)
            {
                ParseSpawnerData(data, true);
            }
        }

        public void OnWorldLoadFinished()
        {
            new Common.DelayedEventListener(EventTypeId.kSeasonTransition, OnSeasonChanged);

            if (SeasonsManager.Enabled)
            {
                ApplySeason(SeasonsManager.CurrentSeason);
            }
        }

        // From InsectSpawner.ParseData
        protected static void ParseSpawnerData(XmlDbData data, bool bStore)
        {
            XmlDbTable table = null;
            data.Tables.TryGetValue("Spawners", out table);
            if (table != null)
            {
                foreach (XmlDbRow row in table.Rows)
                {
                    List<InsectType> list;
                    if (Insect.sInsectsTypes.TryParseSuperSetEnumCommaSeparatedList(row["Spawns"], out list, InsectType.Undefined))
                    {
                        string typeName = row["SpawnerClassName"];

                        // Custom, unsure why it is needed since EA Standard does not have it
                        if (!typeName.Contains(","))
                        {
                            typeName += ",Sims3GameplayObjects";
                        }

                        Type type = null;
                        if (bStore)
                        {
                            string[] strArray = typeName.Split(new char[] { '|' });
                            if (strArray.Length > 0x1)
                            {
                                typeName = strArray[0x0] + ",Sims3StoreObjects";
                                type = Type.GetType(typeName, false, true);
                                if (type == null)
                                {
                                    typeName = strArray[0x0] + ',' + strArray[0x1];
                                }
                            }
                        }

                        if (type == null)
                        {
                            type = Type.GetType(typeName, false, true);
                        }

                        if (type != null)
                        {
                            FieldInfo field = type.GetField("kSpawnerData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                            if (field != null)
                            {
                                InsectSpawner.InsectSpawnerData spawnerData = field.GetValue(null) as InsectSpawner.InsectSpawnerData;
                                if ((spawnerData != null) && (spawnerData.SpawnerTuning != null))
                                {
                                    sOriginalSpawnChance[spawnerData.SpawnerTuning] = spawnerData.SpawnerTuning.SpawnChance;
                                }
                            }
                        }
                    }
                }
            }
        }

        protected static void ApplySeason(Season season)
        {
            ApplySpawnerSuppression(season == Season.Winter);

            switch (season)
            {
                case Season.Spring:
                case Season.Summer:
				case Season.Winter:
					WeatherControl.SetWorldLeavesAmount(0f);
                    foreach (Lot lot in LotManager.Lots)
                    {
                        World.DecayLeaves(lot.LotId, 1f);
                    }
                    break;
            }
        }

        protected static void ApplySpawnerSuppression(bool suppress)
        {
            if ((suppress) && (Tempest.Settings.mSuppressInsectInWinter))
            {
                foreach (KeyValuePair<ObjectSpawner.ObjectSpawnerTuning, float> pair in sOriginalSpawnChance)
                {
                    pair.Key.SpawnChance = 0f;
                }

                foreach (InsectSpawner spawner in Sims3.Gameplay.Queries.GetObjects<InsectSpawner>())
                {
                    foreach (IGameObject obj in new List<IGameObject>(spawner.mObjectSet.Keys))
                    {
                        InsectJig jig = obj as InsectJig;
                        if (jig == null) continue;

                        spawner.mObjectSet.Remove(obj);

                        obj.Destroy();
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<ObjectSpawner.ObjectSpawnerTuning, float> pair in sOriginalSpawnChance)
                {
                    pair.Key.SpawnChance = pair.Value;
                }
            }
        }

        protected static void OnSeasonChanged(Event e)
        {
            SeasonTransitionEvent seasonEvent = e as SeasonTransitionEvent;
            if (seasonEvent == null) return;

            switch (seasonEvent.PreviousSeason)
            {
                case Season.Spring:
                    break;
                case Season.Summer:
                    break;
                case Season.Fall:
                    break;
                case Season.Winter:
                    break;
            }

            ApplySeason(seasonEvent.CurrentSeason);
        }
    }
}
