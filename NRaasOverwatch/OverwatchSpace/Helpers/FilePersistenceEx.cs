using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.CommonSpace.Replacers;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.Entertainment;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace NRaas.OverwatchSpace.Helpers
{
    public class FilePersistenceEx : FilePersistence
    {
        public static IEnumerable<Item> GetChoices(string name)
        {
            List<Item> allOptions = new List<Item>();

            allOptions.Add(null);

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    if (!assembly.GetName().Name.StartsWith("NRaas")) continue;

                    Type common = assembly.GetType("NRaas.CommonSpace.Helpers.Persistence");
                    if (common == null) continue;

                    MethodInfo importer = common.GetMethod("ImportSettings", BindingFlags.Public | BindingFlags.Static);
                    if (importer == null) continue;

                    MethodInfo exporter = common.GetMethod("CreateExportString", BindingFlags.Public | BindingFlags.Static);
                    if (exporter == null) continue;

                    allOptions.Add(new Item(Common.AssemblyCheck.GetNamespace(assembly), importer, exporter));
                }
                catch (Exception e)
                {
                    Common.Exception(assembly.GetName().Name, e);
                }
            }

            CommonSelection<Item> selector = new CommonSelection<Item>(name, allOptions);
            selector.AllOnNull = true;

            CommonSelection<Item>.Results results = selector.SelectMultiple();
            if ((results == null) || (results.Count == 0)) return null;

            return results;
        }

        public class Item : CommonOptionItem
        {
            MethodInfo mImporter;
            MethodInfo mExporter;

            public Item(string nameSpace, MethodInfo importer, MethodInfo exporter)
                : base(Common.LocalizeEAString(nameSpace + ".Root:MenuName").Replace("...", ""))
            {
                mImporter = importer;
                mExporter = exporter;
            }

            public override string DisplayValue
            {
                get { return null; }
            }

            public string CreateExportString()
            {
                try
                {
                    return (string)mExporter.Invoke(null, new object[0]);
                }
                catch (Exception e)
                {
                    Common.Exception(Name, e);
                    return null;
                }
            }

            public void Import(XmlElement element)
            {
                try
                {
                    mImporter.Invoke(null, new object[] { element });
                }
                catch (Exception e)
                {
                    Common.Exception(Name, e);
                }
            }
        }
    }
}
