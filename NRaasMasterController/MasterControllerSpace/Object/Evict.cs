using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.SelectionCriteria;
using NRaas.MasterControllerSpace.Sims;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.Object
{
    public class Evict : SimFromList, IObjectOption
    {
        Lot mTarget;

        public override string GetTitlePrefix()
        {
            return "MakeHomeless";
        }

        public override string HotkeyID
        {
            get { return null; }
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!MasterController.Settings.mMenuVisibleAddSim) return false;

            Door door = parameters.mTarget as Door;
            if (door == null) return false;

            if (!door.IsNPCDoor) return false;

            mTarget = door.LotCurrent;

            // Normally called from the base class
            Reset();
            return true;
        }

        protected override List<SimSelection.ICriteria> GetCriteria(GameHitParameters<GameObject> parameters)
        {
            return null;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.Household == null) return false;

            if (me.LotHome != null) return false;

            if ((me.IsDead) && (!me.IsPlayableGhost)) return false;

            if (me.Household.IsSpecialHousehold)
            {
                if (!me.Household.IsServiceNpcHousehold) return false;
            }

            Lot lot = me.VirtualLotHome;
            if (lot == null) return false;

            if (mTarget != lot) return false;

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            Lot lot = me.VirtualLotHome;
            if (lot == null) return false;

            if (me.Household.IsServiceNpcHousehold)
            {
                lot.VirtualMoveOut(me);
            }
            else
            {
                lot.VirtualMoveOut(me.Household);
            }

            return true;
        }
    }
}
