using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TaggerSpace.Options.FilterOptions
{
    public class FilterOption : FilterBaseOption<GameObject>, IFilterOption        
    {
        public enum FilterType
        {
            LotTag,
            SimTag
        }
        
        FilterType mType;

        public FilterOption(string filter, FilterType type)
        {
            mName = filter;
            mType = type;
        }        

        protected override List<string> Value
        {
            get
            {
                if (mType == FilterType.LotTag)
                {
                    return Tagger.Settings.mCurrentLotFilters;
                }

                return Tagger.Settings.mCurrentSimFilters;
            }
            set
            {                
                if (mType == FilterType.LotTag)
                {
                    if (Tagger.Settings.mCurrentLotFilters.Contains(mName))
                    {
                        Tagger.Settings.mCurrentLotFilters.Remove(mName);
                    }
                    else
                    {
                        Tagger.Settings.mCurrentLotFilters.Add(mName);
                    }
                }
                else
                {
                    if (Tagger.Settings.mCurrentSimFilters.Contains(mName))
                    {
                        Tagger.Settings.mCurrentSimFilters.Remove(mName);
                    }
                    else
                    {
                        Tagger.Settings.mCurrentSimFilters.Add(mName);
                    }
                }
            }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override string GetTitlePrefix()
        {
            return null;
        }

        public override string DisplayValue
        {
            get
            {
                return (Value.Contains(mName) ? "True" : "False");
            }
        }

        public override string Name
        {
            get
            {
                return FilterHelper.StripNamespace(mName);
            }
        }
    }
}