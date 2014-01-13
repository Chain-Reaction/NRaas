using NRaas.CareerSpace.Booters;
using NRaas.Gameplay.Careers;
using NRaas.Gameplay.OmniSpace.Metrics;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.OmniSpace
{
    public class OmniJournalData : BookData
    {
        // Fields
        string mCareerName;
        int mCareerLevel;
        int mCurrentEdition;

        int mMaxEdition;

        static Dictionary<string,Dictionary<int, OmniJournalData>> sJournalDataList = new Dictionary<string,Dictionary<int, OmniJournalData>>();
 
        // Methods
        public OmniJournalData()
        {
            mCurrentEdition = 1;
            mMaxEdition = 1;
        }
        public OmniJournalData(XmlDbRow row, int rowIndex) 
            : base(row, "OmniJournal", rowIndex)
        {
            mCareerName = row.GetString("CareerName");
            mCareerLevel = ParserFunctions.ParseInt(row.GetString("CareerLevel"), 0);
            mCurrentEdition = ParserFunctions.ParseInt(row.GetString("StartingEdition"), 1);

            mMaxEdition = ParserFunctions.ParseInt(row.GetString("MaxEdition"), 1);

            base.MyType = BookData.BookType.MedicalJournal;

            Dictionary<int, OmniJournalData> levels;
            if (!sJournalDataList.TryGetValue(mCareerName, out levels))
            {
                levels = new Dictionary<int, OmniJournalData>();

                sJournalDataList.Add(mCareerName, levels);
            }

            if (!levels.ContainsKey(mCareerLevel))
            {
                levels.Add(mCareerLevel, this);
            }
        }
        public OmniJournalData(OmniJournalData data)
        {
            Title = data.mTitle;
            Length = data.Length;
            Value = data.Value;
            PagesMinNorm = data.PagesMinNorm;
            PagesMinBW = data.PagesMinBW;
            ID = data.ID;
            GeometryState = data.GeometryState;
            MaterialState = data.MaterialState;
            MyType = data.MyType;
            RowIndex = data.RowIndex;
            NotInBookStore = data.NotInBookStore;

            mCareerName = data.mCareerName;
            mCareerLevel = data.mCareerLevel;
            mCurrentEdition = data.mCurrentEdition;
            mMaxEdition = data.mMaxEdition;
            mAuthor = data.mAuthor;
        }

        public int CurrentEdition
        {
            get { return mCurrentEdition; }
        }

        public int CareerLevel
        {
            get { return mCareerLevel; }
        }

        public OmniJournalData Clone()
        {
            return new OmniJournalData(this);
        }

        public static OmniJournalData GetJournalData(string career, int level)
        {
            career = career.Replace("Gameplay/Excel/Careers/CareerList:", "");

            Dictionary<int, OmniJournalData> levels;
            if (!sJournalDataList.TryGetValue(career, out levels))
            {
                return null;
            }

            OmniJournalData data;
            if (!levels.TryGetValue(level, out data))
            {
                return null;
            }

            return data;
        }

        public override string GenerateUIStoreItemID()
        {
            return ("BookOmniJournal_" + ID);
        }

        public string GetTitle(SimDescription actor, int edition)
        {
            try
            {
                int editionIndex = edition;

                if (mMaxEdition > 0)
                {
                    editionIndex = editionIndex % mMaxEdition;
                }

                editionIndex++;

                string index = null;
                if (editionIndex != 1)
                {
                    index = editionIndex.ToString();
                }

                return Common.LocalizeEAString(actor.IsFemale, mTitle + index, new object[] { actor, edition });
            }
            catch (Exception e)
            {
                Common.Exception(actor, e);
            }

            return null;
        }
    }
}
