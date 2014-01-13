using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace
{
    [Persistable]
    public abstract class OptionItem : OperationSettingOption<GameObject>
    {
        public OptionItem()
        { }

        public virtual string HotkeyID
        {
            get 
            {
                string title = GetTitlePrefix();
                if (title == null) return null;

                return title.ToLower();
            }
        }

        public new OptionItem Clone()
        {
            return MemberwiseClone() as OptionItem;
        }
    }
}
