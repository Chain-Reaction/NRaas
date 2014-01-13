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
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.TempestSpace.Helpers
{
    public class WeatherWatch : Common.IWorldLoadFinished
    {
        public void OnWorldLoadFinished()
        {
            new Common.DelayedEventListener(EventTypeId.kWeatherStarted, OnWeather);
        }

        protected static void OnWeather(Event e)
        {
            WeatherEvent weatherEvent = e as WeatherEvent;
            if (weatherEvent == null) return;

            if (Household.ActiveHousehold == null) return;

            if (Household.ActiveHousehold.LotHome == null) return;
            /*
            switch (weatherEvent.Weather)
            {
                case Weather.Rain:
                case Weather.Hail:
                    foreach (Plant plant in Household.ActiveHousehold.LotHome.GetObjects<Plant>())
                    {
                        if ((!plant.IsOutside) || (SeasonsManager.IsShelteredFromPrecipitation(plant))) continue;

                        if (plant.WaterLevel < 100)
                        {
                            plant.WaterLevel = 100;
                        }
                    }
                    break;
            }
            */
            switch (weatherEvent.Weather)
            {
                case Weather.Hail:
                    foreach (Plant plant in Household.ActiveHousehold.LotHome.GetObjects<Plant>())
                    {
                        if ((!plant.IsOutside) || (SeasonsManager.IsShelteredFromPrecipitation(plant))) continue;

                        if (RandomUtil.RandomChance(Tempest.Settings.mHailKillsPlants))
                        {
                            plant.SetDead();
                        }
                    }
                    break;
            }

            switch (weatherEvent.Weather)
            {
                case Weather.Hail:
                case Weather.Snow:
                    foreach (HarvestPlant plant in Sims3.Gameplay.Queries.GetObjects<HarvestPlant>())
                    {
                        if ((!plant.IsOutside) || (SeasonsManager.IsShelteredFromPrecipitation(plant))) continue;

                        if (RandomUtil.RandomChance(Tempest.Settings.mHailKillsHarvestables))
                        {
                            HarvestPlant harvest = plant as HarvestPlant;
                            if (harvest != null)
                            {
                                harvest.SetGrowthStateFromHarvestToNext();
                            }
                        }
                    }
                    break;
            }
        }
    }
}
