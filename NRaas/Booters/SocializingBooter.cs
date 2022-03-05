using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace NRaas.CommonSpace.Booters
{
    public class SocializingBooter : BooterHelper.TableBooter
    {
        public SocializingBooter()
            : this(VersionStamp.sNamespace + ".SocialData", true)
        { }
        public SocializingBooter(string reference, bool testDirect)
            : base("SocializingFile", reference, testDirect)
        { }

        public override BooterHelper.BootFile GetBootFile(string reference, string name, bool primary)
        {
            return new BooterHelper.DocumentBootFile(reference, name, primary);
        }

        protected override bool Perform(BooterHelper.BootFile file)
        {
            if (base.Perform(file)) return true;

            BooterHelper.DocumentBootFile docfile = file as BooterHelper.DocumentBootFile;
            if (docfile == null) return false;

            BooterHelper.DataBootFile availability = new BooterHelper.DataBootFile(file.ToString(), VersionStamp.sNamespace + ".SocializingActionAvailability", false);
            if (!availability.IsValid)
            {
                return false;
            }

            Load(docfile, availability);
            return true;
        }

        protected override void Perform(BooterHelper.BootFile file, XmlDbRow row)
        {
            BooterHelper.DocumentBootFile socialData = new BooterHelper.DocumentBootFile(file.ToString(), row.GetString("SocialData"), false);
            if (!socialData.IsValid)
            {
                BooterLogger.AddTrace(file + ": No SocialData");
                return;
            }

            BooterHelper.DataBootFile availability = new BooterHelper.DataBootFile(file.ToString(), row.GetString("SocializingActionAvailability"), false);
            if (!availability.IsValid)
            {
                BooterLogger.AddTrace(file + ": No SocializingActionAvailability");
                return;
            }

            Load(socialData, availability);
        }

        protected static bool Load(BooterHelper.DocumentBootFile socialData, BooterHelper.DataBootFile actionAvailability)
        {
            bool result = true;

            if (!LoadSocialData(socialData))
            {
                result = false;
            }

            if (!LoadSocializingActionAvailability(actionAvailability))
            {
                result = false;
            }

            return result;
        }

        protected static bool LoadSocialData(BooterHelper.DocumentBootFile socialData)
        {
            if (!socialData.IsValid)
            {
                BooterLogger.AddError(socialData + ": Unknown SocialData File");
                return false;
            }

            XmlElementLookup lookup = new XmlElementLookup(socialData.Document);
            List<XmlElement> actions = lookup["Action"];
            if ((actions == null) || (actions.Count == 0))
            {
                BooterLogger.AddError(socialData + ": No Action");
                return false;
            }

            bool isEp5Installed = GameUtils.IsInstalled(ProductVersion.EP5);

            foreach (XmlElement element in actions)
            {
                XmlElementLookup table = new XmlElementLookup(element);

                CommodityTypes types;
                ParserFunctions.TryParseEnum<CommodityTypes>(element.GetAttribute("com"), out types, CommodityTypes.Undefined);

                ProductVersion version;
                ParserFunctions.TryParseEnum<ProductVersion>(element.GetAttribute("ver"), out version, ProductVersion.BaseGame);

                ActionData data = new ActionData(element.GetAttribute("key"), types, version, table, isEp5Installed);

                List<XmlElement> list = table["LHS"];
                if (list.Count > 0x0)
                {
                    SocialRuleLHS.sDictionary.Remove(data.Key);

                    foreach (XmlElement element3 in list)
                    {
                        SocialRuleLHS lhs = new SocialRuleLHS(data.Key, data.IntendedCommodityString, element3);
                        lhs.ProceduralPrecondition = FindMethod(element3.GetAttribute("ProcTest"));
                    }

                    SocialRuleLHS.Get(data.Key).Sort(new Comparison<SocialRuleLHS>(SocialRuleLHS.SortSocialRules));
                }

                ActionData.Add(data);

                BooterLogger.AddTrace(" " + data.Key + " Added");
            }

            return true;
        }

        protected static bool LoadSocializingActionAvailability(BooterHelper.DataBootFile actionAvailability)
        {
            if (actionAvailability.GetTable("ActiveTopic") == null)
            {
                BooterLogger.AddError(actionAvailability + ": No ActiveTopic");
                return false;
            }

            SocialManager.ParseStcActionAvailability(actionAvailability.Data);
            SocialManager.ParseActiveTopic(actionAvailability.Data);

            return true;
        }

        private static MethodInfo FindMethod(string methodName)
        {
            MethodInfo info = new Common.MethodStore(methodName, null).Method;
            if (info != null)
            {
                return info;
            }

            Type type = typeof(SocialTest);
            return type.GetMethod(methodName);
        }
    }
}
