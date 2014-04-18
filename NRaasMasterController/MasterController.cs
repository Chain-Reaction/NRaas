using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.MasterControllerSpace;
using NRaas.MasterControllerSpace.Helpers;
using NRaas.MasterControllerSpace.Interactions;
using NRaas.MasterControllerSpace.Settings;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
    public class MasterController : Common, Common.IWorldLoadFinished
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        [PersistableStatic]
        protected static PersistedSettings sSettings = null;

        static MasterController()
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

        public void OnWorldLoadFinished()
        {
            DreamCatcher.OnWorldLoadFinishedDreams();

            Settings.ApplyBlacklistParts();

            RemoveActiveTopicLimitSetting.Perform(Settings.mRemoveActiveTopicLimit);

            new AlarmTask(1f, TimeUnit.Seconds, OnStartup);
        }

        // Externalized to Dresser
        public static bool Allow(CASPart part, CASAgeGenderFlags age, CASAgeGenderFlags gender, CASAgeGenderFlags species, bool maternity, OutfitCategories category)
        {
            try
            {
                return (InvalidPartBooter.Allow(new CASParts.Wrapper(part), age, gender, species, maternity, category) == InvalidPartBase.Reason.None);
            }
            catch (Exception e)
            {
                Common.Exception("Allow", e);
                return true;
            }
        }

        // Externalized to StoryProgression
        public static Dictionary<string, Dictionary<SkillNames, int>> GetSkillStamps(Dictionary<string, Dictionary<SkillNames, int>> stamps)
        {
            foreach (SkillStamp stamp in MasterController.Settings.SkillStamps)
            {
                stamps.Add(stamp.Name, new Dictionary<SkillNames,int>(stamp.Skills));
            }

            return stamps;
        }

        protected static void OnStartup()
        {
            try
            {
#if _NEXTPHASE
                SimpleMessageDialog.Show("NEXTPHASE", "This is a Next Phase build of MasterController.");
#endif
            }
            catch (Exception exception)
            {
                Exception("OnStartup", exception);
            }
        }
    }
}
