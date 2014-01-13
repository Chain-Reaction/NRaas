using Sims3.Gameplay.Actors;
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
    public class ZodiacSign : SelectionTestableOptionList<ZodiacSign.Item, Zodiac, Zodiac>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.Zodiac";
        }

        public class Item : TestableOption<Zodiac, Zodiac>
        {
            public Item()
            { }
            public Item(Zodiac value)
            {
                SetValue(value, value);
            }

            public override void SetValue(Zodiac value, Zodiac storeType)
            {
                mValue = value;

                mName = Common.LocalizeEAString("Ui/Caption/HUD/KnownInfoDialog:" + value.ToString());

                SetThumbnail(ResourceKey.CreatePNGKey("sign_" + value.ToString() + "_sm", 0));
            }

            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<Zodiac, Zodiac> results)
            {
                results[me.Zodiac] = me.Zodiac;
                return true;
            }

            public override bool Get(MiniSimDescription me, IMiniSimDescription actor, Dictionary<Zodiac, Zodiac> results)
            {
                results[me.Zodiac] = me.Zodiac;
                return true;
            }
        }
    }
}
