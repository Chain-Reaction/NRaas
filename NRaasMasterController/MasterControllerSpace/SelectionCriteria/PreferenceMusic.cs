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
    public class PreferenceMusic : SelectionTestableOptionList<PreferenceMusic.Item, FavoriteMusicType, FavoriteMusicType>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.PreferenceMusic";
        }

        public class Item : TestableOption<FavoriteMusicType, FavoriteMusicType>
        {
            public Item()
            { }
            public Item(FavoriteMusicType value, int count)
            {
                SetValue(value, value);

                mCount = count;
            }

            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<FavoriteMusicType, FavoriteMusicType> results)
            {
                if (me.mFavouriteMusicType == FavoriteMusicType.None) return false;

                results[me.mFavouriteMusicType] = me.mFavouriteMusicType;
                return true;
            }

            public override void SetValue(FavoriteMusicType value, FavoriteMusicType storeType)
            {
                mValue = value;

                mName = CASCharacter.GetFavoriteMusic(value);

                SetThumbnail(ResourceKey.CreatePNGKey(CASCharacter.GetFavoriteMusicPngName(value), 0x0));
            }
        }
    }
}
