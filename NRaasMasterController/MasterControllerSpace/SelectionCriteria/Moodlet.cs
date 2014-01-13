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
    public class Moodlet : SelectionTestableOptionList<Moodlet.Item, BuffNames, BuffNames>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.Moodlet";
        }

        public class Item : TestableOption<BuffNames, BuffNames>
        {
            public Item()
            { }
            public Item(BuffInstance value, int count)
                : base(value.Guid, GetName(value), count)
            { }

            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<BuffNames,BuffNames> results)
            {
                if (me.CreatedSim == null) return false;

                if (me.CreatedSim.BuffManager == null) return false;

                foreach (BuffInstance buff in me.CreatedSim.BuffManager.List)
                {
                    results[buff.Guid] = buff.Guid;
                }

                return true;
            }

            public override void SetValue(BuffNames value, BuffNames storeType)
            {
 	            mValue = value;

                BuffInstance buff = null;
                if (BuffManager.BuffDictionary.TryGetValue((ulong)value, out buff))
                {
                    mName = GetName(buff);

                    SetThumbnail(buff.ThumbKey);
                }
            }

            protected static string GetName(BuffInstance buff)
            {
                string name = null;
                if (!Localization.GetLocalizedString(buff.BuffName, out name))
                {
                    name = buff.BuffName;
                }

                if (Common.kDebugging)
                {
                    name = "[" + buff.Guid + "] " + name;
                }

                return name;
            }
        }
    }
}
