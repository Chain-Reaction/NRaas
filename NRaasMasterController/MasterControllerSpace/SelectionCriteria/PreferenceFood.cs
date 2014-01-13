using NRaas.CommonSpace.Options;
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
    public class PreferenceFood : SelectionTestableOptionList<PreferenceFood.Item, FavoriteFoodType, FavoriteFoodType>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.PreferenceFood";
        }

        public class Item : TestableOption<FavoriteFoodType, FavoriteFoodType>
        {
            public Item()
            { }
            public Item(FavoriteFoodType value, int count)
            {
                SetValue(value, value);

                mCount = count;
            }

            public override void SetValue(FavoriteFoodType value, FavoriteFoodType storeType)
            {
                mValue = value;

                mName = CASCharacter.GetFavoriteFood(value);

                SetThumbnail(ResourceKey.CreatePNGKey(CASCharacter.GetFavoriteFoodPngName(value), 0x0));
            }

            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<FavoriteFoodType,FavoriteFoodType> results)
            {
                if (me.mFavouriteFoodType == FavoriteFoodType.None) return false;

                results[me.mFavouriteFoodType] = me.mFavouriteFoodType;
                return true;
            }
        }
    }
}
