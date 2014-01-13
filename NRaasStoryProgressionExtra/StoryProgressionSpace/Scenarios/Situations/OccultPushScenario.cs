using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Situations
{
    public class OccultPushScenario : SimScenario
    {
        public OccultPushScenario()
        { }
        protected OccultPushScenario(OccultPushScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "OccultPush";
        }

        protected override int ContinueChance
        {
            get { return 25; }
        }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override int Rescheduling
        {
            get { return 120; }
        }

        protected override int MaximumReschedules
        {
            get { return 4; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.Occults;
        }

        protected override bool Allow()
        {
            if (!Situations.GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (!Situations.Allow(this, sim))
            {
                IncStat("Situations Denied");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            return true;
        }

        protected override bool Push()
        {
            List<OccultTypes> occults = OccultTypeHelper.CreateList(Sim);
            if (occults.Count == 0) return false;

            OccultTypes occult = RandomUtil.GetRandomObjectFromList(occults);

            List<CommercialLotSubType> types = new List<CommercialLotSubType>();

            switch (occult)
            {
                case OccultTypes.Vampire:
                    types.Add(CommercialLotSubType.kEP3_CocktailLoungeVampire);
                    types.Add(CommercialLotSubType.kGraveyard);
                    types.Add(CommercialLotSubType.kEP7_Mausoleum);
                    break;
                case OccultTypes.Werewolf:
                    types.Add(CommercialLotSubType.kEP7_WerewolfBar);
                    types.Add(CommercialLotSubType.kBigPark);
                    types.Add(CommercialLotSubType.kSmallPark);
                    break;
                case OccultTypes.Witch:
                    types.Add(CommercialLotSubType.kEP7_PotionShopConsignmentStore);
                    types.Add(CommercialLotSubType.kEP7_VaultOfAntiquity);
                    break;
                case OccultTypes.Fairy:
                    types.Add(CommercialLotSubType.kEP7_Arboretum);
                    types.Add(CommercialLotSubType.kBigPark);
                    types.Add(CommercialLotSubType.kSmallPark);
                    break;
                case OccultTypes.Mummy:
                    types.Add(CommercialLotSubType.kEP1_ChineseGarden);
                    types.Add(CommercialLotSubType.kEP1_LandmarkTomb);
                    types.Add(CommercialLotSubType.kEP1_HiddenTomb);
                    break;
                case OccultTypes.ImaginaryFriend:
                    types.Add(CommercialLotSubType.kHangout);
                    types.Add(CommercialLotSubType.kBigPark);
                    types.Add(CommercialLotSubType.kSmallPark);
                    break;
            }

            if (types.Count == 0) return false;

            Lot lot = Lots.GetCommunityLot(Sim.CreatedSim, types, false);
            if (lot == null) return false;

            return Situations.PushVisit(this, Sim, lot);
        }

        public override Scenario Clone()
        {
            return new OccultPushScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSituation, OccultPushScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "OccultPush";
            }
        }
    }
}
