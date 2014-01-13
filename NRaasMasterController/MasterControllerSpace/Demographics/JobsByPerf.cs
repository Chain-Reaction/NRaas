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
    public class JobsByPerf : DemographicOption
    {
        public override string GetTitlePrefix()
        {
            return "JobsByPerf";
        }

        protected string GetDetails(List<IMiniSimDescription> sims)
        {
            long lUnemployed = 0, lRetired = 0, lTotalWorking = 0, lNoPerfCount = 0, lNoPerfLevel = 0;

            List<long> negPerfLevel = new List<long>();
            List<long> posPerfLevel = new List<long>();

            List<long> negPerfCount = new List<long>();
            List<long> posPerfCount = new List<long>();

            for (int perf = 0; perf < 10; perf++)
            {
                negPerfLevel.Add(0);
                posPerfLevel.Add(0);

                negPerfCount.Add(0);
                posPerfCount.Add(0);
            }

            foreach (IMiniSimDescription miniSim in sims)
            {
                SimDescription member = miniSim as SimDescription;
                if (member == null) continue;

                if (SimTypes.IsSpecial(member)) continue;

                if (!member.TeenOrAbove) continue;

                if (member.Occupation == null)
                {
                    if (member.CareerManager.RetiredCareer != null)
                    {
                        lRetired++;
                    }
                    else if (!member.IsPet)
                    {
                        lUnemployed++;
                    }
                }
                else
                {
                    Career career = member.Occupation as Career;
                    if ((career != null) && (career.CareerLevel == 1) && (career.CurLevel != null) && (career.CurLevel.DayLength == 0))
                    {
                        lRetired++;
                    }
                    else
                    {
                        float perf = StatusJobPerformance.GetPerformance(member);
                        if (perf == 0)
                        {
                            lNoPerfCount++;

                            lNoPerfLevel += member.Occupation.CareerLevel;
                        }
                        else if (perf > 0)
                        {
                            int iPerf = (int)Math.Ceiling(perf / 10f);
                            if (iPerf > 9) iPerf = 9;
                            posPerfCount[iPerf]++;

                            posPerfLevel[iPerf] += member.Occupation.CareerLevel;
                        }
                        else
                        {
                            int iPerf = (int)Math.Ceiling(-perf / 10f);
                            if (iPerf > 9) iPerf = 9;
                            negPerfCount[iPerf]++;

                            negPerfLevel[iPerf] += member.Occupation.CareerLevel;
                        }

                        lTotalWorking++;
                    }
                }
            }

            string posBody = null, negBody = null;

            for (int perf = negPerfCount.Count - 1; perf >= 1; perf--)
            {
                negBody += Common.Localize("JobsByPerf:Element1", false, new object[] { (perf * 10) });
                if (negPerfCount[perf] == 0)
                {
                    negBody += "-";
                }
                else
                {
                    negBody += Common.Localize("JobsByPerf:Element2", false, new object[] { negPerfCount[perf], (((float)negPerfLevel[perf]) / negPerfCount[perf]) });
                }
            }

            if (lNoPerfCount == 0)
            {
                posBody += "-";
            }
            else
            {
                posBody += Common.Localize("JobsByPerf:Element2", false, new object[] { lNoPerfCount, (((float)lNoPerfLevel) / lNoPerfCount) });
            }

            for (int perf = 1; perf < posPerfCount.Count; perf++)
            {
                posBody += Common.Localize("JobsByPerf:Element3", false, new object[] { (perf * 10) });
                if (posPerfCount[perf] == 0)
                {
                    posBody += "-";
                }
                else
                {
                    posBody += Common.Localize("JobsByPerf:Element2", false, new object[] { posPerfCount[perf], (((float)posPerfLevel[perf]) / posPerfCount[perf]) });
                }
            }

            return Common.Localize("JobsByPerf:Body", false, new object[] { lUnemployed, lRetired, negBody, posBody, lTotalWorking });
        }

        protected override OptionResult RunAll(List<IMiniSimDescription> sims)
        {
            Sims3.UI.SimpleMessageDialog.Show(Common.Localize("JobsByPerf:Header"), GetDetails(sims));
            return OptionResult.SuccessRetain;
        }
    }
}
