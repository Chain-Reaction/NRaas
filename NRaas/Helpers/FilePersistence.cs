using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace NRaas.CommonSpace.Helpers
{
    public class FilePersistence
    {
        public static readonly string sHeader = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";

        public static string ExportContents()
        {
            string text = Persistence.CreateExportString2("Settings");
            if (string.IsNullOrEmpty(text)) return null;

            return sHeader + text;
        }

        public static bool ExportToFile()
        {
            string contents = ExportContents();
            if (string.IsNullOrEmpty(contents)) return false;

            return ExportToFile(contents);
        }
        public static bool ExportToFile(string text)
        {
            string name = null;

            bool found = true;
            while (found)
            {
                name = StringInputDialog.Show(Common.Localize("ExportSettings:MenuName"), Common.Localize("ExportSettings:Prompt"), "Save Settings");
                if (string.IsNullOrEmpty(name)) return false;

                name = "NRaas.Settings." + name;

                BinModel.Singleton.PopulateExportBin();

                found = false;
                foreach (ExportBinContents contents in BinModel.Singleton.ExportBinContents)
                {
                    if (contents.HouseholdName == null) continue;

                    if (!contents.HouseholdName.Contains("NRaas.Settings.")) continue;

                    if (contents.HouseholdName == name)
                    {
                        SimpleMessageDialog.Show(Common.Localize("ExportSettings:MenuName"), Common.Localize("ExportSettings:Exists"));
                        found = true;
                        break;
                    }
                }
            }

            Household saveHouse = Household.Create();

            saveHouse.SetName(name);
            saveHouse.BioText = text;

            BinModel.Singleton.AddToExportBin(saveHouse);

            saveHouse.Destroy();

            SimpleMessageDialog.Show(Common.Localize("ExportSettings:MenuName"), Common.Localize("ExportSettings:Success"));
            return true;
        }

        public static bool ImportFromFile()
        {
            return ImportSettings(ExtractFromFile());
        }
        public static bool ImportFromTuning(string name)
        {
            return ImportSettings(ExtractFromTuning(name));
        }

        protected static bool ImportSettings(XmlElement element)
        {
            if (element == null) return false;

            if (Persistence.ImportSettings(element))
            {
                SimpleMessageDialog.Show(Common.Localize("ImportSettings:MenuName"), Common.Localize("ImportSettings:Success"));
                return true;
            }
            else
            {
                SimpleMessageDialog.Show(Common.Localize("ImportSettings:MenuName"), Common.Localize("ImportSettings:Error"));
                return false;
            }
        }

        public static XmlElement ExtractFromFile()
        {
            BinModel.Singleton.PopulateExportBin();

            List<SaveSetting> settings = new List<SaveSetting>();

            foreach (ExportBinContents contents in BinModel.Singleton.ExportBinContents)
            {
                if (contents.HouseholdName == null) continue;

                if (!contents.HouseholdName.Contains("NRaas.Settings.")) continue;

                settings.Add(new SaveSetting(contents.HouseholdName, contents.HouseholdBio));
            }

            if (settings.Count == 0)
            {
                SimpleMessageDialog.Show(Common.Localize("ImportSettings:MenuName"), Common.Localize("ImportSettings:Error"));
                return null;
            }

            SaveSetting selection = new CommonSelection<SaveSetting>(Common.Localize("ImportSettings:MenuName"), settings).SelectSingle();
            if (selection == null) return null;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(selection.mData);

            return doc.DocumentElement;
        }
        public static XmlElement ExtractFromTuning(string name)
        {
            XmlDocument document = Simulator.LoadXML(name);
            if (document == null) return null;

            return document.DocumentElement;
        }

        protected class SaveSetting : CommonOptionItem
        {
            public string mData;

            public SaveSetting (string name, string data)
                : base(name.Replace("NRaas.Settings.", ""))
            {
                mData = data;
            }

            public override string DisplayValue
            {
                get { return null; }
            }
        }
    }
}

