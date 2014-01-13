using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Demographics
{
    public class JobsByCareerSummary : DemographicOption
    {
        public override string GetTitlePrefix()
        {
            return "JobsByCareerSummary";
        }

        protected string GetDetails(List<IMiniSimDescription> sims)
        {
            long lUnemployed = 0, lRetired = 0, lWorkingPension = 0, lTotalWorking = 0;
            long lDemotionPerf = 0;
            long lPositivePerf = 0;

            Dictionary<OccupationNames, long> list = new Dictionary<OccupationNames, long>();

            foreach (IMiniSimDescription miniSim in sims)
            {
                SimDescription member = miniSim as SimDescription;
                if (member == null) continue;

                try
                {
                    if (member.Occupation == null)
                    {
                        if ((!member.TeenOrAbove) || (!member.IsHuman)) continue;

                        if (SimTypes.IsSpecial(member)) continue;

                        if ((member.CareerManager != null) && (member.CareerManager.RetiredCareer != null))
                        {
                            lRetired++;
                        }
                        else
                        {
                            lUnemployed++;
                        }
                    }
                    else
                    {
                        if (list.ContainsKey(member.Occupation.Guid))
                        {
                            list[member.Occupation.Guid]++;
                        }
                        else
                        {
                            list.Add(member.Occupation.Guid, 1);
                        }

                        float performance = StatusJobPerformance.GetPerformance(member);
                        if (performance >= 0.0)
                        {
                            lPositivePerf++;
                        }
                        else if (performance < Sims3.Gameplay.Careers.Career.kDemotionThreshold)
                        {
                            lDemotionPerf++;
                        }
                        if (member.CareerManager.RetiredCareer != null)
                        {
                            lWorkingPension++;
                        }
                        lTotalWorking++;
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(member, e);
                }
            }

            List<string> sorted = new List<string>();
            foreach (KeyValuePair<OccupationNames, long> value in list)
            {
                Occupation career = CareerManager.GetStaticOccupation(value.Key);
                if (career == null) continue;

                sorted.Add(Common.Localize("JobsByCareerSummary:Element", false, new object[] { career.CareerName, value.Value }));
            }

            sorted.Sort(StringComparer.CurrentCulture);

            string body = null;
            foreach (string value in sorted)
            {
                body += value;
            }

            return Common.Localize("JobsByCareerSummary:Body", false, new object[] { lUnemployed, lRetired, lWorkingPension, body, lTotalWorking, lPositivePerf, lDemotionPerf });
        }

        protected override OptionResult RunAll(List<IMiniSimDescription> sims)
        {
            Sims3.UI.SimpleMessageDialog.Show(Name, GetDetails(sims));
            return OptionResult.SuccessRetain;
        }
    }
}
