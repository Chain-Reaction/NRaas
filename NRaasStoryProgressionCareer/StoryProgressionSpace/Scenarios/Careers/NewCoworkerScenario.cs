using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public class NewCoworkerScenario : CoworkerBaseScenario
    {
        public NewCoworkerScenario()
        { }
        public NewCoworkerScenario(SimDescription sim)
            : base(sim)
        { }
        protected NewCoworkerScenario(NewCoworkerScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "NewCoworker";
        }

        protected override Career Career
        {
            get { return Sim.Occupation as Career; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Careers.Employed;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!base.Allow(sim)) return false;

            if (!(sim.Occupation is Career))
            {
                IncStat("Not Career");
                return false;
            }
            else if (!ManagerCareer.ValidCareer(sim.Occupation))
            {
                IncStat("No Career");
                return false;
            }
            else if (sim.Occupation is Retired)
            {
                IncStat("Retired");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool IsValid(Career job, SimDescription sim, bool checkExisting)
        {
            if (!base.IsValid(job, sim, checkExisting))
            {
                return false;
            }

            Career simJob = sim.Occupation as Career;
            if (simJob == null)
            {
                IncStat("Valid: No Job");
                return false;
            }
            else if (simJob.Guid != job.Guid)
            {
                IncStat("Valid: Wrong Job");
                return false;
            }

            if (sim.Occupation.Coworkers == null)
            {
                sim.Occupation.Coworkers = new List<SimDescription>();
            }

            return true;
        }

        protected bool IsValidPartner(LawEnforcement lawCareer, SimDescription sim, bool force)
        {
            if (lawCareer == null) return false;

            if (sim == null) return false;

            if (!IsValid(lawCareer, sim, false))
            {
                IncStat("Not Valid Partner");
                return false;
            }

            LawEnforcement simJob = sim.Occupation as LawEnforcement;
            if (simJob == null) return false;

            if ((simJob.Partner != null) && (simJob.Partner != lawCareer.OwnerDescription))
            {
                if (!force)
                {
                    IncStat("Already Partnered");
                    return false;
                }
                else if (SimTypes.IsSelectable (simJob.Partner))
                {
                    IncStat("Partner Active");
                    return false;
                }
            }

            return true;
        }

        protected bool HandlePartner(LawEnforcement lawCareer, SimDescription coworker, bool force)
        {
            if (coworker == null) return false;

            SimDescription sim = lawCareer.OwnerDescription;

            LawEnforcement coworkerCareer = coworker.Occupation as LawEnforcement;
            if (coworkerCareer == null)
            {
                IncStat("Law: Not Police");
                return false;
            }
            else if (!IsValidPartner(lawCareer, coworker, force))
            {
                IncStat("Law: Not Valid 1");
                return false;
            }
            else if (!IsValidPartner(coworkerCareer, sim, force))
            {
                IncStat("Law: Not Valid 2");
                return false;
            }
            else
            {
                lawCareer.Partner = coworker;
                if (lawCareer.Boss == coworker)
                {
                    lawCareer.SetBoss(null);

                    lawCareer.AddCoworker(coworker);

                    IncStat("Law: Boss Used");
                }

                coworkerCareer.Partner = sim;
                if (coworkerCareer.Boss == sim)
                {
                    coworkerCareer.SetBoss(null);

                    coworkerCareer.AddCoworker(sim);

                    IncStat("Law: Boss Used");
                }

                mNewCoworkers.Add(coworker);

                IncStat("Law: Success");
                return true;
            }
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Career job = Career;

            LawEnforcement lawCareer = job as LawEnforcement;

            if (lawCareer != null) 
            {
                SimDescription partner = lawCareer.Partner;
                if (partner != null)
                {
                    if (IsValidPartner(lawCareer, partner, false))
                    {
                        try
                        {
                            job.AddCoworker(partner);
                        }
                        catch (Exception e)
                        {
                            Common.Exception(Career.OwnerDescription, partner, e);
                        }
                    }
                }
            }

            base.PrivateUpdate(frame);

            if (lawCareer != null)
            {
                if (!IsValidPartner(lawCareer, lawCareer.Partner, false))
                {
                    lawCareer.Partner = null;

                    if (job.Coworkers != null)
                    {
                        foreach (SimDescription coworker in job.Coworkers)
                        {
                            if (HandlePartner(lawCareer, coworker, SimTypes.IsSelectable(Sim)))
                            {
                                break;
                            }
                        }
                    }

                    if (lawCareer.Partner == null)
                    {
                        HandlePartner(lawCareer, lawCareer.Boss, SimTypes.IsSelectable(Sim));
                    }
                }

                if (lawCareer.Partner != null)
                {
                    if (Relationship.AreStrangers(Sim, lawCareer.Partner))
                    {
                        Relationship.Get(Sim, lawCareer.Partner, true).MakeAcquaintances();
                    }
                }
            }

            Careers.VerifyTone(Sim);
            return (mNewCoworkers.Count > 0);
        }

        public override Scenario Clone()
        {
            return new NewCoworkerScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerCareer, NewCoworkerScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ManageCoworker";
            }
        }
    }
}
