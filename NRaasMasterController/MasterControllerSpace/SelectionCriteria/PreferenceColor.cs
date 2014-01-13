using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class PreferenceColor : SelectionTestableOptionList<PreferenceColor.Item, Color, Color>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.PreferenceColor";
        }

        public class Item : TestableOption<Color,Color>
        {
            public Item()
            { }
            public Item(Color value, int count)
            {
                SetValue(value, value);

                mCount = count;
            }

            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<Color,Color> results)
            {
                if (me.mFavouriteColor == null) return false;

                results[me.mFavouriteColor] = me.mFavouriteColor;
                return true;
            }

            public override void SetValue(Color dataType, Color storeType)
            {
                mValue = dataType;

                mName = CASCharacter.GetFavoriteColor(dataType);

                SetThumbnail(ResourceKey.CreatePNGKey(CASCharacter.GetFavoriteColorPngName(dataType), 0x0));
            }
        }
    }
}
