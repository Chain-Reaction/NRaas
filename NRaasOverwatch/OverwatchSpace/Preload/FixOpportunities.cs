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
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.OverwatchSpace.Preload
{
    public class FixOpportunities : PreloadOption
    {
        public FixOpportunities()
        { }

        public override string GetTitlePrefix()
        {
            return "FixOpportunities";
        }

        public override void OnPreLoad()
        {
            Overwatch.Log(GetTitlePrefix());

            Opportunity opportunity = OpportunityManager.GetStaticOpportunity(OpportunityNames.EP7_FortuneTellerCareer_HelpInvestigation_Police);
            if (opportunity != null)
            {
                if ((opportunity.SharedData.mRequirementList == null) || (opportunity.SharedData.mRequirementList.Count == 0))
                {
                    opportunity.SharedData.mRequirementList = new System.Collections.ArrayList();

                    // FortuneTeller career requirement
                    Opportunity.OpportunitySharedData.RequirementInfo info = new Opportunity.OpportunitySharedData.RequirementInfo();
                    info.mType = RequirementType.Career;
                    info.mGuid = (ulong)OccupationNames.FortuneTeller;
                    info.mMinLevel = 1;
                    info.mMaxLevel = 10;
                    opportunity.SharedData.mRequirementList.Add(info);

                    // Police Station requirement
                    info = new Opportunity.OpportunitySharedData.RequirementInfo();
                    info.mType = RequirementType.WorldHasRabbitHoleType;
                    info.mGuid = (ulong)RabbitHoleType.PoliceStation;
                    opportunity.SharedData.mRequirementList.Add(info);
                }
            }

            foreach (OpportunityNames opp in new OpportunityNames[] { OpportunityNames.EP9_Academics_StudyAtHome, OpportunityNames.EP9_Academics_StudyAtLibrary, OpportunityNames.EP9_Academics_StudyAtQuad, OpportunityNames.EP9_Academics_StudyAtStudentUnion })
            {
                opportunity = OpportunityManager.GetStaticOpportunity(opp);
                if (opportunity != null)
                {
                    if ((opportunity.SharedData.mRequirementList == null) || (opportunity.SharedData.mRequirementList.Count == 0))
                    {
                        opportunity.SharedData.mRequirementList = new System.Collections.ArrayList();

                        // University Student career requirement
                        Opportunity.OpportunitySharedData.RequirementInfo info = new Opportunity.OpportunitySharedData.RequirementInfo();
                        info.mType = RequirementType.Career;
                        info.mGuid = (ulong)OccupationNames.AcademicCareer;
                        info.mMinLevel = 1;
                        info.mMaxLevel = 1;
                        opportunity.SharedData.mRequirementList.Add(info);
                    }
                }
            }
        }
    }
}
