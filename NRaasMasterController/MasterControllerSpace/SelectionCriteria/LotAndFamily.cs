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
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class LotAndFamily : SelectionTestableOptionList<LotAndFamily.Item, Lot, ulong>, IDoesNotNeedSpeciesFilter
    {
        [Persistable(false)]
        Lot mLot = null;

        public LotAndFamily()
        {}
        public LotAndFamily(Lot lot)
        {
            mLot = lot;
        }

        public override string GetTitlePrefix()
        {
            return "Criteria.LotAndFamily";
        }

        public override void GetOptions(List<IMiniSimDescription> actors, List<IMiniSimDescription> allSims, List<Item> items)
        {
            if (mLot != null)
            {
                items.Add(new Item(mLot));
            }
            else
            {
                base.GetOptions(actors, allSims, items);
            }
        }

        public class Item : TestableOption<Lot, ulong>
        {
            public Item()
            { }
            public Item(Lot lot)
                : base(lot.LotId, lot.Name, 0)
            { }

            public override void SetValue(Lot value, ulong storeType)
            {
                if (value == null)
                {
                    mValue = 0;
                }
                else
                {
                    mValue = value.LotId;

                    mName = value.Name;
                }
            }

            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<ulong, Lot> results)
            {
                if ((me.CreatedSim != null) && (me.CreatedSim.LotCurrent != null))
                {
                    results[me.CreatedSim.LotCurrent.LotId] = me.CreatedSim.LotCurrent;
                }

                if (me.LotHome != null)
                {
                    results[me.LotHome.LotId] = me.LotHome;
                }

                return true;
            }
        }
    }
}
