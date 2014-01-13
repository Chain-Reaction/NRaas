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

namespace NRaas.MoverSpace.Options
{
    public class MinMoneyTransferredSetting : IntegerSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        protected override int Value
        {
            get
            {
                return Mover.Settings.mMinMoneyTransferred;
            }
            set
            {
                Mover.Settings.mMinMoneyTransferred = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "MinMoneyTransferred";
        }

        protected override int Validate(int value)
        {
            if (value < 0)
            {
                return 0;
            }

            return value;
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
