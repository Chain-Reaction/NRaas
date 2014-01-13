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
    public class MotiveAdjustmentRatioSetting : IntegerSettingOption<GameObject>, ISettingOption
    {
        string mGuid;

        public MotiveAdjustmentRatioSetting()
        { }
        public MotiveAdjustmentRatioSetting(string guid)
        {
            mGuid = guid;
        }

        protected override int Value
        {
            get
            {
                return Vector.Settings.GetMotiveAdjustmentRatio(mGuid);
            }
            set
            {
                Vector.Settings.SetMotiveAdjustmentRatio (mGuid, value);
            }
        }

        public override string GetTitlePrefix()
        {
            return "MotiveAdjustmentRatio";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!Common.kDebugging) return false;

            return base.Allow(parameters);
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public override void Import(Persistence.Lookup settings)
        {
            Vector.Settings.ClearMotiveAdjustmentRatio();

            foreach (VectorBooter.Data vector in VectorBooter.Vectors)
            {
                int value = settings.GetInt(vector.Guid, 100);
                if (value == 100) continue;

                Vector.Settings.SetMotiveAdjustmentRatio(vector.Guid, value);
            }
        }

        public override void Export(Persistence.Lookup settings)
        {
            List<string> value = new List<string>();
            foreach (VectorBooter.Data vector in VectorBooter.Vectors)
            {
                settings.Add(vector.Guid, Vector.Settings.GetMotiveAdjustmentRatio(vector.Guid));
            }
        }

        public override string PersistencePrefix
        {
            get { return "MotiveAdjustmentRatio"; }
        }
    }
}
