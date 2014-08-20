using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TravelerSpace.Options
{
    public class EnableSeasonsSetting : BooleanSettingOption<GameObject>, IPrimaryOption<GameObject>, Common.IDelayedWorldLoadFinished, Common.IWorldQuit
    {
        protected override bool Value
        {
            get
            {
                return Traveler.Settings.mEnableSeasons;
            }
            set
            {
                Traveler.Settings.mEnableSeasons = value;

                ToggleSeasons();
            }
        }

        public override string GetTitlePrefix()
        {
            return "EnableSeasons";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!GameUtils.IsInstalled(ProductVersion.EP8)) return false;

            WorldName world = GameUtils.GetCurrentWorld();
            if(!Common.IsOnTrueVacation() || (world == WorldName.University || world == WorldName.FutureWorld)) return false;

            return base.Allow(parameters);
        }

        public void OnDelayedWorldLoadFinished()
        {
            if (Common.IsOnTrueVacation())
            {
                ToggleSeasons();
            }
        }

        public void OnWorldQuit()
        {
            if (!GameUtils.IsInstalled(ProductVersion.EP8)) return;

            foreach (MotiveTuning tuning in MotiveTuning.GetAllTunings(CommodityKind.Temperature))
            {
                tuning.Universal = true;
            }
        }

        public static void ToggleSeasons()
        {
            bool enable = Traveler.Settings.mEnableSeasons;
            
            if (enable)
            {                
                if (SeasonsManager.Enabled) return;                

                SeasonsManager.sSeasonsValidForWorld = SeasonsManager.Validity.Valid;
                SeasonsManager.PostWorldStartup();
            }
            else
            {                
                //if (!SeasonsManager.Enabled) return;                

                SeasonsManager.Shutdown();

                SeasonsManager.sSeasonsValidForWorld = SeasonsManager.Validity.Invalid;

                foreach (MotiveTuning tuning in MotiveTuning.GetAllTunings(CommodityKind.Temperature))
                {
                    tuning.Universal = false;
                }
            }

            foreach (Sim actor in LotManager.Actors)
            {
                if (actor.Motives == null) continue;

                if (enable)
                {
                    actor.Motives.CreateMotive(CommodityKind.Temperature);
                }
                else
                {
                    actor.Motives.RemoveMotive(CommodityKind.Temperature);
                }
            }            
        }
    }
}
