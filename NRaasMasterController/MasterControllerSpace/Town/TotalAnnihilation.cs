using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.Sims;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Town
{
    public class TotalAnnihilation : SimFromList, ITownOption
    {
        public TotalAnnihilation()
        {
            ApplyAll = true;
        }

        public override string GetTitlePrefix()
        {
            return "TotalAnnihilation";
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (me.Household == Household.ActiveHousehold)
            {
                return false;
            }

            return base.PrivateAllow(me);
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

        protected override List<SimSelection.ICriteria> GetCriteria(GameHitParameters<GameObject> parameters)
        {
            List<SimSelection.ICriteria> list = new List<SimSelection.ICriteria>();

            list.Add(new SimTypeOr());

            return list;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            return Annihilation.Cleanse(me);
        }

        protected override bool Run(MiniSimDescription me, bool singleSelection)
        {
            return Annihilation.Cleanse(me);
        }
    }
}
