using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Careers;
using NRaas.CareerSpace.Interfaces;
using NRaas.CommonSpace.Helpers;
using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Interactions
{
    public class RegisterAsSelfEmployed : CityHall.RegisterAsSelfEmployed, Common.IPreLoad, Common.IAddInteraction
    {
        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<CityHall, CityHall.RegisterAsSelfEmployed.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
                tuning.Availability.RemoveFlags(Availability.FlagField.DisallowedIfPregnant);
                tuning.CodeVersion = new ProductVersion[] { ProductVersion.BaseGame };
            }

            CityHall.RegisterAsSelfEmployed.Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.ReplaceNoTest<CityHall, CityHall.RegisterAsSelfEmployed.Definition>(Singleton);
        }

        public override void ConfigureInteraction()
        {
            CareerToSet = (InteractionDefinition as Definition).OccupationEntry.Occupation;
            mDisplayGotoCityHallTNS = (CareerToSet == OccupationNames.Undefined);

            //base.ConfigureInteraction();
        }

        public override bool InRabbitHole()
        {
            try
            {
                OpportunityDialog.MaptagObjectInfo info;
                if (!UIUtils.IsOkayToStartModalDialog() || !Sims3.Gameplay.ActiveCareer.ActiveCareer.CanAddActiveCareer(Actor.SimDescription, CareerToSet))
                {
                    return false;
                }

                SkillBasedCareerStaticData occupationStaticData = Occupation.GetOccupationStaticData(CareerToSet) as SkillBasedCareerStaticData;
                Occupation staticOccupation = CareerManager.GetStaticOccupation(CareerToSet);
                string localizedCareerName = Occupation.GetLocalizedCareerName(CareerToSet, Actor.SimDescription);
                string description = Common.LocalizeEAString(Actor.IsFemale, occupationStaticData.CareerDescriptionLocalizationKey, new object[0x0]);
                string reward = string.Empty;
                foreach (string str4 in staticOccupation.ResponsibilitiesLocalizationKeys)
                {
                    reward = reward + Common.LocalizeEAString(Actor.IsFemale, str4, new object[0x0]) + Common.NewLine;
                }
                info.mLotId = 0x0L;
                info.mMapTag = null;
                info.mObjectGuid = ObjectGuid.InvalidObjectGuid;
                info.mHouseholdLotId = ulong.MaxValue;
                bool flag = OpportunityDialog.Show(ThumbnailKey.kInvalidThumbnailKey, Actor.ObjectId, ObjectGuid.InvalidObjectGuid, Actor.Name, OpportunityDialog.OpportunityType.SkillBasedCareer, localizedCareerName, description, string.Empty, reward, info, true, OpportunityDialog.DescriptionBackgroundType.NotSet, Actor.IsFemale, false);
                if (flag)
                {
                    AcquireOccupationParameters occupationParameters = new AcquireOccupationParameters(CareerToSet, true, false);

                    OmniCareer tempCareer = new OmniCareer();
                    tempCareer.mCareerGuid = staticOccupation.Guid;

                    occupationParameters.Location = new CareerLocation();
                    occupationParameters.Location.Career = tempCareer;

                    return Actor.AcquireOccupation(occupationParameters);
                }
                return flag;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        public new sealed class Definition : InteractionDefinition<Sim, CityHall, RegisterAsSelfEmployed>
        {
            public SkillBasedCareer.ValidSkillBasedCareerEntry OccupationEntry;

            public static List<SkillBasedCareer.ValidSkillBasedCareerEntry> GetSkillBasedCareerList(Sim sim)
            {
                List<SkillBasedCareer.ValidSkillBasedCareerEntry> list = new List<SkillBasedCareer.ValidSkillBasedCareerEntry>();
                if (sim.SkillManager != null)
                {
                    foreach (Occupation career in CareerManager.sDictionary.Values)
                    {
                        GreyedOutTooltipCallback greyedOutTooltipCallback = null;
                        if (!career.CanAcceptCareer(sim.ObjectId, ref greyedOutTooltipCallback)) continue;

                        SkillBasedCareerStaticData occupationStaticData = Occupation.GetOccupationStaticData(career.Guid) as SkillBasedCareerStaticData;
                        if (occupationStaticData != null)
                        {
                            int skillLevel = sim.SkillManager.GetSkillLevel(occupationStaticData.CorrespondingSkillName);
                            SkillBasedCareer occupationAsSkillBasedCareer = sim.OccupationAsSkillBasedCareer;
                            if ((occupationAsSkillBasedCareer == null) || (occupationAsSkillBasedCareer.Guid != career.Guid))
                            {
                                SkillBasedCareer.ValidSkillBasedCareerEntry item = new SkillBasedCareer.ValidSkillBasedCareerEntry();
                                item.Occupation = career.Guid;
                                item.SkillLevelMet = skillLevel >= occupationStaticData.MinimumSkillLevel;
                                item.MinimumSkillLevel = occupationStaticData.MinimumSkillLevel;
                                list.Add(item);
                            }
                        }
                    }
                }
                return list;
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, CityHall target, List<InteractionObjectPair> results)
            {
                foreach (SkillBasedCareer.ValidSkillBasedCareerEntry entry in GetSkillBasedCareerList(actor))
                {
                    Definition interaction = new Definition();
                    interaction.OccupationEntry.Occupation = entry.Occupation;
                    interaction.OccupationEntry.MinimumSkillLevel = entry.MinimumSkillLevel;
                    interaction.OccupationEntry.SkillLevelMet = entry.SkillLevelMet;
                    results.Add(new InteractionObjectPair(interaction, target));
                }
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                OccupationEntry.SkillLevelMet = true;
                OccupationEntry.MinimumSkillLevel = -1;
                return base.CreateInstance(ref parameters);
            }

            public override string GetInteractionName(Sim actor, CityHall target, InteractionObjectPair iop)
            {
                return Occupation.GetLocalizedCareerName(OccupationEntry.Occupation, actor.SimDescription);
            }

            public override string[] GetPath(bool isFemale)
            {
                return new string[] { Common.LocalizeEAString(isFemale, "Gameplay/Objects/RabbitHoles/CityHall:RegisterAsSelfEmployedPathName", new object[0x0]) };
            }

            public override bool Test(Sim a, CityHall target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (OccupationEntry.SkillLevelMet)
                {
                    return true;
                }

                greyedOutTooltipCallback = delegate {
                    return Common.LocalizeEAString(a.IsFemale, "Gameplay/Objects/RabbitHoles/CityHall:RegisterAsSelfEmployedMinimumSkillNotMet", new object[] { a, this.OccupationEntry.MinimumSkillLevel });
                };
                return false;
            }
        }
    }
}
