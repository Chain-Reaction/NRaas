using NRaas.CommonSpace.Helpers;
using NRaas.MasterControllerSpace.Sims;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    [Persistable]
    public class SavedFilter : IPersistence
    {
        private string mName;

        private List<SimSelection.ICriteria> mElements = null;

        public SavedFilter()
        {}
        public SavedFilter(string name, IEnumerable<SimSelection.ICriteria> elements)
        {
            mName = name;

            mElements = new List<SimSelection.ICriteria>();
            foreach (SimSelection.ICriteria crit in elements)
            {
                mElements.Add(crit.Clone() as SimSelection.ICriteria);
            }
        }

        public string Name
        {
            get
            {
                return mName;
            }
        }

        public ICollection<SimSelection.ICriteria> Elements
        {
            get
            {
                return mElements;
            }
        }

        public void Import(Persistence.Lookup settings)
        {
            mName = settings.GetString("Name");

            mElements = settings.GetList<SimSelection.ICriteria>("Criteria");
        }

        public void Export(Persistence.Lookup settings)
        {
            if (mName == null) return;

            settings.Add("Name", mName);

            settings.Add("Criteria", mElements);
        }

        public string PersistencePrefix
        {
            get { return null; }
        }

        public static List<SimSelection.ICriteria> GetOptions()
        {
            List<SimSelection.ICriteria> items = new List<SimSelection.ICriteria>();
            
            foreach(SavedFilter filter in NRaas.MasterController.Settings.mFilters)
            {
                items.Add(new Item(filter));
            }

            return items;
        }

        public class Item : SelectionOption
        {
            SavedFilter mFilter;

            public Item(SavedFilter filter)
            {
                mFilter = filter;
            }

            public override string GetTitlePrefix()
            {
                return null;
            }

            public override string Name
            {
                get { return mFilter.mName; }
            }

            protected override bool Allow(SimDescription me, IMiniSimDescription actor)
            {
                if (mFilter == null) return false;

                foreach (SimSelection.ICriteria item in mFilter.mElements)
                {
                    ITestableOption testable = item as ITestableOption;
                    if (testable == null) continue;

                    if (!testable.Test(me, false, actor))
                    {
                        return false;
                    }
                }

                return true;
            }

            protected override bool Allow(MiniSimDescription me, IMiniSimDescription actor)
            {
                if (mFilter == null) return false;

                foreach (SimSelection.ICriteria item in mFilter.mElements)
                {
                    ITestableOption testable = item as ITestableOption;
                    if (testable == null) continue;

                    if (!testable.Test(me, false, actor))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
