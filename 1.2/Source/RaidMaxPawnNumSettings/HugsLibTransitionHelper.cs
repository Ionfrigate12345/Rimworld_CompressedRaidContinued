using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using Verse;

namespace CompressedRaid
{
    [StaticConstructorOnStartup]
    public class HugsLibTransitionHelper
    {
        public const string MOD_NODE_KEY = "CompressedRaid";
        private const string HUGSLIB_FILE_PATH = @"HugsLib\ModSettings.xml";
        private const string TRANSITION_DIR_NAME = MOD_NODE_KEY;
        private static string GetHugslibFullPath() => Path.GetFullPath(Path.Combine(GenFilePaths.SaveDataFolderPath, HUGSLIB_FILE_PATH));
        private static bool m_XMLLoadSucceeded = false;
        private static Dictionary<string, string> m_HugsLibSettings = new Dictionary<string, string>();

        public static bool HugsLibReady() => m_XMLLoadSucceeded;

        #region not used
        //private const string TRANSITION_FILE_NAME = "TransitionComplete.lockfile";
        //private static string GetTransitionDirFullPath() => Path.GetFullPath(Path.Combine(GenFilePaths.SaveDataFolderPath, TRANSITION_DIR_NAME));
        //private static string GetTransitionFileFullPath() => Path.GetFullPath(Path.Combine(GenFilePaths.SaveDataFolderPath, TRANSITION_DIR_NAME, TRANSITION_FILE_NAME));
        //private const string KEY_FILE_MESSAGE = "This file is a flag file indicating that the transition of the configuration data from HugsLib is complete. Please do not delete it.";

        //public static bool NoTransitionCompleted()
        //{
        //    LoadHugsLibModSettings();
        //    if (!m_XMLLoadSucceeded)
        //    {
        //        return false;
        //    }

        //    return !File.Exists(GetTransitionFileFullPath());
        //}

        //public static void CreateTransitionKeyFile()
        //{
        //    string dir = GetTransitionDirFullPath();
        //    if (!Directory.Exists(dir))
        //    {
        //        Directory.CreateDirectory(dir);
        //    }
        //    using (StreamWriter sw = new StreamWriter(GetTransitionFileFullPath(), false, Encoding.Default))
        //    {
        //        sw.Write(KEY_FILE_MESSAGE);
        //    }
        //}

        //public static bool CheckHugsLibModSettings()
        //{
        //    int hugsLibDataCount = 0;
        //    XmlDocument deserializeXmlData;
        //    XmlNode compressedRaidNode;
        //    try
        //    {
        //        string xmlPath = GetHugslibFullPath();
        //        if (!File.Exists(xmlPath))
        //        {
        //            return false;
        //        }

        //        deserializeXmlData = new XmlDocument();
        //        deserializeXmlData.Load(xmlPath);
        //        compressedRaidNode = deserializeXmlData?.DocumentElement?.SelectSingleNode(MOD_NODE_KEY);

        //        if (compressedRaidNode == null)
        //        {
        //            return false;
        //        }

        //        hugsLibDataCount = compressedRaidNode.SelectNodes("./*").Count;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //    finally
        //    {
        //        compressedRaidNode = null;
        //        deserializeXmlData = null;
        //    }
        //    return hugsLibDataCount > 0;
        //}
        #endregion

        public static void LoadHugsLibModSettings()
        {
            m_HugsLibSettings.Clear();
            XmlDocument deserializeXmlData;
            XmlNode compressedRaidNode;
            try
            {
                string xmlPath = GetHugslibFullPath();
                if (!File.Exists(xmlPath))
                {
                    LoadFailed();
                }

                deserializeXmlData = new XmlDocument();
                deserializeXmlData.Load(xmlPath);
                compressedRaidNode = deserializeXmlData?.DocumentElement?.SelectSingleNode(MOD_NODE_KEY);

                if (compressedRaidNode == null)
                {
                    LoadFailed();
                }
                foreach (XmlNode node in compressedRaidNode.SelectNodes("./*"))
                {
                    m_HugsLibSettings[node.Name] = node.InnerText;
                }

                if (m_HugsLibSettings.Any())
                {
                    LoadSucceeded();
                }
            }
            catch
            {
                LoadFailed();
            }
            finally
            {
                compressedRaidNode = null;
                deserializeXmlData = null;
            }
        }

        delegate bool TryParse<T>(string text, out T value) where T : struct;
        private static void NomalizationValue<T>(string variableName, TryParse<T> parse, Func<T, T, T, T> clamp, T min, T max)
            where T : struct
        {
            if (m_HugsLibSettings.TryGetValue(variableName, out string text))
            {
                if (parse(text, out T val))
                {
                    m_HugsLibSettings[variableName] = clamp(val, min, max).ToString();
                }
            }
        }

        private static void LoadSucceeded()
        {
            m_XMLLoadSucceeded = true;

            ////値範囲メモ（HugsLibからの移行時に処理する）
            ////Common
            //maxRaidPawnsCount	1-1000
            //gainFactorMult	0-100f
            //gainFactorMultRaidFriendly	0-100f
            NomalizationValue<int>("maxRaidPawnsCount", int.TryParse, Mathf.Clamp, 1, 1000);
            NomalizationValue<float>("gainFactorMult", float.TryParse, Mathf.Clamp, 0f, 100f);
            NomalizationValue<float>("gainFactorMultRaidFriendly", float.TryParse, Mathf.Clamp, 0f, 100f);

            ////Main
            //gainFactorMaxPain 0-1f
            //gainFactorMaxPawnTrapSpringChance 0-1f
            //gainFactorMult系	0-10f
            //gainFactorMax系	0-10f
            NomalizationValue<float>("gainFactorMultPain", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMultArmorRating_Blunt", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMultArmorRating_Sharp", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMultArmorRating_Heat", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMultMeleeDodgeChance", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMultMeleeHitChance", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMultMoveSpeed", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMultShootingAccuracyPawn", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMultPawnTrapSpringChance", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMaxPain", float.TryParse, Mathf.Clamp, 0f, 1f);
            NomalizationValue<float>("gainFactorMaxArmorRating_Blunt", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMaxArmorRating_Sharp", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMaxArmorRating_Heat", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMaxMeleeDodgeChance", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMaxMeleeHitChance", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMaxMoveSpeed", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMaxShootingAccuracyPawn", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMaxPawnTrapSpringChance", float.TryParse, Mathf.Clamp, 0f, 1f);
            NomalizationValue<float>("gainFactorMultCapacitySight", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMultCapacityMoving", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMultCapacityHearing", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMultCapacityManipulation", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMultCapacityMetabolism", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMultCapacityConsciousness", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMultCapacityBloodFiltration", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMultCapacityBloodPumping", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMultCapacityBreathing", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMaxCapacitySight", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMaxCapacityMoving", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMaxCapacityHearing", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMaxCapacityManipulation", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMaxCapacityMetabolism", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMaxCapacityConsciousness", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMaxCapacityBloodFiltration", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMaxCapacityBloodPumping", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<float>("gainFactorMaxCapacityBreathing", float.TryParse, Mathf.Clamp, 0f, 10f);

            ////AddBionics
            //addBionicsChanceFactor 0-10f
            //addBionicsMaxNumber 1-100
            NomalizationValue<float>("addBionicsChanceFactor", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<int>("addBionicsMaxNumber", int.TryParse, Mathf.Clamp, 1, 100);

            ////RefineGear
            //refineGearChanceFactor 0-10f
            NomalizationValue<float>("refineGearChanceFactor", float.TryParse, Mathf.Clamp, 0f, 10f);

            ////AddDrugEffects
            //addDrugChanceFactor   0-10f
            //addDrugMaxNumber	1-100
            NomalizationValue<float>("addDrugChanceFactor", float.TryParse, Mathf.Clamp, 0f, 10f);
            NomalizationValue<int>("addDrugMaxNumber", int.TryParse, Mathf.Clamp, 1, 100);


            if (m_HugsLibSettings.TryGetValue("enableExclusive", out string enableExclusiveText))
            {
                if (bool.TryParse(enableExclusiveText, out bool enableExclusive))
                {
                    if (enableExclusive)
                    {
                        bool optionsAny = false;
                        bool work;
                        if (m_HugsLibSettings.TryGetValue("enableAddBionicsOption", out string enableAddBionicsOptionText))
                        {
                            if (bool.TryParse(enableExclusiveText, out work))
                            {
                                optionsAny |= work;
                            }
                        }
                        if (!optionsAny && m_HugsLibSettings.TryGetValue("enableRefineGearOption", out string enableRefineGearOptionText))
                        {
                            if (bool.TryParse(enableRefineGearOptionText, out work))
                            {
                                optionsAny |= work;
                            }
                        }
                        if (!optionsAny && m_HugsLibSettings.TryGetValue("enableAddDrugOption", out string enableAddDrugOptionText))
                        {
                            if (bool.TryParse(enableAddDrugOptionText, out work))
                            {
                                optionsAny |= work;
                            }
                        }
                        if (optionsAny)
                        {
                            m_HugsLibSettings["enableMainEnhance"] = false.ToString();
                        }
                    }
                }
            }
        }

        private static void LoadFailed()
        {
            m_XMLLoadSucceeded = false;
        }

        public static string GetValue(string key)
        {
            if (m_XMLLoadSucceeded)
            {
                if (m_HugsLibSettings.TryGetValue(key, out string value))
                {
                    return value;
                }
            }
            return null;
        }

    }
}
