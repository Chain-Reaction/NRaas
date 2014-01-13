using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.PortraitPanelSpace
{
    public class SimSelection : ProtoSimSelection<SimDescription>
    {
        public SimSelection(string title, ICollection<SimDescription> sims, ObjectPickerDialogEx.CommonHeaderInfo<SimDescription> auxillary)
            : base(title, null, sims, true, false)
        {
            if (auxillary != null)
            {
                AddColumn(auxillary);
            }
        }
    }
}
