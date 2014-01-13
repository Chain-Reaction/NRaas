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
    public class ResistanceBooter : BooterHelper.ByRowListingBooter
    {
        static Dictionary<string, Data> sResistances = new Dictionary<string, Data>();

        public ResistanceBooter(string reference)
            : base("Resistance", "ResistancesFile", "File", reference, false)
        { }

        public static IEnumerable<Data> Resistances
        {
            get { return sResistances.Values; }
        }

        protected override void Perform(BooterHelper.BootFile file, XmlDbRow row)
        {
            string guid = row.GetString("GUID");
            if (string.IsNullOrEmpty(guid))
            {
                BooterLogger.AddError("Invalid GUID: " + guid);
                return;
            }
            else if (sResistances.ContainsKey(guid))
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
                sResistances.Add(guid, symptom);
            }
        }

        public static Data GetResistance(string guid)
        {
            Data vector;
            if (!sResistances.TryGetValue(guid, out vector)) return null;

            return vector;
        }

        public abstract class Data
        {
            string mGuid;

            int mDelta;

            VectorBooter.Test mTest;

            public Data(XmlDbRow row, VectorBooter.Test test)
            {
                mGuid = row.GetString("GUID");

                mDelta = row.GetInt("Delta", 0);

                if (mDelta == 0)
                {
                    BooterLogger.AddError(mGuid + " Missing Delta");
                }

                mTest = test;
            }

            public string Guid
            {
                get { return mGuid; }
            }

            public virtual int Delta
            {
                get { return mDelta; }
            }

            public virtual void GetEvents(Dictionary<EventTypeId, bool> events)
            {
                mTest.GetEvents(events);
            }

            public virtual void Perform(Event e, DiseaseVector vector)
            {
                if (!mTest.IsSuccess(e)) return;

                vector.AlterResistance(Delta);

                ScoringLog.sLog.AddStat(Guid + " Resistance", Delta);
            }

            public string GetLocalizedName(bool isFemale)
            {
                return Common.Localize("ResistanceName:" + mGuid, isFemale);
            }

            public override string ToString()
            {
                string result = mGuid;
                result += Common.NewLine + " Type: " + GetType().Name;
                result += Common.NewLine + " Delta: " + mDelta;
                result += Common.NewLine + mTest;

                return result;
            }
        }
    }
}
