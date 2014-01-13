using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Objects.RabbitHoles.PrivateEyePoliceWorkTones;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public abstract class CareerToneScenario : SimScenario, IFormattedStoryScenario
    {
        string mTone = null;

        public CareerToneScenario(SimDescription sim)
            : base (sim)
        { }
        protected CareerToneScenario(CareerToneScenario scenario)
            : base (scenario)
        {
            mTone = scenario.mTone;
        }

        public virtual Manager GetFormattedStoryManager()
        {
            return StoryProgression.Main.Careers;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!base.Allow(sim)) return false;

            if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (sim.CreatedSim.InteractionQueue == null)
            {
                IncStat("No Queue");
                return false;
            }

            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            int elapsedCalendarDays = SimClock.ElapsedCalendarDays();

            RabbitHole.RabbitHoleInteraction<Sim, RabbitHole> work = Sim.CreatedSim.InteractionQueue.GetCurrentInteraction() as RabbitHole.RabbitHoleInteraction<Sim, RabbitHole>;
            if (work == null) return false;

            if (work.Actor == null) return false;

            if (work.mAvailableTones != null)
            {
                int index = 0;
                while (index < work.mAvailableTones.Count)
                {
                    if (work.mAvailableTones[index] == null)
                    {
                        work.mAvailableTones.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
            }

            List<InteractionToneDisplay> displayTones = work.AvailableTonesForDisplay();
            if (displayTones == null)
            {
                IncStat("No Choices");
                return false;
            }

            List<ITone> tones = new List<ITone>();

            foreach (InteractionToneDisplay tone in displayTones)
            {
                if (tone.InteractionTone == null) continue;

                if (tone.InteractionTone.ToString() == mTone) continue;

                if (!ManagerCareer.VerifyTone(tone.InteractionTone as CareerTone)) continue;

                tones.Add(tone.InteractionTone);
            }

            return ConfigureInteraction(work, tones);
        }

        public static void Add<Type>(List<ITone> list, List<ITone> tones) 
            where Type : class, ITone
        {
            if (tones == null) return;

            foreach (ITone displaytone in tones)
            {
                Type tone = displaytone as Type;
                if (tone != null)
                {
                    CareerTone careerTone = tone as CareerTone;
                    if (careerTone != null)
                    {
                        if (!ManagerCareer.VerifyTone(careerTone)) continue;
                    }

                    list.Add(tone as ITone);
                }
            }
        }

        protected static bool IsPracticeTone(ITone tone, SkillNames skill)
        {
            List<SkillNames> customTone = new List<SkillNames>();

            Assembly careerMod = Common.AssemblyCheck.FindAssembly("nraascareer");
            if (careerMod != null)
            {
                Type type = careerMod.GetType("NRaas.Gameplay.Tones.CareerToneEx");
                if (type != null)
                {
                    if (type.IsInstanceOfType(tone))
                    {
                        MethodInfo method = type.GetMethod("GetSkills");
                        if (method != null)
                        {
                            method.Invoke(tone, new object[] { customTone });
                        }
                    }
                }
            }

            if (skill == SkillNames.None)
            {
                if (customTone.Count > 0) return true;

                if (tone is ProSports.WorkOutInGym) return true;
                if (tone is LawEnforcement.WorkoutTone) return true;
                if (tone is Criminal.PracticeIllicitActivities) return true;
                if (tone is Journalism.PracticeWriting) return true;
                if (tone is Music.StudyMusicTheoryTone) return true;
                if (tone is Culinary.PracticeCookingTone) return true;
            }
            else
            {
                if (customTone.Contains(skill)) return true;

                switch (skill)
                {
                    case SkillNames.Athletic:
                        if (tone is ProSports.WorkOutInGym) return true;
                        if (tone is LawEnforcement.WorkoutTone) return true;
                        if (tone is Criminal.PracticeIllicitActivities) return true;
                        break;
                    case SkillNames.Writing:
                        if (tone is Journalism.PracticeWriting) return true;
                        break;
                    case SkillNames.Guitar:
                        if (tone is Music.StudyMusicTheoryTone) return true;
                        break;
                    case SkillNames.Cooking:
                        if (tone is Culinary.PracticeCookingTone) return true;
                        break;
                }
            }

            return false;
        }

        protected static void GatherChoices(Sim sim, List<ITone> allTones, List<ITone> tonechoices, List<SkillNames> skills)
        {
            foreach(Trait trait in sim.TraitManager.List)
            {
                switch (trait.Guid)
                {
                    case TraitNames.Ambitious:
                        Add<WorkHardTone>(tonechoices, allTones);
                        Add<School.WorkOnLateHomeworkTone>(tonechoices, allTones);
                        Add<Business.HoldMeetings>(tonechoices, allTones);

                        Add<MeetCoworkersTone>(tonechoices, allTones);

                        Add<HangWithCoworkersTone>(tonechoices, allTones);
                        Add<SuckUpToBossTone>(tonechoices, allTones);

                        Add<PrivateEyeMeetCopsTone>(tonechoices, allTones);
                        Add<PrivateEyeHangWithCopsTone>(tonechoices, allTones);

                        Add<PrivateEyeWorkHardTone>(tonechoices, allTones);

                        skills.Add(SkillNames.None);
                        break;

                    case TraitNames.Athletic:
                        Add<ProSports.PrepareForGame>(tonechoices, allTones);
                        Add<Medical.PlayGolfTone>(tonechoices, allTones);

                        skills.Add(SkillNames.Athletic);
                        break;
                    case TraitNames.Artistic:
                        skills.Add(SkillNames.Writing);
                        skills.Add(SkillNames.Guitar);
                        skills.Add(SkillNames.Painting);
                        break;
                    case TraitNames.BookWorm:
                        skills.Add(SkillNames.Writing);

                        Add<School.WorkOnLateHomeworkTone>(tonechoices, allTones);
                        break;
                    case TraitNames.Charismatic:
                        Add<MeetCoworkersTone>(tonechoices, allTones);
                        Add<HangWithCoworkersTone>(tonechoices, allTones);
                        Add<SuckUpToBossTone>(tonechoices, allTones);
                        Add<Business.HoldMeetings>(tonechoices, allTones);

                        Add<PrivateEyeMeetCopsTone>(tonechoices, allTones);
                        Add<PrivateEyeHangWithCopsTone>(tonechoices, allTones);

                        skills.Add(SkillNames.Charisma);
                        break;
                    case TraitNames.Brave:
                        Add<TakeARiskTone>(tonechoices, allTones);
                        Add<LawEnforcement.BuildCaseTone>(tonechoices, allTones);
                        break;
                    case TraitNames.Daredevil:
                        Add<TakeARiskTone>(tonechoices, allTones);
                        Add<LawEnforcement.BuildCaseTone>(tonechoices, allTones);
                        break;
                    case TraitNames.Evil:
                        Add<Criminal.DoASideJob>(tonechoices, allTones);
                        Add<Criminal.PracticeIllicitActivities>(tonechoices, allTones);

                        Add<PrivateEyePretendToBePsychic>(tonechoices, allTones);
                        break;
                    case TraitNames.Genius:
                        Add<Science.DoIndependentExperiment>(tonechoices, allTones);
                        Add<LawEnforcement.BuildCaseTone>(tonechoices, allTones);
                        Add<School.WorkOnLateHomeworkTone>(tonechoices, allTones);

                        Add<PrivateEyeWorkHardTone>(tonechoices, allTones);
                        Add<PrivateEyeGainLogicTone>(tonechoices, allTones);

                        skills.Add(SkillNames.None);
                        break;
                    case TraitNames.Loner:
                        skills.Add(SkillNames.None);
                        break;
                    case TraitNames.NaturalCook:
                        skills.Add(SkillNames.Cooking);
                        break;
                    case TraitNames.Virtuoso:
                        skills.Add(SkillNames.Guitar);
                        break;
                    case TraitNames.Schmoozer:
                        Add<MeetCoworkersTone>(tonechoices, allTones);
                        Add<HangWithCoworkersTone>(tonechoices, allTones);
                        Add<SuckUpToBossTone>(tonechoices, allTones);
                        Add<Business.HoldMeetings>(tonechoices, allTones);

                        Add<PrivateEyeMeetCopsTone>(tonechoices, allTones);
                        Add<PrivateEyeHangWithCopsTone>(tonechoices, allTones);
                        break;
                    case TraitNames.Friendly:
                        Add<MeetCoworkersTone>(tonechoices, allTones);
                        Add<HangWithCoworkersTone>(tonechoices, allTones);

                        Add<PrivateEyeMeetCopsTone>(tonechoices, allTones);
                        Add<PrivateEyeHangWithCopsTone>(tonechoices, allTones);
                        break;
                    case TraitNames.GoodSenseOfHumor:
                        Add<MeetCoworkersTone>(tonechoices, allTones);
                        Add<HangWithCoworkersTone>(tonechoices, allTones);

                        Add<PrivateEyeMeetCopsTone>(tonechoices, allTones);
                        Add<PrivateEyeHangWithCopsTone>(tonechoices, allTones);
                        break;
                    case TraitNames.Perfectionist:
                        Add<WorkHardTone>(tonechoices, allTones);

                        Add<PrivateEyeWorkHardTone>(tonechoices, allTones);

                        break;
                    case TraitNames.Workaholic:
                        skills.Add(SkillNames.None);

                        Add<WorkHardTone>(tonechoices, allTones);
                        Add<Business.HoldMeetings>(tonechoices, allTones);
                        Add<School.WorkOnLateHomeworkTone>(tonechoices, allTones);

                        Add<PrivateEyeWorkHardTone>(tonechoices, allTones);

                        break;
                    case TraitNames.Childish:
                        Add<TakeItEasyTone>(tonechoices, allTones);

                        Add<PrivateEyePretendToBePsychic>(tonechoices, allTones);

                        break;
                    case TraitNames.CouchPotato:
                    case TraitNames.Mooch:
                        Add<TakeItEasyTone>(tonechoices, allTones);
                        Add<SleepAtWorkTone>(tonechoices, allTones);
                        Add<Medical.SleepInReadyRoomTone>(tonechoices, allTones);
                        Add<School.SleepInClassTone>(tonechoices, allTones);
                        break;
                    case TraitNames.Insane:
                    case TraitNames.Inappropriate:
                    case TraitNames.MeanSpirited:
                        Add<PrivateEyePretendToBePsychic>(tonechoices, allTones);
                        break;
                }
            }
        }

        protected static void GatherSkillTones(List<SkillNames> skills, List<ITone> allTones, List<ITone> tonechoices)
        {
            foreach (SkillNames skill in skills)
            {
                foreach (ITone choice in allTones)
                {
                    if (IsPracticeTone(choice, skill))
                    {
                        tonechoices.Add(choice);
                    }
                }
            }
        }

        public static bool SetTone<TActor,TTarget>(InteractionInstance<TActor, TTarget> interaction, List<ITone> allTones, ref string toneName)
             where TActor: class, IActor where TTarget: class, IGameObject
        {
            List<ITone> tonechoices = new List<ITone>();

            List<SkillNames> skills = new List<SkillNames>();

            GatherChoices(interaction.InstanceActor, allTones, tonechoices, skills);

            GatherSkillTones(skills, allTones, tonechoices);

            ITone tone = null;

            if (tonechoices.Count > 0)
            {
                tone = RandomUtil.GetRandomObjectFromList(tonechoices);
            }

            if ((tone == null) && (allTones.Count > 0))
            {
                tone = RandomUtil.GetRandomObjectFromList(allTones);
            }

            interaction.CurrentITone = null;
            if (tone != null)
            {
                try
                {
                    interaction.CurrentITone = tone;

                    toneName = tone.Name();
                    return true;
                }
                catch (Exception e)
                {
                    Common.DebugException(tone.Name(), e);
                }
            }
            return false;
        }

        protected virtual bool ConfigureInteraction(RabbitHole.RabbitHoleInteraction<Sim, RabbitHole> wk, List<ITone> allTones)
        {
            string jobimage = null;

            if (wk is GoToSchoolInRabbitHole)
            {
                if (wk.Actor.School != null)
                {
                    jobimage = wk.Actor.School.CareerIconColored;
                }
            }
            else
            {
                if (wk.Actor.Occupation != null)
                {
                    jobimage = wk.Actor.Occupation.CareerIconColored;
                }
            }

            return SetTone(wk, allTones, ref mTone);
        }

        protected override ManagerStory.Story PrintFormattedStory(StoryProgressionObject manager, string text, string summaryKey, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Careers;
            }

            if ((mTone != null) && (DebuggingEnabled))
            {
                text = "(D)" + UnlocalizedName + ": " + mTone;
            }

            return base.PrintFormattedStory(manager, text, summaryKey, parameters, extended, logging);
        }
    }
}
