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
    public class MovePerPersonPercentageSetting : IntegerSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        protected override int Value
        {
            get
            {
                return Mover.Settings.mMovePerPersonPercentage;
            }
            set
            {
                Mover.Settings.mMovePerPersonPercentage = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "MovePerPersonPercentage";
        }

        protected override int Validate(int value)
        {
            if (value < 0)
            {
                return 0;
            }
            else if (value > 100)
            {
                value = 100;
            }

            return value;
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
