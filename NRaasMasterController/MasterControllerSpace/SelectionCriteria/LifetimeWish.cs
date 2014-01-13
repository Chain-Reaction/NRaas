using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
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
    public class LifetimeWish : SelectionTestableOptionList<LifetimeWish.Item,uint,uint>, IDoesNotNeedSpeciesFilter
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.LifetimeWish";
        }

        public class Item : TestableOption<uint,uint>
        {
            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<uint, uint> results)
            {
                results[me.LifetimeWish] = me.LifetimeWish;
                return true;
            }

            public override void SetValue(uint value, uint storeType)
            {
                mValue = value;

                if (value == 0)
                {
                    mName = Common.Localize("Criteria.LifetimeWish:None");
                }
                else
                {
                    DreamNodeInstance wish = DreamsAndPromisesManager.GetMajorDream(value) as DreamNodeInstance;
                    if (wish != null)
                    {
                        mName = wish.GetMajorWishName(Sim.ActiveActor.SimDescription);
                        SetThumbnail(wish.PrimaryIconKey);
                    }
                }
            }
        }
    }
}
