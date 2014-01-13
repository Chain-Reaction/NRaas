using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RelativitySpace.Options.Speeds.Interval
{
    public class ListingOption : InteractionOptionList<IIntervalOption, GameObject>, ISpeedsOption
    {
        SpeedInterval mInterval;

        new int mCount;

        public ListingOption(int count, SpeedInterval interval)
        {
            mCount = count;
            mInterval = interval;
        }

        public override string GetTitlePrefix()
        {
            return null;
        }

        public override string Name
        {
            get
            {
                return mCount.ToString("D2") + ": " + mInterval.ToString();
            }
        }

        public override string DisplayValue
        {
            get
            {
                return EAText.GetNumberString(mInterval.mSpeed);
            }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new Speeds.ListingOption(); }
        }

        public override List<IIntervalOption> GetOptions()
        {
            List<IIntervalOption> results = new List<IIntervalOption>();

            foreach (IIntervalOption option in Common.DerivativeSearch.Find<IIntervalOption>())
            {
                results.Add(option.Clone(mInterval));
            }

            return results;
        }
    }
}
