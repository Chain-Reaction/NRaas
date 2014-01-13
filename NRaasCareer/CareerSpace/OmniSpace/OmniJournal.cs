using NRaas;
using NRaas.CommonSpace.Helpers;
using NRaas.CareerSpace;
using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.OmniSpace;
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

namespace Sims3.CareerSpace.OmniSpace
{
    public class OmniJournal : Book, INonPurgeableFromNPCInventory, IGameObject, IScriptObject, IScriptLogic, IHasScriptProxy, IObjectUI, IExportableContent
    {
        // Fields
        public string mBookCareer;
        public int mBookLevelId;
        public int mJournalEdition;
        public AlarmHandle mJournalUpdate;
        public SimDescription mOwner;
        public bool mUpdatedToday;

        // Methods
        public static OmniJournal CreateOutOfWorld(OmniJournalData data, SimDescription Actor)
        {
            OmniJournal journal = ObjectCreation.CreateObject(0x6694B72C99D44369, ProductVersion.BaseGame, null) as OmniJournal;
            if (journal == null)
            {
                return null;
            }

            journal.Data = data.Clone();
            journal.mJournalEdition = data.CurrentEdition + SimClock.ElapsedCalendarDays();
            journal.Data.ID = data.ID + journal.mJournalEdition;

            Career career = Actor.Occupation as Career;
            if (career != null)
            {
                journal.mBookCareer = career.SharedData.Name;
            }

            journal.mBookLevelId = Actor.Occupation.CareerLevel;
            journal.mBookId = journal.Data.ID;
            journal.mOwner = Actor;
            journal.mUpdatedToday = true;
            journal.SetGeometryState(journal.Data.GeometryState);
            journal.SetMaterial(journal.Data.MaterialState);
            return journal;
        }

        public override bool ExportContent(ResKeyTable resKeyTable, ObjectIdTable objIdTable, IPropertyStreamWriter writer)
        {
            return false;
        }

        public override string GetLocalizedName()
        {
            return ToTooltipString();
        }

        public override bool ImportContent(ResKeyTable resKeyTable, ObjectIdTable objIdTable, IPropertyStreamReader reader)
        {
            return false;
        }

        public static string LocalizeString(SimDescription sim, string name, params object[] parameters)
        {
            return OmniCareer.LocalizeString(sim, "Journal:" + name, "NRaas.Careers.Journal:" + name, parameters);
        }

        public override void OnLoad()
        {
            try
            {
                OmniJournalData data = OmniJournalData.GetJournalData(mBookCareer, mBookLevelId);
                if (data != null)
                {
                    base.Data = data.Clone();
                    base.Data.ID = base.Data.ID + this.mJournalEdition;
                }
            }
            catch (Exception e)
            {
                Common.Exception(mBookCareer + " - " + mBookLevelId, e);
            }
        }

        public override bool TestReadBook(Sim Actor, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            GreyedOutTooltipCallback callback = null;
            OmniJournalData data = base.Data as OmniJournalData;

            OmniCareer career = Actor.Occupation as OmniCareer;
            if (career == null) return true;

            if ((career.CurLevel.HasMetricType(typeof(MetricJournals))) && ((data != null) && (career.CareerLevel >= data.CareerLevel)))
            {
                return true;
            }
            if (callback == null)
            {
                callback = delegate
                {
                    return LocalizeString(Actor.SimDescription, "NonDoctorReadFailureTooltip", new object[] { Actor });
                };
            }
            greyedOutTooltipCallback = callback;
            return false;
        }

        public override string ToTooltipString()
        {
            if (base.Data is OmniJournalData)
            {
                return (base.Data as OmniJournalData).GetTitle(mOwner, mJournalEdition);
            }
            return "";
        }

        public void UpdateJournal(Career job)
        {
            if (!mUpdatedToday)
            {
                if (job.SharedData == null) return;

                mUpdatedToday = true;
                OmniJournalData data = OmniJournalData.GetJournalData(job.SharedData.Name, job.CareerLevel);

                if (data != null)
                {
                    data = data.Clone();
                    if (data != null)
                    {
                        mJournalEdition = data.CurrentEdition + SimClock.ElapsedCalendarDays();
                        base.Data = data;
                        base.Data.ID = data.ID + mJournalEdition;
                        base.BookId = base.Data.ID;

                        mBookCareer = job.SharedData.Name;
                        mBookLevelId = job.CareerLevel;
                    }
                }
            }
        }

        // Properties
        public override string Title
        {
            get { return ToTooltipString(); }
        }
    }
}
