using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using NRaas.MasterControllerSpace.Sims.Advanced.Books;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Town
{
    public class PurgeWrittenBooks : OptionItem, ITownOption
    {
        public override string GetTitlePrefix()
        {
            return "PurgeWrittenBooks";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (BookData.BookWrittenDataList == null) return false;

            if (BookData.BookWrittenDataList.Count == 0) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            List<BooksRead.Item> items = new List<BooksRead.Item>();
            foreach (KeyValuePair<string, BookWrittenData> pair in BookData.BookWrittenDataList)
            {
                items.Add(new BooksRead.Item(pair.Value, pair.Key, false));
            }

            CommonSelection<BooksRead.Item>.Results selection = new CommonSelection<BooksRead.Item>(Name, items).SelectMultiple();
            if ((selection == null) || (selection.Count == 0)) return OptionResult.Failure;

            CommonSelection<BooksRead.Item>.HandleAllOrNothing(selection);

            foreach (BooksRead.Item item in selection)
            {
                BookData.BookWrittenDataList.Remove(item.mKey);
            }

            return OptionResult.SuccessClose;
        }
    }
}
