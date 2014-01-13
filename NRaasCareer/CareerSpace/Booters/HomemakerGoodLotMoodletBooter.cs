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
    public class HomemakerGoodLotMoodletBooter : BooterHelper.TableBooter, IHomemakerBooter
    {
        static Dictionary<BuffNames, bool> sData = new Dictionary<BuffNames, bool>();

        public HomemakerGoodLotMoodletBooter()
            : base("GoodLotMoodlet", "NRaas.Homemaker", true)
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

            BooterLogger.AddTrace(" Good Lot Moodlet: " + row.GetString("Moodlet"));
        }
    }
}
