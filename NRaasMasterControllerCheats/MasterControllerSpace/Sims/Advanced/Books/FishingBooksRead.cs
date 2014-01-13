using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Skills;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced.Books
{
    public class FishingBooksRead : BooksRead
    {
        public override string GetTitlePrefix()
        {
            return "FishingBooksRead";
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.SkillManager == null) return false;

            return (me.SkillManager.GetSkill<Fishing>(SkillNames.Fishing) != null);
        }

        protected override List<BooksRead.Item> GetOptions(SimDescription me, Dictionary<BookData,bool> lookup)
        {
            List<Item> allOptions = new List<Item>();

            allOptions.Add(null);

            foreach (BookFishData data in BookData.FishBookDataList.Values)
            {
                allOptions.Add(new Item(data, lookup.ContainsKey(data)));
            }

            return allOptions;
        }

        protected override void Perform(SimDescription me, BookData book, bool addToList)
        {
            if (!me.ReadBookDataList.ContainsKey(book.ID))
            {
                base.Perform(me, book, addToList);
            }

            BookFishData data = book as BookFishData;
            if (data != null)
            {
                Fishing skill = me.SkillManager.GetSkill<Fishing>(SkillNames.Fishing);

                foreach (FishType type in data.FishTypes)
                {
                    skill.LearnedAbout(type);
                }
            }
        }
    }
}
