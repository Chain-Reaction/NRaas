using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using NRaas.MasterControllerSpace.Sims;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace
{
    public class SimSelection : ProtoSimSelection<IMiniSimDescription>
    {
        SimFromList mSimFromList;

        private SimSelection(string title, IMiniSimDescription me, SimFromList simsFromList, bool lastFirst, bool addAll)
            : this(title, me, simsFromList, null, lastFirst, addAll)
        { }
        private SimSelection(string title, IMiniSimDescription me, SimFromList simsFromList, ICollection<IMiniSimDescription> sims, bool lastFirst, bool addAll)
            : base(title, me, sims, lastFirst, addAll)
        {
            mSimFromList = simsFromList;

            ObjectPickerDialogEx.CommonHeaderInfo<IMiniSimDescription> column = null;

            if (mSimFromList != null)
            {
                column = mSimFromList.GetAuxillaryColumn();
            }

            if (column == null)
            {
                column = new SimAgeColumn();
            }
            
            AddColumn(column);
        }

        public static SimSelection Create(string title, IMiniSimDescription me, SimFromList simsFromList, ICollection<ICriteria> criteria, bool lastFirst, bool addAll, out bool criteriaCanceled)
        {
            return Create(title, me, simsFromList, criteria, lastFirst, addAll, false, out criteriaCanceled);
        }
        public static SimSelection Create(string title, IMiniSimDescription me, SimFromList simsFromList, ICollection<ICriteria> criteria, bool lastFirst, bool addAll, bool automatic, out bool criteriaCanceled)
        {
            SimSelection selection = new SimSelection(title, me, simsFromList, lastFirst, addAll);
            selection.FilterSims(criteria, SavedFilter.GetOptions(), automatic, out criteriaCanceled);
            return selection;
        }
        public static SimSelection Create(string title, IMiniSimDescription me, SimFromList simsFromList, ICollection<IMiniSimDescription> sims, bool lastFirst, bool addAll)
        {
            return new SimSelection(title, me, simsFromList, sims, lastFirst, addAll);
        }

        protected override bool Allow(IMiniSimDescription item)
        {
            if (mSimFromList != null)
            {
                return mSimFromList.Test(item);
            }

            return true;
        }

        protected override IEnumerable<ICriteria> AlterCriteria(IEnumerable<ICriteria> allCriteria, bool manual, bool canceled)
        {
            if (mSimFromList != null)
            {
                allCriteria = mSimFromList.AlterCriteria(allCriteria, manual, canceled);
            }

            if ((mSimFromList != null) && (MasterController.Settings.mDefaultSpecies.Count > 0) && (manual))
            {
                List<ICriteria> results = new List<ICriteria>(allCriteria);

                bool speciesFound = false;
                foreach (ICriteria criteria in allCriteria)
                {
                    if (criteria is MostRecentFilter)
                    {
                        results.Remove(criteria);

                        results.AddRange(MasterController.Settings.mMostRecentFilter.Elements);
                    }

                    // handles adding random criteria to manual user selection
                    /*
                    if (criteria is Any)
                    {
                        List<ICriteria> available = new List<ICriteria>(SelectionOptionBase.List);
                        int num = 0;
                        int max = 0;
                        if (criteria.MinRandomOptions > 0 || criteria.MaxRandomOptions > 0)
                        {
                            max = RandomUtil.GetInt(criteria.MinRandomOptions, criteria.MaxRandomOptions);
                        }
                        else
                        {
                            max = 1;
                        }

                        while (num < max && available.Count > 0)
                        {
                            ICriteria crit = RandomUtil.GetRandomObjectFromList<ICriteria>(available);

                            available.Remove(crit);
                            results.Add(crit);
                            num ++;
                        }
                    }
                     */

                    if (criteria is IDoesNotNeedSpeciesFilter)
                    {
                        speciesFound = true;
                        break;
                    }
                }

                if (!speciesFound)
                {
                    IEnumerable<CASAgeGenderFlags> species = mSimFromList.GetSpeciesFilter();
                    if (species != null)
                    {
                        results.Add(new Species(species));
                    }
                }

                MasterController.Settings.mMostRecentFilter = new SavedFilter("MostRecent", results);

                return results;
            }
            else
            {
                return allCriteria;
            }
        }

        protected class SimAgeColumn : ObjectPickerDialogEx.CommonHeaderInfo<IMiniSimDescription>
        {
            public SimAgeColumn()
                : base("NRaas.MasterController.SimSelection:AgeTitle", "NRaas.MasterController.SimSelection:AgeTooltip", 40)
            { }

            public override ObjectPicker.ColumnInfo GetValue(IMiniSimDescription sim)
            {
                if (sim == null)
                {
                    return new ObjectPicker.TextColumn("");
                }
                else
                {
                    return new ObjectPicker.TextColumn(((int)Aging.GetCurrentAgeInDays(sim)).ToString("D4"));
                }
            }
        }
    }
}
