using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
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

namespace NRaas.MasterControllerSpace.Sims
{
    public class FilteredSelection : SimFromList
    {
        public TestFunc mOnTest;

        public delegate bool TestFunc(SimDescription sim);

        protected FilteredSelection(TestFunc onTest)
        {
            mOnTest = onTest;
        }

        public override string GetTitlePrefix()
        {
            return null;
        }

        protected override int GetMaxSelection()
        {
            return 1;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (mOnTest != null)
            {
                if (!mOnTest(me)) return false;
            }

            return true;
        }

        protected override bool AllowRunOnActive
        {
            get { return false; }
        }

        public static List<IMiniSimDescription> Selection(IMiniSimDescription me, string title, TestFunc onTest, ICollection<SimSelection.ICriteria> criteria, int maxSelection, bool canApplyAll, out bool okayed)
        {
            return new FilteredSelection(onTest).GetSelection(me, title, criteria, maxSelection, canApplyAll, out okayed);
        }
    }
}
