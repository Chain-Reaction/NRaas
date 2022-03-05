﻿using NRaas.CareerSpace.Careers;
using NRaas.CommonSpace.Booters;
using Sims3.Gameplay.Utilities;
using System.Collections.Generic;

namespace NRaas.CareerSpace.Booters
{
    public class HomemakerBooter : BooterHelper.TableBooter, IHomemakerBooter
    {
        static Dictionary<Homemaker.StipendValue, Data> sData = new Dictionary<Homemaker.StipendValue, Data>();

        public HomemakerBooter()
            : base("Stipend", "NRaas.Homemaker", true)
        { }

        public static bool HasValue
        {
            get { return (sData.Count > 0); }
        }

        public override BooterHelper.BootFile GetBootFile(string reference, string name, bool primary)
        {
            return new BooterHelper.DataBootFile(reference, name, primary);
        }

        protected override void Perform(BooterHelper.BootFile file, XmlDbRow row)
        {
            Homemaker.StipendValue key;
            if (!ParserFunctions.TryParseEnum<Homemaker.StipendValue>(row.GetString("Key"), out key, Homemaker.StipendValue.Undefined))
            {
                BooterLogger.AddError("Stipend Key Missing: " + row.GetString("Key"));
                return;
            }

            int maximum = row.GetInt("Maximum");

            int factor = row.GetInt("Factor");

            bool positive = row.GetBool("Positive");

            sData.Add(key, new Data (maximum, factor, positive));

            BooterLogger.AddTrace(" Stipend Loaded: " + row.GetString("Key"));
        }

        public static Data GetStipend(Homemaker.StipendValue value)
        {
            return sData[value];
        }

        public class Data
        {
            public readonly int mMaximum;

            public readonly int mFactor;

            public readonly bool mPositive;

            public Data(int maximum, int factor, bool positive)
            {
                mMaximum = maximum;
                mFactor = factor;
                mPositive = positive;
            }
        }
    }
}
