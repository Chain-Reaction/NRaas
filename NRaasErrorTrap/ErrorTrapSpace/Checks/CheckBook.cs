using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Checks
{
    public class CheckBook : Check<Book>
    {
        protected override bool PrePerform(Book book, bool postLoad)
        {
            if (book.mBookId == null)
            {
                book.mBookId = "";

                if (book.Data != null)
                {
                    book.Data = new BookData();
                }

                //LogCorrection("Book Id Replaced");
            }

            return true;
        }
    }
}
