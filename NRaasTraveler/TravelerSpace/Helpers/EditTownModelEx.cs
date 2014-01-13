using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;

namespace NRaas.TravelerSpace.Helpers
{
    public class EditTownModelEx
    {
        public static bool IsAnyUnoccupiedLotStatic()
        {
            foreach (Lot lot in LotManager.AllLots)
            {
                if ((lot.LotType == LotType.Residential) && (lot.Household == null))
                {
                    switch (lot.ResidentialLotSubType)
                    {
                        case ResidentialLotSubType.kResidentialUndefined:
                        case ResidentialLotSubType.kEP9_Apartment:
                            return true;
                    }
                }
            }
            return false;
        }

        private static bool ChangeLotTypeHelper(Lot lot, bool deleteInvalidObjects, LotType newType, CommercialLotSubType commercialSubType, ResidentialLotSubType residentialSubType)
        {
            List<GameObject> objects = null;

            if (newType == LotType.Commercial)
            {
                // Custom
                //objects = lot.GetObjects<GameObject>(new Predicate<GameObject>(EditTownModel.IsNotValidCommunityLotObject));

                if (commercialSubType == CommercialLotSubType.kEP10_Resort)
                {
                    foreach (RabbitHole hole in lot.GetObjects<RabbitHole>(new Predicate<RabbitHole>(EditTownModel.IsInvestable)))
                    {
                        objects.Add(hole);
                    }

                    if (lot.ResortManager == null)
                    {
                        lot.ResortManager = new ResortManager(lot);
                    }
                }
            }
            else
            {
                // Custom
                //objects = lot.GetObjects<GameObject>(new Predicate<GameObject>(EditTownModel.IsNotValidResidentialLotObject));
            }

            if ((objects != null) && (objects.Count > 0))
            {
                if (!deleteInvalidObjects)
                {
                    return false;
                }

                foreach (GameObject obj2 in objects)
                {
                    int num = 0;
                    bool flag = false;
                    while (obj2.ActorsUsingMe.Count > num)
                    {
                        Sim sim = obj2.ActorsUsingMe[num];
                        if (sim != null)
                        {
                            sim.SetObjectToReset();
                            sim.InteractionQueue.PurgeInteractions(obj2);
                            flag = true;
                        }
                        num++;
                    }
                    if (flag)
                    {
                        SpeedTrap.Sleep(0);
                    }
                    lot.RemoveObjectFromLot(obj2.ObjectId, true);
                    obj2.Destroy();
                }

                ThumbnailKey key = new ThumbnailKey(new ResourceKey(lot.LotId, 0x436fee4c, 0), ThumbnailSize.Large);
                ThumbnailManager.InvalidateThumbnail(key);
                EditTownModel.UpdateDirtyLotThumbnailsTask(true);
            }

            if (lot.CommercialLotSubType == CommercialLotSubType.kEP10_Resort)
            {
                foreach (IResortBuffetTable table in lot.GetObjects<IResortBuffetTable>())
                {
                    table.ClearTable();
                }
            }

            if (GameStates.IsEditTownState)
            {
                BinCommon.KickSimsOffLot(lot, true);
            }

            return true;
        }

        public static bool ChangeLotType(ulong lotId, bool deleteInvalidObjects, LotType newType, CommercialLotSubType commercialSubType, ResidentialLotSubType residentialSubType)
        {
            EditTownModel ths = EditTownController.Instance.mModel as EditTownModel;

            Lot lot = LotManager.GetLot(lotId);
            if (lot == null)
            {
                return false;
            }
            if (newType == LotType.Residential)
            {
                commercialSubType = CommercialLotSubType.kCommercialUndefined;
            }
            else
            {
                residentialSubType = ResidentialLotSubType.kResidentialUndefined;
            }

            if ((newType != LotType.Residential) && (commercialSubType == CommercialLotSubType.kCommercialUndefined))
            {
                return false;
            }

            if (!ChangeLotTypeHelper(lot, deleteInvalidObjects, newType, commercialSubType, residentialSubType))
            {
                return false;
            }

            ths.GetInWorldCommunityLotBinInfoList();
            ths.GetInWorldLotBinInfoList();
            lot.LotType = newType;
            lot.CommercialLotSubType = commercialSubType;
            lot.ResidentialLotSubType = residentialSubType;
            if (lot.IsCommunityLot)
            {
                UIBinInfo item = ths.FindLotBinInfo(lotId);
                bool flag2 = true;
                if (item == null)
                {
                    flag2 = false;
                    item = ths.FindCommunityLotBinInfo(lotId);
                }
                if (item != null)
                {
                    item.LotTypeFilter = 0x8;
                    item.LotType = newType;
                    item.CommercialLotSubType = commercialSubType;
                    item.ResidentialLotSubType = residentialSubType;
                    if (flag2)
                    {
                        ths.mInWorldLotBin.Remove(item);
                        ths.mInWorldCommunityLotBin.Add(item);
                    }
                }
            }
            else
            {
                UIBinInfo info2 = ths.FindCommunityLotBinInfo(lotId);
                bool flag3 = true;
                if (info2 == null)
                {
                    flag3 = false;
                    info2 = ths.FindLotBinInfo(lotId);
                }
                if (info2 != null)
                {
                    if (World.LotIsEmpty(lotId) && lot.IsLotEmptyFromObjects())
                    {
                        info2.LotTypeFilter = 0x2;
                    }
                    else
                    {
                        info2.LotTypeFilter = 0x4;
                    }
                    info2.LotType = newType;
                    info2.CommercialLotSubType = commercialSubType;
                    info2.ResidentialLotSubType = residentialSubType;
                    if (flag3)
                    {
                        ths.mInWorldCommunityLotBin.Remove(info2);
                        ths.mInWorldLotBin.Add(info2);
                    }
                }
            }

            SpeedTrap.Sleep();
            lot.EnsureLotObjects();

            try
            {
                Sims3.Gameplay.Services.Services.ClearServicesForLot(lot);
            }
            catch (Exception e)
            {
                Common.Exception(lot, e);
            }

            Occupation.ValidateJobConsistencyWithLotType(lot);
            lot.UpdateCachedValues();
            return true;
        }
    }
}
