using NRaas.CareerSpace.Helpers;
using NRaas.CommonSpace.Booters;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.CareerSpace.Booters
{
    public class AfterschoolActivityBooter : BooterHelper.ByRowListingBooter
    {
        static Dictionary<AfterschoolActivityType, AfterschoolActivityData> sActivities = null;

        public AfterschoolActivityBooter()
            : this(VersionStamp.sNamespace + ".AfterschoolActivity")
        { }
        public AfterschoolActivityBooter(string reference)
            : base("AfterschoolActivity", "AfterschoolActivityFile", "File", reference)
        { }

        public static Dictionary<AfterschoolActivityType, AfterschoolActivityData> Activities
        {
            get
            {
                if (sActivities == null)
                {
                    sActivities = new Dictionary<AfterschoolActivityType, AfterschoolActivityData>();

                    foreach (AfterschoolActivityType type in Enum.GetValues(typeof(AfterschoolActivityType)))
                    {
                        AfterschoolActivityData data = new AfterschoolActivityData();

                        data.mGenders = CASAgeGenderFlags.GenderMask;

                        data.mActivity = new AfterschoolActivity(type);
                        switch (type)
                        {
                            case AfterschoolActivityType.Ballet:
                            case AfterschoolActivityType.Scouts:
                                data.mAges |= CASAgeGenderFlags.Child;
                                break;
                            case AfterschoolActivityType.StudyClub:
                            case AfterschoolActivityType.ArtClub:
                                data.mAges = CASAgeGenderFlags.Child | CASAgeGenderFlags.Teen;
                                break;
                            default:
                                data.mAges |= CASAgeGenderFlags.Teen;
                                break;
                        }

                        switch(type)
                        {
                            case AfterschoolActivityType.StudyClub:
                                data.mPreLoop = new Common.MethodStore("NRaasCareer", "NRaas.CareerSpace.Helpers.AfterschoolActivityEx","PerformPreLoopStudyClub", new Type[] { typeof(GoToSchoolInRabbitHole), typeof(AfterschoolActivity) }).Method;
                                break;
                            case AfterschoolActivityType.ArtClub:
                                data.mPostLoop = new Common.MethodStore("NRaasCareer", "NRaas.CareerSpace.Helpers.AfterschoolActivityEx","PerformPostLoopArtClub", new Type[] { typeof(GoToSchoolInRabbitHole), typeof(AfterschoolActivity) }).Method;
                                break;
                        }

                        sActivities.Add(type, data);
                    }
                }

                return sActivities;
            }
        }

        protected override void Perform(BooterHelper.BootFile file, XmlDbRow row)
        {
            string guid = row.GetString("Guid");
            if (string.IsNullOrEmpty(guid))
            {
                BooterLogger.AddError(file + ": Guid empty");
                return;
            }

            AfterschoolActivityType type = unchecked((AfterschoolActivityType)ResourceUtils.HashString64(guid));

            AfterschoolActivityData data = new AfterschoolActivityData();

            data.mActivity = new AfterschoolActivity(type);

            List<string> skills = row.GetStringList("Skills", ',');
            if (skills != null)
            {
                foreach (string skillStr in skills)
                {
                    SkillNames skill = SkillManager.sSkillEnumValues.ParseEnumValue(skillStr);
                    if (skill == SkillNames.None)
                    {
                        BooterLogger.AddError(file + ": " + guid + " Unknown skill " + skillStr);
                        return;
                    }
                    else
                    {
                        data.mActivity.ActivitySkillNameList.Add(skill);
                    }
                }
            }

            if (data.mActivity.ActivitySkillNameList.Count == 0)
            {
                BooterLogger.AddError(file + ": " + guid + " Activity Must Have a Skill");
                return;
            }

            DaysOfTheWeek days;
            if (!row.TryGetEnum<DaysOfTheWeek>("DaysOfTheWeek", out days, DaysOfTheWeek.All))
            {
                BooterLogger.AddError(file + ": " + guid + " Unknown DaysOfTheWeek " + row.GetString("DaysOfTheWeek"));
                return;
            }

            data.mActivity.DaysForActivity = days;

            CASAgeGenderFlags genders;
            if (!row.TryGetEnum<CASAgeGenderFlags>("Genders", out genders, CASAgeGenderFlags.GenderMask))
            {
                BooterLogger.AddError(file + ": " + guid + " Unknown Genders " + row.GetString("Genders"));
                return;
            }

            data.mGenders = genders;

            CASAgeGenderFlags ages;
            if (!row.TryGetEnum<CASAgeGenderFlags>("Ages", out ages, CASAgeGenderFlags.AgeMask))
            {
                BooterLogger.AddError(file + ": " + guid + " Unknown Ages " + row.GetString("Ages"));
                return;
            }

            data.mAges = ages;

            if (row.Exists("PreLoop"))
            {
                data.mPreLoop = new Common.MethodStore(row.GetString("PreLoop"), new Type[] { typeof(GoToSchoolInRabbitHole), typeof(AfterschoolActivity) }).Method;
                if (data.mPreLoop == null)
                {
                    BooterLogger.AddError(file + ": " + guid + " Missing PreLoop " + row.GetString("PreLoop"));
                    return;
                }
            }

            if (row.Exists("Loop"))
            {
                data.mLoop = new Common.MethodStore(row.GetString("Loop"), new Type[] { typeof(GoToSchoolInRabbitHole), typeof(AfterschoolActivity) }).Method;
                if (data.mLoop == null)
                {
                    BooterLogger.AddError(file + ": " + guid + " Missing Loop " + row.GetString("Loop"));
                    return;
                }
            }

            if (row.Exists("PostLoop"))
            {
                data.mPostLoop = new Common.MethodStore(row.GetString("PostLoop"), new Type[] { typeof(GoToSchoolInRabbitHole), typeof(AfterschoolActivity) }).Method;
                if (data.mPostLoop == null)
                {
                    BooterLogger.AddError(file + ": " + guid + " Missing PostLoop " + row.GetString("PostLoop"));
                    return;
                }
            }

            Activities.Add(type, data);

            BooterLogger.AddTrace(file + ": " + guid + " Loaded");
        }

        public static List<AfterschoolActivity> GetActivityList(SimDescription sim)
        {
            List<AfterschoolActivity> activities = new List<AfterschoolActivity>();

            foreach (AfterschoolActivityData data in Activities.Values)
            {                
                if (!data.IsValidFor(sim)) continue;

                activities.Add(data.mActivity);
            }

            return activities;
        }

        public static AfterschoolActivityData GetActivity(AfterschoolActivityType activityType)
        {
            AfterschoolActivityData data;
            if (!Activities.TryGetValue(activityType, out data)) return null;

            return data;
        }

        public class AfterschoolActivityFixup : Common.IWorldLoadFinished
        {
            AfterschoolActivityFixup()
            { }

            public void OnWorldLoadFinished()
            {
                foreach (SimDescription sim in Household.AllSimsLivingInWorld())
                {
                    if (sim.CareerManager == null) continue;

                    School school = sim.CareerManager.School;
                    if (school == null) continue;

                    if (school.AfterschoolActivities == null) continue;

                    for (int i=school.AfterschoolActivities.Count-1; i>=0; i--)
                    {
                        AfterschoolActivity activity = school.AfterschoolActivities[i];
                        if (activity == null)
                        {
                            school.AfterschoolActivities.RemoveAt(i);
                        }
                        else
                        {
                            AfterschoolActivityData staticActivity = GetActivity(activity.CurrentActivityType);
                            if ((staticActivity == null) || (staticActivity.mActivity == null))
                            {
                                school.AfterschoolActivities.RemoveAt(i);
                            }
                            else
                            {
                                activity.ActivitySkillNameList = new List<SkillNames>(staticActivity.mActivity.ActivitySkillNameList);
                            }
                        }
                    }
                }
            }
        }
    }
}
