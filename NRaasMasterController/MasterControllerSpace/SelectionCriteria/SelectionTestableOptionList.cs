using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.Sims;
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
    public abstract class SelectionTestableOptionList<TOption, TDataType, TStoreType> : SelectionOptionBaseList<TOption>
        where TOption : TestableOption<TDataType, TStoreType>, new()
    {
        public SelectionTestableOptionList()
        { }
        public SelectionTestableOptionList(List<TOption> options)
            : base(options)
        { }

        public override void GetOptions(List<IMiniSimDescription> actors, List<IMiniSimDescription> allSims, List<TOption> items)
        {
            Dictionary<TStoreType, int> list = new Dictionary<TStoreType, int>();

            Dictionary<TStoreType, TDataType> data = new Dictionary<TStoreType, TDataType>();

            TOption test = new TOption();

            foreach (IMiniSimDescription member in actors)
            {
                foreach (IMiniSimDescription me in allSims)
                {
                    Dictionary<TStoreType, TDataType> types = new Dictionary<TStoreType, TDataType>();

                    SimDescription trueSim = me as SimDescription;
                    if (trueSim != null)
                    {
                        if (!test.Get(trueSim, member, types)) continue;
                    }
                    else
                    {
                        MiniSimDescription miniSim = me as MiniSimDescription;
                        if (miniSim != null)
                        {
                            if (!test.Get(miniSim, member, types)) continue;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    foreach (KeyValuePair<TStoreType, TDataType> type in types)
                    {
                        data[type.Key] = type.Value;

                        int value;
                        if (!list.TryGetValue(type.Key, out value))
                        {
                            value = 0;
                        }

                        list[type.Key] = (value + 1);
                    }
                }
            }

            foreach (KeyValuePair<TStoreType, int> value in list)
            {
                TOption item = new TOption();
                item.SetValue(data[value.Key], value.Key);
                item.Count = value.Value;

                items.Add(item);
            }
        }
    }
}
