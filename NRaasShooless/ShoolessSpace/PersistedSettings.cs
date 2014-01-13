using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ShoolessSpace
{
    [Persistable]
    public class PersistedSettings
    {
        [Tunable, TunableComment("Whether to use the Naked Outfit while on the toilet")]
        public static bool kNakedToilet = false;

        List<PrivacyRoom> mPrivacy = new List<PrivacyRoom>();

        public bool mNakedToilet = kNakedToilet;

        [Persistable(false)]
        Dictionary<PrivacyRoom, bool> mLookup = null;

        public Dictionary<PrivacyRoom, bool> Lookup
        {
            get
            {
                if (mLookup == null)
                {
                    mLookup = new Dictionary<PrivacyRoom, bool>();

                    foreach (PrivacyRoom obj in mPrivacy)
                    {
                        if (mLookup.ContainsKey(obj)) continue;

                        mLookup.Add(obj, true);
                    }
                }

                return mLookup;
            }
        }

        public bool GetGlobalPrivacy()
        {
            return !Lookup.ContainsKey(new PrivacyRoom(LotManager.GetWorldLot().LotId, 0));
        }

        public bool GetPrivacy(GameObject obj)
        {
            if (obj == null) return true;

            if (Common.IsRootMenuObject(obj))
            {
                return GetGlobalPrivacy();
            }
            else
            {
                if (!GetGlobalPrivacy()) return false;

                return !Lookup.ContainsKey(new PrivacyRoom(obj.LotCurrent.LotId, obj.RoomId));
            }
        }

        public void ToggleGlobalPrivacy()
        {
            TogglePrivacy(new PrivacyRoom(LotManager.GetWorldLot().LotId, 0));
        }
        public void TogglePrivacy(GameObject obj)
        {
            TogglePrivacy(new PrivacyRoom(obj.LotCurrent.LotId, obj.RoomId));
        }

        protected void TogglePrivacy(PrivacyRoom room)
        {
            mPrivacy.Remove(room);

            if (!Lookup.ContainsKey(room))
            {
                Lookup.Add(room, true);

                mPrivacy.Add(room);
            }
            else
            {
                Lookup.Remove(room);
            }
        }

        public void Cleanup()
        {
            mLookup = null;

            List<PrivacyRoom> remove = new List<PrivacyRoom>();
            foreach (PrivacyRoom obj in mPrivacy)
            {
                Lot lot = LotManager.GetLot(obj.mLot);
                if (lot == null)
                {
                    remove.Add(obj);
                }
                else if (!lot.IsWorldLot)
                {
                    bool found = false;
                    foreach (Sims3.Gameplay.Scenarios.IFloodWhenBroken roomObj in lot.GetObjects<Sims3.Gameplay.Scenarios.IFloodWhenBroken>())
                    {
                        if (roomObj.RoomId == obj.mRoom)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        remove.Add(obj);
                    }
                }
            }

            foreach (PrivacyRoom obj in remove)
            {
                mPrivacy.Remove(obj);
            }
        }
    }
}
