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
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate.Favorites
{
    public class ChangeFavoriteColor : SimFromList, IFavoritesOption
    {
        Color mFavoriteColor;

        public override string GetTitlePrefix()
        {
            return "FavoriteColor";
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
                List<PreferenceColor.Item> allOptions = new List<PreferenceColor.Item>();

                foreach (CASCharacter.NameColorPair color in CASCharacter.kColors)
                {
                    allOptions.Add(new PreferenceColor.Item(color.mColor, (me.FavoriteColor == color.mColor) ? 1 : 0));
                }

                PreferenceColor.Item choice = new CommonSelection<PreferenceColor.Item>(Name, me.FullName, allOptions).SelectSingle();
                if (choice == null) return false;

                mFavoriteColor = choice.Value;
            }

            me.FavoriteColor = mFavoriteColor;

            if (PlumbBob.SelectedActor == me.CreatedSim)
            {
                (Sims3.Gameplay.UI.Responder.Instance.HudModel as HudModel).OnSimFavoritesChanged(me.CreatedSim.ObjectId);
            }

            return true;
        }
    }
}
