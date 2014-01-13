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
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class Occult : SelectionTestableOptionList<Occult.Item, Occult.Values, Occult.Values>, IDoesNotNeedSpeciesFilter
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.Occult";
        }

        public static List<Item> GetDeathOptions()
        {
            List<Item> list = new List<Item>();
            foreach (SimDescription.DeathType type in Enum.GetValues(typeof(SimDescription.DeathType)))
            {
                switch (type)
                {
                    case SimDescription.DeathType.PetOldAgeGood:
                    case SimDescription.DeathType.PetOldAgeBad:
                    case SimDescription.DeathType.None:
                        break;

                    default:
                        list.Add(new Item(OccultTypes.None, type, -1, null));
                        break;
                }
            }
            return list;
        }

        public struct Values
        {
            public readonly OccultTypes mOccult;
            public readonly SimDescription.DeathType mDeathType;

            public Values(OccultTypes occult, SimDescription.DeathType deathType)
            {
                mOccult = occult;
                mDeathType = deathType;
            }
        }

        public class Item : TestableOption<Values, Values>
        {
            string mDisplayKey;

            public Item()
            { }
            public Item(OccultTypes occult, SimDescription.DeathType deathType, int count, string displayKey)
            {
                Values value = new Values(occult, deathType);
                SetValue(value, value);

                mCount = count;

                mDisplayKey = displayKey;
            }

            public override void SetValue(Values value, Values storeType)
            {
                mValue = value;

                if (value.mOccult != OccultTypes.None)
                {
                    mName = OccultTypeHelper.GetLocalizedName(value.mOccult);
                }
                else if (value.mDeathType != SimDescription.DeathType.None)
                {
                    mName = Urnstones.GetLocalizedString(false, value.mDeathType);
                }
                else
                {
                    mName = Common.Localize("Species:Human");
                }
            }

            protected bool PrivateGet(IMiniSimDescription me, IMiniSimDescription actor, Dictionary<Values, Values> results)
            {
                List<OccultTypes> types = new List<OccultTypes>();

                foreach (OccultTypes type in Enum.GetValues(typeof(OccultTypes)))
                {
                    if (type == OccultTypes.Ghost)
                    {
                        if (me.IsDead)
                        {
                            types.Add(type);
                        }
                    }
                    else if (SimTypes.IsOccult(me, type))
                    {
                        types.Add(type);
                    }
                }

                if (types.Count == 0)
                {
                    types.Add(OccultTypes.None);
                }

                foreach (OccultTypes type in types)
                {
                    Values value = new Values(type, SimDescription.DeathType.None);

                    results[value] = value;
                }

                return true;
            }

            public override bool Get(MiniSimDescription me, IMiniSimDescription actor, Dictionary<Values, Values> results)
            {
                return PrivateGet(me, actor, results);
            }
            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<Values, Values> results)
            {
                if (me.DeathStyle != SimDescription.DeathType.None)
                {
                    Values value = new Values(OccultTypes.None, me.DeathStyle);

                    results[value] = value;
                }

                return PrivateGet(me, actor, results);
            }

            public override string DisplayKey
            {
                get { return mDisplayKey; }
            }
        }
    }
}
