using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.CAS;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CommonSpace.Proxies
{
    public class MiniSimDescriptionProxy : IMiniSimDescription
    {
        public IMiniSimDescription mSim;

        public MiniSimDescriptionProxy(IMiniSimDescription sim)
        {
            mSim = sim;
        }

        public string FirstName { get { return mSim.FirstName; } }
        public string FullName { get { return mSim.FullName; } }
        public bool IsFemale { get { return mSim.IsFemale; } }
        public string LastName { get { return mSim.LastName; } }

        public bool AdultOrBelow { get { return mSim.AdultOrBelow; } }
        public CASAgeGenderFlags Age { get { return mSim.Age; } }
        public bool Baby { get { return mSim.Baby; } }
        public IGenealogy CASGenealogy { get { return mSim.CASGenealogy; } set { mSim.CASGenealogy = value; } }
        public uint CelebrityLevel { get { return mSim.CelebrityLevel; } }
        public bool Child { get { return mSim.Child; } }
        public bool ChildOrAbove { get { return mSim.ChildOrAbove; } }
        public bool ChildOrBelow { get { return mSim.ChildOrBelow; } }
        public CASAgeGenderFlags Gender { get { return mSim.Gender; } }
        public WorldName HomeWorld { get { return mSim.HomeWorld; } }
        public bool IsADogSpecies { get { return mSim.IsADogSpecies; } }
        public bool IsCat { get { return mSim.IsCat; } }
        public bool IsCelebrity { get { return mSim.IsCelebrity; } }
        public bool IsContactable { get { return mSim.IsContactable; } }
        public bool IsDead { get { return mSim.IsDead; } }
        public bool IsFoal { get { return mSim.IsFoal; } }
        public bool IsFullSizeDog { get { return mSim.IsFullSizeDog; } }
        public bool IsGenie { get { return mSim.IsGenie; } }
        public bool IsHorse { get { return mSim.IsHorse; } }
        public bool IsHuman { get { return mSim.IsHuman; } }
        public bool IsKitten { get { return mSim.IsKitten; } }
        public bool IsLittleDog { get { return mSim.IsLittleDog; } }
        public bool IsMale { get { return mSim.IsMale; } }
        public bool IsMarried { get { return mSim.IsMarried; } }
        public bool IsMummy { get { return mSim.IsMummy; } }
        public bool IsPet { get { return mSim.IsPet; } }
        public bool IsPlayableGhost { get { return mSim.IsPlayableGhost; } }
        public bool IsPuppy { get { return mSim.IsPuppy; } }
        public bool IsPregnant { get { return mSim.IsPregnant; } }
        public bool IsServicePerson { get { return mSim.IsServicePerson; } }
        public bool IsStray { get { return mSim.IsStray; } }
        public bool IsUnicorn { get { return mSim.IsUnicorn; } }
        public bool IsVampire { get { return mSim.IsVampire; } }
        public bool IsFairy { get { return mSim.IsFairy; } }
        public bool IsWerewolf { get { return mSim.IsWerewolf; } }
        public bool IsWitch { get { return mSim.IsWitch; } }
        public ulong LotHomeId { get { return mSim.LotHomeId; } }
        public ulong SimDescriptionId { get { return mSim.SimDescriptionId; } }
        public CASAgeGenderFlags Species { get { return mSim.Species; } }
        public bool Teen { get { return mSim.Teen; } }
        public bool TeenOrAbove { get { return mSim.TeenOrAbove; } }
        public bool TeenOrBelow { get { return mSim.TeenOrBelow; } }
        public bool ToddlerOrBelow { get { return mSim.ToddlerOrBelow; } }
        public float YearsSinceLastAgeTransition { get { return mSim.YearsSinceLastAgeTransition; } }
        public bool YoungAdultOrAbove { get { return mSim.YoungAdultOrAbove; } }

        public int CountVisibleTraits()
        {
            return mSim.CountVisibleTraits();
        }

        public ThumbnailKey GetDeceasedThumbnailKey(ThumbnailSize size, int thumbIndex)
        {
            if (mSim is MiniSimDescription)
            {
                return GetThumbnailKey(size, thumbIndex);
            }
            else
            {
                return mSim.GetDeceasedThumbnailKey(size, thumbIndex);
            }
        }

        public IMiniRelationship GetMiniRelationship(IMiniSimDescription otherSim)
        {
            return mSim.GetMiniRelationship(otherSim);
        }

        public ThumbnailKey GetThumbnailKey(ThumbnailSize size, int thumbIndex)
        {
            return MiniSims.GetThumbnailKey(mSim, size, thumbIndex);
        }

        public bool HasConflictingTrait(ulong guid)
        {
            return mSim.HasConflictingTrait(guid);
        }

        public bool HasSameHomeLot(IMiniSimDescription desc)
        {
            return mSim.HasSameHomeLot(desc);
        }

        public bool HasTrait(ulong guid)
        {
            return mSim.HasTrait(guid);
        }

        public bool IsMemberOfMyHousehold(IMiniSimDescription desc)
        {
            return mSim.IsMemberOfMyHousehold(desc);
        }

        public void OnPickFromPanel(UIMouseEventArgs eventArgs, GameObjectHit gameObjectHit)
        {
            mSim.OnPickFromPanel(eventArgs, gameObjectHit);
        }

        public override string ToString()
        {
            return mSim.ToString();
        }

        public override bool Equals(object o)
        {
            IMiniSimDescription right = null;

            MiniSimDescriptionProxy proxy = o as MiniSimDescriptionProxy;
            if (proxy != null)
            {
                right = proxy.mSim;
            }
            else
            {
                right = o as IMiniSimDescription;
            }

            return mSim.Equals(right);
        }

        public override int GetHashCode()
        {
            return mSim.GetHashCode();
        }

        public bool IsAlien
        {
            get { return mSim.IsAlien; }
        }

        public bool IsFrankenstein
        {
            get { return mSim.IsFrankenstein; }
        }

        public ulong PartnerId
        {
            get { return mSim.PartnerId; }
        }

        public bool IsMermaid
        {
            get { return mSim.IsMermaid; }
        }


        public bool IsEP11Bot
        {
            get { return mSim.IsEP11Bot; }
        }

        public bool IsRobot
        {
            get { return mSim.IsRobot; }
        }
    }
}
