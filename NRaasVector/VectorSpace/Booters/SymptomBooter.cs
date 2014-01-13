using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.VectorSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Rewards;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace NRaas.VectorSpace.Booters
{
    public class SymptomBooter : BooterHelper.ByRowListingBooter
    {
        static Dictionary<string, Data> sSymptoms = new Dictionary<string, Data>();

        public SymptomBooter(string reference)
            : base("Symptom", "SymptomsFile", "File", reference, false)
        { }

        public static IEnumerable<Data> Symptoms
        {
            get { return sSymptoms.Values; }
        }

        protected override void Perform(BooterHelper.BootFile file, XmlDbRow row)
        {
            string guid = row.GetString("GUID");
            if (string.IsNullOrEmpty(guid))
            {
                BooterLogger.AddError("Invalid GUID: " + guid);
                return;
            }
            else if (sSymptoms.ContainsKey(guid))
            {
                BooterLogger.AddError("Duplicate GUID: " + guid);
                return;
            }

            Type type = row.GetClassType("FullClassName");
            if (type == null)
            {
                BooterLogger.AddError(guid + " Invalid FullClassName: " + row.GetString("FullClassName"));
                return;
            }

            Data symptom = null;

            try
            {
                symptom = type.GetConstructor(new Type[] { typeof(XmlDbRow) }).Invoke(new object[] { row }) as Data;
            }
            catch (Exception e)
            {
                BooterLogger.AddError("Contructor Fail: " + row.GetString("FullClassName"));

                Common.Exception(guid + Common.NewLine + row.GetString("FullClassName") + " Fail", e);
            }

            if (symptom != null)
            {
                sSymptoms.Add(guid, symptom);
            }
        }

        public static Data GetSymptom(string guid)
        {
            Data vector;
            if (!sSymptoms.TryGetValue(guid, out vector)) return null;

            return vector;
        }

        public abstract class Data
        {
            string mGuid;

            public Data(XmlDbRow row)
            {
                mGuid = row.GetString("GUID");
            }

            public abstract void Perform(Sim sim, DiseaseVector vector);

            public string Guid
            {
                get { return mGuid; }
            }

            public string GetLocalizedName(bool isFemale)
            {
                return Common.Localize("SymptomName:" + mGuid, isFemale);
            }

            public override string ToString()
            {
                string result = mGuid;
                result += Common.NewLine + " Type: " + GetType().Name;

                return result;
            }
        }
    }
}
