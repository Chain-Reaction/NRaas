using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate.Careers
{
    public class AssignAcademic : SimFromList, ICareerOption
    {
        public override string GetTitlePrefix()
        {
            return "AssignAcademic";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool AllowSpecies(IMiniSimDescription me)
        {
            return me.IsHuman;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (me.ChildOrBelow) return false;

            return base.PrivateAllow(me);
        }

        protected override OptionResult RunAll(List<IMiniSimDescription> sims)
        {
            List<SimDescription> trueSims = new List<SimDescription>();
            foreach (IMiniSimDescription miniSim in sims)
            {
                SimDescription trueSim = miniSim as SimDescription;
                if (trueSim == null) continue;

                trueSims.Add(trueSim);
            }

            int termLen = 0;
            int totalCost = 0;

            List<IEnrollmentData> list = EnrollmentDialogEx.Show(trueSims[0], trueSims, true, out termLen, out totalCost);
            if ((list != null) && (list.Count > 0))
            {
                AcademicCareer.GlobalTermLength = (AcademicCareer.TermLength)termLen;

                foreach (IEnrollmentData data in list)
                {
                    SimDescription enrollingSimDesc = data.EnrollingSimDesc as SimDescription;
                    if ((enrollingSimDesc != null) && 
                        (enrollingSimDesc.CareerManager != null) && 
                        (enrollingSimDesc.CareerManager.DegreeManager != null))
                    {
                        enrollingSimDesc.CareerManager.DegreeManager.SetEnrollmentData(data);

                        if (enrollingSimDesc.CreatedSim != null)
                        {
                            EventTracker.SendEvent(new AcademicEvent(EventTypeId.kEnrolledInUniversity, enrollingSimDesc.CreatedSim, (AcademicDegreeNames)data.AcademicDegreeName));
                        }

                        AcademicCareer.EnrollSimInAcademicCareer(enrollingSimDesc, (AcademicDegreeNames)data.AcademicDegreeName, data.CourseLoad);

                        CustomAcademicDegrees.AdjustCustomAcademics(enrollingSimDesc);
                    }
                }
            }

            return OptionResult.SuccessRetain;
        }
    }
}
