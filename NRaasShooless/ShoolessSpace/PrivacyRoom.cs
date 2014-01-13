using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.ShoolessSpace
{
    [Persistable]
    public class PrivacyRoom
    {
        public ulong mLot;
        public int mRoom;

        public PrivacyRoom()
        { }
        public PrivacyRoom(ulong lot, int room)
        {
            mLot = lot;
            mRoom = room;
        }

        public override bool Equals(object obj)
        {
            PrivacyRoom room = obj as PrivacyRoom;
            if (room == null) return false;

            if (mLot != room.mLot) return false;

            return (mRoom == room.mRoom);
        }

        public override int GetHashCode()
        {
            return (int)mLot + mRoom;
        }
    }
}


