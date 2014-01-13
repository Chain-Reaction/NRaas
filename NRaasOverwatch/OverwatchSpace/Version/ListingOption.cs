using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.OverwatchSpace.Version
{
    public class ListingOption : OptionList<ListingOption.Item>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "Version";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override List<ListingOption.Item> GetOptions()
        {
            List<Item> results = new List<Item>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assembly.GetName().Name.StartsWith("NRaas")) continue;

                Type type = assembly.GetType("NRaas.VersionStamp");
                if (type == null) continue;

                string nameSpace = Common.AssemblyCheck.GetNamespace(assembly);

                FieldInfo versionField = type.GetField("sVersion", BindingFlags.Static | BindingFlags.Public);
                if (versionField == null) continue;

                int version = (int)versionField.GetValue(null);

                string prompt = null;

                Type externalVersion = assembly.GetType("NRaas.CommonSpace.Options.ExternalVersion");
                if (externalVersion != null)
                {
                    MethodInfo func = externalVersion.GetMethod("ExternalVersionPrompt", BindingFlags.Public | BindingFlags.Static);
                    if (func != null)
                    {
                        StringBuilder builder = new StringBuilder();

                        func.Invoke(null, new object[] { builder });

                        prompt = builder.ToString();
                    }
                }

                results.Add(new Item(nameSpace, version, prompt));
            }

            return results;
        }

        public class Item : OperationSettingOption<GameObject>
        {
            string mNameSpace;
            string mPrompt;
            int mVersion;

            public Item(string nameSpace, int version, string prompt)
            {
                mNameSpace = nameSpace;
                mPrompt = prompt;
                if (string.IsNullOrEmpty(mPrompt))
                {
                    mPrompt = Common.LocalizeEAString(false, mNameSpace + ".Version:Prompt", new object[] { version });
                }
                mVersion = version;
            }

            public override string GetTitlePrefix()
            {
                return null;
            }

            public override string Name
            {
                get { return Common.LocalizeEAString(mNameSpace + ".Root:MenuName").Replace("...",""); }
            }

            public override string DisplayValue
            {
                get { return EAText.GetNumberString(mVersion); }
            }

            protected override OptionResult Run(GameHitParameters< GameObject> parameters)
            {
                SimpleMessageDialog.Show(Name, mPrompt);
                return OptionResult.SuccessRetain;
            }
        }
    }
}
