using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
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
using System.Runtime.CompilerServices;
using System.Text;

namespace NRaas
{
    public class MasterController : Common, Common.IWorldLoadFinished
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        [PersistableStatic]
        protected static PersistedSettings sSettings = null;

        protected static string sNamespace = string.Empty;
        protected static List<string> sForbiddenCrit;

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
            SaveFilterSetting setting = new SaveFilterSetting();

            return setting.Find(name);
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
                bool simSpecific = false;
                foreach (SimSelection.ICriteria crit in filter.Elements)
                {                   
                    if (crit is ActiveSim || crit is PriorCareer || crit is RelationBase || crit is LongTermRelationshipEx || crit is RelationListing)
                    {                       
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

                bool simSpecific = false;
                foreach (SimSelection.ICriteria crit in filter.Elements)
                {
                    if (crit is ActiveSim || crit is PriorCareer || crit is RelationBase || crit is LongTermRelationshipEx || crit is RelationListing)
                    {                       
                        simSpecific = true;
                        break;
                    }
                }                

                result.Add(filter.Name, simSpecific);
                break;
            }

            return result;
        }

        // use mc to define criteria layout, no hard set filter matching as with go here.
        // seperate menu to define score hit or miss values. Hit/miss can be negative or pos
        // way to prevent scores from getting out of hand or user overwhelment by providing general "matters a bit", "matters a lot" values that 
        // hard set generic values (which can be customized)
        // fiter update by testing if sim matches values and apply the hit miss
        public static string GetLocalizedFilterCriteria(List<string> filters)
        {
            string result = string.Empty;

            foreach (string filter in filters)
            {
                SavedFilter filter2 = GetFilter(filter);

                if (filter2 != null)
                {
                    foreach (SimSelection.ICriteria crit in filter2.Elements)
                    {
                        ITestableOption option = crit as ITestableOption;
                        result += crit.Name + Common.NewLine;
                        if (option != null)
                        {
                            // this is returning all because OptionName is a loop of the options internally
                            result += option.OptionName + Common.NewLine;
                        }
                    }
                }
            }

            return result;
        }

        public static List<string> GetAllCriteria(bool unUsed)
        {
            return new List<string>(SelectionOption.StringList.Keys);
        }

        public static List<string> GetAllCriteriaOptions(string criteria, IMiniSimDescription actor)
        {
            List<string> results = new List<string>();

            SimSelection.ICriteria pick = null;

            if (!SelectionOption.StringList.ContainsKey(criteria))
            {
                return results;
            }

            pick = SelectionOption.StringList[criteria];

            if (pick != null)
            {
                if (actor == null && PlumbBob.SelectedActor != null)
                {
                    actor = PlumbBob.SelectedActor.SimDescription.GetMiniSimDescription();
                }

                List<IMiniSimDescription> picks = new List<IMiniSimDescription>();
                foreach (List<IMiniSimDescription> sims in SimListing.AllSims(actor, false).Values)
                {
                    foreach (IMiniSimDescription sim in sims)
                    {
                        if (SimSelection.IsSpecial(sim))
                        {
                            continue;
                        }

                        picks.Add(sim);
                    }
                }

                List<ICommonOptionItem> options = pick.GetOptions(actor, new List<SimSelection.ICriteria>(), picks);

                foreach (ICommonOptionItem opt in options)
                {
                    results.Add(opt.Name);
                }
            }

            return results;
        }

        // settled on a seperate interface within attraction profiles to set certain criteria as random which will use the above to pull the options allowing users to allow/disallow criteria. Problems begin with localization of values when handling them as strings. I'll deal with that later.
        public static string CreateFilterWithCriteria(string callingNamespace, IMiniSimDescription actor, List<SimSelection.ICriteriaCreation> creationData)
        {
            if (creationData == null)
            {
                Common.Notify("Creation Data null");
                return string.Empty;
            }

            if (actor == null && PlumbBob.SelectedActor != null)
            {
                actor = PlumbBob.SelectedActor.SimDescription.GetMiniSimDescription();
            }

            List<IMiniSimDescription> picks = new List<IMiniSimDescription>();
            foreach (List<IMiniSimDescription> sims in SimListing.AllSims(actor, false).Values)
            {
                foreach (IMiniSimDescription sim in sims)
                {
                    if (SimSelection.IsSpecial(sim))
                    {
                        continue;
                    }

                    picks.Add(sim);
                }
            }

            List<SimSelection.ICriteria> finalCriteria = new List<SimSelection.ICriteria>();

            foreach (SimSelection.ICriteriaCreation cData in creationData)
            {
                List<SimSelection.ICriteria> validCriteria = new List<SimSelection.ICriteria>();

                List<string> forbiddenCrit = new List<string>();
                if (cData.RandomCriteria && cData.ForbiddenRandomCriteria != null)
                {
                    forbiddenCrit = cData.ForbiddenRandomCriteria;
                }

                foreach (SimSelection.ICriteria crit in SelectionOption.List)
                {
                    if (crit.Name == cData.CriteriaName)
                    {
                        validCriteria.Add(crit);
                        break;
                    }

                    if (!forbiddenCrit.Contains(crit.Name))
                    {
                        validCriteria.Add(crit);
                    }
                }

                SimSelection.ICriteria pickedCritera = null;
                if (validCriteria.Count == 1)
                {
                    pickedCritera = validCriteria[0];
                }
                else
                {
                    int loop = 0;
                    while (true)
                    {
                        if(loop > 4)
                        {
                            break;
                        }

                        pickedCritera = RandomUtil.GetRandomObjectFromList<SimSelection.ICriteria>(validCriteria);

                        if(finalCriteria.Contains(pickedCritera))
                        {
                            loop++;
                            continue;
                        }
                    }
                }

                if (pickedCritera == null)
                {
                    Common.Notify("pickedCriteria was null");
                    continue;
                }

                List<ICommonOptionItem> criteriaOptions = pickedCritera.GetOptions(actor, new List<SimSelection.ICriteria>(), picks);

                if (criteriaOptions == null || criteriaOptions.Count == 0)
                {
                    Common.Notify("criteriaOptions null or 0");
                    continue;
                }

                List<string> forbiddenOptions = new List<string>();
                if (cData.RandomOptions && cData.ForbiddenRandomOptions != null)
                {
                    forbiddenOptions = cData.ForbiddenRandomOptions;
                }

                List<ICommonOptionItem> finalOptions = new List<ICommonOptionItem>();
                List<ICommonOptionItem> validRandomOptions = new List<ICommonOptionItem>();

                foreach (ICommonOptionItem opt in criteriaOptions)
                {
                    if (cData.CriteriaOptions != null && cData.CriteriaOptions.Contains(opt.Name) && !finalOptions.Contains(opt))
                    {
                        finalOptions.Add(opt);

                        if (validRandomOptions.Contains(opt))
                        {
                            validRandomOptions.Remove(opt);
                        }

                        continue;
                    }

                    if (cData.RandomOptions && !forbiddenOptions.Contains(opt.Name) && !finalOptions.Contains(opt))
                    {
                        validRandomOptions.Add(opt);
                    }
                }

                if (validRandomOptions.Count > 0)
                {
                    List<ICommonOptionItem> pickedRandomOptions = new List<ICommonOptionItem>();
                    if (cData.MinMaxRandomOptions != null || cData.MinMaxRandomOptions.Length == 2)
                    {
                        int numOpt = RandomUtil.GetInt(cData.MinMaxRandomOptions[0], cData.MinMaxRandomOptions[1]);

                        if (numOpt != 0)
                        {
                            while (true)
                            {
                                if (validRandomOptions.Count == 0 || pickedRandomOptions.Count == numOpt)
                                {
                                    break;
                                }

                                ICommonOptionItem opt = RandomUtil.GetRandomObjectFromList<ICommonOptionItem>(validRandomOptions);

                                if (opt != null)
                                {
                                    pickedRandomOptions.Add(opt);
                                    validRandomOptions.Remove(opt);
                                }
                            }
                        }

                        finalOptions.AddRange(pickedRandomOptions);
                    }
                }

                pickedCritera.SetOptions(finalOptions);

                finalCriteria.Add(pickedCritera);
            }

            if (finalCriteria.Count > 0)
            {
                string filterName;

                while (true)
                {
                    filterName = callingNamespace + ".SimAttractionFilter" + RandomGen.NextDouble();

                    if (GetFilter(filterName) != null)
                    {
                        continue;
                    }
                    else
                    {
                        MasterController.Settings.mFilters.Add(new SavedFilter(filterName, finalCriteria));
                        break;
                    }
                }

                return filterName;
            }

            return string.Empty;
        }

        // this needs to be updated to use the cleaner ICreationData interface
        public static string CreateAndReturnRandomFilter(string callingNamespace, IMiniSimDescription actor, List<string> forbiddenCrit, Dictionary<string, string> forbiddenOptions, int[] minMaxCrit, Dictionary<string, int[]> minMaxOptions)
        {
            List<SimSelection.ICriteria> validCriteria = new List<SimSelection.ICriteria>();

            foreach (SimSelection.ICriteria crit in SelectionOption.List)
            {
                if (!forbiddenCrit.Contains(crit.Name))
                {
                    validCriteria.Add(crit);
                }
            }

            if (validCriteria.Count == 0)
            {
                return string.Empty;
            }

            if (actor == null && PlumbBob.SelectedActor != null)
            {
                actor = PlumbBob.SelectedActor.SimDescription.GetMiniSimDescription();
            }

            List<IMiniSimDescription> picks = new List<IMiniSimDescription>();
            foreach (List<IMiniSimDescription> sims in SimListing.AllSims(actor, false).Values)
            {
                foreach (IMiniSimDescription sim in sims)
                {
                    if (SimSelection.IsSpecial(sim))
                    {
                        continue;
                    }

                    picks.Add(sim);
                }
            }

            if (picks.Count == 0)
            {
                return string.Empty;
            }

            if (minMaxCrit.Length < 2)
            {
                minMaxCrit[0] = 1;
                minMaxCrit[1] = 2;
            }

            int critpicks = RandomUtil.GetInt(minMaxCrit[0], minMaxCrit[1]);

            Common.Notify("Picking " + critpicks + " from " + validCriteria.Count);

            List<SimSelection.ICriteria> finalPicks = new List<SimSelection.ICriteria>();

            if (validCriteria.Count == critpicks)
            {
                finalPicks = validCriteria;
            }
            else
            {
                while (true)
                {
                    if (validCriteria.Count < critpicks && finalPicks.Count == validCriteria.Count)
                    {
                        break;
                    }

                    if (finalPicks.Count < critpicks)
                    {
                        SimSelection.ICriteria critpick = RandomUtil.GetRandomObjectFromList<SimSelection.ICriteria>(validCriteria);
                        if (!finalPicks.Contains(critpick))
                        {
                            finalPicks.Add(critpick);
                        }
                            continue;
                        }

                    break;
                }
            }

            bool failed = false;
            foreach (SimSelection.ICriteria crit2 in finalPicks)
            {
                Common.Notify("Picked " + crit2.Name);
                List<ICommonOptionItem> finalOpts = new List<ICommonOptionItem>();
                List<ICommonOptionItem> opts = crit2.GetOptions(actor, finalPicks, picks);

                if (opts != null && opts.Count > 0)
                {
                    Common.Notify("Opts not null");
                    int optpicks = 0;

                    if (minMaxOptions.ContainsKey(crit2.Name) && minMaxOptions[crit2.Name].Length > 1)
                    {
                        optpicks = RandomUtil.GetInt(minMaxOptions[crit2.Name][0], minMaxOptions[crit2.Name][1]);
                    }
                    else
                    {
                        optpicks = 1;
                    }

                    Common.Notify("Picking " + optpicks + " from " + opts.Count);

                    if (opts.Count == optpicks)
                    {
                        finalOpts = opts;
                    }
                    else
                    {
                        while (true)
                        {
                            if (opts.Count < optpicks && finalOpts.Count == opts.Count)
                            {
                                break;
                            }

                            if (finalOpts.Count < optpicks)
                            {
                                ICommonOptionItem opt = RandomUtil.GetRandomObjectFromList<ICommonOptionItem>(opts);
                                ITestableOption testOpt = opt as ITestableOption;
                                if (!finalOpts.Contains(opt) && (!forbiddenOptions.ContainsKey(crit2.Name) || (testOpt != null && !forbiddenOptions[crit2.Name].Contains(testOpt.OptionName))))
                                {
                                    // test if this gives me the name, if so we can revert the changes to ITestableOption
                                    Common.Notify("Picked " + opt.Name);
                                    finalOpts.Add(opt);
                                }
                                continue;
                            }
                            break;
                        }
                    }

                    crit2.SetOptions(finalOpts);
                }
                else
                {
                    failed = true;
                    break;
                }
            }

            if (failed)
            {
                Common.Notify("Failed");
                return string.Empty;
            }

            string filterName;

            while (true)
            {
                filterName = callingNamespace + ".SimAttractionFilter" + RandomGen.NextDouble();

                if (GetFilter(filterName) != null)
                {
                    continue;
                }
                else
                {
                    MasterController.Settings.mFilters.Add(new SavedFilter(filterName, finalPicks));
                    break;
                }
            }

            Common.Notify("Set and returning filter " + filterName);

            return filterName;
        }
                       

        public static void OnMenuAlarmCreate()
        {            
            new MasterControllerSpace.Settings.SaveFilterSetting().RunExternal(sNamespace, sForbiddenCrit);
        }

        public static void OnMenuAlarmDelete()
        {
            new MasterControllerSpace.Settings.DeleteFilterSetting().RunExternal(sNamespace); 
        }

        // this is like this because the dialog system apparently doesn't like being called via invocation. It throws a attempting to yield in an unyielding context error when the second dialog is displayed to select the critera (due to the call to pause the game which all twallan dialogs are set to do and I don't want to touch that). I don't know why but, although hacky, this works.
        public static OptionResult CreateFilter(string callingNamespace, List<string> forbiddenCrit)
        {
            sNamespace = callingNamespace;
            sForbiddenCrit = forbiddenCrit;
            new AlarmTask(1, TimeUnit.Seconds, OnMenuAlarmCreate);            

            return OptionResult.Unset;
        }

        public static OptionResult DeleteFilter(string callingNamespace)
        {
            sNamespace = callingNamespace;
            new AlarmTask(1, TimeUnit.Seconds, OnMenuAlarmDelete);

            return OptionResult.Unset;
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
