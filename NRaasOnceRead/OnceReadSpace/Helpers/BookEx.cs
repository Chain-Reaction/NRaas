using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.OnceReadSpace.Helpers
{
    public class BookEx
    {
        public static float GetInterestInBook(Sim actor, Book book)
        {
            if ((book is BookGeneral) || (book is BookWritten))
            {
                float num = 0f;
                float num2 = 0f;
                if (book.Data == null)
                {
                    return num;
                }

                if (ReadBookData.HasSimStartedBook(actor, book.Data.ID, false))
                {
                    num += 10f;
                }

                DateAndTime previousDateAndTime = ReadBookData.WhenSimFinishedBook(actor, book.Data.ID);
                float num3 = SimClock.ElapsedTime(TimeUnit.Days, previousDateAndTime);
                if (num3 >= 1f)
                {
                    num3 = 1f + Math.Min(num3, 4f);
                    num += num3;
                }

                if (actor.SimDescription.TraitManager.HasAnyPreferredGenres)
                {
                    string genreLocalizedString = null;

                    if (book.Data is BookGeneralData)
                    {
                        genreLocalizedString = (book.Data as BookGeneralData).GenreLocalizedString;
                    }
                    else if (book.Data is BookWrittenData)
                    {
                        genreLocalizedString = (book.Data as BookWrittenData).GenreString;
                    }

                    if (actor.SimDescription.TraitManager.PrefersGenre(genreLocalizedString))
                    {
                        num++;
                    }
                }
                num2 = 0.5f + ((num * 6.25f) * 0.01f);

                if (actor.TraitManager.HasElement(TraitNames.NightOwlTrait) && actor.BuffManager.HasElement(BuffNames.PastBedTime))
                {
                    num2 += TraitTuning.NightOwlFunModifier;
                }
                return num2;
            }
            else
            {
                return 0f;
            }
        }
    }
}


