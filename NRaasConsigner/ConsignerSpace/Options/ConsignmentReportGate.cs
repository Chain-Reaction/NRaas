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

namespace NRaas.ConsignerSpace.Options
{
    public class ConsignmentReportGate : IntegerSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        protected override int Value
        {
            get
            {
                return Consigner.Settings.mReportGate;
            }
            set
            {
                Consigner.Settings.mReportGate = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "ConsignmentReportGate";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}
