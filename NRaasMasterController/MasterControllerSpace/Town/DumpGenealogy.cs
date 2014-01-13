using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.Sims;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Town
{
    public class DumpGenealogy : OptionItem, ITownOption
    {
        public override string GetTitlePrefix()
        {
            return "DumpGenealogy";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            return Perform(SimListing.AllSims<IMiniSimDescription>(null, true));
        }

        private static void AppendEvent(StringBuilder builder, string key, int data)
        {
            AppendEvent(builder, key, EAText.GetNumberString(data));
        }
        private static void AppendEvent(StringBuilder builder, string key, string data)
        {
            builder.Append(Common.NewLine + "1 EVEN");
            builder.Append(Common.NewLine + "2 TYPE " + Common.Localize(key + ":MenuName"));
            builder.Append(Common.NewLine + "2 NOTE " + data);
        }

        public static OptionResult Perform (Dictionary<ulong,List<IMiniSimDescription>> sims)
        {
            ProgressDialog.Show(Responder.Instance.LocalizationModel.LocalizeString("Ui/Caption/Global:Processing", new object[0x0]), false);

            try
            {
                StringBuilder total = new StringBuilder();

                total.Append("0 HEAD");
                total.Append(Common.NewLine + "1 GEDC");
                total.Append(Common.NewLine + "2 VERS 5.5");
                total.Append(Common.NewLine + "2 FORM LINEAGE-LINKED");
                total.Append(Common.NewLine + "1 CHAR ANSI");
                total.Append(Common.NewLine + "1 LANG English");
                total.Append(Common.NewLine + "1 SOUR SIMS3");
                total.Append(Common.NewLine + "2 NAME NRaas MasterController");

                Dictionary<IMiniSimDescription, string> ids = new Dictionary<IMiniSimDescription, string>();

                Dictionary<IMiniSimDescription, StringBuilder> dataLookup = new Dictionary<IMiniSimDescription, StringBuilder>();

                int count = 0;

                foreach (List<IMiniSimDescription> miniSims in sims.Values)
                {
                    foreach (IMiniSimDescription miniSim in miniSims)
                    {
                        SimDescription trueSim = miniSim as SimDescription;
                        if (trueSim != null)
                        {
                            if ((trueSim.Household != null) && (SimTypes.IsSpecial(trueSim)) && (!trueSim.IsDead))
                            {
                                continue;
                            }
                        }

                        count++;

                        string id = "@I" + count.ToString() + "@";

                        ids.Add(miniSim, id);

                        StringBuilder data = new StringBuilder();

                        data.Append(Common.NewLine + "0 " + id + " INDI");
                        data.Append(Common.NewLine + "1 NAME " + miniSim.FirstName + " /" + miniSim.LastName + "/");
                        data.Append(Common.NewLine + "2 GIVN " + miniSim.FirstName);
                        data.Append(Common.NewLine + "2 SURN " + miniSim.LastName);

                        if (miniSim.IsMale)
                        {
                            data.Append(Common.NewLine + "1 SEX M");
                        }
                        else
                        {
                            data.Append(Common.NewLine + "1 SEX F");
                        }

                        if (miniSim.IsDead)
                        {
                            data.Append(Common.NewLine + "1 DEAT");
                        }

                        AppendEvent(data, "Criteria.Age", (int)Aging.GetCurrentAgeInDays(miniSim));
                        AppendEvent(data, "Criteria.Celebrity", (int)miniSim.CelebrityLevel);

                        if (trueSim != null)
                        {
                            if (trueSim.LotHome != null)
                            {
                                AppendEvent(data, "Criteria.LotName", trueSim.LotHome.Name);
                                AppendEvent(data, "Criteria.LotAddress", trueSim.LotHome.Address);
                            }

                            AppendEvent(data, "AlmaMater", trueSim.AlmaMaterName);
                            AppendEvent(data, "Criteria.Zodiac", Common.LocalizeEAString("Ui/Caption/HUD/KnownInfoDialog:" + trueSim.Zodiac.ToString()));

                            if (trueSim.Occupation != null)
                            {
                                AppendEvent(data, "Criteria.Career", trueSim.Occupation.CareerName + " (" + trueSim.Occupation.CareerLevel + ")");
                            }

                            if (trueSim.CareerManager != null)
                            {
                                School school = trueSim.CareerManager.School;
                                if (school != null)
                                {
                                    AppendEvent(data, "Criteria.School", school.CareerName);
                                }

                                if (trueSim.CareerManager.QuitCareers != null)
                                {
                                    foreach (Occupation career in trueSim.CareerManager.QuitCareers.Values)
                                    {
                                        if (career == null) continue;

                                        if (career is School)
                                        {
                                            AppendEvent(data, "Criteria.PriorSchool", career.CareerName + " (" + career.CareerLevel + ")");
                                        }
                                        else
                                        {
                                            AppendEvent(data, "Criteria.PriorCareer", career.CareerName + " (" + career.CareerLevel + ")");
                                        }
                                    }
                                }
                            }

                            string LTWName = LifetimeWants.GetName(trueSim);
                            if (!string.IsNullOrEmpty(LTWName))
                            {
                                AppendEvent(data, "Criteria.LifetimeWish", LTWName);
                            }

                            AppendEvent(data, "Criteria.PreferenceColor", CASCharacter.GetFavoriteColor(trueSim.FavoriteColor));
                            AppendEvent(data, "Criteria.PreferenceFood", CASCharacter.GetFavoriteFood(trueSim.FavoriteFood));
                            AppendEvent(data, "Criteria.PreferenceMusic", CASCharacter.GetFavoriteMusic(trueSim.FavoriteMusic));

                            if (trueSim.TraitManager != null)
                            {
                                foreach (Trait trait in trueSim.TraitManager.List)
                                {
                                    if (trait.IsReward) continue;

                                    AppendEvent(data, "Criteria.Trait", trait.TraitName(trueSim.IsFemale));
                                }
                            }

                            if (trueSim.SkillManager != null)
                            {
                                foreach (Skill skill in trueSim.SkillManager.List)
                                {
                                    AppendEvent(data, "Criteria.Skill", skill.Name + " (" + skill.SkillLevel + ")");
                                }
                            }
                        }
                        else
                        {
                            MiniSimDescription miniDesc = miniSim as MiniSimDescription;
                            if (miniDesc != null)
                            {
                                AppendEvent(data, "AlmaMater", miniDesc.AlmaMaterName);
                                AppendEvent(data, "Criteria.Zodiac", Common.LocalizeEAString("Ui/Caption/HUD/KnownInfoDialog:" + miniDesc.Zodiac.ToString()));
                                AppendEvent(data, "Criteria.Career", miniDesc.JobOrServiceName);
                                AppendEvent(data, "Criteria.School", miniDesc.SchoolName);

                                if (miniDesc.Traits != null)
                                {
                                    foreach (TraitNames traitName in miniDesc.Traits)
                                    {
                                        Trait trait = TraitManager.GetTraitFromDictionary(traitName);
                                        if (trait == null) continue;

                                        if (trait.IsReward) continue;

                                        AppendEvent(data, "Criteria.Trait", trait.TraitName(miniDesc.IsFemale));
                                    }
                                }
                            }
                        }

                        dataLookup.Add(miniSim, data);
                    }
                }

                count = 0;

                StringBuilder familyOutput = new StringBuilder();

                Dictionary<string, bool> families = new Dictionary<string, bool>();

                foreach (List<IMiniSimDescription> miniSims in sims.Values)
                {
                    foreach (IMiniSimDescription miniSim in miniSims)
                    {
                        SimDescription trueSim = miniSim as SimDescription;
                        if (trueSim != null)
                        {
                            if ((trueSim.Household != null) && (SimTypes.IsSpecial(trueSim)) && (!trueSim.IsDead))
                            {
                                continue;
                            }
                        }

                        Genealogy genealogy = miniSim.CASGenealogy as Genealogy;
                        if (genealogy == null) continue;

                        IMiniSimDescription p1 = null, p2 = null;

                        if (genealogy.Parents.Count == 1)
                        {
                            p1 = genealogy.Parents[0].IMiniSimDescription;
                        }
                        else if (genealogy.Parents.Count == 2)
                        {
                            p1 = genealogy.Parents[0].IMiniSimDescription;
                            p2 = genealogy.Parents[1].IMiniSimDescription;
                        }
                        else
                        {
                            continue;
                        }

                        string father = null;
                        string mother = null;

                        if (p1 != null)
                        {
                            if (p1.IsMale)
                            {
                                if (ids.ContainsKey(p1))
                                {
                                    father = ids[p1];
                                }
                            }
                            else
                            {
                                if (ids.ContainsKey(p1))
                                {
                                    mother = ids[p1];
                                }
                            }
                        }

                        if (p2 != null)
                        {
                            if (father == null)
                            {
                                if (ids.ContainsKey(p2))
                                {
                                    father = ids[p2];
                                }
                            }
                            else
                            {
                                if (ids.ContainsKey(p2))
                                {
                                    mother = ids[p2];
                                }
                            }
                        }

                        if (p1 == null)
                        {
                            p1 = p2;
                            p2 = null;
                        }

                        if (families.ContainsKey(father + mother))
                        {
                            continue;
                        }
                        families.Add(father + mother, true);

                        count++;

                        string familyID = "@F" + count.ToString() + "@";

                        familyOutput.Append(Common.NewLine + "0 " + familyID + " FAM");

                        if (father != null)
                        {
                            familyOutput.Append(Common.NewLine + "1 HUSB " + father);
                        }

                        if (mother != null)
                        {
                            familyOutput.Append(Common.NewLine + "1 WIFE " + mother);
                        }

                        Genealogy p1Genealogy = p1.CASGenealogy as Genealogy;
                        if (p1Genealogy == null) continue;

                        Genealogy p2Genealogy = null;
                        if (p2 != null)
                        {
                            p2Genealogy = p2.CASGenealogy as Genealogy;
                            if (p2Genealogy == null) continue;
                        }

                        foreach (Genealogy child in p1Genealogy.Children)
                        {
                            if (child == null) continue;

                            if (child.SimDescription == null) continue;

                            if (p2 == null)
                            {
                                if (child.Parents.Count != 1) continue;
                            }
                            else
                            {
                                if (!child.Parents.Contains(p2Genealogy)) continue;
                            }

                            if (dataLookup.ContainsKey(child.SimDescription))
                            {
                                dataLookup[child.SimDescription].Append(Common.NewLine + "1 FAMC " + familyID);
                            }

                            if (ids.ContainsKey(child.SimDescription))
                            {
                                familyOutput.Append(Common.NewLine + "1 CHIL " + ids[child.SimDescription]);
                            }
                        }

                        if (p1 != null)
                        {
                            if (dataLookup.ContainsKey(p1))
                            {
                                dataLookup[p1].Append(Common.NewLine + "1 FAMS " + familyID);
                            }
                        }

                        if (p2 != null)
                        {
                            if (dataLookup.ContainsKey(p2))
                            {
                                dataLookup[p2].Append(Common.NewLine + "1 FAMS " + familyID);
                            }
                        }

                        if ((p1 != null) && (p2 != null) && (p1Genealogy.Spouse == p2Genealogy))
                        {
                            familyOutput.Append(Common.NewLine + "1 MARR");
                        }
                    }
                }

                foreach (StringBuilder data in dataLookup.Values)
                {
                    total.Append(data.ToString());
                }

                total.Append(familyOutput);

                total.Append(Common.NewLine + "0 TRLR");

                NRaas.MasterController.WriteLog(total.ToString(), false);
            }
            finally
            {
                ProgressDialog.Close();
            }

            SimpleMessageDialog.Show(Common.Localize("DumpGenealogy:MenuName"), Common.Localize("DumpGenealogy:Prompt"));
            return OptionResult.SuccessClose;
        }
    }
}
