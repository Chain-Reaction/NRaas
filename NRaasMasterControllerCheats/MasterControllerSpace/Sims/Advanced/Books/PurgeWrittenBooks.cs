using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
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

namespace NRaas.MasterControllerSpace.Sims.Advanced.Books
{
    public class PurgeWrittenBooks : SimFromList, IBooksOption
    {
        public override string GetTitlePrefix()
        {
            return "PurgeWrittenBooks";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.SkillManager == null) return false;

            Writing skill = me.SkillManager.GetSkill<Writing>(SkillNames.Writing);
            if (skill == null) return false;

            if (skill.WrittenBookDataList == null) return false;

            return (skill.WrittenBookDataList.Count > 0);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                if (!AcceptCancelDialog.Show(Common.Localize(GetTitlePrefix() + ":Prompt", me.IsFemale, new object[] { me }))) return false;
            }

            if (me.SkillManager != null)
            {
                int count = 0;

                Writing skill = me.SkillManager.GetSkill<Writing>(SkillNames.Writing);
                if ((skill != null) && (skill.WrittenBookDataList != null))
                {
                    foreach (WrittenBookData book in skill.WrittenBookDataList.Values)
                    {
                        string id = book.Title + book.Author;

                        if (BookData.BookWrittenDataList.ContainsKey(id))
                        {
                            BookData.BookWrittenDataList.Remove(id);
                            count++;
                        }
                    }

                    skill.WrittenBookDataList.Clear();
                }

                if (count > 0)
                {
                    Common.Notify(me, Common.Localize(GetTitlePrefix() + ":Success", me.IsFemale, new object[] { count, me }));
                }
            }

            return true;
        }
    }
}
