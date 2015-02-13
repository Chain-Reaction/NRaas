using System;
using Sims3.SimIFace;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Actors;
using Sims3.UI.Hud;
using System.Collections.Generic;
using Sims3.UI;
using System.Text;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.ActorSystems;

namespace ani_WorkManager
{
    public class NoFreePerformance
    {
        [Tunable]
        protected static bool tyoNuuttis;
        [Tunable]
        protected static bool removePayments;


        // Methods
        static NoFreePerformance()
        {
            World.sOnWorldLoadFinishedEventHandler += new EventHandler(NoFreePerformance.World_OnWorldLoadFinishedEventHandler);
        }

        static void World_OnWorldLoadFinishedEventHandler(object sender, EventArgs e)
        {
            EventTracker.AddListener(EventTypeId.kFinishedWork, new ProcessEventDelegate(NoFreePerformance.LeftWork));
            EventTracker.AddListener(EventTypeId.kGotPaid, new ProcessEventDelegate(NoFreePerformance.GotPaid));
        }

        protected static ListenerAction GotPaid(Event e)
        {
            Sim sim = e.Actor as Sim;
            if (removePayments)
            {
                if (sim != null && !sim.Household.IsActive)
                {
                    if (sim.CareerManager != null && sim.CareerManager.RetiredCareer != null)
                    {
                        int x = sim.Household.FamilyFunds;
                        sim.Household.ModifyFamilyFunds(-sim.CareerManager.RetiredCareer.PensionAmount() / 2);
                    }
                }
            }
            return ListenerAction.Keep;
        }

        protected static ListenerAction LeftWork(Event e)
        {
            bool meetsExpectations = true;
            Sim sim = e.Actor as Sim;

            if ((sim != null) && (sim.LotHome != null))
            {
                if (sim.Household.IsActive)
                {
                    if (sim.CareerManager != null && sim.CareerManager.Occupation != null)
                    {
                        List<PerfFactor> perfFactors = sim.CareerManager.Occupation.PerfFactors;
                        if (perfFactors != null)
                        {
                            foreach (var perf in perfFactors)
                            {
                                //Find the skill
                                ISkillJournalEntry skill = sim.SkillManager.SkillJournalEntries.Find(delegate(ISkillJournalEntry s) { return s.SkillGuid == perf.SkillGuid; });
                                                                  
                                //Don't take the mood into consideration
                                if (perf.SkillGuid != 0)
                                    if (skill == null || (skill != null && skill.SkillLevel < perf.PosPerfLevel))
                                    {
                                        meetsExpectations = false;
                                    }
                            }


                        }
                        else
                        {
                            meetsExpectations = false;
                        }
                    }
                    if (!meetsExpectations)
                    {
                        if (sim.CareerManager.Occupation.Performance > 0)
                        {
                            sim.CareerManager.Occupation.UpdatePerformanceOrExperience(-sim.CareerManager.Occupation.Performance);
                            sim.CareerManager.UpdatePerformanceUI(sim.CareerManager.Occupation);
                        }
                    }
                }
                else
                {
                    //Reset wage
                    if (removePayments && sim.CareerManager != null && sim.CareerManager.Occupation != null)
                    {
                        int pay = (int)((sim.CareerManager.Occupation.FinishTime - sim.CareerManager.Occupation.StartTime) * sim.CareerManager.Occupation.PayPerHourOrStipend);
                        sim.Household.ModifyFamilyFunds(-pay);
                    }
                }
            }

            return ListenerAction.Keep;
        }





    }
}
