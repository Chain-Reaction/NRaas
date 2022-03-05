using NRaas.CommonSpace.Booters;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using System.Collections.Generic;

namespace NRaas.CareerSpace.Booters
{
    public class HomemakerChildSkillBooter : BooterHelper.TableBooter, IHomemakerBooter
    {
        static Dictionary<SkillNames, bool> sData = new Dictionary<SkillNames, bool>();

        public HomemakerChildSkillBooter()
            : base("ValidChildSkill", "NRaas.Homemaker", true)
        { }

        public static bool IsSkill(SkillNames skill)
        {
            return sData.ContainsKey(skill);
        }

        public override BooterHelper.BootFile GetBootFile(string reference, string name, bool primary)
        {
            return new BooterHelper.DataBootFile(reference, name, primary);
        }

        protected override void Perform(BooterHelper.BootFile file, XmlDbRow row)
        {
            SkillNames skill;
            if (!ParserFunctions.TryParseEnum<SkillNames>(row.GetString("Skill"), out skill, SkillNames.None))
            {
                BooterLogger.AddError("Invalid Skill: " + row.GetString("Skill"));
                return;
            }

            sData.Add(skill, true);

            BooterLogger.AddTrace(" Child Skill: " + row.GetString("Skill"));
        }
    }
}
