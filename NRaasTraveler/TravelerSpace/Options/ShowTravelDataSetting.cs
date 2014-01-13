using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.TravelerSpace.Booters;
using NRaas.TravelerSpace.Helpers;
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
    public class ShowTravelDataSetting : OperationSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "ShowTravelData";
        }

        public override string DisplayValue
        {
            get { return null; }
        }

        protected override bool Allow(GameHitParameters< GameObject> parameters)
        {
            if (!Common.kDebugging) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters< GameObject> parameters)
        {
            Common.StringBuilder msg = new Common.StringBuilder();

            msg += Common.NewLine + "GameStates.IsOnVacation = " + GameStates.IsOnVacation;
            msg += Common.NewLine + "GameUtils.IsOnVacation = " + GameUtils.IsOnVacation();

            msg += Common.NewLine + " CurrentWorldType = " + GameUtils.GetCurrentWorldType();

            if (GameStates.sTravelData != null)
            {
                msg += Common.NewLine + "sTravelData";
                msg += Common.NewLine + "  mCurrentDayOfTrip = " + GameStates.sTravelData.mCurrentDayOfTrip;
                msg += Common.NewLine + "  mHomeWorld = " + GameStates.sTravelData.mHomeWorld;
                msg += Common.NewLine + "  mSaveName = " + GameStates.sTravelData.mSaveName;
                msg += Common.NewLine + "  mTimeInHomeworld = " + GameStates.sTravelData.mTimeInHomeworld;

                if (GameStates.sTravelData.mTravelHouse != null)
                {
                    msg += Common.NewLine + "  mTravelHouse = " + GameStates.sTravelData.mTravelHouse.Name;

                    foreach (SimDescription member in Households.All(GameStates.sTravelData.mTravelHouse))
                    {
                        msg += Common.NewLine + "    " + member.FullName;
                    }
                }
                else
                {
                    msg += Common.NewLine + "  mTravelHouse = null";
                }
            }
            else
            {
                msg += Common.NewLine + "sTravelData = null";
            }

            msg += FileNameBooter.LookupToString();

            msg += WorldData.GetWorldDataToString();

            Common.Notify(msg);

            Common.WriteLog(msg);

            return OptionResult.SuccessClose;
        }
    }
}
