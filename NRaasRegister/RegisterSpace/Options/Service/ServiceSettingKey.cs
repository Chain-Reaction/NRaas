using NRaas.CommonSpace.Helpers;
using NRaas.RegisterSpace.Helpers;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using System;
using System.Collections.Generic;

namespace NRaas.RegisterSpace.Options.Service
{
    [Persistable]
    public class ServiceSettingKey : IPersistence
    {
        private ServiceType type;
        public CASAgeGenderFlags validAges;
        public int numInPool = 0;
        public int cost = 0;
        public bool reoccuring = false;

        [Persistable(false)]
        public ServiceSettingKey tuningDefault;
       
        public ServiceSettingKey()
        { }
        public ServiceSettingKey(Sims3.Gameplay.Services.Service service)
        {
            type = service.ServiceType;            
            validAges = Sims3.Gameplay.Services.ServiceNPCSpecifications.GetAppropriateAges(service.ServiceType.ToString());
            numInPool = service.Tuning.kMaxNumNPCsInPool;
            cost = service.Tuning.kCost;
            reoccuring = service.Tuning.kIsRecurrent;
        }
        public ServiceSettingKey(Sims3.Gameplay.Services.Service service, CASAgeGenderFlags flags, int poolSetting, int serviceCost, bool reoccur)
        {
            type = service.ServiceType;
            validAges |= flags;
            poolSetting = numInPool;
            cost = serviceCost;
            reoccuring = reoccur;

            tuningDefault = new ServiceSettingKey(service);
        }

        public bool ValidAge(CASAgeGenderFlags ages)
        {
            return (ages & validAges) != CASAgeGenderFlags.None;
        }

        public List<CASAgeGenderFlags> AgeSpeciesToList()
        {
            List<CASAgeGenderFlags> results = new List<CASAgeGenderFlags>();

            ServiceSettingKey source = this; // this.tuningDefault == null ? this : this.tuningDefault;

            foreach (CASAgeGenderFlags ageSpecies in Enum.GetValues(typeof(CASAgeGenderFlags)))
            {
                if ((source.validAges & ageSpecies) == ageSpecies)
                {
                    if (ageSpecies != CASAgeGenderFlags.None)
                    {
                        // I'm not sure why this is needed but it throws the list count off otherwise
                        results.Add(ageSpecies);
                    }
                }
            }

            return results;
        }

        public void SetSettings(Sims3.Gameplay.Services.Service service)
        {
            if (Register.Settings.serviceSettings.ContainsKey(service.ServiceType))
            {
                Register.Settings.serviceSettings[service.ServiceType] = this;
            }
            else
            {
                this.tuningDefault = new ServiceSettingKey(service);
                Register.Settings.serviceSettings.Add(service.ServiceType, this);
            }

            Register.InitDefaultServiceTunings();
            ServiceCleanup.Task.Perform();
            ServicePoolCleanup.Task.Perform();
        }

        public void Import(Persistence.Lookup settings)
        {            
        }

        public void Export(Persistence.Lookup settings)
        {            
        }

        public string PersistencePrefix
        {
            get { return type.ToString(); }
        }

    }
}
