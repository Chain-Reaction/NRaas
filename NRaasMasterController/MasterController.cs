using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.MasterControllerSpace;
using NRaas.MasterControllerSpace.Helpers;
using NRaas.MasterControllerSpace.Interactions;
using NRaas.MasterControllerSpace.SelectionCriteria;
using NRaas.MasterControllerSpace.Sims;
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

        public static SavedFilter GetFilter(string name)
        {
            // duplicated from FilterSettingOption but eh, didn't feel it best to make a method in there static
            name = name.ToLower();

            SavedFilter picked = null;
            foreach (SavedFilter filter in MasterController.Settings.mFilters)
            {
                if (filter.Name.ToLower() == name)
                {
                    picked = filter;
                    break;
                }
            }

            return picked;
        }        
        
        // Externalized to filterable mods
        public static List<ulong> GetSimsMatchingFilter(List<object> filter)
        {
            List<ulong> results = new List<ulong>();

            List<IMiniSimDescription> sims = GetSimsMatchingFilterAsMinis((string)filter[0], (ulong)filter[1]);

            foreach (IMiniSimDescription desc in sims)
            {
                results.Add(desc.SimDescriptionId);
            }

            return results;
        }       

        public static List<IMiniSimDescription> GetSimsMatchingFilterAsMinis(string name, ulong sim)
        {
            List<IMiniSimDescription> results = new List<IMiniSimDescription>();            

            SavedFilter filter = GetFilter(name);

            if (sim == 0 && PlumbBob.SelectedActor != null)
            {
                sim = PlumbBob.SelectedActor.SimDescription.SimDescriptionId;
            }

            if (filter != null && sim != 0)
            {
                bool okayed;
                IMiniSimDescription mini = SimDescription.Find(sim);
                if (mini != null)
                {
                    List<SimSelection.ICriteria> crit = new List<SimSelection.ICriteria>();
                    crit.Add(new SavedFilter.Item(filter));

                    SimSelection selection = SimSelection.Create("", mini, null, crit, true, false, out okayed);
                    if (selection.All != null && selection.All.Count > 0)
                    {
                        results.AddRange(selection.All);
                    }
                }
            }

            return results;
        }        

        // Externalized to filterable mods
        public static Dictionary<string, bool> GetAllFilters(bool unused)
        {
            Dictionary<string, bool> results = new Dictionary<string, bool>();
            
            foreach (SavedFilter filter in MasterController.Settings.mFilters)
            {
                //Common.DebugNotify("GetAllFilters: " + filter.Name);
                bool simSpecific = false;
                foreach (SimSelection.ICriteria crit in filter.Elements)
                {
                   // Common.DebugNotify("crit is " + crit.GetType().ToString());
                    if (crit is ActiveSim || crit is PriorCareer || crit is RelationBase || crit is LongTermRelationshipEx || crit is RelationListing)
                    {
                       // Common.DebugNotify("Filter is SimSpecific");
                        simSpecific = true;
                        break;
                    }
                }

                results.Add(filter.Name, simSpecific);
            }

            return results;
        }

        public static Dictionary<string, bool> GetSingleFilter(string mFilter)
        {
            Dictionary<string, bool> result = new Dictionary<string, bool>();

            foreach (SavedFilter filter in MasterController.Settings.mFilters)
            {
                if (filter.Name != mFilter) continue;

               // Common.DebugNotify("GetSingleFilter: " + mFilter);

                bool simSpecific = false;
                foreach (SimSelection.ICriteria crit in filter.Elements)
                {
                    if (crit is ActiveSim || crit is PriorCareer || crit is RelationBase || crit is LongTermRelationshipEx || crit is RelationListing)
                    {
                       // Common.DebugNotify("Filter is SimSpecific");
                        simSpecific = true;
                        break;
                    }
                }

                result.Add(filter.Name, simSpecific);
                break;
            }

            return result;
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
