using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Status
{
    public class PersonalStatus : SimFromList, IStatusOption
    {
        public override string Name
        {
	        get 
	        { 
                return Common.Localize("PersonalStatus:MenuName");
	        }
        }

        protected override bool TestValid
        {
            get { return false; }
        }

        protected override OptionResult RunResult
        {
            get { return OptionResult.SuccessRetain; }
        }

        public override string GetTitlePrefix()
        {
            return "Status";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override bool PrivateAllow(MiniSimDescription me)
        {
            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            return Perform(me);
        }
        protected override bool Run(MiniSimDescription me, bool singleSelection)
        {
            return Perform(me);
        }

        public static OptionResult Export(List<IMiniSimDescription> sims, IStatusOption status)
        {
            StringBuilder builder = new StringBuilder();

            foreach (IMiniSimDescription sim in sims)
            {
                builder.Append(Common.NewLine + Common.NewLine + status.GetDetails(sim));
            }

            Common.WriteLog(builder.ToString(), false);

            Common.Notify(Common.Localize("Status:Exported", false, new object[] { sims.Count }));

            return OptionResult.SuccessRetain;
        }

        public static string GetHeader(IMiniSimDescription me)
        {
            SimDescription simDesc = me as SimDescription;
            MiniSimDescription miniDesc = me as MiniSimDescription;

            float agingYearsSinceLastAgeTransition = 0;
            if (simDesc != null)
            {
                agingYearsSinceLastAgeTransition = simDesc.AgingYearsSinceLastAgeTransition;
            }
            else if (miniDesc != null)
            {
                agingYearsSinceLastAgeTransition = miniDesc.AgingYearsSinceLastAgeTransition;
            }

            int trueAge = (int)Aging.GetCurrentAgeInDays(me);
            int daysToTransition = (int)(AgingManager.Singleton.AgingYearsToSimDays(AgingManager.GetMaximumAgingStageLength(me)) - AgingManager.Singleton.AgingYearsToSimDays(agingYearsSinceLastAgeTransition));

            string simName = me.FullName.Trim();

            if (string.IsNullOrEmpty(simName))
            {
                Genealogy genealogy = me.CASGenealogy as Genealogy;
                if (genealogy != null)
                {
                    simName = genealogy.Name + Common.NewLine + me.SimDescriptionId;
                }
            }

            string header = Common.Localize("Status:NameAge", me.IsFemale, new object[] { simName, SelectionCriteria.Age.Item.GetName(me.Age), trueAge, daysToTransition });

            if (me.IsFemale)
            {
                header += Common.NewLine + Common.Localize("Criteria.GenderFemale:MenuName");
            }
            else
            {
                header += Common.NewLine + Common.Localize("Criteria.GenderMale:MenuName");
            }

            if (me.IsAlien)
            {
                header += " " + Common.Localize("Species:Alien");
            }
            else
            {
                header += " " + Common.Localize("Species:" + me.Species);
            }
            return header;
        }

        public string GetDetails(IMiniSimDescription me)
        {
            Common.StringBuilder msg = new Common.StringBuilder();

            try
            {
                msg += GetHeader(me);

                SimDescription simDesc = me as SimDescription;
                MiniSimDescription miniDesc = me as MiniSimDescription;

                if (simDesc != null)
                {
                    if (!simDesc.AgingEnabled)
                    {
                        msg += Common.Localize("Status:AgingDisabled", me.IsFemale);
                    }

                    msg += Common.NewLine + Common.LocalizeEAString("Ui/Caption/HUD/KnownInfoDialog:" + simDesc.Zodiac.ToString());
                }
                else if (miniDesc != null)
                {
                    if (!miniDesc.mbAgingEnabled)
                    {
                        msg += Common.Localize("Status:AgingDisabled", me.IsFemale);
                    }
                }

                SimDescription.DeathType deathType = SimDescription.DeathType.None;
                if (simDesc != null)
                {
                    deathType = simDesc.DeathStyle;
                }
                else if (miniDesc != null)
                {
                    deathType = miniDesc.mDeathStyle;
                }

                if (deathType != SimDescription.DeathType.None)
                {
                    msg += Common.Localize("Status:Death", me.IsFemale) + Urnstones.GetLocalizedString(me.IsFemale, deathType);
                }

                List<OccultTypes> occultTypes = new List<OccultTypes>();

                OccultTypes primaryOccult = OccultTypes.None;

                if (simDesc != null)
                {
                    if (simDesc.OccultManager != null)
                    {
                        foreach (OccultTypes type in Enum.GetValues(typeof(OccultTypes)))
                        {
                            if (type == OccultTypes.None) continue;

                            if (simDesc.OccultManager.HasOccultType(type))
                            {
                                occultTypes.Add(type);
                            }
                        }
                    }

                    if (simDesc.SupernaturalData != null)
                    {
                        primaryOccult = simDesc.SupernaturalData.OccultType;
                    }
                }
                else if (miniDesc != null)
                {
                    if (miniDesc.IsVampire)
                    {
                        occultTypes.Add(OccultTypes.Vampire);
                    }
                    if (miniDesc.IsFrankenstein)
                    {
                        occultTypes.Add(OccultTypes.Frankenstein);
                    }
                    if (miniDesc.IsMummy)
                    {
                        occultTypes.Add(OccultTypes.Mummy);
                    }
                    if (miniDesc.IsUnicorn)
                    {
                        occultTypes.Add(OccultTypes.Unicorn);
                    }
                    if (miniDesc.IsGenie)
                    {
                        occultTypes.Add(OccultTypes.Unicorn);
                    }
                    if (miniDesc.IsWerewolf)
                    {
                        occultTypes.Add(OccultTypes.Werewolf);
                    }
                    if (miniDesc.IsWitch)
                    {
                        occultTypes.Add(OccultTypes.Witch);
                    }
                    if (miniDesc.IsFairy)
                    {
                        occultTypes.Add(OccultTypes.Fairy);
                    }
                }

                foreach (OccultTypes type in occultTypes)
                {
                    string isPrimary = null;
                    if (primaryOccult == type)
                    {
                        isPrimary = " (+)";
                    }

                    msg += Common.Localize("Status:Occult", me.IsFemale, new object[] { OccultTypeHelper.GetLocalizedName(type) + isPrimary });
                }

                if (simDesc != null)
                {
                    if (simDesc.LotHome != null)
                    {
                        msg += Common.Localize("Status:TypeResidentV2", me.IsFemale);
                    }
                    else if (simDesc.Household == null)
                    {
                        msg += Common.Localize("Status:TypeOutOfTowner", me.IsFemale);
                    }
                    else if (simDesc.AssignedRole != null)
                    {
                        msg += Common.Localize("Status:TypeService", me.IsFemale, new object[] { Roles.GetLocalizedName(simDesc.AssignedRole) });
                    }
                    else if (simDesc.Household.IsServiceNpcHousehold)
                    {
                        if (SimTypes.InServicePool(simDesc))
                        {
                            msg += Common.Localize("Status:TypeService", me.IsFemale, new object[] { Common.LocalizeEAString("Ui/Caption/Services/Service:" + simDesc.CreatedByService.ServiceType.ToString()) });
                        }
                        else
                        {
                            msg += Common.Localize("Status:TypeOutOfTowner", me.IsFemale);
                        }
                    }
                    else if (simDesc.Household.IsTouristHousehold)
                    {
                        msg += Common.Localize("Status:TypeTourist", me.IsFemale);
                    }
                    else if (simDesc.Household.IsTravelHousehold)
                    {
                        msg += Common.Localize("Status:TypeTravel", me.IsFemale);
                    }
                    else
                    {
                        msg += Common.Localize("Status:TypeHomeless", me.IsFemale);
                    }
                }
                else if (miniDesc != null)
                {
                    msg += Common.Localize("Status:TypeOutOfTowner", me.IsFemale);
                }

                string worldName = me.HomeWorld.ToString();
                if (!Enum.IsDefined(typeof(WorldName), me.HomeWorld))
                {
                    worldName = ((ulong)me.HomeWorld).ToString();
                }

                string homeWorld = Common.LocalizeEAString("Ui/Caption/Global/WorldName/EP01:" + worldName);// Sims3.UI.Responder.Instance.HudModel.LocationName(me.HomeWorld);
                if ((!string.IsNullOrEmpty(homeWorld)) && (homeWorld != "Ui/Caption/Global/WorldName/EP01:" + worldName))
                {
                    msg += Common.Localize("Status:HomeWorld", me.IsFemale, new object[] { homeWorld });
                }

                if (simDesc != null)
                {
                    msg += Common.Localize("Status:Favorites", me.IsFemale, new object[] { CASCharacter.GetFavoriteColor(simDesc.FavoriteColor), CASCharacter.GetFavoriteFood(simDesc.FavoriteFood), CASCharacter.GetFavoriteMusic(simDesc.FavoriteMusic) });

                    string LTWName = LifetimeWants.GetName(simDesc);
                    if (!string.IsNullOrEmpty(LTWName))
                    {
                        msg += Common.Localize("Status:LTW", me.IsFemale, new object[] { LTWName, Common.Localize("YesNo:" + simDesc.HasCompletedLifetimeWish.ToString()) });
                    }
                    else
                    {
                        msg += Common.Localize("Status:NoLTW", me.IsFemale);
                    }

                    msg += Common.Localize("Status:LifetimeReward", me.IsFemale, new object[] { simDesc.LifetimeHappiness, simDesc.SpendableHappiness });

                    if (simDesc.CreatedSim != null)
                    {
                        if (simDesc.LotHome == simDesc.CreatedSim.LotCurrent)
                        {
                            msg += Common.Localize("Status:LocationHome", me.IsFemale);
                        }
                        else if ((simDesc.CreatedSim.LotCurrent != null) && (!simDesc.CreatedSim.LotCurrent.IsWorldLot))
                        {
                            msg += Common.Localize("Status:LocationAt", me.IsFemale, new object[] { simDesc.CreatedSim.LotCurrent.Name });
                        }
                        else
                        {
                            msg += Common.Localize("Status:LocationTransit", me.IsFemale);
                        }

                        msg += Common.Localize("Status:Mood" + simDesc.CreatedSim.MoodManager.MoodFlavor.ToString(), me.IsFemale);

                        if (simDesc.CreatedSim.Autonomy != null)
                        {
                            if (simDesc.CreatedSim.Autonomy.AllowedToRunMetaAutonomy)
                            {
                                msg += Common.Localize("Status:Autonomous", me.IsFemale);
                            }

                            foreach(Situation situation in simDesc.CreatedSim.Autonomy.SituationComponent.Situations)
                            {
                                msg += Common.Localize("Status:Situation", me.IsFemale, new object[] { situation.ToString() });
                            }
                        }
                    }
                    else
                    {
                        msg += Common.Localize("Status:LocationOutOfTown", me.IsFemale);
                    }
                }

                string traits = null;
                int traitCount = 0;

                if (simDesc != null)
                {
                    if (simDesc.TraitManager != null)
                    {
                        foreach (Trait trait in simDesc.TraitManager.List)
                        {
                            if (trait.IsReward) continue;

                            traits += Common.NewLine + trait.TraitName(me.IsFemale);
                            traitCount++;
                        }
                    }
                }
                else if (miniDesc != null)
                {
                    if (miniDesc.Traits != null)
                    {
                        foreach (TraitNames traitName in miniDesc.Traits)
                        {
                            Trait trait = TraitManager.GetTraitFromDictionary(traitName);
                            if (trait == null) continue;

                            if (trait.IsReward) continue;

                            traits += Common.NewLine + trait.TraitName(me.IsFemale);
                            traitCount++;
                        }
                    }
                }

                if (traitCount > 10)
                {
                    msg += Common.Localize("Status:TraitsOverTen", me.IsFemale, new object[] { traitCount });
                }
                else if (traitCount > 0)
                {
                    msg += Common.Localize("Status:Traits", me.IsFemale, new object[] { traits });
                }
            }
            catch (Exception e)
            {
                Common.Exception(me.FullName, e);

                msg += Common.NewLine + "END OF LINE";
            }

            return msg.ToString();
        }

        protected bool Perform(IMiniSimDescription me)
        {
            SimpleMessageDialog.Show(Name, GetDetails(me));
            return true;
        }
    }
}
