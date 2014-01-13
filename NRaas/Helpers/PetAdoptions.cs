using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class PetAdoptions
    {
        public delegate void Logger(string text);

        public static bool Cleanup(Logger log)
        {
            try
            {
                if (PetAdoption.sNeighborAdoption != null)
                {
                    if (PetAdoption.sNeighborAdoption.mPetsToAdopt != null)
                    {
                        for (int i = PetAdoption.sNeighborAdoption.mPetsToAdopt.Count - 1; i >= 0; i--)
                        {
                            SimDescription sim = PetAdoption.sNeighborAdoption.mPetsToAdopt[i];

                            if ((!sim.IsValidDescription) || (sim.Genealogy == null))
                            {
                                if (log != null)
                                {
                                    log("Removed Broken Sim " + sim.FullName);
                                }

                                PetAdoption.sNeighborAdoption.mPetsToAdopt.RemoveAt(i);
                            }
                        }
                    }

                    if (PetAdoption.sNeighborAdoption.mMother != null)
                    {
                        if ((!PetAdoption.sNeighborAdoption.mMother.IsValidDescription) || (PetAdoption.sNeighborAdoption.mMother.Household == null))
                        {
                            PetAdoption.sNeighborAdoption.mMother = null;

                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("Cleanup", e);
            }

            return false;
        }

        public static void Stop(Logger log)
        {
            try
            {
                PetAdoption.ResetNeighborAdoption();

                if (PetAdoption.sPetFromNeighborAddingAlarm != AlarmHandle.kInvalidHandle)
                {
                    AlarmManager.Global.RemoveAlarm(PetAdoption.sPetFromNeighborAddingAlarm);
                    PetAdoption.sPetFromNeighborAddingAlarm = AlarmHandle.kInvalidHandle;

                    if (log != null)
                    {
                        log(" Alarm Disabled");
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("Stop", e);
            }
        }
    }
}

