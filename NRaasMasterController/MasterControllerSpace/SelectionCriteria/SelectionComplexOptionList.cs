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
    public abstract class SelectionComplexOptionList<TOption> : SelectionOptionBaseList<TOption>
        where TOption : SelectionOptionBase
    {
        public SelectionComplexOptionList()
        { }
        public SelectionComplexOptionList(List<TOption> options)
            : base(options)
        { }

        public override void GetOptions(List<IMiniSimDescription> actors, List<IMiniSimDescription> allSims, List<TOption> items)
        {
            items.Clear();

            foreach (TOption option in Common.DerivativeSearch.Find<TOption>())
            {
                items.Add(option.Clone() as TOption);
            }

            foreach (TOption option in items)
            {
                option.Reset();

                foreach (IMiniSimDescription member in actors)
                {
                    foreach (IMiniSimDescription me in allSims)
                    {
                        if (option.Test(me, false, member))
                        {
                            option.IncCount();
                        }
                    }
                }
            }
        }
    }
}
