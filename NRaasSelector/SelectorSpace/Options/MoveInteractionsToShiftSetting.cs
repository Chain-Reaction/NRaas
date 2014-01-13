using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.SelectorSpace.Options
{
    public class MoveInteractionsToShiftSetting : BooleanSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        protected override bool Value
        {
            get
            {
                return Selector.Settings.mMoveInteractionsToShift;
            }
            set
            {
                Selector.Settings.mMoveInteractionsToShift = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "MoveInteractionsToShift";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
