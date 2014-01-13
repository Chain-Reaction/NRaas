using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate.Favorites
{
    public class ChangeFavoriteMusic : SimFromList, IFavoritesOption
    {
        FavoriteMusicType mFavoriteMusic = FavoriteMusicType.None;

        public override string GetTitlePrefix()
        {
            return "FavoriteMusic";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                List<PreferenceMusic.Item> allOptions = new List<PreferenceMusic.Item>();

                foreach (FavoriteMusicType type in Enum.GetValues(typeof(FavoriteMusicType)))
                {
                    if ((type == FavoriteMusicType.None) || (type == FavoriteMusicType.Count)) continue;

                    allOptions.Add(new PreferenceMusic.Item(type, (me.FavoriteMusic == type) ? 1 : 0));
                }

                PreferenceMusic.Item choice = new CommonSelection<PreferenceMusic.Item>(Name, me.FullName, allOptions).SelectSingle();
                if (choice == null) return false;

                mFavoriteMusic = choice.Value;
            }

            me.mFavouriteMusicType = mFavoriteMusic;

            if (PlumbBob.SelectedActor == me.CreatedSim)
            {
                (Sims3.Gameplay.UI.Responder.Instance.HudModel as HudModel).OnSimFavoritesChanged(me.CreatedSim.ObjectId);
            }

            return true;
        }
    }
}
