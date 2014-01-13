using NRaas.CommonSpace.Options;
using NRaas.PortraitPanelSpace.Dialogs;
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

namespace NRaas.PortraitPanelSpace.Options.Columns
{
    public class ColumnFilter1Setting : ColumnFilterBaseSetting
    {
        public override string GetTitlePrefix()
        {
            return "ColumnFilter1";
        }

        protected override Proxy GetList()
        {
            return new ListProxy(PortraitPanel.Settings.mColumnFilter1);
        }
    }
}
