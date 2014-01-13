using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.ChildrenObjects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.Plumbing;
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
    public class BoardBreakAlteration : IAlteration
    {
        float[] mSkillGainRateToPractice;

        public BoardBreakAlteration()
        {}

        public void Store()
        {
            mSkillGainRateToPractice = BoardBreaker.kSkillGainRateToPractice;

            BoardBreaker.kSkillGainRateToPractice = new float[mSkillGainRateToPractice.Length];

            for (int i = 0; i < mSkillGainRateToPractice.Length; i++)
            {
                BoardBreaker.kSkillGainRateToPractice[i] = mSkillGainRateToPractice[i] * Relativity.Settings.GetSkillFactor(SkillNames.MartialArts);
            }
        }

        public void Revert()
        {
            BoardBreaker.kSkillGainRateToPractice = mSkillGainRateToPractice;
        }
    }
}
