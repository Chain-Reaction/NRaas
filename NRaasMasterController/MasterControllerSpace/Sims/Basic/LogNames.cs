using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Basic
{
    public class LogNames : SimFromList, IBasicOption
    {
        public override string GetTitlePrefix()
        {
            return "LogNames";
        }

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            return true;
        }
        protected override bool PrivateAllow(MiniSimDescription me)
        {
            return true;
        }

        protected override OptionResult RunAll(List<IMiniSimDescription> sims)
        {
            string msg = null;

            foreach (IMiniSimDescription sim in sims)
            {
                msg += Common.NewLine + sim.FullName;
            }

            Common.WriteLog(msg, false);

            SimpleMessageDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Success"));

            return OptionResult.SuccessRetain;
        }
    }
}
