using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Town
{
    public abstract class MausoleumBase : OptionItem, ITownOption
    {
        public class Item : ValueSettingOption<IMausoleum>
        {
            public Item()
            { }
            public Item(string name, IMausoleum mausoleum)
                : base(mausoleum, name, 0)
            { }
        }

        protected abstract OptionResult Run(IActor actor, IMausoleum mausoleum);

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            List<IMausoleum> mausoleums = new List<IMausoleum>(Sims3.Gameplay.Queries.GetObjects<IMausoleum>());
            if (mausoleums.Count == 0)
            {
                SimpleMessageDialog.Show(Name, Common.Localize ("ManageTheDead:Error"));
                return OptionResult.Failure;
            }

            IMausoleum mausoleum = null;
            if (mausoleums.Count > 1)
            {
                List<Item> options = new List<Item>();

                foreach (IMausoleum element in mausoleums)
                {
                    options.Add(new Item(element.LotCurrent.Name, element));
                }

                Item selection = new CommonSelection<Item>(Common.Localize("ManageTheDead:ListTitle"), options).SelectSingle();
                if (selection == null) return OptionResult.Failure;

                mausoleum = selection.Value;
            }
            else
            {
                mausoleum = mausoleums[0];
            }

            return Run(parameters.mActor, mausoleum);
        }
    }
}
