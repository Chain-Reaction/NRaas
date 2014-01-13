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
    public class ChangeFavoriteFood : SimFromList, IFavoritesOption
    {
        FavoriteFoodType mFavoriteFood = FavoriteFoodType.None;

        public override string GetTitlePrefix()
        {
            return "FavoriteFood";
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
                List<PreferenceFood.Item> allOptions = new List<PreferenceFood.Item>();

                foreach (FavoriteFoodType type in Enum.GetValues(typeof(FavoriteFoodType)))
                {
                    if ((type == FavoriteFoodType.None) || (type == FavoriteFoodType.Count)) continue;

                    allOptions.Add(new PreferenceFood.Item(type, (me.FavoriteFood == type) ? 1 : 0));
                }

                PreferenceFood.Item choice = new CommonSelection<PreferenceFood.Item>(Name, me.FullName, allOptions).SelectSingle();
                if (choice == null) return false;

                mFavoriteFood = choice.Value;
            }

            me.mFavouriteFoodType = mFavoriteFood;

            if (PlumbBob.SelectedActor == me.CreatedSim)
            {
                (Sims3.Gameplay.UI.Responder.Instance.HudModel as HudModel).OnSimFavoritesChanged(me.CreatedSim.ObjectId);
            } 
            
            return true;
        }
    }
}
