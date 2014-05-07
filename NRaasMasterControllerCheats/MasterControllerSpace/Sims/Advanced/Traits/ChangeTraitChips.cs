using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced.Traits
{
    public class ChangeTraitChips : SimFromList, ITraitOption
    {
        public override string GetTitlePrefix()
        {
            return "ChangeTraitChips";
        }

        protected override int GetMaxSelection()
        {
            return 7;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.CreatedSim == null || me.CreatedSim.Inventory == null) return false;

            return (me.TraitManager != null && me.TraitChipManager != null && me.IsEP11Bot);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {            
            List<BotChip.Item> allOptions = new List<BotChip.Item>();

            List<TraitChip> list = new List<TraitChip>();
            list = me.TraitChipManager.GetInstalledTraitChips();

            List<TraitChipName> installed = new List<TraitChipName>();
            foreach(TraitChip chip in list)
            {
                installed.Add(chip.TraitChipName);
            }

            foreach(TraitChipName name in Enum.GetValues(typeof(TraitChipName)))
            {
                TraitChipStaticData data = TraitChipManager.GetStaticElement(name);
                if (data == null) continue;                

                int count = 0;
                if (installed.Contains(data.Guid))
                {
                    count++;
                }

                allOptions.Add(new BotChip.Item(data.Guid, count));
            }

            CommonSelection<BotChip.Item>.Results selection = new CommonSelection<BotChip.Item>(Name, me.FullName, allOptions, new BotChip.AuxillaryColumn()).SelectMultiple();
            if (selection.Count == 0) return false;
            
            List<TraitChipStaticData> chipsToInstall = new List<TraitChipStaticData>();
            foreach (BotChip.Item item in selection)
            {
                if (item == null) continue;

                TraitChipName chipName = item.Value;

                TraitChipStaticData chip = TraitChipManager.GetStaticElement(chipName);
                if (chip != null)
                {
                    if (installed.Contains(chipName))
                    {
                        foreach (TraitChip chipInBot in list)
                        {
                            if (chipInBot.TraitChipName == chipName)
                            {
                                me.TraitChipManager.RemoveTraitChip(chipInBot.mTraitChipSlot);
                                chipInBot.RemoveTraitChipFromSim();                                
                            }
                        }
                    }
                    else
                    {
                        chipsToInstall.Add(chip);
                    }
                }
            }           

            bool exceeded = false;
            foreach (TraitChipStaticData chip in chipsToInstall)
            {
                TraitChipName chipName = chip.Guid;
                
                TraitChip chipToUse = null;
                foreach (TraitChip inChip in me.CreatedSim.Inventory.FindAll<TraitChip>(false))
                {
                    if (inChip.TraitChipName == chipName)
                    {
                        chipToUse = inChip;                        
                        break;
                    }
                }

                if (chipToUse == null && me.TraitChipManager.Owner != null)
                {
                    SimDescription owner = me.TraitChipManager.Owner;
                    if (owner.CreatedSim != null && owner.CreatedSim.Inventory != null)
                    {
                        foreach (TraitChip inChip in owner.CreatedSim.Inventory.FindAll<TraitChip>(false))
                        {
                            if (inChip.TraitChipName == chipName)
                            {
                                chipToUse = inChip;                                
                                break;
                            }
                        }
                    }
                }

                bool created = false;
                if (chipToUse == null)
                {                    
                    chipToUse = TraitChipManager.CreateTraitChip(chipName);
                    if (chipToUse != null)
                    {
                        me.CreatedSim.Inventory.TryToAdd(chipToUse);
                        chipToUse.CreatorSim = me.SimDescriptionId;
                        chipToUse.SetOwner(me);
                        created = true;                        
                    }
                }

                if (chipToUse != null)
                {
                    int count = me.TraitChipManager.GetInstalledTraitChips().Count;                    
                    count++;
                    
                    if (count > me.TraitChipManager.NumTraitSlots && me.TraitChipManager.NumTraitSlots < me.TraitChipManager.MaxNumTraitSlots)
                    {                        
                        me.TraitChipManager.UpgradeNumTraitChips(count);
                    }                    

                    int newSlot = me.TraitChipManager.GetOpenTraitChipSlot();
                    if (newSlot != -1)
                    {                        
                        me.TraitChipManager.AddTraitChip(chipToUse, newSlot);
                    }
                    else
                    {                        
                        exceeded = true;
                        if (created)
                        {
                            chipToUse.Destroy();
                        }
                        break;
                    }
                }                
            }     

            if (exceeded)
            {
                // tried to expand this beyond 7 but the change trait chip UI has a cow, chicken, pig and a plumbot when it encounters > 7
                Common.Notify(Common.Localize("ChangeTraitChips:MaxExceeded", me.IsFemale, new object[] { me, 7, 7 }));
            }            

            return true;
        }
    }
}