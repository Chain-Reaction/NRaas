using NRaas.CommonSpace.Booters;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System.Collections.Generic;

namespace NRaas.CareerSpace.Booters
{
    public class AcademicDegreeBooter : BooterHelper.ByRowListingBooter
    {
        public AcademicDegreeBooter()
            : this(VersionStamp.sNamespace + ".AcademicDegree", true)
        { }
        public AcademicDegreeBooter(string reference, bool testDirect)
            : base("Degrees", "AcademicDegreeFile", "File", reference, testDirect)
        { }

        // From AcademicDegreeManager:CreateAcademicDegreeTable
        protected override void Perform(BooterHelper.BootFile file, XmlDbRow row)
        {
            AcademicDegreeNames academicDegree = AcademicDegreeNames.Undefined;

            if (!row.TryGetEnum<AcademicDegreeNames>("AcademicDegreeEnum", out academicDegree, AcademicDegreeNames.Undefined))
            {
                academicDegree = unchecked((AcademicDegreeNames)ResourceUtils.HashString64(row.GetString("AcademicDegreeEnum")));
            }

            BooterLogger.AddTrace("AcademicDegreeEnum: " + row.GetString("AcademicDegreeEnum"));

            string degreeNameKey = row.GetString("AcademicDegreeName");
            string degreeDescKey = row.GetString("AcademicDegreeDesc");
            string degreeIcon = row.GetString("AcademicDegreeIcon");
            int degreeCreditHours = row.GetInt("AcademicDegreeRequiredCreditHours");
            float degreeCostPerCredit = row.GetFloat("AcademicDegreeCostPerCredit");
            string degreeResponsibilitiesKey = row.GetString("ResponsibilityTooltipKey");

            JobId academicDefaultCourseID;
            if (!row.TryGetEnum<JobId>("AcademicDefaultCourseID", out academicDefaultCourseID, JobId.AcademicsGenericRabbitHoleCourse))
            {
                // Custom
                academicDefaultCourseID = unchecked((JobId)ResourceUtils.HashString64(row.GetString("AcademicDefaultCourseID")));
            }

            JobId academicDefaultLectureID;
            if (!row.TryGetEnum<JobId>("AcademicDefaultLectureID", out academicDefaultLectureID, JobId.Invalid))
            {
                // Custom
                academicDefaultLectureID = unchecked((JobId)ResourceUtils.HashString64(row.GetString("AcademicDefaultLectureID")));                
            }

            JobId academicDefaultLabID;
            if (!row.TryGetEnum<JobId>("AcademicDefaultLabID", out academicDefaultLabID, JobId.Invalid))
            {
                // Custom
                academicDefaultLabID = unchecked((JobId)ResourceUtils.HashString64(row.GetString("AcademicDefaultLabID")));
            }

            List<OccupationNames> associatedOccupations = new List<OccupationNames>();
            List<string> list2 = row.GetStringList("AssociatedOccupationNameEnum", ',', true);
            for (int i = 0x0; i < list2.Count; i++)
            {
                OccupationNames occupation;
                if (!ParserFunctions.TryParseEnum<OccupationNames>(list2[i], out occupation, OccupationNames.Undefined))
                {
                    occupation = unchecked((OccupationNames)ResourceUtils.HashString64(row.GetString(list2[i])));
                }

                if (occupation != OccupationNames.Undefined)
                {
                    associatedOccupations.Add(occupation);
                }
            }

            OccupationNames grantedOccupation;
            if (!ParserFunctions.TryParseEnum<OccupationNames>("GrantedOccupationNameEnum", out grantedOccupation, OccupationNames.Undefined))
            {
                // Custom
                grantedOccupation = unchecked((OccupationNames)ResourceUtils.HashString64(row.GetString("GrantedOccupationNameEnum")));
            }

            List<TraitNames> beneficialTraits;
            List<TraitNames> detrimentalTraits;
            List<TraitNames> suggestedTraits;
            ParserFunctions.TryParseCommaSeparatedList<TraitNames>(row["BeneficialTraits"], out beneficialTraits, TraitNames.Unknown);
            ParserFunctions.TryParseCommaSeparatedList<TraitNames>(row["DetrimentalTraits"], out detrimentalTraits, TraitNames.Unknown);
            ParserFunctions.TryParseCommaSeparatedList<TraitNames>(row["SuggestedTraits"], out suggestedTraits, TraitNames.Unknown);

            AcademicDegreeStaticData staticData = null;
            if (!AcademicDegreeManager.sDictionary.TryGetValue((ulong)academicDegree, out staticData))
            {
                staticData = new AcademicDegreeStaticData(academicDegree, degreeNameKey, degreeDescKey, degreeResponsibilitiesKey, degreeIcon, degreeCreditHours, degreeCostPerCredit, academicDefaultCourseID, academicDefaultLectureID, academicDefaultLabID, associatedOccupations, grantedOccupation, beneficialTraits, detrimentalTraits, suggestedTraits);
            }
            else
            {
                staticData.BeneficialTraits.AddRange(beneficialTraits);
                staticData.DetrimentalTraits.AddRange(detrimentalTraits);
                staticData.SuggestedTraits.AddRange(suggestedTraits);
                staticData.AssociatedOccupations.AddRange(associatedOccupations);
            }

            string skillsThatGrantXP = row.GetString("SkillsThatGrantXP");
            if (!string.IsNullOrEmpty(skillsThatGrantXP))
            {
                foreach (string str6 in skillsThatGrantXP.Split(new char[] { ';' }))
                {
                    string[] strArray2 = str6.Split(new char[] { ',' });
                    if (strArray2.Length != 0x2) continue;

                    float num4 = ParserFunctions.ParseFloat(strArray2[0x1], -1234123f);
                    if (num4 == -1234123f) continue;

                    // Custom
                    SkillNames skillName = SkillManager.sSkillEnumValues.ParseEnumValue(strArray2[0x0]);
                    if (skillName == SkillNames.None) continue;

                    staticData.AddSkill(skillName, num4);
                }
            }

            AcademicDegreeManager.sDictionary[(ulong)academicDegree] = staticData;
        }
    }
}
