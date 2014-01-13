using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.VectorSpace.Options.Diseases
{
    public class WatchesSetting : BooleanSettingOption<GameObject>, ISettingOption
    {
        string mGuid;

        public WatchesSetting()
        { }
        public WatchesSetting(string guid)
        {
            mGuid = guid;
        }

        protected override bool Value
        {
            get
            {
                return !Vector.Settings.IsIgnored(mGuid);
            }
            set
            {
                Vector.Settings.SetIgnore(mGuid, !value);
            }
        }

        public override string GetTitlePrefix()
        {
            return "Watches";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public override void Import(Persistence.Lookup settings)
        {
            Vector.Settings.ClearIgnore();

            foreach(string ignore in settings.GetStringList("Ignored"))
            {
                Vector.Settings.SetIgnore(ignore, true);
            }
        }

        public override void Export(Persistence.Lookup settings)
        {
            List<string> value = new List<string>();
            foreach(VectorBooter.Data vector in VectorBooter.Vectors)
            {
                if (Vector.Settings.IsIgnored(vector.Guid))
                {
                    value.Add(vector.Guid);
                }
            }

            settings.Add("Ignored", value);
        }

        public override string PersistencePrefix
        {
            get { return "Disease"; }
        }
    }
}
