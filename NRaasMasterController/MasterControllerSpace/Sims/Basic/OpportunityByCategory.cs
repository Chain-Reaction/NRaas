using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.HobbiesSkills.Band;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Basic
{
    public class OpportunityByCategory : SimFromList, IBasicOption
    {
        public override string GetTitlePrefix()
        {
            return "OpportunityByCategory";
        }

        protected override bool AllowSpecies(IMiniSimDescription me)
        {
            return me.IsHuman;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.CreatedSim == null) return false;

            if (me.CreatedSim.OpportunityManager == null) return false;

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            List<Opportunity> allOpportunities = OpportunityEx.GetAllOpportunities(me.CreatedSim, OpportunityCategory.None);
            if (allOpportunities.Count == 0)
            {
                SimpleMessageDialog.Show(Name, Common.Localize("Opportunity:None"));
                return false;
            }

            Dictionary<string, Item> lookup = new Dictionary<string, Item>();

            string all = "(" + Common.LocalizeEAString("Ui/Caption/ObjectPicker:All") + ")";

            foreach (Opportunity opp in allOpportunities)
            {
                string name = null;
                switch(opp.OpportunityCategory)
                {
                    case OpportunityCategory.Career:
                        name = Common.LocalizeEAString("Ui/Caption/HUD/CareerPanel:Career");
                        break;
                    case OpportunityCategory.Special:
                        name = Common.Localize("Opportunity:Celebrity");
                        break;
                    case OpportunityCategory.AdventureChina:
                    case OpportunityCategory.AdventureEgypt:
                    case OpportunityCategory.AdventureFrance:
                        name = Common.Localize("Opportunity:Adventure");
                        break;
                    case OpportunityCategory.Dare:
                    case OpportunityCategory.DayJob:
                    case OpportunityCategory.SocialGroup:
                        name = Common.LocalizeEAString("Gameplay/Objects/PostBoxJobBoard:" + opp.OpportunityCategory);
                        break;
                    case OpportunityCategory.Skill:
                        bool found = false;

                        foreach (Opportunity.OpportunitySharedData.RequirementInfo info in opp.SharedData.mRequirementList)
                        {
                            if (info.mType == RequirementType.Skill)
                            {
                                SkillNames guid = (SkillNames)info.mGuid;

                                Skill staticSkill = SkillManager.GetStaticSkill(guid);
                                if (staticSkill.NonPersistableData != null)
                                {
                                    name = Common.LocalizeEAString(staticSkill.NonPersistableData.Name);
                                    found = true;
                                }

                                if (found)
                                {
                                    break;
                                }
                            }
                        }

                        if (opp is GigOpportunity)
                        {
                            Common.DebugNotify(opp.Name);

                            Occupation band = CareerManager.GetStaticOccupation(OccupationNames.RockBand);
                            if (band != null)
                            {
                                name = band.CareerName;
                            }
                        }
                        break;
                }
                
                if (string.IsNullOrEmpty(name))
                {
                    name = Common.Localize("Opportunity:Generic");
                }

                Item item;
                if (!lookup.TryGetValue(name, out item))
                {
                    item = new Item(name);
                    lookup.Add(name, item);
                }

                item.Add(opp);

                if (!lookup.TryGetValue(all, out item))
                {
                    item = new Item(all);
                    lookup.Add(all, item);
                }

                item.Add(opp);
            }

            Item choice = new CommonSelection<Item>(Name, me.FullName, lookup.Values).SelectSingle();
            if (choice == null) return false;

            return choice.Run(me);
        }

        public class Item : ValueSettingOption<List<Opportunity>>
        {
            public Item()
            {
                mValue = new List<Opportunity>();
            }
            public Item(string name)
                : base(new List<Opportunity>(), name, 0)
            { }

            public void Add(Opportunity opp)
            {
                Value.Add(opp);
            }

            public bool Run(SimDescription me)
            {
                List<SelectOpportunity.Item> allOptions = new List<SelectOpportunity.Item>();
                foreach (Opportunity item in Value)
                {
                    allOptions.Add(new SelectOpportunity.Item(item, me.CreatedSim, item.Name, Value));
                }

                SelectOpportunity.Item selection = new CommonSelection<SelectOpportunity.Item>(mName, allOptions, new SelectOpportunity.CompletedColumn()).SelectSingle();
                if (selection == null) return false;

                return OpportunityEx.Perform(me, selection.Value.Guid);
            }
        }
    }
}
