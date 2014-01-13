using Sims3.Gameplay;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public enum LifetimeWant : uint
    {
        None = 0,

        /* Handled */
        BecomeAMasterThief = 902904880,
        HitMovieComposer = 506114036 ,
        TheEmperorOfEvil = 4194242280 ,
        BecomeACreatureRobotCrossBreeder = 4227566771 ,
        LeaderOfTheFreeWorld  = 4175236640 ,
        BecomeAnAstronaut = 992169064 ,
        CEOOfAMegaCorporation = 268290160 ,
        WorldRenownedSurgeon = 198603941 ,
        BecomeASuperstarAthlete = 235178832 ,
        RockStar = 782846499 ,
        StarNewsAnchor = 1695049514 ,
        ForensicSpecialistDynamicDNAProfiler = 962996558 ,
        InternationalSuperSpy = 2954453247 ,
        JackOfAllTrades = 2212165128 ,
        BottomlessNectarCellar = 1665285010 ,
        SwimmingInCash = 3609988628 ,
        CelebratedFiveStarChef = 2297166653 ,
        LivingInTheLapOfLuxury = 2004388412 ,
        SuperPopular = 3034414675 ,
        PresentingThePerfectPrivateAquarium = 3074810660 ,
        SurroundedByFamily = 2220612846,
        ProfessionalAuthor = 39298250,//(Royalties)
        Heartbreaker = 725721958,//(Multiple Partners)
        GoldDigger = 1505354317,//(Widower)
        ChessLegend = 2287246491,//(Top Chess champion)
        MartialArtsMaster = 3638862807,//(Top Spar)
        ParanormalProfiteer = 160825286,//(Top of Ghost Hunter)
        FashionPhenomenon = 161283248 ,//(Top of Stylist Career)
        FilmDirector = 171540745,
        FileActor = 171540746,
        MasterRomancer = 171540815,

        /* Requires User Intervention */
        ThePerfectGarden = 3631643856 ,
        WorldClassGallery = 2289029705 ,
        PrivateMuseum = 717200158 ,
        MonsterMaker = 153251944 ,
        SeasonedTraveler = 2420072933 ,
        PervasivePrivateEye = 153251936 ,
        GreatExplorer = 866663208 ,
        HomeDesignHotshot = 153251931 ,
        Zoologist = 191068915, // Collect 20 minor pets
         
        /* Skill based */
        PerfectMindPerfectBody = 1457442002 , //(Logic, Athletic)
        TheTinkerer = 797244627 ,//(Handiness, Logic)
        GoldenTongueGoldenFingers = 2844103135 ,//(Charisma, Guitar)
        DescendantOfDaVinci = 153401271 ,//(Sculpting, Inventing, Painting)
        PhysicalPerfection = 2443924614 ,//(Martial Arts, Athletic)
        RenaissanceSim = 2281530272 ,//(Any Three)
        MasterOfTheArts = 1432152990 ,//(Guitar, Painting)
        IllustriousAuthor = 3588921655 ,//(Writing, Painting)
        Visionary = 1802509420 ,//(Painting and Photography)
        MasterOfAllInstruments = 171384826, //(Guitar,Drums,Piano,Bass)
        MasterBartender = 171369858,//(Bartending)
        Jockey = 191068912, // Level 10 Riding Skill

        /* Possible */
        TheCulinaryLibrarian = 2393916713 ,//(All recipes)
        FirefighterSuperHero = 153251928 ,//(Rescue Sims)
        PossessionIsNineTenthsOfTheLaw = 153251938, //(Steal stuff)
        LifestyleOfRichAndFamous = 169226867, // Level 5 Celeb and lots of money

        AnimalRescuer = 191068913,  // Adopt 6 Strays
        ArkBuilder = 191068914, // Two of each pets
        CanineCompanion = 191068911, // Friends with 15 dogs
        CatHerder = 191068910, // Friend with 15 cats
        FairyTaleFinder = 191068916, // Adopt a Unicorn

        DeepSeaDiver = 256311307, // Max Diving Skill, money from collectables
        ResortEmpire = 256311299, // Five Star Resort
        GrandExplorer = 256311295, // Own all hidden islands
        SeasideSavior = 256311289, // Max Lifeguard

        SocialGroupie = 203587206, // Max influence skills
        BlogArtist = 203587100, // Five Star Blogger
        ScientificSpecialist = 203587091, // Level 10 Science skill, Science/Medical/Business career

        MagicMakeover = 203587040, // Grant Inner Beauty
        MysticHealer = 203587039, // Cure sims
        ZombieMaker = 203587038, // Make zombies
        TurnTheTown = 203587020, // Make Vampires
        LeaderOfThePack = 203586998, // Make Werewolves
        LightMagic = 203586979, // Cast charms
        DarkMagic = 203586978, // Cast curses
        Trickster = 203586963, // Trick sims
        AlchemyArtisan = 203586953, // Level 10 Spellcraft, Use Alchemy potions
        FortuneTellerMystic = 203586899, // Level 10 Fortune Teller, Mystic branch
        FortuneTellerScam = 203586898, // Level 10 Fortune Teller, Scam Artist branch
        GreenerGardens = 203586960, // Level 10 Gardening, Cast Bloom

        MasterAcrobat = 207663733, // Level 10 Acrobat
        MasterMagician = 207663732, // Level 10 Magician
        VocalLegend = 207663731, // Level 10 Singer

        MajorMaster = 203587192, // Earn degrees
        PerfectStudent = 203587191, // Perfect GPA        
        StreetCredible = 203587078, // Level 10 Street Art, Masterpiece Murals

        VideoGameDesigner = 203587204, // Max Video Game Career

        HighTechCollector = 203587374, // Has all the new objects, and Level 10 Advanced Tech Skill
        MoreOfAMachine = 203587375, // Level 10 Bot Building Skill
        MadeTheMostOfMyTime = 203587376, // Visited Dystopia, Utopia, and has a Statue Record
    }

    public class LifetimeWants
    {
        public static bool SetLifetimeWant(SimDescription me)
        {
            List<IInitialMajorWish> topMajorDreamMatches = DreamsAndPromisesManager.GetTopMajorDreamMatches(me);
            List<IInitialMajorWish> allMajorDreamMatches = DreamsAndPromisesManager.GetAllMajorDreamMatches(me);
            if ((topMajorDreamMatches.Count > 0x0) || (allMajorDreamMatches.Count > 0x0))
            {
                uint oldLifetimeWish = me.LifetimeWish;
                uint newLifetimeWish = LifetimeWishSelectionDialog.Show(me, Localization.LocalizeString("Ui/Caption/LifetimeWishSelectionDialog:InitialCaption", new object[0x0]), topMajorDreamMatches, allMajorDreamMatches, true);
                if ((oldLifetimeWish != newLifetimeWish) && (newLifetimeWish != 0x0))
                {
                    me.HasCompletedLifetimeWish = false;
                    me.LifetimeWish = newLifetimeWish;

                    if (me.CreatedSim.DreamsAndPromisesManager != null)
                    {
                        me.CreatedSim.DreamsAndPromisesManager.TryAddLifetimeWish();
                        return true;
                    }
                }
            }

            return false;
        }

        public static string GetName(SimDescription sim)
        {
            DreamNodeInstance instance = null;
            DreamsAndPromisesManager.sMajorWishes.TryGetValue(sim.LifetimeWish, out instance);

            string name = null;

            if (instance != null)
            {
                string sKey = instance.MaleNameKey;
                if (sim.IsFemale)
                {
                    sKey = instance.FemaleNameKey;
                }

                if (!Localization.GetLocalizedString(sKey, out name))
                {
                    name = instance.GetMajorWishName(sim);
                }

                if ((sim.CreatedSim != null) && (sim.CreatedSim.DreamsAndPromisesManager != null))
                {
                    ActiveDreamNode node = sim.CreatedSim.DreamsAndPromisesManager.LifetimeWishNode;
                    if (node != null)
                    {
                        name = node.Name;
                    }
                }
            }

            return name;
        }
    }
}

