using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Stores;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced
{
    public class Selectability : SimFromList, IAdvancedOption
    {
        bool mSelectable = false;

        public override string GetTitlePrefix()
        {
            return "Selectability";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.LotHome == null) return false;

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                string msg = "Selectability:UnPrompt";
                if (me.IsNeverSelectable)
                {
                    msg = "Selectability:Prompt";
                }

                if (!AcceptCancelDialog.Show(Common.Localize(msg, me.IsFemale, new object[] { me })))
                {
                    return false;
                }

                mSelectable = !me.IsNeverSelectable;
            }

            me.IsNeverSelectable = mSelectable;

            if ((!me.IsNeverSelectable) && (me.CreatedSim != null) && (me.Household == Household.ActiveHousehold))
            {
                using (SafeStore store = new SafeStore(me, SafeStore.Flag.None))
                {
                    if (Household.RoommateManager.IsNPCRoommate(me))
                    {
                        Household.RoommateManager.MakeRoommateSelectable(me);
                    }
                    else
                    {
                        me.CreatedSim.OnBecameSelectable();
                    }
                }
            }
            return true;
        }
    }
}
