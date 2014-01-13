using NRaas.CommonSpace.Booters;
using NRaas.CareerSpace.Careers;
using NRaas.CareerSpace.Helpers;
using NRaas.CareerSpace.Interfaces;
using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Booters
{
    public class HomemakerParentSkillBooter : BooterHelper.TableBooter, IHomemakerBooter
    {
        static Dictionary<SkillNames, bool> sData = new Dictionary<SkillNames, bool>();

        public HomemakerParentSkillBooter()
            : base("ValidParentSkill", "NRaas.Homemaker", true)
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

            BooterLogger.AddTrace(" Parent Skill: " + row.GetString("Skill"));
        }
    }
}
