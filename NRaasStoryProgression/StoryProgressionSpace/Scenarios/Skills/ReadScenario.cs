using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Skills
{
    public abstract class ReadScenario : SkillGainScenario
    {
        Dictionary<SkillNames, Book> mBooks = new Dictionary<SkillNames, Book>();

        public ReadScenario()
        { }
        public ReadScenario(SimDescription sim)
            : base(sim)
        { }
        protected ReadScenario(ReadScenario scenario)
            : base (scenario)
        {
            mBooks = scenario.mBooks;
        }

        protected override int ContinueChance
        {
            get { return 50; }
        }

        protected abstract bool AllowSkill
        {
            get;
        }

        protected abstract bool AllowNormal
        {
            get;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.LotHome == null)
            {
                IncStat("Homeless");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool Perform(SkillNames skill)
        {
            Book book;
            if (!mBooks.TryGetValue(skill, out book)) return false;

            return Perform(book);
        }

        protected bool Perform(Book book)
        {
            if ((book != null) && (Inventories.TryToMove(book, Sim.CreatedSim)))
            {
                if (Situations.PushInteraction(this, Sim, book, ReadBook.FromSimInventorySingleton))
                {
                    return true;
                }
            }

            return false;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            List<Book> choices = new List<Book>();

            if ((choices.Count == 0) && (AllowSkill))
            {
                mBooks.Clear();

                foreach (BookSkill book in Inventories.InventoryDuoFindAll<BookSkill,Book>(Sim))
                {
                    GreyedOutTooltipCallback greyedOutTooltipCallback = null;
                    if (!book.TestReadBook(Sim.CreatedSim, false, ref greyedOutTooltipCallback)) continue;

                    BookSkillData data = book.Data as BookSkillData;
                    if (data == null) continue;

                    if (mBooks.ContainsKey(data.SkillGuid)) continue;

                    mBooks.Add(data.SkillGuid, book);
                }

                foreach (Lot lot in ManagerLot.GetOwnedLots(Sim))
                {
                    foreach (BookSkill book in lot.GetObjects<BookSkill>())
                    {
                        GreyedOutTooltipCallback greyedOutTooltipCallback = null;
                        if (!book.TestReadBook(Sim.CreatedSim, false, ref greyedOutTooltipCallback)) continue;

                        BookSkillData data = book.Data as BookSkillData;
                        if (data == null) continue;

                        if (mBooks.ContainsKey(data.SkillGuid)) continue;

                        mBooks.Add(data.SkillGuid, book);
                    }
                }

                if ((!Sim.CreatedSim.LotCurrent.IsResidentialLot) && (!Sim.CreatedSim.LotCurrent.IsWorldLot))
                {
                    foreach (BookSkill book in Sim.CreatedSim.LotCurrent.GetObjects<BookSkill>())
                    {
                        GreyedOutTooltipCallback greyedOutTooltipCallback = null;
                        if (!book.TestReadBook(Sim.CreatedSim, false, ref greyedOutTooltipCallback)) continue;

                        BookSkillData data = book.Data as BookSkillData;
                        if (data == null) continue;

                        if (mBooks.ContainsKey(data.SkillGuid)) continue;

                        mBooks.Add(data.SkillGuid, book);
                    }
                }

                if (mBooks.Count > 0)
                {
                    if (base.PrivateUpdate(frame))
                    {
                        return true;
                    }
                    else
                    {
                        if (choices.Count == 0)
                        {
                            choices.AddRange(mBooks.Values);
                        }
                    }
                }
            }

            if ((choices.Count == 0) && (AllowNormal))
            {
                Dictionary<BookData, Book> books = new Dictionary<BookData, Book>();

                foreach (BookGeneral book in Inventories.InventoryDuoFindAll<BookGeneral, Book>(Sim))
                {
                    GreyedOutTooltipCallback greyedOutTooltipCallback = null;
                    if (!book.TestReadBook(Sim.CreatedSim, false, ref greyedOutTooltipCallback)) continue;

                    if (Sim.ReadBookDataList.ContainsKey(book.Data.ID)) continue;

                    if (books.ContainsKey(book.Data)) continue;

                    books.Add(book.Data, book);
                }

                foreach (Lot lot in ManagerLot.GetOwnedLots(Sim))
                {
                    foreach (BookGeneral book in lot.GetObjects<BookGeneral>())
                    {
                        GreyedOutTooltipCallback greyedOutTooltipCallback = null;
                        if (!book.TestReadBook(Sim.CreatedSim, false, ref greyedOutTooltipCallback)) continue;

                        if (Sim.ReadBookDataList.ContainsKey(book.Data.ID)) continue;

                        if (books.ContainsKey(book.Data)) continue;

                        books.Add(book.Data, book);
                    }
                }

                if ((!Sim.CreatedSim.LotCurrent.IsResidentialLot) && (!Sim.CreatedSim.LotCurrent.IsWorldLot))
                {
                    foreach (BookGeneral book in Sim.CreatedSim.LotCurrent.GetObjects<BookGeneral>())
                    {
                        GreyedOutTooltipCallback greyedOutTooltipCallback = null;
                        if (!book.TestReadBook(Sim.CreatedSim, false, ref greyedOutTooltipCallback)) continue;

                        if (Sim.ReadBookDataList.ContainsKey(book.Data.ID)) continue;

                        if (books.ContainsKey(book.Data)) continue;

                        books.Add(book.Data, book);
                    }
                }

                if (books.Count > 0)
                {
                    choices.AddRange(books.Values);
                }
            }

            if (choices.Count > 0)
            {
                return Perform(RandomUtil.GetRandomObjectFromList(choices));
            }

            return false;
        }
    }
}
