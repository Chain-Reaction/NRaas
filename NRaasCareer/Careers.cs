using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Stores;
using NRaas.CareerSpace;
using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
    public class Careers : Common, Common.IWorldLoadFinished
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        [PersistableStatic]
        static PersistedSettings sSettings = null;

        static Careers()
        {
            sEnableLoadLog = true;

            Bootstrap();

            // Loaded in a Specific order
            BooterHelper.Add(new SkillBooter(BooterHelper.sBootStrapFile, false));
            BooterHelper.Add(new AfterschoolActivityBooter(BooterHelper.sBootStrapFile));
            BooterHelper.Add(new OpportunityBooter(BooterHelper.sBootStrapFile, false));
            BooterHelper.Add(new SkillBasedCareerBooter(BooterHelper.sBootStrapFile, false));
            BooterHelper.Add(new CareersBooter(BooterHelper.sBootStrapFile, false));
            BooterHelper.Add(new ToneBooter(BooterHelper.sBootStrapFile));
            BooterHelper.Add(new BookBooter(BooterHelper.sBootStrapFile, false));
            BooterHelper.Add(new AcademicDegreeBooter(BooterHelper.sBootStrapFile, false));
            BooterHelper.Add(new JobsBooter(BooterHelper.sBootStrapFile, false));

            foreach (IHomemakerBooter booter in DerivativeSearch.Find<IHomemakerBooter>())
            {
                BooterHelper.Add(booter);
            }

            BooterHelper.Add(new SocializingBooter());
            BooterHelper.Add(new SocializingBooter(BooterHelper.sBootStrapFile, false));

            //BooterLogger.AddError("Force Log");
        }

        public void OnWorldLoadFinished()
        {
            CareerStore.OnWorldLoadFinished();

            kDebugging = Settings.Debugging;

            if (Sim.kTeenEndCurfewHour > 3)
            {
                Sim.kTeenEndCurfewHour = 3;
            }

            // Fix EA's genius wake up times            
            foreach (Career career in CareerManager.CareerList)
            {               
                foreach(KeyValuePair<string, Dictionary<int, CareerLevel>> data in career.CareerLevels)
                {                    
                    foreach (KeyValuePair<int, CareerLevel> data2 in data.Value)
                    {                        
                        if (data2.Value.StartTime - data2.Value.WakeupTime == 1f)
                        {                           
                            data2.Value.WakeupTime = data2.Value.StartTime - 2f;
                        }
                    }
                }                
            }
            
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

        // Externalized to MasterController and StoryProgression
        public static bool PerformAfterschoolPreLoop(GoToSchoolInRabbitHole interaction, AfterschoolActivity activity)
        {
            try
            {
                return AfterschoolActivityEx.PerformAfterschoolPreLoop(interaction, activity);
            }
            catch (Exception e)
            {
                Common.Exception("GetAfterSchoolDelegate", e);
                return false;
            }
        }

        // Externalized to MasterController and StoryProgression
        public static InteractionInstance.InsideLoopFunction PerformAfterschoolLoop(GoToSchoolInRabbitHole interaction, AfterschoolActivity activity)
        {
            try
            {
                return AfterschoolActivityEx.PerformAfterschoolLoop(interaction, activity);
            }
            catch (Exception e)
            {
                Common.Exception("GetAfterSchoolDelegate", e);
                return null;
            }
        }

        // Externalized to MasterController and StoryProgression
        public static bool PerformAfterschoolPostLoop(GoToSchoolInRabbitHole interaction, AfterschoolActivity activity)
        {
            try
            {
                return AfterschoolActivityEx.PerformAfterschoolPostLoop(interaction, activity);
            }
            catch (Exception e)
            {
                Common.Exception("GetAfterSchoolDelegate", e);
                return false;
            }
        }

        // Externalized to MasterController and StoryProgression
        public static List<AfterschoolActivity> GetAfterSchoolActivityList(SimDescription sim)
        {
            try
            {
                return AfterschoolActivityBooter.GetActivityList(sim);
            }
            catch (Exception e)
            {
                Common.Exception("GetAfterSchoolActivityList", e);
                return null;
            }
        }

        // Externalized to MasterController and Traveler
        public static string JobToString (JobId job)
        {
            try
            {
                return JobsBooter.JobToString(job);
            }
            catch (Exception e)
            {
                Common.Exception("JobToString", e);
                return null;
            }
        }
    }
}
