using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class Mood : SelectionTestableOptionList<Mood.Item, MoodFlavor, MoodFlavor>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.Mood";
        }

        public class Item : TestableOption<MoodFlavor, MoodFlavor>
        {
            public override void SetValue(MoodFlavor value, MoodFlavor storeType)
            {
                mValue = value;

                mName = Common.LocalizeEAString("Ui/Tooltip/HUD/SimDisplay:MoodFlavor" + ((int)value).ToString());
            }

            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<MoodFlavor, MoodFlavor> results)
            {
                if (me.CreatedSim == null) return false;

                if (me.CreatedSim.MoodManager == null) return false;

                MoodFlavor flavor = me.CreatedSim.MoodManager.MoodFlavor;

                results[flavor] = flavor;
                return true;
            }
        }
    }
}
