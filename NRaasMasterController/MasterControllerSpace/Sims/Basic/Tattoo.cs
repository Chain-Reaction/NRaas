using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Basic
{
    public class Tattoo : CASBase, IBasicOption
    {
        public override string GetTitlePrefix()
        {
            return "Tattoo";
        }

        protected override bool AllowSpecies(IMiniSimDescription me)
        {
            return me.IsHuman;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (me.Baby) return false;

            if (me.IsEP11Bot) return false;

            return base.PrivateAllow(me);
        }

        protected override CASMode GetMode(SimDescription sim, ref OutfitCategories startCategory, ref int startIndex, ref EditType editType)
        {
            return CASMode.Tattoo;
        }
    }
}
