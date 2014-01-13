using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;

namespace NRaas.RetunerSpace
{
    [Persistable]
    public abstract class SettingsKey : IPersistence
    {
        public readonly static Season sAllSeasons = Season.Spring | Season.Summer | Season.Fall | Season.Winter;

        public Vector2 mHours;

        public SettingsKey()
            : this(new Vector2(-1, 25))
        { }
        public SettingsKey(Vector2 hours)
        {
            mHours = hours;
        }

        public virtual bool IsActive
        {
            get
            {
                if (IsDefault)
                {
                    return true;
                }
                else
                {
                    return SimClock.IsTimeBetweenTimes(mHours.x, mHours.y);
                }
            }
        }

        public bool IsDefault
        {
            get
            {
                return ((mHours.x == -1) && (mHours.y == 25));
            }
        }

        public virtual string LocalizedName
        {
            get
            {
                string result = null;

                if (!IsDefault)
                {
                    result += Common.Localize("Hours:Name", false, new object[] { mHours.x, mHours.y });
                }

                return result;
            }
        }

        public virtual Season Season
        {
            get
            {
                return sAllSeasons;
            }
        }

        public override int GetHashCode()
        {
            return (int)(mHours.x * mHours.y);
        }

        public override bool Equals(object o)
        {
            SettingsKey key = o as SettingsKey;
            if (key == null) return false;

            return IsEqual(key);
        }

        public virtual bool IsEqual(SettingsKey key)
        {
            if (mHours != key.mHours) return false;

            return true;
        }

        public string PersistencePrefix
        {
            get { return null; }
        }

        public virtual void Export(Persistence.Lookup settings)
        {
            settings.Add("StartHour", mHours.x);
            settings.Add("EndHour", mHours.y);
        }

        public virtual void Import(Persistence.Lookup settings)
        {
            mHours.x = settings.GetFloat("StartHour", -1);
            mHours.y = settings.GetFloat("EndHour", 25);
        }

        public virtual string ToXMLString()
        {
            string result = null;

            if (!IsDefault)
            {
                result += Common.NewLine + "    <StartHour>" + mHours.x + "</StartHour>";
                result += Common.NewLine + "    <EndHour>" + mHours.y + "</EndHour>";
            }

            return result;
        }

        public override string ToString()
        {
            return ToXMLString();
        }
    }
}
