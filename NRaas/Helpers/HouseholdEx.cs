using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CustomContent;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class HouseholdEx
    {
        public static void AddSims(Household ths, List<Sim> sims, Household oldHousehold, bool bDeleteHouseholdIfEmpty, bool willBeActive, bool dreamCatcher)
        {
            bool isActive = ths.IsActive || willBeActive;

            List<SimDescription> existing = new List<SimDescription>(Households.All(ths));

            foreach (Sim sim in sims)
            {
                try
                {
                    SimDescription simDescription = sim.SimDescription;
                    if (simDescription.ServiceHistory != ServiceType.None)
                    {
                        ths.AddServiceUniform(simDescription.ServiceHistory);
                    }

                    if (isActive)
                    {
                        /*
                        if (((!simDescription.IsMummy && !simDescription.IsFrankenstein) && (!simDescription.IsImaginaryFriend && !sim.IsActiveFirefighter)) && simDescription.ReplaceHiddenCasParts())
                        {
                            sim.PushSwitchToOutfitInteraction(Sim.ClothesChangeReason.Force, OutfitCategories.Everyday);
                        }
                        */

                        if (simDescription.CreatedByService != null)
                        {
                            simDescription.CreatedByService.OnMoveInActiveHousehold(sim);
                        }
                    }

                    if (oldHousehold != null)
                    {
                        oldHousehold.RemoveSim(sim, false, bDeleteHouseholdIfEmpty);
                    }

                    if (sim.Household != ths)
                    {
                        ths.Add(simDescription);
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(sim, e);
                }
            }

            if ((ths.LotHome != null) && (ths.LotHome.IsApartmentLot))
            {
                if (isActive)
                {
                    foreach (SimDescription member in existing)
                    {
                        Household.RoommateManager.AddRoommate(member, ths);
                    }

                    Household.RoommateManager.StartAcceptingRoommates(8, ths);
                }
                else
                {
                    Household.RoommateManager.MakeAllRoommatesSelectable(ths);
                }
            }

            ths.AddWardrobeToWardrobe(oldHousehold.Wardrobe);
            ths.AddServiceUniforms(oldHousehold.mServiceUniforms);

            foreach (Sim sim2 in sims)
            {
                DreamCatcher.AdjustSelectable(sim2.SimDescription, isActive, dreamCatcher);
            }
        }

        public static bool ImportContent(Household me, ResKeyTable resKeyTable, ObjectIdTable objIdTable, IPropertyStreamReader reader)
        {
            ulong num2;
            int num3;
            ulong[] numArray;
            ulong[] numArray2;
            ulong num8;

            reader.ReadString(0x50e3d25c, out me.mName, "");
            reader.ReadInt32(0x33b96433, out me.mFamilyFunds, 0x0);
            reader.ReadString(0xd96b03f8, out me.mBioText, "");
            reader.ReadInt32(0x8af84d8, out me.mUnpaidBills, 0x0);
            reader.ReadInt32(0xc6fdb12d, out me.mLotHomeWorth, 0x0);
            long num = DateTime.Now.ToBinary();
            reader.ReadInt64(0x6384bbe4, out num, DateTime.Now.ToBinary());
            me.mExportTime = DateTime.FromBinary(num);
            reader.ReadBool(0x2213ca71, out me.mbLifetimeHappinessNotificationShown, false);
            IPropertyStreamReader child = reader.GetChild(0x91fa3ace);
            if (child != null)
            {
                me.mMembers.ImportContent(resKeyTable, objIdTable, child);

                // Custom to handle import of custom careers
                HouseholdMemberEx.ImportContent(me.mMembers, resKeyTable, objIdTable, child);
            }

            foreach (SimDescription description in Households.All(me))
            {
                try
                {
                    // Custom
                    if (Household.sOldIdToNewSimDescriptionMap != null)
                    {
                        if ((description.mOldSimDescriptionId != 0) &&
                            (!Household.sOldIdToNewSimDescriptionMap.ContainsKey(description.mOldSimDescriptionId)))
                        {
                            Household.sOldIdToNewSimDescriptionMap.Add(description.mOldSimDescriptionId, description);
                        }
                        if (!Household.sOldIdToNewSimDescriptionMap.ContainsKey(description.mSimDescriptionId))
                        {
                            Household.sOldIdToNewSimDescriptionMap.Add(description.mSimDescriptionId, description);
                        }
                    }

                    description.OnHouseholdChanged(me, true);
                    description.CareerManager.OnLoadFixup();

                    Occupation occupation = description.CareerManager.Occupation;
                    if (occupation != null)
                    {
                        occupation.RepairLocation();
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(description, e);
                }
            }

            foreach (SimDescription description2 in Households.All(me))
            {
                try
                {
                    if (description2.Pregnancy != null)
                    {
                        SimDescription description3;
                        if ((Household.sOldIdToNewSimDescriptionMap != null) && Household.sOldIdToNewSimDescriptionMap.TryGetValue(description2.Pregnancy.DadDescriptionId, out description3))
                        {
                            description2.Pregnancy.DadDescriptionId = description3.SimDescriptionId;
                        }
                        else if (!Household.IsTravelImport && (me.mMembers.GetSimDescriptionFromId(description2.Pregnancy.DadDescriptionId) == null))
                        {
                            description2.Pregnancy.DadDescriptionId = 0x0L;
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(description2, e);
                }
            }

            child = reader.GetChild(0xa14ee9b6);
            if (child != null)
            {
                // Custom Function
                ImportRelationships(me, resKeyTable, objIdTable, child);
            }

            reader.ReadInt32(0x872a7c9, out me.mAncientCoinCount, 0x0);
            reader.ReadUint64(0x4723b840, out num2, 0x0L);
            me.UniqueObjectsObtained = (UniqueObjectKey)num2;
            me.mKeystonePanelsUsed = new PairedListDictionary<WorldName, List<string>>();
            reader.ReadInt32(0x7bcd11d, out num3, 0x0);
            for (uint i = 0x0; i < num3; i++)
            {
                int num5;
                string[] strArray;
                reader.ReadInt32(0x8bc9c54 + i, out num5, -1);
                WorldName name = (WorldName)num5;
                reader.ReadString(0x9bcd0e7 + i, out strArray);
                me.mKeystonePanelsUsed[name] = new List<string>(strArray);
            }
            me.mCompletedHouseholdOpportunities.Clear();
            reader.ReadUint64(0x8eae351, out numArray);
            foreach (ulong num6 in numArray)
            {
                me.mCompletedHouseholdOpportunities.Add(num6, true);
            }
            reader.ReadUint32(0x8eae352, out me.mMoneySaved);
            reader.ReadUint64(0x92b562c, out numArray2);
            foreach (ulong num7 in numArray2)
            {
                me.mWardrobeCasParts.Add(num7);
            }
            reader.ReadUint64(0x95175f0, out num8, 0x0L);
            me.AddServiceUniforms((ServiceType)((int)num8));
            return true;
        }

        private static bool ImportRelationships(Household me, ResKeyTable resKeyTable, ObjectIdTable objIdTable, IPropertyStreamReader reader)
        {
            Common.StringBuilder msg = new Common.StringBuilder("ImportRelationships");

            try
            {
                uint num = 0x0;
                reader.ReadUint32(0x1265c8a0, out num, 0x0);

                msg += "A" + num;

                for (uint i = 0x0; i < num; i++)
                {
                    msg += "B";

                    ulong id;
                    ulong simDescriptionId;
                    IPropertyStreamReader child = reader.GetChild(i);
                    if (((child != null) && child.ReadUint64(0xc9c68605, out id, 0x0L)) && child.ReadUint64(0xbf0a71c2, out simDescriptionId, 0x0L))
                    {
                        msg += "C";

                        if (Household.sOldIdToNewSimDescriptionMap != null)
                        {
                            SimDescription description;
                            if (Household.sOldIdToNewSimDescriptionMap.TryGetValue(id, out description))
                            {
                                id = description.SimDescriptionId;
                            }

                            if (Household.sOldIdToNewSimDescriptionMap.TryGetValue(simDescriptionId, out description))
                            {
                                simDescriptionId = description.SimDescriptionId;
                            }
                        }

                        msg += "D";

                        SimDescription simDescriptionFromId = me.mMembers.GetSimDescriptionFromId(id);
                        SimDescription y = me.mMembers.GetSimDescriptionFromId(simDescriptionId);

                        msg += "E";

                        // Custom Try/Catch
                        try
                        {
                            Relationship relation = Relationship.Get(simDescriptionFromId, y, true);
                            if (relation != null)
                            {
                                relation.ImportContent(resKeyTable, objIdTable, child);
                            }
                        }
                        catch (Exception e)
                        {
                            Common.Exception(simDescriptionFromId, y, e);
                        }

                        msg += "F";
                    }
                }
            }
            catch(Exception e)
            {
                Common.DebugException(msg, e);
            }
            return true;
        }
    }
}
