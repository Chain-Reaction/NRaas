using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Replacers;
using NRaas.CommonSpace.Selection;
using NRaas.VectorSpace.Booters;
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
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Rewards;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace NRaas.VectorSpace.Interactions
{
    public class Vaccinate : Common.IPreLoad
    {
        public void OnPreLoad()
        {
            if (GameUtils.IsInstalled(ProductVersion.EP2))
            {
                BooterLogger.AddError(SocialRHSReplacer.Perform<Vaccinate>("Vaccinate", "BeforeVaccinate"));
                BooterLogger.AddError(SocialRHSReplacer.Perform<Vaccinate>("Diagnose", "BeforeDiagnose"));
            }
        }

        public static void BeforeVaccinate(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                VaccinationSessionSituation vaccinationSessionSituation = VaccinationSessionSituation.GetVaccinationSessionSituation(actor);
                if (vaccinationSessionSituation != null)
                {
                    vaccinationSessionSituation.NumVaccinations++;
                    vaccinationSessionSituation.AddToIgnoreList(target);
                    vaccinationSessionSituation.BringRandomSimsToSession(0x1);

                    /*
                    HealthManager healthManager = target.SimDescription.HealthManager;
                    if (healthManager != null)
                    {
                        healthManager.Vaccinate();
                    }
                    */
                }
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }

        public static void BeforeDiagnose(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                FreeClinicSessionSituation freeClinicSessionSituation = FreeClinicSessionSituation.GetFreeClinicSessionSituation(actor);
                if (freeClinicSessionSituation != null)
                {
                    freeClinicSessionSituation.NumVaccinations++;
                    freeClinicSessionSituation.AddToIgnoreList(target);
                    freeClinicSessionSituation.BringRandomSimsToSession(0x1);

                    /*
                    HealthManager healthManager = target.SimDescription.HealthManager;
                    if (healthManager != null)
                    {
                        healthManager.Vaccinate();
                    }
                    */
                }
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }

        public static bool OnTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                //bool isAutonomous = Common.IsAutonomous(actor);

                VaccinationSessionSituation situation = VaccinationSessionSituation.GetVaccinationSessionSituation(actor);
                if (situation != null)
                {
                    if (situation.IsInIgnoreList(target))
                    {
                        return false;
                    }
                    else if (!situation.IsInSeekersList(target) && !situation.IsInInterruptedList(target))
                    {
                        return false;
                    }
                    else if (isAutonomous && (actor.GetDistanceToObject(target) > AutographSessionSituation.MaxDistanceForAutonomousSign))
                    {
                        return false;
                    }

                    return true;
                }
                else
                {
                    FreeClinicSessionSituation freeClinicSessionSituation = FreeClinicSessionSituation.GetFreeClinicSessionSituation(actor);
                    if (freeClinicSessionSituation != null)
                    {
                        if (freeClinicSessionSituation.IsInIgnoreList(target))
                        {
                            return false;
                        }
                        else if (!freeClinicSessionSituation.IsInSeekersList(target) && !freeClinicSessionSituation.IsInInterruptedList(target))
                        {
                            return false;
                        }
                        else if (isAutonomous && (actor.GetDistanceToObject(target) > AutographSessionSituation.MaxDistanceForAutonomousSign))
                        {
                            return false;
                        }
                    }
                    else if (isAutonomous)
                    {
                        return false;
                    }
                }

                Medical medical = actor.Occupation as Medical;
                return ((medical != null) && (medical.Level >= 3));
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static void OnAccept(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                VaccinationSessionSituation situation = VaccinationSessionSituation.GetVaccinationSessionSituation(actor);
                if (situation != null)
                {
                    situation.NumVaccinations++;
                    situation.AddToIgnoreList(target);
                    situation.BringRandomSimsToSession(0x1);
                }
                else
                {
                    FreeClinicSessionSituation freeClinicSessionSituation = FreeClinicSessionSituation.GetFreeClinicSessionSituation(actor);
                    if (freeClinicSessionSituation != null)
                    {
                        freeClinicSessionSituation.NumVaccinations++;
                        freeClinicSessionSituation.AddToIgnoreList(target);
                        freeClinicSessionSituation.BringRandomSimsToSession(0x1);
                    }
                }

                Medical medical = actor.Occupation as Medical;
                if ((medical != null) && (medical.Level >= 3))
                {
                    string vectors = null;

                    List<VectorBooter.Item> items = new List<VectorBooter.Item>();

                    int cost = 0;

                    foreach (VectorBooter.Data vector in VectorBooter.Vectors)
                    {
                        if (VectorControl.Inoculate(target.SimDescription, vector, true, true))
                        {
                            vectors += Common.NewLine + " " + vector.GetLocalizedName(target.IsFemale);

                            cost += (vector.InoculationCost / 25);
                        }
                    }

                    if (string.IsNullOrEmpty(vectors))
                    {
                        if (situation == null)
                        {
                            Common.Notify(target, Common.Localize("Inoculate:None", target.IsFemale, new object[] { target }));
                        }
                    }
                    else
                    {
                        string paid = null;
                        if (cost > target.FamilyFunds)
                        {
                            cost = target.FamilyFunds;
                        }

                        target.ModifyFunds(-cost);

                        if (target.Household != actor.Household)
                        {
                            actor.ModifyFunds(cost);

                            if (cost > 0)
                            {
                                paid += Common.NewLine + Common.NewLine + Common.Localize("Inoculate:Paid", target.IsFemale, new object[] { cost });
                            }
                        }
                        else
                        {
                            if (cost > 0)
                            {
                                paid += Common.NewLine + Common.NewLine + Common.Localize("Inoculate:Charged", target.IsFemale, new object[] { cost });
                            }
                        }

                        Common.Notify(target, Common.Localize("Inoculate:Success", target.IsFemale, new object[] { target }) + vectors + paid);
                    }
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }
    }
}
