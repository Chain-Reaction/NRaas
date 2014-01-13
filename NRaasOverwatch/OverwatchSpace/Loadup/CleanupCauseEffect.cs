using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupCauseEffect : Common.IWorldLoadFinished
    {
        public void OnWorldLoadFinished()
        {
            Overwatch.Log("CleanupCauseEffect");

            CauseEffectService instance = CauseEffectService.GetInstance();
            if (instance != null)
            {
                ulong timeTravelerSimID = instance.GetTimeTravelerSimID();
                if (MiniSimDescription.Find(timeTravelerSimID) == null)
                {
                    CauseEffectService.sPersistableData.TimeTravelerSimID = 0;

                    Overwatch.Log(" Removed Corrupt Time Traveler Referece");
                }

                List<ITimeStatueUiData> timeStatueData = instance.GetTimeAlmanacTimeStatueData();
                if (timeStatueData != null)
                {
                    foreach (ITimeStatueUiData data in timeStatueData)
                    {
                        TimeStatueRecordData record = data as TimeStatueRecordData;
                        if (record == null) continue;

                        if (MiniSimDescription.Find(record.mRecordHolderId) == null)
                        {
                            record.mRecordHolderId = 0;

                            Overwatch.Log(" Removed Corrupt Record Holder Referece");
                        }
                    }
                }
            }
        }
    }
}
