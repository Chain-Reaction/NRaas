using NRaas.ChemistrySpace.Helpers;
using NRaas.MasterControllerSpace.SelectionCriteria;
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

namespace NRaas.ChemistrySpace.SelectionCriteria
{
    public class Any : SelectionOption, ICloneable
    {
        public List<int> mMinHitValues = new List<int>();
        public List<int> mMaxHitValues = new List<int>();
        public List<int> mMinMissValues = new List<int>();
        public List<int> mMaxMissValues = new List<int>();

        public List<int> mHitValues = new List<int>();
        public List<int> mMissValues = new List<int>();

        public List<bool> mCanBeRandomValues = new List<bool>();

        public Dictionary<int, Dictionary<int, float>> mAttrILChance = new Dictionary<int, Dictionary<int, float>>();

        public List<string> mValidRandomCrit = new List<string>();
        public Dictionary<int, List<string>> mValidRandomOpts = new Dictionary<int, List<string>>();

        public string mCriteria = string.Empty;

        public Any()
        {
        }

        public override string GetTitlePrefix()
        {
            return "Criteria.Any";
        }

        protected override bool Allow(CommonSpace.Options.MiniSimDescriptionParameters parameters)
        {
            return base.Allow(parameters);
        }

        protected override bool Allow(SimDescription me, IMiniSimDescription actor)
        {
            return true;
        }

        public override void Reset()
        {
            base.Reset();

            mMinHitValues.Clear();
            mMaxHitValues.Clear();

            mMinMissValues.Clear();
            mMaxMissValues.Clear();

            mHitValues.Clear();
            mMissValues.Clear();

            mCanBeRandomValues.Clear();

            mAttrILChance.Clear();

            mCriteria = string.Empty;
        }

        public Any Clone()
        {
            Any any = (Any)this.MemberwiseClone();
            any.mAttrILChance = new Dictionary<int, Dictionary<int, float>>(mAttrILChance);
            return any;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
