using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.Dialogs;
using NRaas.MasterControllerSpace.SelectionCriteria;
using NRaas.MasterControllerSpace.Sims;
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
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Basic.Party
{
    public class ThrowHome : SimFromList, IPartyOption
    {
        static bool sSuppressCriteria;

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return (parameters.mActor == parameters.mTarget);
        }

        public override string Name
        {
            get { return Common.Localize("ThrowHome:MenuName"); }
        }

        public override string GetTitlePrefix()
        {
            return null;
        }

        private static bool CanSimBeInvitedToParty(SimDescription simDescription, Lot partyVenue, Sim host, float fMaxLTR, bool bHostIsLegendary, bool needToCheckWhetherSimWantsToCome)
        {
            if (simDescription.AssignedRole != null)
            {
                return false;
            }
            else if ((simDescription.CreatedSim != null) && (simDescription.CreatedSim.LotHome == partyVenue))
            {
                return false;
            }
            if (((host != null) && needToCheckWhetherSimWantsToCome) && !WillSimComeToParty(simDescription, host, fMaxLTR, bHostIsLegendary))
            {
                return false;
            }
            return true;
        }

        public override IEnumerable<SimSelection.ICriteria> AlterCriteria(IEnumerable<SimSelection.ICriteria> allCriteria, bool manual, bool canceled)
        {
            if (canceled)
            {
                List<SimSelection.ICriteria> results = new List<SimSelection.ICriteria>();

                results.Add(new FullFamily());
                results.Add(new PartyGuest());

                return results;
            }
            else
            {
                return base.AlterCriteria(allCriteria, manual, canceled);
            }
        }

        private static bool WillSimComeToParty(SimDescription guest, Sim host, float fMaxLTR, bool bHostIsLegendary)
        {
            if (bHostIsLegendary || host.HasTrait(TraitNames.PartyAnimal))
            {
                return true;
            }

            Relationship relationshipToParty = null;

            try
            {
                relationshipToParty = HostedSituation.GetRelationshipToHostedSituation(host.SimDescription, guest);
            }
            catch
            { }

            if (relationshipToParty == null)
            {
                return false;
            }
            if (relationshipToParty.AreFriends() || relationshipToParty.AreRomantic())
            {
                return true;
            }
            float num = MathUtils.Clamp(relationshipToParty.LTR.Liking, HouseParty.kLTRMinToInvite, fMaxLTR);
            return RandomUtil.InterpolatedChance(HouseParty.kLTRMinToInvite, fMaxLTR, (float)HouseParty.kChanceToComeAtMinLTR, (float)HouseParty.kChanceToComeAtMaxLTR, num);
        }

        protected virtual Lot GetVenue(IActor actor, out bool hasExclusiveAccess)
        {
            hasExclusiveAccess = false;
            return actor.LotHome;
        }

        protected override List<SimSelection.ICriteria> GetCriteria(GameHitParameters<GameObject> parameters)
        {
            if (sSuppressCriteria)
            {
                List<SimSelection.ICriteria> results = new List<SimSelection.ICriteria>();

                results.Add(new PartyGuest());

                return results;
            }
            else
            {
                return SelectionCriteria.SelectionOption.List;
            }
        }

        protected override bool AllowSpecies(IMiniSimDescription me)
        {
            return me.IsHuman;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            // Base class checks IsValidDescription
            //if (!base.Allow(me)) return false;

            if (me.AssignedRole != null) return false;

            if (me.ToddlerOrBelow) return false;

            if (me.IsGhost)
            {
                if (Sims3.Gameplay.Objects.Urnstone.FindGhostsGrave(me) == null) return false;
            }
            else
            {
                if ((me.Household != null) && (me.Household.IsActive)) return false;
            }

            return true;
        }

        private static void EnsureFianceeIsInvitedToWeddingParty(Sim host, List<SimDescription> currentGuests)
        {
            if (((host.Partner != null) && !currentGuests.Contains(host.Partner)) && (host.Household != host.Partner.Household))
            {
                currentGuests.Add(host.Partner);
            }
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            //if (!UIUtils.IsOkayToStartModalDialog()) return false;

            bool hasExclusiveAccess;
            Lot partyVenue = GetVenue(parameters.mActor, out hasExclusiveAccess);

            float num2;
            OutfitCategories formalwear;
            Sims3.Gameplay.Situations.Party party = null;
            bool isPartyAtHome = (partyVenue == parameters.mActor.LotHome);
            if (partyVenue == null)
            {
                return OptionResult.Failure;
            }
            if (!parameters.mActor.IsSelectable)
            {
                return OptionResult.Failure;
            }

            PartyPickerDialog.PartyType partyTypes = PartyPickerDialog.PartyType.kAll;

            // Keep as GameUtils
            if (GameUtils.IsOnVacation())
            {
                partyTypes &= ~PartyPickerDialog.PartyType.kBirthday;
            }

            if (partyVenue.LastDiedSim == null)
            {
                partyTypes &= ~PartyPickerDialog.PartyType.kFuneral;
            }

            Sim actorSim = parameters.mActor as Sim;

            Political job = actorSim.Occupation as Political;
            if ((job == null) || (!job.HasCampaignMoneyMetric()))
            {
                partyTypes &= ~PartyPickerDialog.PartyType.kCampaign;
            }

            partyTypes &= ~PartyPickerDialog.PartyType.kWedding;

            foreach (Sim sim in CommonSpace.Helpers.Households.AllSims(parameters.mActor.Household))
            {
                if (sim.IsEngaged)
                {
                    partyTypes |= PartyPickerDialog.PartyType.kWedding;
                    break;
                }
            }

            if (!GameUtils.IsInstalled(ProductVersion.EP4))
            {
                partyTypes &= ~PartyPickerDialog.PartyType.kBachelorParty;
                partyTypes &= ~PartyPickerDialog.PartyType.kChildSlumberParty;
                partyTypes &= ~PartyPickerDialog.PartyType.kTeenParty;
                partyTypes &= ~PartyPickerDialog.PartyType.kTeenSlumberParty;
            }
            else
            {
                if (isPartyAtHome)
                {
                    if (!actorSim.SimDescription.Child)
                    {
                        partyTypes &= ~PartyPickerDialog.PartyType.kChildSlumberParty;
                    }

                    if (!actorSim.SimDescription.Teen)
                    {
                        partyTypes &= ~PartyPickerDialog.PartyType.kTeenParty;
                        partyTypes &= ~PartyPickerDialog.PartyType.kTeenSlumberParty;
                    }
                }
                else
                {
                    partyTypes &= ~PartyPickerDialog.PartyType.kChildSlumberParty;
                    partyTypes &= ~PartyPickerDialog.PartyType.kTeenParty;
                    partyTypes &= ~PartyPickerDialog.PartyType.kTeenSlumberParty;
                }
            }

            partyTypes &= ~PartyPickerDialog.PartyType.kPoolParty;
            partyTypes &= ~PartyPickerDialog.PartyType.kFeastParty;
            partyTypes &= ~PartyPickerDialog.PartyType.kCostumeParty;
            partyTypes &= ~PartyPickerDialog.PartyType.kGiftGivingParty;
            if (GameUtils.IsInstalled(ProductVersion.EP8) /*&& isPartyAtHome*/)
            {
                partyTypes |= PartyPickerDialog.PartyType.kFeastParty;
                partyTypes |= PartyPickerDialog.PartyType.kCostumeParty;
                partyTypes |= PartyPickerDialog.PartyType.kGiftGivingParty;
                //if (PoolParty.CanSimThrowPoolParty(actorSim))
                if (partyVenue.GetSwimmingPoolCount() > 0)
                {
                    partyTypes |= PartyPickerDialog.PartyType.kPoolParty;
                }
            }

            partyTypes &= ~PartyPickerDialog.PartyType.kJuiceKeggerParty;
            partyTypes &= ~PartyPickerDialog.PartyType.kBonfire;
            partyTypes &= ~PartyPickerDialog.PartyType.kTailgatingParty;
            partyTypes &= ~PartyPickerDialog.PartyType.kVideoGameLANParty;
            partyTypes &= ~PartyPickerDialog.PartyType.kMasqueradeBall;
            partyTypes &= ~PartyPickerDialog.PartyType.kVictoryParty;

            if (GameUtils.IsInstalled(ProductVersion.EP9))
            {
                partyTypes |= PartyPickerDialog.PartyType.kTailgatingParty;
                partyTypes |= PartyPickerDialog.PartyType.kVideoGameLANParty;
                partyTypes |= PartyPickerDialog.PartyType.kMasqueradeBall;
                partyTypes |= PartyPickerDialog.PartyType.kVictoryParty;

                if (JuiceKeggerParty.CanSimThrowJuiceKeggerParty(actorSim))
                {
                    partyTypes |= PartyPickerDialog.PartyType.kJuiceKeggerParty;
                }

                if (BonfireParty.CanSimThrowBonfire(actorSim))
                {
                    partyTypes |= PartyPickerDialog.PartyType.kBonfire;
                }
            }

            bool criteriaCanceled;
            SimSelection list = SimSelection.Create(Common.Localize("Party:SelectTitle"), actorSim.SimDescription, this, GetCriteria(parameters), false, false, out criteriaCanceled);

            if (list.IsEmpty)
            {
                SimpleMessageDialog.Show(Common.LocalizeEAString("Gameplay/Objects/Electronics/Phone/CallThrowParty:NoSimsWT"), Common.LocalizeEAString(parameters.mActor.IsFemale, "Gameplay/Objects/Electronics/Phone/CallThrowParty:NoSims", new object[] { parameters.mActor }), ModalDialog.PauseMode.PauseSimulator);
                return OptionResult.Failure;
            }

            float openHour = -1f;
            float closingHour = -1f;

            PartyPickerDialog.PartyInfo info = PartyPickerDialogEx.Show(partyTypes, list.GetPickerInfo(), parameters.mActor.GetThumbnailKey(), isPartyAtHome, 25, -1, openHour, closingHour, PartyPickerDialog.ClothingType.kNone, actorSim.IsFemale);
            if ((info == null) || (info.PartyType == PartyPickerDialog.PartyType.kNone))
            {
                return OptionResult.Failure;
            }

            float hoursPassedOfDay = SimClock.HoursPassedOfDay;
            if (hoursPassedOfDay > info.Time)
            {
                num2 = 24f - (hoursPassedOfDay - info.Time);
            }
            else
            {
                num2 = info.Time - hoursPassedOfDay;
            }
            if (num2 < 1f)
            {
                num2 += 24f;
            }
            long ticks = SimClock.ConvertToTicks(num2, TimeUnit.Hours);
            DateAndTime startTime = SimClock.CurrentTime() + new DateAndTime(ticks);
            bool bHostIsLegendary = actorSim.HasTrait(TraitNames.LegendaryHost);
            float fMaxLTR = 0f;
            LTRData data = LTRData.Get(LongTermRelationshipTypes.Friend);
            if (data != null)
            {
                fMaxLTR = data.Liking - 1;
            }
            List<SimDescription> simList = new List<SimDescription>();
            foreach (object obj2 in info.SimList)
            {
                SimDescription simDescription = obj2 as SimDescription;
                if ((simDescription != null) && CanSimBeInvitedToParty(simDescription, partyVenue, actorSim, fMaxLTR, bHostIsLegendary, true))
                {
                    if (!simList.Contains(simDescription))
                    {
                        simList.Add(simDescription);
                        if (simDescription.TraitManager.HasElement(TraitNames.PartyAnimal))
                        {
                            Sim createdSim = simDescription.CreatedSim;
                            if (createdSim != null)
                            {
                                TraitTipsManager.ShowTraitTip(13271263770231522640L, createdSim, TraitTipsManager.TraitTipCounterIndex.PartyAnimal, TraitTipsManager.kPartyAnimalCountOfParties);
                            }
                        }
                        if (simDescription.IsCelebrity)
                        {
                            EventTracker.SendEvent(EventTypeId.kPartyInviteCeleb, parameters.mActor);
                        }
                    }

                    bool bShouldMatchAge = (simDescription.Age == actorSim.SimDescription.Age) && ((simDescription.Teen) || (simDescription.Child));
                    if (!hasExclusiveAccess && RandomUtil.RandomChance(HouseParty.HousePartyParams.PercentageChanceOfBringingAFriend))
                    {
                        SimDescription friend = SocialComponent.FindFriendNotInList(simDescription, simList, parameters.mActor.LotHome, bShouldMatchAge);
                        if ((friend != null) && CanSimBeInvitedToParty(friend, partyVenue, null, 0f, false, false))
                        {
                            simList.Add(friend);
                        }
                    }
                }
            }

            DateAndTime time = startTime;
            time.Ticks -= SimClock.ConvertToTicks(Sims3.Gameplay.Situations.Party.HoursToStartRentBeforePartyStart, TimeUnit.Hours);
            if (time.CompareTo(SimClock.CurrentTime()) < 0)
            {
                time = SimClock.Add(SimClock.CurrentTime(), TimeUnit.Minutes, 2f);
            }

            if ((partyVenue != parameters.mActor.LotHome) && !RentScheduler.Instance.RentLot(partyVenue, actorSim, time, simList, hasExclusiveAccess))
            {
                SimpleMessageDialog.Show(string.Empty, Phone.Call.LocalizeCallString("ThrowParty", "CantRent", new object[] { parameters.mActor }), ModalDialog.PauseMode.PauseSimulator);
                return OptionResult.Failure;
            }

            switch (info.ClothingType)
            {
                case PartyPickerDialog.ClothingType.kFormal:
                    formalwear = OutfitCategories.Formalwear;
                    break;

                case PartyPickerDialog.ClothingType.kCasual:
                    formalwear = OutfitCategories.Everyday;
                    break;

                case PartyPickerDialog.ClothingType.kSwimwear:
                    formalwear = OutfitCategories.Swimwear;
                    break;

                case PartyPickerDialog.ClothingType.kCostumes:
                    formalwear = OutfitCategories.Everyday;
                    break;

                default:
                    formalwear = OutfitCategories.Everyday;
                    break;
            }

            float infoTime = info.Time;

            switch (info.PartyType)
            {
                case PartyPickerDialog.PartyType.kCampaign:
                    SimpleMessageDialog.Show(Common.LocalizeEAString(actorSim.IsFemale, "Gameplay/Situations/Party:FundraiserTitle"), Common.LocalizeEAString(actorSim.IsFemale, "Gameplay/Situations/Party:FundraiserStart", new object[] { infoTime }), ModalDialog.PauseMode.PauseSimulator);
                    party = new CampaignFundraiser(partyVenue, actorSim, simList, formalwear, startTime);
                    EventTracker.SendEvent(EventTypeId.kThrewFundraiser, parameters.mActor);
                    break;
                case PartyPickerDialog.PartyType.kBirthday:
                    SimpleMessageDialog.Show(Common.LocalizeEAString(actorSim.IsFemale, "Gameplay/Situations/Party:BirthdayTitle"), Common.LocalizeEAString(actorSim.IsFemale, "Gameplay/Situations/Party:BirthdayStart", new object[] { infoTime }), ModalDialog.PauseMode.PauseSimulator);
                    party = new BirthdayParty(partyVenue, actorSim, simList, formalwear, startTime);
                    break;
                case PartyPickerDialog.PartyType.kWedding:
                    string messageText = string.Empty;
                    if (GameUtils.IsInstalled(ProductVersion.EP4))
                    {
                        messageText = Localization.LocalizeString(actorSim.IsFemale, "Gameplay/Situations/Party:WeddingStartWithArch", new object[] { infoTime });
                    }
                    else
                    {
                        messageText = Localization.LocalizeString(actorSim.IsFemale, "Gameplay/Situations/Party:WeddingStart", new object[] { infoTime });
                    }

                    SimpleMessageDialog.Show(Common.LocalizeEAString(actorSim.IsFemale, "Gameplay/Situations/Party:WeddingTitle"), messageText, ModalDialog.PauseMode.PauseSimulator);

                    EnsureFianceeIsInvitedToWeddingParty(actorSim, simList);

                    party = new WeddingParty(partyVenue, actorSim, simList, formalwear, startTime);
                    break;
                case PartyPickerDialog.PartyType.kFuneral:
                    SimpleMessageDialog.Show(Common.LocalizeEAString(actorSim.IsFemale, "Gameplay/Situations/Party:FuneralTitle"), Common.LocalizeEAString(actorSim.IsFemale, "Gameplay/Situations/Party:FuneralStart", new object[] { infoTime }), ModalDialog.PauseMode.PauseSimulator);
                    party = new Funeral(partyVenue, actorSim, simList, formalwear, startTime);
                    break;
                case PartyPickerDialog.PartyType.kBachelorParty:
                    SimpleMessageDialog.Show(Common.LocalizeEAString(actorSim.IsFemale, "Gameplay/Situations/Party:BachelorPartyTitle"), Common.LocalizeEAString(actorSim.IsFemale, "Gameplay/Situations/Party:BachelorStart", new object[] { infoTime }), ModalDialog.PauseMode.PauseSimulator);
                    party = new BachelorParty(partyVenue, actorSim, simList, formalwear, startTime);
                    EventTracker.SendEvent(EventTypeId.kThrewBachelorParty, actorSim);
                    actorSim.SimDescription.SetHadBachelorParty();
                    break;
                case PartyPickerDialog.PartyType.kTeenParty:
                    SimpleMessageDialog.Show(Common.LocalizeEAString(actorSim.IsFemale, "Gameplay/Situations/Party:TeenPartyTitle"), Common.LocalizeEAString(actorSim.IsFemale, "Gameplay/Situations/Party:TeenStart", new object[] { infoTime }), ModalDialog.PauseMode.PauseSimulator);
                    party = new TeenParty(partyVenue, actorSim, simList, formalwear, startTime);
                    break;
                case PartyPickerDialog.PartyType.kChildSlumberParty:
                case PartyPickerDialog.PartyType.kTeenSlumberParty:
                    SimpleMessageDialog.Show(Common.LocalizeEAString(actorSim.IsFemale, "Gameplay/Situations/Party:SlumberPartyTitle"), Common.LocalizeEAString(actorSim.IsFemale, "Gameplay/Situations/Party:SlumberStart", new object[] { infoTime, actorSim }), ModalDialog.PauseMode.PauseSimulator);
                    party = new SlumberParty(partyVenue, actorSim, simList, formalwear, startTime);
                    break;
                case PartyPickerDialog.PartyType.kCostumeParty:
                    SimpleMessageDialog.Show(Localization.LocalizeString(actorSim.IsFemale, "Gameplay/Situation/Party:CostumePartyTitle", new object[0x0]), Localization.LocalizeString(actorSim.IsFemale, "Gameplay/Situation/Party:CostumePartyStart", new object[] { infoTime }), ModalDialog.PauseMode.PauseSimulator);
                    party = new CostumeParty(partyVenue, actorSim, simList, formalwear, startTime);
                    EventTracker.SendEvent(EventTypeId.kCostumePartyScheduled, actorSim);
                    break;

                case PartyPickerDialog.PartyType.kGiftGivingParty:
                    SimpleMessageDialog.Show(Localization.LocalizeString(actorSim.IsFemale, "Gameplay/Situation/Party:GiftGivingPartyTitle", new object[0x0]), Localization.LocalizeString(actorSim.IsFemale, "Gameplay/Situation/Party:GiftGivingPartyStart", new object[] { infoTime }), ModalDialog.PauseMode.PauseSimulator);
                    party = new GiftGivingParty(partyVenue, actorSim, simList, formalwear, startTime);
                    EventTracker.SendEvent(EventTypeId.kGiftGivingPartyScheduled, actorSim);
                    break;

                case PartyPickerDialog.PartyType.kPoolParty:
                    SimpleMessageDialog.Show(Localization.LocalizeString(actorSim.IsFemale, "Gameplay/Situation/Party:PoolPartyTitle", new object[0x0]), Localization.LocalizeString(actorSim.IsFemale, "Gameplay/Situation/Party:PoolPartyStart", new object[] { infoTime }), ModalDialog.PauseMode.PauseSimulator);
                    party = new PoolParty(partyVenue, actorSim, simList, formalwear, startTime);
                    EventTracker.SendEvent(EventTypeId.kSchedulePoolParty, actorSim);
                    break;

                case PartyPickerDialog.PartyType.kFeastParty:
                    SimpleMessageDialog.Show(Localization.LocalizeString(actorSim.IsFemale, "Gameplay/Situation/Party:FeastPartyTitle", new object[0x0]), Localization.LocalizeString(actorSim.IsFemale, "Gameplay/Situation/Party:FeastPartyStart", new object[] { infoTime }), ModalDialog.PauseMode.PauseSimulator);
                    party = new FeastParty(partyVenue, actorSim, simList, formalwear, startTime);
                    EventTracker.SendEvent(EventTypeId.kFeastPartyScheduled, actorSim);
                    break;

                case PartyPickerDialog.PartyType.kJuiceKeggerParty:
                    SimpleMessageDialog.Show(Localization.LocalizeString(actorSim.IsFemale, "Gameplay/Situations/Party:JuiceKeggerPartyTitle", new object[0]), Localization.LocalizeString(actorSim.IsFemale, "Gameplay/Situations/Party:JuiceKeggerStart", new object[] { infoTime }), ModalDialog.PauseMode.PauseSimulator);
                    party = new JuiceKeggerParty(partyVenue, actorSim, simList, formalwear, startTime);
                    EventTracker.SendEvent(EventTypeId.kThrewJuiceKeggerParty, actorSim);
                    break;

                case PartyPickerDialog.PartyType.kTailgatingParty:
                    SimpleMessageDialog.Show(Localization.LocalizeString(actorSim.IsFemale, "Gameplay/Situations/Party:TailgatingPartyTitle", new object[0]), Localization.LocalizeString(actorSim.IsFemale, "Gameplay/Situations/Party:TailgatingStart", new object[] { infoTime }), ModalDialog.PauseMode.PauseSimulator);
                    party = new TailgatingParty(partyVenue, actorSim, simList, formalwear, startTime);
                    EventTracker.SendEvent(EventTypeId.kThrewTailgatingParty, actorSim);
                    break;

                case PartyPickerDialog.PartyType.kBonfire:
                    SimpleMessageDialog.Show(Localization.LocalizeString(actorSim.IsFemale, "Gameplay/Situations/Party:BonfireTitle", new object[0]), Localization.LocalizeString(actorSim.IsFemale, "Gameplay/Situations/Party:BonfireStart", new object[] { infoTime }), ModalDialog.PauseMode.PauseSimulator);
                    party = new BonfireParty(partyVenue, actorSim, simList, formalwear, startTime);
                    EventTracker.SendEvent(EventTypeId.kThrewBonfireParty, actorSim);
                    break;

                case PartyPickerDialog.PartyType.kVideoGameLANParty:
                    SimpleMessageDialog.Show(Localization.LocalizeString(actorSim.IsFemale, "Gameplay/Situations/Party:VideoGameLANPartyTitle", new object[0]), Localization.LocalizeString(actorSim.IsFemale, "Gameplay/Situations/Party:VideoGameLANStart", new object[] { infoTime }), ModalDialog.PauseMode.PauseSimulator);
                    party = new VideoGameLANParty(partyVenue, actorSim, simList, formalwear, startTime);
                    EventTracker.SendEvent(EventTypeId.kThrewVideoGameLANParty, actorSim);
                    break;

                case PartyPickerDialog.PartyType.kMasqueradeBall:
                    SimpleMessageDialog.Show(Localization.LocalizeString(actorSim.IsFemale, "Gameplay/Situations/Party:MasqueradeBallTitle", new object[0]), Localization.LocalizeString(actorSim.IsFemale, "Gameplay/Situations/Party:MasqueradeStart", new object[] { infoTime }), ModalDialog.PauseMode.PauseSimulator);
                    party = new MasqueradeBall(partyVenue, actorSim, simList, formalwear, startTime);
                    EventTracker.SendEvent(EventTypeId.kThrewMasqueradeBall, actorSim);
                    break;

                case PartyPickerDialog.PartyType.kVictoryParty:
                    SimpleMessageDialog.Show(Localization.LocalizeString(actorSim.IsFemale, "Gameplay/Situations/Party:VictoryPartyTitle", new object[0]), Localization.LocalizeString(actorSim.IsFemale, "Gameplay/Situations/Party:VictoryStart", new object[] { infoTime }), ModalDialog.PauseMode.PauseSimulator);
                    party = new VictoryParty(partyVenue, actorSim, simList, formalwear, startTime);
                    EventTracker.SendEvent(EventTypeId.kThrewVictoryParty, actorSim);
                    break;

                default:
                    SimpleMessageDialog.Show(Common.LocalizeEAString(actorSim.IsFemale, "Gameplay/Situations/Party:GenericTitle"), Common.LocalizeEAString(actorSim.IsFemale, "Gameplay/Situations/Party:HouseStart", new object[] { infoTime }), ModalDialog.PauseMode.PauseSimulator);
                    party = new HouseParty(partyVenue, actorSim, simList, formalwear, startTime);
                    break;
            }

            if (party == null) return OptionResult.Failure;

            foreach (SimDescription sim in party.GuestDescriptions)
            {
                Instantiation.EnsureInstantiate(sim, party.Lot);
            }

            EventTracker.SendEvent(new PartyEvent(EventTypeId.kThrewParty, actorSim, actorSim.SimDescription, party));
            if (actorSim.HasTrait(TraitNames.PartyAnimal))
            {
                TraitTipsManager.ShowTraitTip(13271263770231522640L, actorSim, TraitTipsManager.TraitTipCounterIndex.PartyAnimal, TraitTipsManager.kPartyAnimalCountOfParties);
            }
            return OptionResult.SuccessClose;
        }

        public class SuppressCriteria : IDisposable
        {
            public SuppressCriteria()
            {
                sSuppressCriteria = !MasterController.Settings.mUsePartyFilter;
            }

            public void Dispose()
            {
                sSuppressCriteria = false;
            }
        }
    }
}
