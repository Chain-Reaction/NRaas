using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced.Traits
{
    public class PopulateTraits : SimFromList, ITraitOption
    {
        public override string GetTitlePrefix()
        {
            return "PopulateTraits";
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

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.TraitManager == null || me.IsEP11Bot) return false;

            if (me.TraitManager.TraitsMaxed()) return false;

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            SimDescription mom = null, dad = null;
            Relationships.GetParents(me, out mom, out dad);

            Sims3.Gameplay.CAS.Genetics.AssignTraits(me, dad, mom, false, 0, new System.Random());

            return true;
        }
    }
}
