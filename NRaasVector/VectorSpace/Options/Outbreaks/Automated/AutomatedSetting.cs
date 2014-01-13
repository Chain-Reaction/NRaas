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

namespace NRaas.VectorSpace.Options.Outbreaks.Automated
{
    public class AutomatedSetting : BooleanSettingOption<GameObject>, IAutomatedOption
    {
        string mGuid;

        public AutomatedSetting()
        { }
        public AutomatedSetting(string guid)
        {
            mGuid = guid;
        }

        protected override bool Value
        {
            get
            {
                return Vector.Settings.IsAutomated(mGuid);
            }
            set
            {
                Vector.Settings.SetAutomated(mGuid, value);
            }
        }

        public override string GetTitlePrefix()
        {
            return "AutomatedOutbreaks";
        }

        public override string Name
        {
            get
            {
                return VectorBooter.GetVector(mGuid).GetLocalizedName(false);
            }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public override void Import(Persistence.Lookup settings)
        {
            Vector.Settings.ClearAutomated();

            foreach (string guid in settings.GetStringList("Automated"))
            {
                Vector.Settings.SetAutomated(guid, true);
            }
        }

        public override void Export(Persistence.Lookup settings)
        {
            List<string> value = new List<string>();
            foreach (VectorBooter.Data vector in VectorBooter.Vectors)
            {
                if (Vector.Settings.IsAutomated(vector.Guid))
                {
                    value.Add(vector.Guid);
                }
            }

            settings.Add("Automated", value);
        }

        public override string PersistencePrefix
        {
            get { return "Outbreaks"; }
        }
    }
}
