using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.SimIFace;
using Sims3.SimIFace.CustomContent;
using System;

namespace NRaas.CommonSpace.Proxies
{
    public class InventoryProxy : IExportableContent
    {
        Inventory mInventory;

        public InventoryProxy(Inventory inventory)
        {
            mInventory = inventory;
        }

        public bool ExportContent(ResKeyTable resKeyTable, ObjectIdTable objIdTable, IPropertyStreamWriter writer)
        {
            return mInventory.ExportContent(resKeyTable, objIdTable, writer);
        }

        public bool ImportContent(ResKeyTable resKeyTable, ObjectIdTable objIdTable, IPropertyStreamReader reader)
        {
            Inventory ths = mInventory;

            uint[] numArray;
            ths.DestroyItems();
            ths.Owner.InventoryComp.InventoryUIModel.ClearInvalidItemStacks();
            if (!reader.ReadUint32(0x804459ab, out numArray))
            {
                return false;
            }

            foreach (uint num in numArray)
            {
                IGameObject obj2 = GameObject.GetObject<IGameObject>(objIdTable[(int)num]);
                if (obj2 != null)
                {
                    IPropertyStreamReader child = reader.GetChild(num);
                    if (child != null)
                    {
                        bool flag = true;
                        try
                        {
                            Urnstone urnstone = obj2 as Urnstone;
                            if (urnstone != null)
                            {
                                flag = urnstone.ImportContent(resKeyTable, objIdTable, child, ths.Owner);
                            }
                            else if (obj2 is IPhoneCell)
                            {
                                if (ths.AmountIn<IPhoneCell>() == 0x1)
                                {
                                    flag = false;
                                }
                            }
                            else
                            {
                                flag = obj2.ImportContent(resKeyTable, objIdTable, child);
                            }
                        }
                        catch (Exception)
                        {
                            flag = false;
                        }
                        if (!flag)
                        {
                            obj2.Destroy();
                            continue;
                        }
                    }

                    // Custom
                    if (!Inventories.TryToMove(obj2, ths, false))
                    {
                        obj2.Destroy();
                    }
                }
            }
            reader.ReadInt32(0x86c4285, out ths.mMaxInventoryCapacity, 0x0);
            return true;
        }
    }
}
