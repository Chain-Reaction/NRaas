using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Dialogs;
using Sims3.Gameplay;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Moving;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NRaas.CommonSpace.Helpers
{
    public class CustomAcademicDegrees
    {
        static Common.MethodStore sCareerJobToString = new Common.MethodStore("NRaasCareer", "NRaas.Careers", "JobToString", new Type[] { typeof(JobId) });

        protected static string JobToString(JobId job)
        {
            if (!sCareerJobToString.Valid)
            {
                return null;
            }

            return sCareerJobToString.Invoke<string>(new object[] { job });
        }

        public static void AdjustCustomAcademics(SimDescription sim)
        {
            AcademicCareer academicCareer = sim.OccupationAsAcademicCareer;
            if (academicCareer == null) return;

            string progressStr = null;

            int progressKey = 0;

            float progress = academicCareer.mDegree.Progress;
            if (progress <= 0.25f)
            {
                progressStr = AcademicCareer.LocalizeString("FreshmanCourseLevelKey", new object[0x0]);
            }
            else if (progress <= 0.5f)
            {
                progressKey = 1;
                progressStr = AcademicCareer.LocalizeString("SophomoreCourseLevelKey", new object[0x0]);
            }
            else if (progress <= 0.75f)
            {
                progressKey = 2;
                progressStr = AcademicCareer.LocalizeString("JuniorCourseLevelKey", new object[0x0]);
            }
            else
            {
                progressKey = 3;
                progressStr = AcademicCareer.LocalizeString("SeniorCourseLevelKey", new object[0x0]);
            }

            AcademicDegreeStaticData staticData = academicCareer.mDegree.AcademicDegreeStaticData;

            foreach (AcademicCareer.CourseCreationSpec spec in academicCareer.mCourseSchedule)
            {
                string key = JobToString(spec.mJobCreationSpec.mJobId);
                if (string.IsNullOrEmpty(key)) continue;

                key = staticData.mDegreeDescKey + ":" + key + progressKey;

                spec.mCourseLocalizedName = Common.LocalizeEAString(sim.IsFemale, key, new object[] { progressStr });
            }
        }
    }
}