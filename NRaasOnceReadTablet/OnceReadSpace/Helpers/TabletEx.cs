using NRaas.CommonSpace.Helpers;
using NRaas.OnceReadSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.Store.Objects;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.OnceReadSpace.Helpers
{
    public class TabletEx
    {
        public static List<Book> GetBooksInTown(Sim actor, bool justFirst, bool noDuplicates, bool autonomous)
        {
            List<Book> list = new List<Book>();

            if (actor != null)
            {
                foreach (Book b in Sims3.Gameplay.Queries.GetObjects<Book>())
                {
                    // Custom
                    if (autonomous)
                    {
                        if (ReadBookData.HasSimFinishedBook(actor, b.Data.ID)) continue;
                    }

                    if (!justFirst && noDuplicates)
                    {

                        // Fixed Twallan's mistake => he stored the predicate result in a variable outside the loop, so the first "true" value encountered was making it skip all books
                        if (list.Exists((Book toTest) => toTest.BookId == b.BookId))
                        {
                            continue;
                        }
                    }
                    list.Add(b);
                    if (justFirst)
                    {
                        return list;
                    }
                }
            }
            return list;
        }

        public static void PopulateAudioPrograms(Sim actor, ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
        {
            NumSelectableRows = 0x1;
            headers = new List<ObjectPicker.HeaderInfo>();
            listObjs = new List<ObjectPicker.TabInfo>();
            headers.Add(new ObjectPicker.HeaderInfo(Tablet.sLocalizationKeyAudio + ":Title", "Ui/Tooltip/ObjectPicker:Name", 0xfa));
            List<ObjectPicker.RowInfo> rowInfo = new List<ObjectPicker.RowInfo>();

            SkillNames[] collection = new SkillNames[] { SkillNames.Charisma, SkillNames.Cooking, SkillNames.Gardening, SkillNames.Guitar, SkillNames.Handiness, SkillNames.Nectar };
            List<SkillNames> list2 = new List<SkillNames>(collection);
            foreach (Skill staticSkill in SkillManager.SkillDictionary)
            {
                if (staticSkill.IsHiddenSkill(actor.SimDescription.GetCASAGSAvailabilityFlags())) continue;

                // Don't include my custom skills as they increase points differently
                if (staticSkill.GetType().Assembly.FullName.Contains("NRaas")) continue;

                Skill element = actor.SkillManager.GetElement(staticSkill.Guid);
                if ((element == null) || !element.ReachedMaxLevel())
                {
                    string title = staticSkill.Name;
                    if (Localization.HasLocalizationString(Tablet.sLocalizationKeyAudio + ":" + staticSkill.Guid))
                    {
                        title = Localization.LocalizeString(Tablet.sLocalizationKeyAudio + ":" + staticSkill.Guid, new object[0]);
                    }

                    List<ObjectPicker.ColumnInfo> columnInfo = new List<ObjectPicker.ColumnInfo>();
                    ThumbnailKey thumbnail = new ThumbnailKey(staticSkill.IconKey, ThumbnailSize.Medium);
                    columnInfo.Add(new ObjectPicker.ThumbAndTextColumn(thumbnail, title));
                    ObjectPicker.RowInfo info = new ObjectPicker.RowInfo(staticSkill.Guid, columnInfo);
                    rowInfo.Add(info);
                }
            }

            ObjectPicker.TabInfo item = new ObjectPicker.TabInfo("Coupon", Tablet.LocalizeAudioProgram("TabName", new object[0x0]), rowInfo);
            listObjs.Add(item);
        }
    }
}


