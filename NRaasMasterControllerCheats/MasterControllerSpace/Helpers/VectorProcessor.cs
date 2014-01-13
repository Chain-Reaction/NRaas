using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Helpers
{
    public class VectorProcessor : ObjectProcessor
    {
        Vector3 mPosition;

        int mRadius;

        public VectorProcessor(string localizeKey, Vector3 position)
            : base(localizeKey)
        {
            mPosition = position;
        }

        protected override bool Initialize()
        {
            string text = StringInputDialog.Show(Common.Localize(LocalizeKey + ":MenuName"), Common.Localize(LocalizeKey + ":Prompt"), "0", 1, StringInputDialog.Validation.Number);
            if (string.IsNullOrEmpty(text)) return false;

            if (!int.TryParse(text, out mRadius))
            {
                SimpleMessageDialog.Show(Common.Localize(LocalizeKey + ":MenuName"), Common.Localize("Numeric:Error", false, new object[] { text }));
                return false;
            }

            return true;
        }

        protected override void GetObjects(Dictionary<string, Item> results)
        {
            foreach (GameObject obj in Sims3.Gameplay.Queries.GetObjects<GameObject>(mPosition, mRadius))
            {
                string name = GetName(obj, true);

                Item item;
                if (!results.TryGetValue(name, out item))
                {
                    AddItem(results, name, obj);
                }
                else
                {
                    item.Add(obj);
                }
            }
        }
    }
}
