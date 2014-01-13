using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RetunerSpace.Options.ITUN
{
    public class ObjectSeasonOption : InteractionOptionList<IObjectOption, GameObject>, IObjectOption
    {
        SettingsKey mSeason;

        GameObject mTarget;

        public ObjectSeasonOption(SettingsKey season, GameObject target)
            : base(season.LocalizedName)
        {
            mSeason = season;
            mTarget = target;
        }

        public override string GetTitlePrefix()
        {
            return null;
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public static List<IObjectOption> GetOptions(GameObject target)
        {
            List<IObjectOption> results = new List<IObjectOption>();

            if (target != null)
            {
                Dictionary<InteractionTuning, bool> lookup = new Dictionary<InteractionTuning, bool>();

                Dictionary<string, ListingOption> nameLookup = new Dictionary<string, ListingOption>();

                List<InteractionObjectPair> interactions = target.GetAllInteractionsForActor(Sim.ActiveActor);
                interactions.AddRange(target.GetAllInventoryInteractionsForActor(Sim.ActiveActor));

                foreach (InteractionObjectPair pair in interactions)
                {
                    InteractionTuning tuning = pair.Tuning;
                    if (tuning == null) continue;

                    if (lookup.ContainsKey(tuning)) continue;
                    lookup.Add(tuning, true);

                    string name = tuning.ShortInteractionName;
                    try
                    {
                        InteractionInstanceParameters parameters = new InteractionInstanceParameters(pair, Sim.ActiveActor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true);
                        name = pair.InteractionDefinition.GetInteractionName(ref parameters);

                        string[] path = pair.InteractionDefinition.GetPath(Sim.ActiveActor.IsFemale);

                        foreach (string value in path)
                        {
                            name = value + " \\ " + name;
                        }
                    }
                    catch
                    { }

                    ListingOption option = new ListingOption(name, tuning);

                    ListingOption original;
                    if (nameLookup.TryGetValue(option.Name, out original))
                    {
                        option.AppendKey();

                        original.AppendKey();
                    }
                    else
                    {
                        nameLookup.Add(option.Name, option);
                    }

                    results.Add(option);
                }
            }

            return results;
        }

        public override List<IObjectOption> GetOptions()
        {
            return GetOptions(mTarget);
        }

        public override OptionResult Perform(GameHitParameters<GameObject> parameters)
        {
            using (Retuner.ActiveSettingsToggle activeSeason = new Retuner.ActiveSettingsToggle(mSeason))
            {
                return base.Perform(parameters);
            }
        }
    }
}
