using NRaas.CommonSpace.Booters;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Utilities;
using System.Collections.Generic;

namespace NRaas.CareerSpace.Booters
{
    public class HomemakerGoodParentMoodletBooter : BooterHelper.TableBooter, IHomemakerBooter
    {
        static Dictionary<BuffNames, bool> sData = new Dictionary<BuffNames, bool>();

        public HomemakerGoodParentMoodletBooter()
            : base("GoodParentMoodlet", "NRaas.Homemaker", true)
        { }

        public static bool IsBuff(BuffNames moodlet)
        {
            return sData.ContainsKey(moodlet);
        }

        public override BooterHelper.BootFile GetBootFile(string reference, string name, bool primary)
        {
            return new BooterHelper.DataBootFile(reference, name, primary);
        }

        protected override void Perform(BooterHelper.BootFile file, XmlDbRow row)
        {
            BuffNames buff;
            if (!ParserFunctions.TryParseEnum<BuffNames>(row.GetString("Moodlet"), out buff, BuffNames.Undefined))
            {
                BooterLogger.AddError("Invalid Moodlet: " + row.GetString("Moodlet"));
                return;
            }

            sData.Add(buff, true);

            BooterLogger.AddTrace(" Good Parent Moodlet: " + row.GetString("Moodlet"));
        }
    }
}
