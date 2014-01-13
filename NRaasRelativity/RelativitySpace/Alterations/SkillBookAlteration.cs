using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RelativitySpace.Alterations
{
    public class SkillBookAlteration : IAlteration
    {
        Dictionary<BookSkillData,float> mOriginal = new Dictionary<BookSkillData,float>();

        public SkillBookAlteration()
        { }

        public void Store()
        {
            foreach (BookSkillData skillBook in BookData.BookSkillDataList.Values)
            {
                mOriginal.Add(skillBook, skillBook.SkillPointsPerPage);

                skillBook.SkillPointsPerPage *= Relativity.Settings.GetSkillFactor(skillBook.SkillGuid);
            }
        }

        public void Revert()
        {
            foreach(KeyValuePair<BookSkillData,float> original in mOriginal)
            {
                original.Key.SkillPointsPerPage = original.Value;
            }
        }
    }
}
