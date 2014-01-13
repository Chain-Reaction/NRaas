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
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CommonSpace.Helpers
{
    public class HudModelEx
    {
        public static List<KnownInfo> GetKnownInfoAboutSelf(HudModel ths, IMiniSimDescription simDescription)
        {
            List<KnownInfo> infos = new List<KnownInfo>();
            SimDescription description = simDescription as SimDescription;
            if (description != null)
            {
                string firstName;
                bool isHuman = description.IsHuman;
                bool teen = (description.Age & CASAgeGenderFlags.Teen) != CASAgeGenderFlags.None;
                bool child = (description.Age & CASAgeGenderFlags.Child) != CASAgeGenderFlags.None;
                foreach (Trait trait in description.TraitManager.List)
                {
                    if (trait.IsVisible && Localization.HasLocalizationString(trait.TraitNameInfo))
                    {
                        infos.Add(new KnownInfo(trait.TraitName(description.IsFemale), trait.IconKey, KnownInfoType.Trait));
                    }
                }

                if (isHuman && !child)
                {
                    if (GameUtils.GetCurrentWorld() != description.HomeWorld)
                    {
                        MiniSimDescription description2 = MiniSimDescription.Find(description.SimDescriptionId);
                        if (description2 != null)
                        {
                            description2.GetKnownJobInfo(infos);
                        }
                    }
                    else if ((description.CareerManager != null) && (description.CareerManager.Occupation != null))
                    {
                        infos.Add(new KnownInfo(description.CareerManager.Occupation.CurLevelJobTitle, ResourceKey.CreatePNGKey(description.CareerManager.Occupation.CareerIcon, 0), KnownInfoType.Career));
                    }
                    else if (description.CreatedByService != null)
                    {
                        string str2 = Localization.LocalizeString(description.IsFemale, "Gameplay/Services/Title:" + description.CreatedByService.ServiceType.ToString(), new object[0]);
                        infos.Add(new KnownInfo(str2, ResourceKey.kInvalidResourceKey, KnownInfoType.Career));
                    }
                    else
                    {
                        infos.Add(new KnownInfo(Localization.LocalizeString(description.IsFemale, "Ui/Caption/HUD/KnownInfoDialog:Unemployed", new object[0]), ResourceKey.kInvalidResourceKey, KnownInfoType.Career));
                    }
                }

                ResourceKey kInvalidResourceKey = ResourceKey.kInvalidResourceKey;
                if (description.Partner != null)
                {
                    firstName = description.Partner.FirstName;
                    Relationship relationship = description.GetRelationship(description.Partner, false);

                    // Custom
                    if (relationship != null)
                    {
                        kInvalidResourceKey = RelationshipFunctions.GetLTRRelationshipImageKey(relationship.LTR.CurrentLTR, relationship.IsPetToPetRelationship);
                    }
                }
                else
                {
                    firstName = Localization.LocalizeString("Ui/Caption/HUD/KnownInfoDialog:None", new object[0]);
                }

                infos.Add(new KnownInfo(firstName, kInvalidResourceKey, KnownInfoType.Partner));
                if ((isHuman && (teen || child)) && ((description.CareerManager != null) && (description.CareerManager.School != null)))
                {
                    infos.Add(new KnownInfo(description.CareerManager.School.Name, ResourceKey.CreatePNGKey(description.CareerManager.School.CareerIcon, 0), KnownInfoType.School));
                }

                if (GameUtils.IsInstalled(ProductVersion.EP9))
                {
                    List<IDegreeEntry> completedDegreeEntries = description.CareerManager.DegreeManager.GetCompletedDegreeEntries();
                    if (completedDegreeEntries.Count > 0)
                    {
                        KnownInfo item = new KnownInfo("", ResourceKey.kInvalidResourceKey, KnownInfoType.Degree);
                        foreach (IDegreeEntry entry in completedDegreeEntries)
                        {
                            if (item.mDegreeIcons.Count == 5)
                            {
                                infos.Add(item);
                                item = new KnownInfo("", ResourceKey.kInvalidResourceKey, KnownInfoType.Degree);
                            }
                            item.mDegreeIcons.Add(ResourceKey.CreatePNGKey(entry.DegreeIcon, 0));
                        }
                        infos.Add(item);
                    }
                }

                if (description.IsRich)
                {
                    infos.Add(new KnownInfo(Localization.LocalizeString(description.IsFemale, "Ui/Caption/HUD/KnownInfoDialog:IsRich", new object[0]), ResourceKey.CreatePNGKey("w_simoleon_32", 0), KnownInfoType.Rich));
                }
                return infos;
            }

            MiniSimDescription description3 = simDescription as MiniSimDescription;
            if (description3 != null)
            {
                infos = description3.GetKnownInfoOfSelf();
            }
            return infos;
        }

        public static List<KnownInfo> GetKnownInfo(HudModel ths, IMiniSimDescription simDesc)
        {
            if ((ths.mSavedCurrentSim != null) && (ths.mSavedCurrentSim.SimDescription != null) && (simDesc != null))
            {
                if (ths.mSavedCurrentSim.SimDescription == simDesc)
                {
                    return GetKnownInfoAboutSelf(ths, simDesc);
                }

                if (simDesc is SimDescription)
                {
                    if (ths.mSavedCurrentSim.SocialComponent != null)
                    {
                        foreach (Relationship relationship in ths.mSavedCurrentSim.SocialComponent.Relationships)
                        {
                            if (relationship == null) continue;

                            SimDescription otherSimDescription = relationship.GetOtherSimDescription(ths.mSavedCurrentSim.SimDescription);
                            if (otherSimDescription != simDesc) continue;

                            InformationLearnedAboutSim learnedInfo = relationship.InformationAbout(otherSimDescription);
                            return GetKnownInfoList(ths, otherSimDescription, learnedInfo);
                        }
                    }
                }
                else
                {
                    MiniSimDescription tsd = MiniSimDescription.Find(ths.mSavedCurrentSim.SimDescription.SimDescriptionId);
                    if (tsd != null)
                    {
                        foreach (MiniRelationship relationship in tsd.MiniRelationships)
                        {
                            if (relationship == null) continue;

                            if (relationship.SimDescriptionId != simDesc.SimDescriptionId) continue;

                            MiniSimDescription otherTsd = MiniSimDescription.Find(relationship.SimDescriptionId);
                            return relationship.GetKnownInfo(tsd, otherTsd);
                        }
                    }
                }
            }

            return new List<KnownInfo>();
        }

        public static List<KnownInfo> GetKnownInfoList(HudModel ths, SimDescription otherSimDesc, InformationLearnedAboutSim learnedInfo)
        {
            List<KnownInfo> list = new List<KnownInfo>();

            Common.StringBuilder msg = new Common.StringBuilder("GetKnownInfoList" + Common.NewLine);

            try
            {
                if (otherSimDesc != null)
                {
                    bool isHuman = otherSimDesc.IsHuman;
                    bool isTeen = (otherSimDesc.Age & CASAgeGenderFlags.Teen) != CASAgeGenderFlags.None;
                    bool isChild = (otherSimDesc.Age & CASAgeGenderFlags.Child) != CASAgeGenderFlags.None;
                    string str = Localization.LocalizeString("Ui/Caption/HUD/RelationshipsPanel:UnknownTrait", new object[0x0]);

                    msg += "A";

                    foreach (Trait trait in otherSimDesc.TraitManager.List)
                    {
                        if (!trait.IsVisible || !Localization.HasLocalizationString(trait.TraitNameInfo))
                        {
                            continue;
                        }
                        bool flag4 = false;
                        foreach (TraitNames names in learnedInfo.Traits)
                        {
                            if (trait.Guid == names)
                            {
                                list.Add(new KnownInfo(trait.TraitName(otherSimDesc.IsFemale), trait.IconKey, KnownInfoType.Trait));
                                flag4 = true;
                                break;
                            }
                        }
                        if (!flag4)
                        {
                            list.Add(new KnownInfo(str, ResourceKey.CreatePNGKey("trait_unknown", 0x0), KnownInfoType.TraitUnknown));
                        }
                    }

                    msg += "B";

                    if ((isHuman && learnedInfo.CareerKnown) && !isChild)
                    {
                        bool flag5 = false;
                        WorldName currentWorld = GameUtils.GetCurrentWorld();
                        if ((otherSimDesc.HomeWorld != currentWorld) && (GameUtils.GetWorldType(currentWorld) == WorldType.Vacation))
                        {
                            MiniSimDescription description = MiniSimDescription.Find(otherSimDesc.SimDescriptionId);
                            if (description != null)
                            {
                                ResourceKey iconKey = string.IsNullOrEmpty(description.JobIcon) ? ResourceKey.kInvalidResourceKey : ResourceKey.CreatePNGKey(description.JobIcon, 0x0);
                                list.Add(new KnownInfo(Localization.LocalizeString(description.IsFemale, description.JobOrServiceName, new object[0x0]), iconKey, KnownInfoType.Career));
                                flag5 = true;
                            }
                        }

                        msg += "C";

                        if (!flag5)
                        {
                            CareerManager careerManager = otherSimDesc.CareerManager;
                            if ((careerManager != null) && (careerManager.Occupation != null))
                            {
                                msg += "C1";

                                Occupation occupation = careerManager.Occupation;
                                string careerIcon = occupation.CareerIcon;
                                ResourceKey key2 = string.IsNullOrEmpty(careerIcon) ? ResourceKey.kInvalidResourceKey : ResourceKey.CreatePNGKey(careerIcon, 0x0);
                                KnownInfoType type = occupation.IsAcademicCareer ? KnownInfoType.AcademicCareer : KnownInfoType.Career;
                                list.Add(new KnownInfo(occupation.CurLevelJobTitle, key2, type));
                            }
                            else if ((otherSimDesc.CreatedByService != null) && Sims3.Gameplay.Services.Services.IsSimDescriptionInAnyServicePool(otherSimDesc))
                            {
                                string str4 = Localization.LocalizeString(otherSimDesc.IsFemale, "Gameplay/Services/Title:" + otherSimDesc.CreatedByService.ServiceType.ToString(), new object[0x0]);
                                list.Add(new KnownInfo(str4, ResourceKey.kInvalidResourceKey, KnownInfoType.Career));
                            }
                            else if ((otherSimDesc.AssignedRole != null) && !string.IsNullOrEmpty(otherSimDesc.AssignedRole.CareerTitleKey))
                            {
                                list.Add(new KnownInfo(Localization.LocalizeString(otherSimDesc.IsFemale, otherSimDesc.AssignedRole.CareerTitleKey, new object[0x0]), ResourceKey.kInvalidResourceKey, KnownInfoType.Career));
                            }
                            else if (((careerManager != null) && (careerManager.Occupation == null)) && (careerManager.RetiredCareer != null))
                            {
                                list.Add(new KnownInfo(Localization.LocalizeString("Ui/Caption/HUD/Career:Retired", new object[0x0]), ResourceKey.CreatePNGKey(careerManager.RetiredCareer.CareerIcon, 0x0), KnownInfoType.Career));
                            }
                            else
                            {
                                list.Add(new KnownInfo(Localization.LocalizeString(otherSimDesc.IsFemale, "Ui/Caption/HUD/KnownInfoDialog:Unemployed", new object[0x0]), ResourceKey.kInvalidResourceKey, KnownInfoType.Career));
                            }
                        }
                    }

                    msg += "D";

                    if (learnedInfo.PartnerKnown)
                    {
                        string firstName = null;
                        ResourceKey relationshipImageKey = ResourceKey.kInvalidResourceKey;
                        if (otherSimDesc.Partner != null)
                        {
                            msg += "D1";

                            firstName = otherSimDesc.Partner.FirstName;

                            // Custom : false -> true
                            Relationship relationship = otherSimDesc.GetRelationship(otherSimDesc.Partner, true);
                            if (relationship != null)
                            {
                                relationshipImageKey = RelationshipFunctions.GetLTRRelationshipImageKey(relationship.LTR.CurrentLTR, relationship.IsPetToPetRelationship);
                            }
                        }
                        else
                        {
                            msg += "D2";

                            if (otherSimDesc.HomeWorld != GameUtils.GetCurrentWorld())
                            {
                                MiniSimDescription description2 = MiniSimDescription.Find(otherSimDesc.SimDescriptionId);
                                if ((description2 != null) && (description2.PartnerId != 0x0L))
                                {
                                    MiniSimDescription otherSim = MiniSimDescription.Find(description2.PartnerId);
                                    if (otherSim != null)
                                    {
                                        firstName = otherSim.FirstName;
                                        MiniRelationship miniRelationship = description2.GetMiniRelationship(otherSim) as MiniRelationship;
                                        if (miniRelationship != null)
                                        {
                                            relationshipImageKey = RelationshipFunctions.GetLTRRelationshipImageKey(miniRelationship.CurrentLTR, miniRelationship.IsPetToPetRelationship);
                                        }
                                    }
                                }
                            }

                            if (string.IsNullOrEmpty(firstName))
                            {
                                firstName = Localization.LocalizeString("Ui/Caption/HUD/KnownInfoDialog:None", new object[0x0]);
                            }
                        }
                        list.Add(new KnownInfo(firstName, relationshipImageKey, KnownInfoType.Partner));
                    }

                    msg += "E";

                    if ((isHuman && (isTeen || isChild)) && ((otherSimDesc.CareerManager != null) && (otherSimDesc.CareerManager.School != null)))
                    {
                        list.Add(new KnownInfo(otherSimDesc.CareerManager.School.Name, ResourceKey.CreatePNGKey(otherSimDesc.CareerManager.School.CareerIcon, 0x0), KnownInfoType.School));
                    }

                    if (learnedInfo.IsRichAndKnownToBeRich)
                    {
                        list.Add(new KnownInfo(Localization.LocalizeString(otherSimDesc.IsFemale, "Ui/Caption/HUD/KnownInfoDialog:IsRich", new object[0x0]), ResourceKey.CreatePNGKey("w_simoleon_32", 0x0), KnownInfoType.Rich));
                    }

                    if (isHuman && learnedInfo.SignKnown)
                    {
                        Zodiac zodiac = otherSimDesc.Zodiac;
                        list.Add(new KnownInfo(Localization.LocalizeString(otherSimDesc.IsFemale, "Ui/Caption/HUD/KnownInfoDialog:" + zodiac.ToString(), new object[0x0]), ResourceKey.CreatePNGKey("sign_" + zodiac.ToString() + "_sm", 0x0), KnownInfoType.Zodiac));
                    }

                    if (isHuman && learnedInfo.AlmaMaterKnown)
                    {
                        if (otherSimDesc.AlmaMater != AlmaMater.None)
                        {
                            list.Add(new KnownInfo(otherSimDesc.AlmaMaterName, ResourceKey.CreatePNGKey("w_simple_school_career_s", 0), KnownInfoType.AlmaMater));
                        }
                        if (((otherSimDesc.CareerManager != null) && (otherSimDesc.CareerManager.DegreeManager != null)) && (otherSimDesc.CareerManager.DegreeManager.GetDegreeEntries().Count > 0))
                        {
                            list.Add(new KnownInfo(Localization.LocalizeString("Ui/Caption/HUD/KnownInfoTooltip:UniversityAlmaMater", new object[0]), ResourceKey.CreatePNGKey("moodlet_just_graduated", ResourceUtils.ProductVersionToGroupId(ProductVersion.EP9)), KnownInfoType.AlmaMater));
                        }
                    }

                    msg += "G";

                    uint celebrityLevel = otherSimDesc.CelebrityLevel;
                    if (celebrityLevel > 0x0)
                    {
                        KnownInfo item = new KnownInfo(otherSimDesc.CelebrityManager.LevelName, ResourceKey.CreatePNGKey("hud_i_celebrity_page", 0x0), KnownInfoType.CelebrityLevel);
                        item.mCelebrityLevel = celebrityLevel;
                        list.Add(item);
                    }

                    msg += "H";

                    if (learnedInfo.IsSocialGroupsKnown)
                    {
                        TraitManager traitManager = otherSimDesc.TraitManager;
                        SkillManager skillManager = otherSimDesc.SkillManager;
                        if ((traitManager != null) && (skillManager != null))
                        {
                            if (traitManager.HasElement(TraitNames.InfluenceNerd))
                            {
                                InfluenceSkill skill = skillManager.GetSkill<InfluenceSkill>(SkillNames.InfluenceNerd);
                                if ((skill != null) && (skill.SkillLevel > 0))
                                {
                                    list.Add(new KnownInfo(string.Concat(new object[] { Localization.LocalizeString("Ui/Tooltips/SocialGroup:Nerd", new object[0]), "(", skill.SkillLevel, ")" }), ResourceKey.CreatePNGKey("trait_SocialGroup01_s", ResourceUtils.ProductVersionToGroupId(ProductVersion.EP9)), KnownInfoType.SocialGroup));
                                }
                            }
                            if (traitManager.HasElement(TraitNames.InfluenceSocialite))
                            {
                                InfluenceSkill skill2 = skillManager.GetSkill<InfluenceSkill>(SkillNames.InfluenceSocialite);
                                if ((skill2 != null) && (skill2.SkillLevel > 0))
                                {
                                    list.Add(new KnownInfo(string.Concat(new object[] { Localization.LocalizeString("Ui/Tooltips/SocialGroup:Jock", new object[0]), "(", skill2.SkillLevel, ")" }), ResourceKey.CreatePNGKey("trait_SocialGroup03_s", ResourceUtils.ProductVersionToGroupId(ProductVersion.EP9)), KnownInfoType.SocialGroup));
                                }
                            }
                            if (traitManager.HasElement(TraitNames.InfluenceRebel))
                            {
                                InfluenceSkill skill3 = skillManager.GetSkill<InfluenceSkill>(SkillNames.InfluenceRebel);
                                if ((skill3 != null) && (skill3.SkillLevel > 0))
                                {
                                    list.Add(new KnownInfo(string.Concat(new object[] { Localization.LocalizeString("Ui/Tooltips/SocialGroup:Rebel", new object[0]), "(", skill3.SkillLevel, ")" }), ResourceKey.CreatePNGKey("trait_SocialGroup02_s", ResourceUtils.ProductVersionToGroupId(ProductVersion.EP9)), KnownInfoType.SocialGroup));
                                }
                            }
                        }
                    }

                    msg += "I";

                    if (learnedInfo.NumDegreesKnown() > 0)
                    {
                        KnownInfo info2 = new KnownInfo("", ResourceKey.kInvalidResourceKey, KnownInfoType.Degree);
                        otherSimDesc.CareerManager.DegreeManager.GetCompletedDegreeEntries();
                        foreach (AcademicDegreeNames names2 in learnedInfo.Degrees)
                        {
                            AcademicDegree element = otherSimDesc.CareerManager.DegreeManager.GetElement(names2);
                            if ((element != null) && element.IsDegreeCompleted)
                            {
                                if (info2.mDegreeIcons.Count == 5)
                                {
                                    list.Add(info2);
                                    info2 = new KnownInfo("", ResourceKey.kInvalidResourceKey, KnownInfoType.Degree);
                                }
                                ResourceKey key4 = ResourceKey.CreatePNGKey(element.DegreeIcon, 0);
                                info2.mDegreeIcons.Add(key4);
                                info2.mIconKey = key4;
                                info2.mInfo = element.DegreeName;
                            }
                        }
                        list.Add(info2);
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(otherSimDesc, null, msg, e);
            }

            return list;
        }
    }
}
