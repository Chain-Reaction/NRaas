using NRaas.CommonSpace.Helpers;
using NRaas.HybridSpace;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Alchemy;
using Sims3.Gameplay.Objects.Island;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Seasons;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.Store.Objects;
using Sims3.UI.Hud;
using System.Collections.Generic;

namespace NRaas
{
    public class Hybrid : Common, Common.IPreLoad, Common.IWorldLoadFinished
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        [PersistableStatic]
        static PersistedSettings sSettings = null;

        static Hybrid()
        {
            Bootstrap();
        }

        public static PersistedSettings Settings
        {
            get
            {
                if (sSettings == null)
                {
                    sSettings = new PersistedSettings();
                }

                return sSettings;
            }
        }

        public static void ResetSettings()
        {
            sSettings = null;
        }
        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.GetTuning<ShowerOutdoor, ShowerOutdoor.TakeShower.Definition>();
            if (tuning != null)
            {
                CommodityChange change = new CommodityChange(CommodityKind.BePlantSim, 40, false, 0, OutputUpdateType.ContinuousFlow, false, false, UpdateAboveAndBelowZeroType.Either);
                tuning.mTradeoff.mOutputs.Add(change);
            }

            tuning = Tunings.GetTuning<Terrain, Terrain.PlayInOcean.Definition>();

            if (tuning != null)
            {
                CommodityChange change = new CommodityChange(CommodityKind.MermaidDermalHydration, 15, false, 30, OutputUpdateType.ContinuousFlow, false, false, UpdateAboveAndBelowZeroType.Either);
                tuning.mTradeoff.mOutputs.Add(change);

                change = new CommodityChange(CommodityKind.Hygiene, 45, false, 60, OutputUpdateType.ContinuousFlow, false, false, UpdateAboveAndBelowZeroType.Either);
                tuning.mTradeoff.mOutputs.Add(change);
            }

            tuning = Tunings.GetTuning<Sim, SwimAroundInOcean.Definition>();

            if (tuning != null)
            {
                foreach (CommodityChange mOutput in tuning.mTradeoff.mOutputs)
                {
                    if (mOutput.Commodity == CommodityKind.MermaidDermalHydration)
                    {
                        mOutput.mConstantChange = 201;
                        //mOutput.mActualValue = 60;
                    }
                }
            }

            /*
            tuning = Tunings.GetTuning<Terrain, SwimHere.Definition>();

            if (tuning != null)
            {
                CommodityChange change = new CommodityChange(CommodityKind.MermaidDermalHydration, 202, false, 15, OutputUpdateType.ContinuousFlow, false, false, UpdateAboveAndBelowZeroType.Either);
                tuning.mTradeoff.mOutputs.Add(change);
            }
            */

            tuning = Tunings.GetTuning<Terrain, Terrain.SplashInPuddle.Definition>();

            if (tuning != null)
            {
                CommodityChange change = new CommodityChange(CommodityKind.MermaidDermalHydration, 7, false, 15, OutputUpdateType.ContinuousFlow, false, false, UpdateAboveAndBelowZeroType.Either);
                tuning.mTradeoff.mOutputs.Add(change);

                change = new CommodityChange(CommodityKind.Hygiene, 11, false, 15, OutputUpdateType.ContinuousFlow, false, false, UpdateAboveAndBelowZeroType.Either);
                tuning.mTradeoff.mOutputs.Add(change);
            }

            tuning = Tunings.GetTuning<Sim, Sim.AutonomousSplashInPuddle.Definition>();

            if (tuning != null)
            {
                CommodityChange change = new CommodityChange(CommodityKind.MermaidDermalHydration, 7, false, 15, OutputUpdateType.ContinuousFlow, false, false, UpdateAboveAndBelowZeroType.Either);
                tuning.mTradeoff.mOutputs.Add(change);

                change = new CommodityChange(CommodityKind.Hygiene, 11, false, 15, OutputUpdateType.ContinuousFlow, false, false, UpdateAboveAndBelowZeroType.Either);
                tuning.mTradeoff.mOutputs.Add(change);
            }

            tuning = Tunings.GetTuning<Pool, PoolWaterfallBase.PlayInPoolWaterfall.Definition>();

            if (tuning != null)
            {
                CommodityChange change = new CommodityChange(CommodityKind.MermaidDermalHydration, 7, false, 30, OutputUpdateType.ContinuousFlow, false, false, UpdateAboveAndBelowZeroType.Either);
                tuning.mTradeoff.mOutputs.Add(change);
            }

            tuning = Tunings.GetTuning<Pool, GetInPool.Definition>();

            if (tuning != null)
            {
                CommodityChange change = new CommodityChange(CommodityKind.MermaidDermalHydration, 201, false, 0, OutputUpdateType.ContinuousFlow, false, false, UpdateAboveAndBelowZeroType.Either);
                tuning.mTradeoff.mOutputs.Add(change);
            }

            /*
            tuning = Tunings.GetTuning<Pool, SwimAround.Definition>();

            if (tuning != null)
            {
                foreach (CommodityChange mOutput in tuning.mTradeoff.mOutputs)
                {
                    if (mOutput.Commodity == CommodityKind.MermaidDermalHydration)
                    {
                        mOutput.mActualValue = 20;
                    }
                }
            }
            */
        }

        public void OnWorldLoadFinished()
        {
            kDebugging = Settings.Debugging;

            foreach (AlchemyRecipe recipe in AlchemyRecipe.AlchemyRecipes)
            {
                recipe.mNonPersistableData.mCanConsumeIfPregnant = true;
            }

            if (GameUtils.IsInstalled(ProductVersion.EP8))
            {
                new Common.AlarmTask(6, Sims3.Gameplay.Utilities.TimeUnit.Minutes, OccultWeatherTask.Perform, 2, Sims3.Gameplay.Utilities.TimeUnit.Minutes);
            }

            OccultPlantSim.kWaterMotiveIncrement = 0;
            OccultMermaid.kTemperatureForDryingUpBuff = 500f;
            OccultMermaid.kRainHydrationAmount = new float[3] { 0f, 0f, 0f };
        }

        public static bool IsValidOccult(Sim sim, List<OccultTypes> types)
        {
            if (sim == null) return false;

            return IsValidOccult(sim.SimDescription, types);
        }
        public static bool IsValidOccult(SimDescription sim, List<OccultTypes> types)
        {
            if (sim == null) return false;

            OccultManager manager = sim.OccultManager;
            if (manager == null) return false;

            foreach (OccultTypes type in types)
            {
                if (manager.HasOccultType(type)) return true;
            }

            return false;
        }

        public class OccultWeatherTask : Common.FunctionTask
        {
            protected OccultWeatherTask()
            {
            }

            public static void Perform()
            {
                new OccultWeatherTask().AddToSimulator();
            }

            protected override void OnPerform()
            {
                if (!SeasonsManager.Enabled) return;

                PrecipitationIntensity intensity;
                bool raining = SeasonsManager.IsRaining(out intensity);

                foreach (Sim sim in LotManager.Actors)
                {
                    if (sim == null) continue;

                    if (sim.SimDescription == null) continue;

                    if (!sim.SimDescription.IsMatureMermaid && !sim.SimDescription.IsPlantSim) continue;

                    Motive motive = null;
                    if (sim.SimDescription.IsMatureMermaid)
                    {
                        if (sim.CurrentInteraction != null && sim.CurrentInteraction.HasCommodityUpdate(CommodityKind.MermaidDermalHydration)) continue;

                        motive = sim.Motives.GetMotive(CommodityKind.MermaidDermalHydration);
                    }
                    else
                    {
                        if (sim.CurrentInteraction != null && sim.CurrentInteraction.HasCommodityUpdate(CommodityKind.Hygiene)) continue;

                        motive = sim.Motives.GetMotive(CommodityKind.Hygiene);
                    }

                    if (motive == null)
                    {
                        continue;
                    }

                    if (!sim.IsOutside)
                    {
                        motive.mInstanceHasDecay = true;
                        continue;
                    }

                    // EA doesn't apply this logic to platsims so I'll do it
                    if (SimTemperature.OutsideSimIsConsideredInside(sim))
                    {
                        motive.mInstanceHasDecay = true;
                        continue;
                    }

                    if (sim.SimDescription.IsMatureMermaid)
                    {
                        if (SeasonsManager.Temperature >= Hybrid.Settings.mTemperatureForDryingUpBuff && SeasonsManager.CurrentWeather == Weather.Sunny && motive != null && motive.Delta <= 0f && !sim.BuffManager.HasElement(BuffNames.DryingUp))
                        {
                            sim.BuffManager.AddElement(BuffNames.DryingUp, Origin.FromTheSun);
                        }
                    }

                    if (!raining)
                    {
                        motive.mInstanceHasDecay = true;
                        continue;
                    }

                    if (SeasonsManager.IsShelteredFromPrecipitation(sim))
                    {
                        motive.mInstanceHasDecay = true;
                        continue;
                    }

                    BeachTowel beachTowel = sim.Posture.Container as BeachTowel;
                    if ((beachTowel != null && beachTowel.UmbrellaIsUp) || Umbrella.SearchForHoldingUmbrellaPosture(sim) != null)
                    {
                        motive.mInstanceHasDecay = true;
                        continue;
                    }

                    if (sim.SimDescription.IsMatureMermaid)
                    {
                        motive.mInstanceHasDecay = true;

                        float val = Hybrid.Settings.mMermaidRainHydrationAmount[0];
                        motive.UIArrow = Motives.MotiveChangeArrows.OnePositiveArrow;

                        if (intensity == PrecipitationIntensity.Heavy)
                        {
                            val = Hybrid.Settings.mMermaidRainHydrationAmount[2];
                            motive.UIArrow = Motives.MotiveChangeArrows.ThreePositiveArrow;
                        }
                        else if (intensity == PrecipitationIntensity.Moderate)
                        {
                            val = Hybrid.Settings.mMermaidRainHydrationAmount[1];
                            motive.UIArrow = Motives.MotiveChangeArrows.TwoPositiveArrow;
                        }

                        motive.mLastTimeUIArrowUpdated += 2 * 60;

                        if (motive != null)
                        {
                            float hoursPassed = 2 / 60;
                            motive.SetValue(sim, motive.Value + val, hoursPassed);
                            motive.mInstanceHasDecay = false;
                        }
                    }
                    else
                    {
                        float val = Hybrid.Settings.mPlantSimRainHydrationAmount[0];
                        motive.UIArrow = Motives.MotiveChangeArrows.OnePositiveArrow;

                        if (intensity == PrecipitationIntensity.Heavy)
                        {
                            val = Hybrid.Settings.mPlantSimRainHydrationAmount[2];
                            motive.UIArrow = Motives.MotiveChangeArrows.ThreePositiveArrow;
                        }
                        else if (intensity == PrecipitationIntensity.Moderate)
                        {
                            val = Hybrid.Settings.mPlantSimRainHydrationAmount[1];
                            motive.UIArrow = Motives.MotiveChangeArrows.TwoPositiveArrow;


                            if (motive != null)
                            {
                                float hoursPassed = 2 / 60;
                                motive.SetValue(sim, motive.Value + val, hoursPassed);
                                motive.mInstanceHasDecay = false;
                            }
                        }
                    }
                }
            }
        }
    }
}
