using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NRaas.TaggerSpace.Helpers
{
    public class TagDataHelper
    {
        static Common.MethodStore sGetClanInfo = new Common.MethodStore("NRaasStoryProgression", "NRaas.StoryProgression", "FetchLocalizedPersonalityInfo", new Type[] { typeof(SimDescription) });
        static Common.MethodStore sGetDebtAndNetworth = new Common.MethodStore("NRaasStoryProgression", "NRaas.StoryProgression", "GetLocalizedDebtAndNetworth", new Type[] { typeof(SimDescription), typeof(int) });
        
        public enum TagRelationshipType
        {
            Acquaintance,
            Family,
            Coworker,
            Enemy,
            Friend,
            Romantic,
            Unknown
        }

        public enum TagOrientationType
        {
            Asexual,
            Bicurious,
            Bisexual,
            Gay,
            Straight,
            Undecided
        }

        public enum TagDataType
        {
            LifeStage,
            AgeInDays,
            DaysTillNextStage,
            Occult,
            PregnancyInfo,
            PartnerInfo,
            MotiveInfo,
            CurrentInteraction,
            Cash,
            NetWorth,
            PersonalityInfo,
            Debt,
            Mood,
            Job,
            Orientation,
            AgeInYears
        }

        public static Dictionary<ulong, int> moneyGraph = new Dictionary<ulong, int>();

        public static string GetRoleHours(SimDescription sim)
        {
            DateAndTime start = DateAndTime.Invalid;
            DateAndTime end = DateAndTime.Invalid;
            if (Roles.GetRoleHours(sim, ref start, ref end))
            {
                return Common.LocalizeEAString(false, "Gameplay/MapTags/MapTag:ProprietorHours", new object[] { start.ToString(), end.ToString() });
            }
            else
            {
                return null;
            }
        }

        public static TagOrientationType GetOrientation(SimDescription sim)
        {
            if ((sim.mGenderPreferenceFemale == 0) && (sim.mGenderPreferenceMale == 0))
            {
                return TagOrientationType.Undecided;
            }
            else if (sim.mGenderPreferenceFemale > 0)
            {
                if (sim.mGenderPreferenceMale <= 0)
                {
                    if (sim.IsMale)
                    {
                        return TagOrientationType.Straight;
                    }
                    else
                    {
                        return TagOrientationType.Gay;
                    }
                }
                else if (sim.mGenderPreferenceFemale > sim.mGenderPreferenceMale)
                {
                    if (sim.IsMale)
                    {
                        return TagOrientationType.Bicurious;
                    }
                    else
                    {
                        return TagOrientationType.Bicurious;
                    }
                }
                else if (sim.mGenderPreferenceFemale == sim.mGenderPreferenceMale)
                {
                    if (sim.IsMale)
                    {
                        return TagOrientationType.Bisexual;
                    }
                    else
                    {
                        return TagOrientationType.Bisexual;
                    }
                }
                else
                {
                    if (sim.IsMale)
                    {
                        return TagOrientationType.Bicurious;
                    }
                    else
                    {
                        return TagOrientationType.Bicurious;
                    }
                }
            }
            else
            {
                if (sim.mGenderPreferenceMale <= 0)
                {
                    if (sim.IsMale)
                    {
                        return TagOrientationType.Asexual;
                    }
                    else
                    {
                        return TagOrientationType.Asexual;
                    }
                }
                else
                {
                    if (sim.IsMale)
                    {
                        return TagOrientationType.Gay;
                    }
                    else
                    {
                        return TagOrientationType.Straight;
                    }
                }
            }            
        }

        public static string GetStatus(SimDescription sim)
        {
            string str = sim.FullName;

            bool serviceOrRole = false;
            if (sim.AssignedRole != null)
            {
                serviceOrRole = true;
                ShoppingRegister register = sim.AssignedRole.RoleGivingObject as ShoppingRegister;
                if (register != null)
                {
                    str += ", " + register.RegisterRoleName(sim.IsFemale);
                }
                else
                {
                    string roleName;
                    if (Localization.GetLocalizedString(sim.AssignedRole.CareerTitleKey, out roleName))
                    {
                        str += ", " + roleName;
                    }
                }

                string hours = GetRoleHours(sim);
                if (!string.IsNullOrEmpty(hours))
                {
                    str += Common.NewLine + hours;
                }
            }
            else if (SimTypes.InServicePool(sim))
            {
                serviceOrRole = true; 

                string serviceName;
                if (Localization.GetLocalizedString("Ui/Caption/Services/Service:" + sim.CreatedByService.ServiceType.ToString(), out serviceName))
                {
                    str += ", " + serviceName;
                }
            }

            List<string> customTitles;
            if (Tagger.Settings.mCustomSimTitles.TryGetValue(sim.SimDescriptionId, out customTitles))
            {                
                int perline = serviceOrRole ? 1 : 2;
                serviceOrRole = true;
                foreach (string title in customTitles)
                {
                    if (perline == 0)
                    {
                        str += Common.NewLine;
                        perline = 3;
                    }
                    str += ", " + title;
                    perline--;
                }
            }

            if ((serviceOrRole) || (Tagger.Settings.mTagDataSettings[TagDataType.Orientation] && Tagger.Settings.mTagDataSettings[TagDataType.LifeStage] && (Tagger.Settings.mTagDataSettings[TagDataType.AgeInDays] || Tagger.Settings.mTagDataSettings[TagDataType.AgeInYears] || (Tagger.Settings.mTagDataSettings[TagDataType.DaysTillNextStage] && sim.AgingEnabled))))
            {
                str += Common.NewLine;
            }
            else
            {
                str += " - ";
            }

            if (sim.TeenOrAbove && !sim.IsPet && Tagger.Settings.mTagDataSettings[TagDataType.Orientation])
            {
                str += Common.Localize("SimType:" + TagDataHelper.GetOrientation(sim).ToString());
            }

            if (Tagger.Settings.mTagDataSettings[TagDataType.LifeStage])
            {
                if (Tagger.Settings.mTagDataSettings[TagDataType.Orientation])
                {
                    str += " ";
                }

                str += sim.AgeLocalizedText;
            }

            if (Tagger.Settings.mTagDataSettings[TagDataType.AgeInDays] || Tagger.Settings.mTagDataSettings[TagDataType.AgeInYears] || Tagger.Settings.mTagDataSettings[TagDataType.DaysTillNextStage])
            {
                str += " (";
            }

            if (Tagger.Settings.mTagDataSettings[TagDataType.AgeInDays] || Tagger.Settings.mTagDataSettings[TagDataType.AgeInYears])
            {
                str += Common.Localize("TagData:Age", sim.IsFemale);
            }

            if (Tagger.Settings.mTagDataSettings[TagDataType.AgeInDays] && !Tagger.Settings.mTagDataSettings[TagDataType.AgeInYears])
            {
                str += " " + Common.Localize("TagData:Days", sim.IsFemale, new object[] { Math.Round(Aging.GetCurrentAgeInDays(sim as IMiniSimDescription)) });
            }

            if (Tagger.Settings.mTagDataSettings[TagDataType.AgeInYears])
            {
                str += " " + Common.Localize("TagData:Years", sim.IsFemale, new object[] { Math.Round(Aging.GetCurrentAgeInDays(sim as IMiniSimDescription) / Tagger.Settings.TagDataAgeInYearsLength) });
            }

            if (sim.AgingEnabled)
            {
                if (Tagger.Settings.mTagDataSettings[TagDataType.DaysTillNextStage])
                {
                    str += ", " + Common.Localize("TagData:Birthday", sim.IsFemale, new object[] { (int)(AgingManager.Singleton.AgingYearsToSimDays(AgingManager.GetMaximumAgingStageLength(sim)) - AgingManager.Singleton.AgingYearsToSimDays(sim.AgingYearsSinceLastAgeTransition)) });
                }
            }

            if (Tagger.Settings.mTagDataSettings[TagDataType.AgeInDays] || Tagger.Settings.mTagDataSettings[TagDataType.AgeInYears] || Tagger.Settings.mTagDataSettings[TagDataType.DaysTillNextStage])
            {
                str += ")";
            }

            if (Tagger.Settings.mTagDataSettings[TagDataType.Occult] && sim.OccultManager != null)
            {
                List<OccultTypes> types = OccultTypeHelper.CreateList(sim, false);                

                if (types.Count > 0)
                {
                    str += Common.NewLine;

                    string occultString = "";
                    foreach (OccultTypes type in types)
                    {
                        occultString += ", " + OccultTypeHelper.GetLocalizedName(type);
                    }

                    str += Common.Localize("TagData:OccultTag", sim.IsFemale, new object[] { occultString.Remove(0, 2) });                
                }
            }            

            Sim createdSim = sim.CreatedSim;

            if (Tagger.Settings.Debugging)
            {
                if (createdSim != null)
                {
                    str += Common.NewLine + "Autonomy: ";
                    if (createdSim.Autonomy == null)
                    {
                        str += "None";
                    }
                    else
                    {
                        if (createdSim.Autonomy.AutonomyDisabled)
                        {
                            str += "Disabled";
                        }
                        else if (!AutonomyRestrictions.IsAnyAutonomyEnabled(createdSim))
                        {
                            str += "User Disabled";
                        }
                        else if (createdSim.Autonomy.IsRunningHighLODSimulation)
                        {
                            str += "High";
                        }
                        else
                        {
                            str += "Low";
                        }

                        if (createdSim.Autonomy.ShouldRunLocalAutonomy)
                        {
                            str += " Local";
                        }

                        if (createdSim.CanRunAutonomyImmediately())
                        {
                            str += " Ready";
                        }
                        else if (!createdSim.mLastInteractionWasAutonomous)
                        {
                            str += " Push";
                        }
                        else if (!createdSim.mLastInteractionSucceeded)
                        {
                            str += " Fail";
                        }

                        if (createdSim.Autonomy.InAutonomyManagerQueue)
                        {
                            str += " Queued";
                        }
                    }
                }
            }

            if (createdSim != null)
            {
                if (Tagger.Settings.mTagDataSettings[TagDataType.Mood])
                {
                    str += Common.NewLine;
                    int flavour = (int)createdSim.MoodManager.MoodFlavor;

                    str += Common.Localize("TagData:MoodTag", sim.IsFemale, new object[] { Common.LocalizeEAString(false, "Ui/Tooltip/HUD/SimDisplay:MoodFlavor" + flavour.ToString()) }) + " ";
                }

                if (Tagger.Settings.mTagDataSettings[TagDataType.MotiveInfo])
                {
                    if (!Tagger.Settings.mTagDataSettings[TagDataType.Mood])
                    {
                        str += Common.NewLine;
                    }

                    string motives = ", ";
                    int num = 0;
                    foreach (CommodityKind kind in (Sims3.UI.Responder.Instance.HudModel as HudModel).GetMotives(sim.CreatedSim))
                    {
                        if (sim.CreatedSim.Motives.HasMotive(kind))
                        {
                            if (num >= 6)
                            {
                                break;
                            }

                            motives += FetchMotiveLocalization(sim.Species, kind) + ": " + "(" + sim.CreatedSim.Motives.GetValue(kind).ToString("0.") + ") ";
                        }

                        num++;
                    }

                    if (sim.CreatedSim.Motives.HasMotive(CommodityKind.Temperature))
                    {
                        motives += FetchMotiveLocalization(sim.Species, CommodityKind.Temperature) + ": " + "(" + sim.CreatedSim.Motives.GetValue(CommodityKind.Temperature).ToString("0.") + ") ";
                    }

                    str += Common.Localize("TagData:Motives", sim.IsFemale, new object[] { motives.Remove(0, 2) });
                }

                if (Tagger.Settings.mTagDataSettings[TagDataType.CurrentInteraction] && createdSim.CurrentInteraction != null)
                {
                    str += Common.NewLine;

                    try
                    {
                        str += createdSim.CurrentInteraction.ToString();
                    }
                    catch (Exception e)
                    {
                        Common.DebugException(createdSim, e);

                        str += createdSim.CurrentInteraction.GetType();
                    }

                    Tone tone = createdSim.CurrentInteraction.CurrentTone;
                    if (tone != null)
                    {
                        str += Common.NewLine + tone.ToString();
                    }

                    SocialInteractionBase social = createdSim.CurrentInteraction as SocialInteractionBase;
                    if ((social != null) && (social.Target != null))
                    {
                        str += " " + Common.Localize("TagData:With", sim.IsFemale, new object[] { social.Target.Name });
                    }

                    if (createdSim.CurrentInteraction is Terrain.GoHereWith)
                    {
                        InviteToLotSituation situtation = InviteToLotSituation.FindInviteToLotSituationInvolving(createdSim);
                        if (situtation != null)
                        {
                            if (situtation.SimA != createdSim)
                            {
                                str += " " + situtation.SimA.Name;
                            }
                            else if (situtation.SimB != createdSim)
                            {
                                str += situtation.SimB.Name;
                            }
                        }
                    }

                }
            }

            if (!SimTypes.IsSpecial(sim))
            {
                str += Common.NewLine + Common.Localize("TagData:CashTag", sim.IsFemale, new object[] { sim.FamilyFunds });

                if ((Tagger.Settings.mTagDataSettings[TagDataType.Debt] || Tagger.Settings.mTagDataSettings[TagDataType.NetWorth]) && sGetDebtAndNetworth.Valid)
                {
                    int bit = 0;
                    if (Tagger.Settings.mTagDataSettings[TagDataType.Debt])
                    {
                        bit = bit + 1;
                    }
                    if (Tagger.Settings.mTagDataSettings[TagDataType.NetWorth])
                    {
                        bit = bit + 4;
                    }

                    str += sGetDebtAndNetworth.Invoke<string>(new object[] { sim, bit });
                }

                if (Tagger.Settings.mTagDataSettings[TagDataType.Job])
                {
                    if (sim.Occupation != null)
                    {
                        str += Common.NewLine;
                        if (sim.Occupation.OfficeLocation != null)
                        {
                            str += Common.Localize("TagData:JobAt", sim.IsFemale, new object[] { sim.Occupation.CurLevelJobTitle, sim.Occupation.OfficeLocation.GetLocalizedName() });
                        }
                        else
                        {
                            str += Common.Localize("TagData:JobTag", sim.IsFemale, new object[] { sim.Occupation.CurLevelJobTitle });
                        }
                    }
                }

                if (Tagger.Settings.mTagDataSettings[TagDataType.PartnerInfo])
                {
                    if (sim.Partner != null)
                    {
                        Relationship rel = sim.GetRelationship(sim.Partner, false);
                        string status = "Happily";
                        if (rel != null)
                        {                            
                            if (rel.CurrentLTRLiking < -15)
                            {
                                status = "Unhappily";
                            }                            
                        }

                        str += Common.NewLine;

                        if (sim.IsMarried)
                        {
                            str += Common.Localize("TagData:Spouse", sim.IsFemale, new object[] { Common.Localize("TagData:" + status), sim.Partner });
                        }
                        else
                        {
                            str += Common.Localize("TagData:Partner", sim.IsFemale, new object[] { sim.Partner });
                        }
                    }
                }
            }

            if (sim.IsPregnant && Tagger.Settings.mTagDataSettings[TagDataType.PregnancyInfo])
            {
                IMiniSimDescription father = SimDescription.Find(sim.Pregnancy.DadDescriptionId);
                if (father == null)
                {
                    father = MiniSimDescription.Find(sim.Pregnancy.DadDescriptionId);
                }

                str += Common.NewLine;

                if (father != null)
                {
                    if (sim.Partner != null && father != sim.Partner && !sim.IsPet)
                    {
                        string uhoh = Common.Localize("TagData:Uhoh");
                        str += Common.Localize("TagData:Pregnancy", sim.IsFemale, new object[] { father, uhoh });
                    }
                    else
                    {
                        str += Common.Localize("TagData:Pregnancy", sim.IsFemale, new object[] { father });
                    }
                }
                else
                {
                    str += Common.Localize("TagData:PregnancyUnknown", sim.IsFemale);
                }
            }

            if (Tagger.Settings.mTagDataSettings[TagDataType.PersonalityInfo] && sGetClanInfo.Valid)
            {
                List<string> info = sGetClanInfo.Invoke<List<string>>(new object [] { sim });
                foreach (string personality in info)
                {
                    str += Common.NewLine + personality;
                }
            }

            return str;
        }

        public static string FetchMotiveLocalization(CASAgeGenderFlags flag, CommodityKind motive)
        {
            string str = "";
            switch (motive)
            {
                case CommodityKind.Hygiene:
                    if (flag == CASAgeGenderFlags.Cat) str = "Scratch";
                    if (flag == CASAgeGenderFlags.Dog) str = "Destruction";
                    if (flag == CASAgeGenderFlags.Horse) str = "Exercise";
                break;
                case CommodityKind.Fun:
                    if (flag == CASAgeGenderFlags.Horse) str = "Thirst";
                break;
                case CommodityKind.Temperature:
                    return Common.Localize("TagData:Temperature");                
                default:
                break;
            }

            return Common.LocalizeEAString("Ui/Caption/HUD/MotivesPanel:Motive" + str + (flag == CASAgeGenderFlags.Human ? motive.ToString() : flag.ToString()));
        }

        public static void GenerateMoneyGraphData()
        {
            moneyGraph.Clear();

            int max = 0;
            int min = 0;
            Dictionary<ulong, int> cashInfo = new Dictionary<ulong, int>();

            foreach (KeyValuePair<ulong, SimDescription> val in SimListing.GetResidents(false))
            {                
                if (!SimTypes.IsSpecial(val.Value) && val.Value.Household != null)
                {
                    Common.Notify("GenerateMoneyGraph: Working on " + val.Value.FullName + "(" + val.Value.SimDescriptionId + ")");                   
                    int debtnum = 0;
                    if (sGetDebtAndNetworth.Valid)
                    {
                        Common.Notify("Pulling debt");
                        string debt = string.Empty;
                        debt = sGetDebtAndNetworth.Invoke<string>(new object[] { val.Value, 1 });

                        Common.Notify("Debt returned: " + debt);

                        if (debt != string.Empty)
                        {
                            Match match = Regex.Match(debt, @"(\d+)");
                            if (match.Success)
                            {
                                debtnum = int.Parse(match.Groups[1].Value);
                                Common.Notify("Found debt: " + debtnum);
                            }
                        }
                    }

                    int cash = val.Value.Household.FamilyFunds - debtnum - val.Value.Household.UnpaidBills;
                    Common.Notify("Cash: " + cash + "FF: " + val.Value.Household.FamilyFunds + " UB: " + val.Value.Household.UnpaidBills);
                    cashInfo.Add(val.Value.SimDescriptionId, cash);

                    if (cash > max)
                    {                        
                        max = cash;
                    }
                    
                    if (cash < min || min == 0)
                    {                        
                        min = cash;
                    }
                }
            }

            foreach (KeyValuePair<ulong, int> vals in cashInfo)
            {
                int townWealthPercent = (vals.Value - min);
                float twp = townWealthPercent / (max - min) * 100;
                townWealthPercent = (int)Math.Floor(twp);

                Common.Notify("TWP (" + vals.Key + "): " + townWealthPercent + " Value: " + vals.Value + " min: " + min + " max: " + max);
                moneyGraph.Add(vals.Key, townWealthPercent);
            }
        }

        public static Color ColorizePercent(int percent)
        {
            // this returns R/G/B for a percent. Only supports Red(0) Green(100) or Yellow(50) or in between such
            /*
             * n is in range 0 .. 100
               R = (255 * n) / 100
               G = (255 * (100 - n)) / 100 
               B = 0
             */

            double r = 255 * Math.Sqrt(Math.Sin(percent * Math.PI / 200));
            double g = 255 * Math.Sqrt(Math.Cos(percent * Math.PI / 200));
            int b = 0;
            
            return new Color((int)r, (int)g, b);
        }

        public static Color ColorizePercentCustom(Color color1, Color color2, int percent)
        {
            // this returns R/G/B for a percent with custom colors.

            int color1Ratio = (255 * percent) / 100;
            int color2Ratio = 255 - color1Ratio;

            int r = ((color1.Red * color1Ratio + color2.Red * color2Ratio) / 255);
            int g = ((color1.Green * color1Ratio + color2.Green * color2Ratio) / 255);
            int b = ((color1.Blue * color1Ratio + color2.Blue * color2Ratio) / 255);
 
            return new Color(r, g, b);
        }
    }
}
