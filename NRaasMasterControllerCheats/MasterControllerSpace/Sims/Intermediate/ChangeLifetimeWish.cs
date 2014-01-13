using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate
{
    public class ChangeLifetimeWish : SimFromList, IIntermediateOption
    {
        public class Item : ValueSettingOption<uint>
        {
            public Item()
            { }
            public Item(string name, uint id, ResourceKey icon)
                : base(id, name, 0, icon)
            { }
        }

        public override string GetTitlePrefix()
        {
            return "ChangeLifetimeWish";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool AllowSpecies(IMiniSimDescription me)
        {
            return me.IsHuman;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (SimTypes.IsSpecial(me)) return false;

            return (me.CreatedSim != null);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (me.CreatedSim == null) return false;

            if (me.LifetimeWish != 0)
            {
                IInitialMajorWish wish = DreamsAndPromisesManager.GetMajorDream(me.LifetimeWish);
                if (wish != null)
                {
                    if (!AcceptCancelDialog.Show(Common.Localize("ChangeLifetimeWish:Prompt", me.IsFemale, new object[] { me, wish.GetMajorWishName(me) })))
                    {
                        return false;
                    }
                }
            }

            LifetimeWants.SetLifetimeWant(me);
            return true;
        }
    }
}
