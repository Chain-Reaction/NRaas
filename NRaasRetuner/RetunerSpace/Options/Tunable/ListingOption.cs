using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RetunerSpace.Options.Tunable
{
    public class ListingOption : InteractionOptionList<TunableNamespaceOption, GameObject>, ITuningOption
    {
        static Dictionary<string,Dictionary<Type, List<FieldInfo>>> sTunables = null;

        public override string GetTitlePrefix()
        {
            return "TunableRoot";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override List<TunableNamespaceOption> GetOptions()
        {
            if (sTunables == null)
            {
                sTunables = new Dictionary<string,Dictionary<Type, List<FieldInfo>>>();

                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (!type.ToString().StartsWith("Sims3.")) continue;

                        List<FieldInfo> fields = new List<FieldInfo>();
                        foreach (FieldInfo field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                        {
                            foreach (object attribute in field.GetCustomAttributes(true))
                            {
                                if (attribute is TunableAttribute)
                                {
                                    fields.Add(field);
                                }
                            }
                        }

                        if (fields.Count > 0)
                        {
                            Dictionary<Type, List<FieldInfo>> types;
                            if (!sTunables.TryGetValue(type.Namespace, out types))
                            {
                                types = new Dictionary<Type, List<FieldInfo>>();
                                sTunables.Add(type.Namespace, types);
                            }

                            types.Add(type, fields);
                        }
                    }
                }
            }

            List<TunableNamespaceOption> results = new List<TunableNamespaceOption>();

            foreach (KeyValuePair<string, Dictionary<Type, List<FieldInfo>>> pair in sTunables)
            {
                results.Add(new TunableNamespaceOption(pair.Key, pair.Value));
            }

            return results;
        }
    }
}
