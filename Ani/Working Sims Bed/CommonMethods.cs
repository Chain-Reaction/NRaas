using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.UI;
using Sims3.SimIFace;

namespace Sims3.Gameplay.Objects.Decorations.WorkingSimsBed
{
    public enum WooHooSkill
    {
        Athletic = 0,
        Charisma = 1
    }
    public class WooHooConfig
    {
        public int pay { get; set; }
        public WooHooSkill skill { get; set; }
        public int STD { get; set; }
        public int pregnancy { get; set; }
        public Boolean Moodlets { get; set; }       
        public Boolean TeenPregnancy { get; set; }

        public WooHooConfig()
        {

        }
    }
    public class CommonMethods
    {
        #region Post WooHoo

        public static void HandlePostWoohoo(Sim client, Sim solicitor, ObjectGuid bedId)
        {
            //Get paid once
            if (client == WooHooBedList.listWP[bedId].client)
            {
                WooHooConfig whc = GetWooHooConfig();

                GetPaid(client, solicitor, whc.pay, whc.skill);

                if (whc.Moodlets)
                {
                    SetMoodlets(client, solicitor);
                }

                Consequence(client, solicitor, whc.STD); 
            }
        }

        public static WooHooConfig GetWooHooConfig()
        {
            WooHooConfig whc = new WooHooConfig();

            XmlDbData xdb = XmlDbData.ReadData("WooHooForMoneyConfig");
            if ((xdb != null) && (xdb.Tables != null))
            {
                XmlDbTable table;

                if (xdb.Tables.TryGetValue("WooHooConfig", out table) && (table != null))
                {
                    foreach (XmlDbRow row in table.Rows)
                    {
                        whc.pay = row.GetInt("pay");
                        whc.skill = (WooHooSkill)row.GetInt("skill");
                        whc.pregnancy = row.GetInt("preg");
                        whc.TeenPregnancy = row.GetBool("tpreg");
                        whc.STD = row.GetInt("std");
                        whc.Moodlets = row.GetBool("moodlets");
                    }

                }
            }
            return whc;
        }

        public static void SetMoodlets(Sim client, Sim solicitor)
        {
            float oneDay = 1440;
            int lucky = 20;
            int embarrassed = -50;

            //Client moodlet, get a kick out of the action
            client.BuffManager.AddElement(BuffNames.Pumped, lucky, oneDay, Origin.FromLover);

            //Solisiter moodlets
            if (solicitor.TraitManager.HasElement(TraitNames.NeverNude) || solicitor.TraitManager.HasElement(TraitNames.Good))
            {
                //Get embarrassed
                if (solicitor.BuffManager.HasElement(BuffNames.Embarrassed))
                {
                    //If we are already embaressed, add to the current value 
                    if (solicitor.BuffManager.GetElement(BuffNames.Embarrassed).BuffOrigin == Origin.FromBeingNaked)
                    {
                        oneDay += solicitor.BuffManager.GetElement(BuffNames.Embarrassed).TimeoutCount;
                    }
                }
                solicitor.BuffManager.AddElement(BuffNames.Embarrassed, embarrassed, oneDay, Origin.FromBeingNaked);
            }

        }

        public static void GetPaid(Sim client, Sim solicitor, int pay, WooHooSkill skill)
        {
            if (solicitor != null && client != null)
            {
                int skillLevel = 0;

                switch (skill)
                {
                    case WooHooSkill.Athletic:
                        skillLevel = solicitor.SkillManager.GetSkillLevel(Sims3.Gameplay.Skills.SkillNames.Athletic);
                        break;
                    case WooHooSkill.Charisma:
                        skillLevel = solicitor.SkillManager.GetSkillLevel(Sims3.Gameplay.Skills.SkillNames.Charisma);
                        break;
                    default:
                        break;
                }

                if (skillLevel > 1)
                {
                    pay = (pay * skillLevel);
                }

                //if the soliciter is middle aged, half the wages
                if (solicitor.SimDescription.AdultOrAbove)
                {
                    pay /= 2;
                }

                //Do the payments if different households
                if (solicitor.Household != client.Household)
                {
                    //Pay if the client has money
                    if (client.FamilyFunds >= pay)
                    {
                        solicitor.Household.ModifyFamilyFunds(pay);

                        client.Household.ModifyFamilyFunds(-pay);
                        StyledNotification.Show(new StyledNotification.Format(LocalizeString("GotPayed", new object[] { solicitor.Name, pay }), StyledNotification.NotificationStyle.kGameMessagePositive));
                    }
                    else
                    {
                        //Start hateing the client
                        solicitor.SocialComponent.AddRelationshipUpdate(client, Sims3.Gameplay.Socializing.CommodityTypes.Insulting, -10f, -10f);
                        StyledNotification.Show(new StyledNotification.Format(LocalizeString("DidNotGetPayed", new object[] { client.Name, solicitor.Name, 20 }), StyledNotification.NotificationStyle.kGameMessageNegative));
                    }
                }
            }

        }

        public static void Consequence(Sim client, Sim solicitor, int stdPercent)
        {
            //Get an STD
            float twoDays = 1440 * 2;
            float clientSickTime = twoDays;
            float soliciterSickTime = twoDays;

            int sick = -50;

            //Give the STD moodlets

            Origin sickOrigin = Origin.FromLover;
            BuffNames sickBuffName = BuffNames.Itchy;

            if (FuckMeUp(stdPercent) || HasMoodlet(solicitor, sickBuffName, sickOrigin) || HasMoodlet(client, sickBuffName, sickOrigin))
            {
                //If we are already carrying an std, add to the current value 
                if (HasMoodlet(solicitor, sickBuffName, sickOrigin))
                {
                    soliciterSickTime += solicitor.BuffManager.GetElement(sickBuffName).TimeoutCount;
                }

                if (HasMoodlet(client, sickBuffName, sickOrigin))
                {
                    clientSickTime += client.BuffManager.GetElement(sickBuffName).TimeoutCount;
                }


                //Give std moodlet
                client.BuffManager.AddElement(sickBuffName, sick, clientSickTime, sickOrigin);
                solicitor.BuffManager.AddElement(sickBuffName, sick, soliciterSickTime, sickOrigin);

                //Notify
                StyledNotification.Show(new StyledNotification.Format(LocalizeString("STD", new object[] { client.Name }), StyledNotification.NotificationStyle.kGameMessageNegative));
                StyledNotification.Show(new StyledNotification.Format(LocalizeString("STD", new object[] { solicitor.Name }), StyledNotification.NotificationStyle.kGameMessageNegative));

            }



            //Get stress relieve
            if (HasMoodlet(client, BuffNames.Stressed, Origin.None))
            {
                client.BuffManager.RemoveElement(BuffNames.Stressed);
            }

            if (HasMoodlet(client, BuffNames.Overworked, Origin.None))
            {
                client.BuffManager.RemoveElement(BuffNames.Overworked);
            }

            //Increase fun and social
            float currentFun = client.Motives.GetValue(CommodityKind.Fun);
            float currentSocial = client.Motives.GetValue(CommodityKind.Social);

            int charisma = solicitor.SkillManager.GetSkillLevel(Sims3.Gameplay.Skills.SkillNames.Charisma);

            if (charisma <= 5)
            {
                client.Motives.SetValue(CommodityKind.Fun, currentFun + 20);
                client.Motives.SetValue(CommodityKind.Social, currentSocial + 20);
            }
            else if (charisma >= 6 && charisma <= 9)
            {
                client.Motives.SetValue(CommodityKind.Fun, currentFun + 40);
                client.Motives.SetValue(CommodityKind.Social, currentSocial + 40);
            }
            else if (charisma == 10)
            {
                client.Motives.ForceSetMax(CommodityKind.Fun);
                client.Motives.ForceSetMax(CommodityKind.Social);
            }
        }

        public static Boolean HasMoodlet(Sim sim, BuffNames b, Origin o)
        {
            if (sim.BuffManager.HasElement(b))
            {
                if (sim.BuffManager.GetElement(b).BuffOrigin == o)
                {
                    return true;

                }
                else
                {
                    //Just check do we have the moodlet
                    if (sim.BuffManager.GetElement(b).BuffOrigin == Origin.None)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static Boolean FuckMeUp(int percent)
        {

            int maxValue = 0x65;
            int num2 = new Random().Next(maxValue);
            return ((num2 > 0) && (num2 <= percent));

            //Boolean fuckmeup = false;

            //int max = 101;

            //Random r = new Random();

            //int randomNumber = r.Next(max);

            //if (randomNumber > 0 && randomNumber <= percent)
            //{
            //    fuckmeup = true;
            //}

            //return fuckmeup;
        }
        #endregion

        #region Localization
        public static string LocalizeString(string name, params object[] parameters)
        {
            return Localization.LocalizeString("TheWorkingSimsBed:" + name, parameters);
        }
        #endregion
    }
}
