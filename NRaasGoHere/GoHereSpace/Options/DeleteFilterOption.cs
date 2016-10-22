using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.GoHereSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.GoHereSpace.Options
{
    public class DeleteFilterOption : OperationSettingOption<GameObject>, DoorFiltersGlobal.IDoorGlobalOption, DoorFilters.IDoorOption
    {
        public override string GetTitlePrefix()
        {
            return "DeleteFilter";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            foreach (KeyValuePair<string, bool> pair in FilterHelper.GetFilters())
            {
                if (pair.Key.StartsWith("nraas")) return true;
            }

            return false;
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            FilterHelper.DeleteFilter();

            /*
             * Need to see what to do with this because no easy way to make sure the user is done before doing this
             * Maybe set an alarm that checks if a dialog is up?
            foreach(KeyValuePair<ObjectGuid, DoorPortalComponentEx.DoorSettings> mSettings in GoHere.Settings.mDoorSettings)
            {
                GoHere.Settings.GetDoorSettings(mSettings.Key); // validates filters on pull                
            }

            foreach (string mFilter in new List<string>(GoHere.Settings.mGlobalIgnoreAllDoorOptionsFilterOption))
            {
                if (!FilterHelper.IsValidFilter(mFilter))
                {
                    GoHere.Settings.mGlobalIgnoreAllDoorOptionsFilterOption.Remove(mFilter);
                }
            }

            foreach (string mFilter in new List<string>(GoHere.Settings.mGlobalIgnoreDoorCostFilterOption))
            {
                if (!FilterHelper.IsValidFilter(mFilter))
                {
                    GoHere.Settings.mGlobalIgnoreDoorCostFilterOption.Remove(mFilter);
                }
            }

            foreach (string mFilter in new List<string>(GoHere.Settings.mGlobalIgnoreDoorFiltersFilterOption))
            {
                if (!FilterHelper.IsValidFilter(mFilter))
                {
                    GoHere.Settings.mGlobalIgnoreDoorFiltersFilterOption.Remove(mFilter);
                }
            }

            foreach (string mFilter in new List<string>(GoHere.Settings.mGlobalIgnoreDoorTimeLocksFilterOption))
            {
                if (!FilterHelper.IsValidFilter(mFilter))
                {
                    GoHere.Settings.mGlobalIgnoreDoorTimeLocksFilterOption.Remove(mFilter);
                }
            }
             */

            return OptionResult.SuccessClose;
        }
    }
}