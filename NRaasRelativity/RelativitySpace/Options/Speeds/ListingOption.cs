using NRaas.CommonSpace.Helpers;
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

namespace NRaas.RelativitySpace.Options.Speeds
{
    public class ListingOption : InteractionOptionList<ISpeedsOption, GameObject>, IPrimaryOption<GameObject>, IPersistence
    {
        public override string GetTitlePrefix()
        {
            return "Intervals";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override List<ISpeedsOption> GetOptions()
        {
            List<ISpeedsOption> results = new List<ISpeedsOption>();

            results.Add(new AddInterval());
            results.Add(new DumpSpeeds());

            int count = 1;

            foreach (SpeedInterval interval in Relativity.Settings.Intervals)
            {
                if ((interval.mWorld != GameUtils.GetCurrentWorld()) && (interval.mWorld != WorldName.Undefined)) continue;

                results.Add(new Interval.ListingOption(count, interval));
                count++;
            }

            return results;
        }

        public void Import(Persistence.Lookup settings)
        {
            Relativity.Settings.Intervals.Clear();
            
            foreach(SpeedInterval interval in settings.GetList<SpeedInterval>("Intervals"))
            {
                Relativity.Settings.Add(interval);
            }
        }

        public void Export(Persistence.Lookup settings)
        {
            settings.Add("Intervals", Relativity.Settings.Intervals);
        }

        public string PersistencePrefix
        {
            get { return GetTitlePrefix(); }
        }
    }
}
