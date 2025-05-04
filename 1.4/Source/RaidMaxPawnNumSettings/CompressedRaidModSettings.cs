using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;

namespace CompressedRaid
{
    [StaticConstructorOnStartup]
    public class CompressedRaidModSettings : ModSettings
    {
        public void InitVariablesFromHugsLib(bool merge)
        {
            if (merge)
            {
                SetHugsLibValue<float>("chanceOfCompression", float.TryParse, ref this.chanceOfCompression, ref UIUtility.NumericBuffer.chanceOfCompression);
                SetHugsLibValue<bool>("allowMechanoids", bool.TryParse, ref this.allowMechanoids);
                SetHugsLibValue<bool>("allowInsectoids", bool.TryParse, ref this.allowInsectoids);
                SetHugsLibValue<int>("maxRaidPawnsCount", int.TryParse, ref this.maxRaidPawnsCount, ref UIUtility.NumericBuffer.maxRaidPawnsCount);
                SetHugsLibValue<float>("gainFactorMult", float.TryParse, ref this.gainFactorMult, ref UIUtility.NumericBuffer.gainFactorMult);
                SetHugsLibValue<float>("chanceOfEnhancement", float.TryParse, ref this.chanceOfEnhancement, ref UIUtility.NumericBuffer.chanceOfEnhancement);
                SetHugsLibValue<bool>("calcBaseMultByEnhanced", bool.TryParse, ref this.calcBaseMultByEnhanced);
                SetHugsLibValue<bool>("allowRaidFriendly", bool.TryParse, ref this.allowRaidFriendly);
                SetHugsLibValue<bool>("allowManhunters", bool.TryParse, ref this.allowManhunters);
                SetHugsLibValue<bool>("displayMessage", bool.TryParse, ref this.displayMessage);
                SetHugsLibValue<float>("gainFactorMultRaidFriendly", float.TryParse, ref this.gainFactorMultRaidFriendly, ref UIUtility.NumericBuffer.gainFactorMultRaidFriendly);
                SetHugsLibValue<bool>("enableMainEnhance", bool.TryParse, ref this.enableMainEnhance);
                SetHugsLibValue<float>("gainFactorMultPain", float.TryParse, ref this.gainFactorMultPain, ref UIUtility.NumericBuffer.gainFactorMultPain);
                SetHugsLibValue<float>("gainFactorMultArmorRating_Blunt", float.TryParse, ref this.gainFactorMultArmorRating_Blunt, ref UIUtility.NumericBuffer.gainFactorMultArmorRating_Blunt);
                SetHugsLibValue<float>("gainFactorMultArmorRating_Sharp", float.TryParse, ref this.gainFactorMultArmorRating_Sharp, ref UIUtility.NumericBuffer.gainFactorMultArmorRating_Sharp);
                SetHugsLibValue<float>("gainFactorMultArmorRating_Heat", float.TryParse, ref this.gainFactorMultArmorRating_Heat, ref UIUtility.NumericBuffer.gainFactorMultArmorRating_Heat);
                SetHugsLibValue<float>("gainFactorMultMeleeDodgeChance", float.TryParse, ref this.gainFactorMultMeleeDodgeChance, ref UIUtility.NumericBuffer.gainFactorMultMeleeDodgeChance);
                SetHugsLibValue<float>("gainFactorMultMeleeHitChance", float.TryParse, ref this.gainFactorMultMeleeHitChance, ref UIUtility.NumericBuffer.gainFactorMultMeleeHitChance);
                SetHugsLibValue<float>("gainFactorMultMoveSpeed", float.TryParse, ref this.gainFactorMultMoveSpeed, ref UIUtility.NumericBuffer.gainFactorMultMoveSpeed);
                SetHugsLibValue<float>("gainFactorMultShootingAccuracyPawn", float.TryParse, ref this.gainFactorMultShootingAccuracyPawn, ref UIUtility.NumericBuffer.gainFactorMultShootingAccuracyPawn);
                SetHugsLibValue<float>("gainFactorMultPawnTrapSpringChance", float.TryParse, ref this.gainFactorMultPawnTrapSpringChance, ref UIUtility.NumericBuffer.gainFactorMultPawnTrapSpringChance);
                SetHugsLibValue<float>("gainFactorMaxPain", float.TryParse, ref this.gainFactorMaxPain, ref UIUtility.NumericBuffer.gainFactorMaxPain);
                SetHugsLibValue<float>("gainFactorMaxArmorRating_Blunt", float.TryParse, ref this.gainFactorMaxArmorRating_Blunt, ref UIUtility.NumericBuffer.gainFactorMaxArmorRating_Blunt);
                SetHugsLibValue<float>("gainFactorMaxArmorRating_Sharp", float.TryParse, ref this.gainFactorMaxArmorRating_Sharp, ref UIUtility.NumericBuffer.gainFactorMaxArmorRating_Sharp);
                SetHugsLibValue<float>("gainFactorMaxArmorRating_Heat", float.TryParse, ref this.gainFactorMaxArmorRating_Heat, ref UIUtility.NumericBuffer.gainFactorMaxArmorRating_Heat);
                SetHugsLibValue<float>("gainFactorMaxMeleeDodgeChance", float.TryParse, ref this.gainFactorMaxMeleeDodgeChance, ref UIUtility.NumericBuffer.gainFactorMaxMeleeDodgeChance);
                SetHugsLibValue<float>("gainFactorMaxMeleeHitChance", float.TryParse, ref this.gainFactorMaxMeleeHitChance, ref UIUtility.NumericBuffer.gainFactorMaxMeleeHitChance);
                SetHugsLibValue<float>("gainFactorMaxMoveSpeed", float.TryParse, ref this.gainFactorMaxMoveSpeed, ref UIUtility.NumericBuffer.gainFactorMaxMoveSpeed);
                SetHugsLibValue<float>("gainFactorMaxShootingAccuracyPawn", float.TryParse, ref this.gainFactorMaxShootingAccuracyPawn, ref UIUtility.NumericBuffer.gainFactorMaxShootingAccuracyPawn);
                SetHugsLibValue<float>("gainFactorMaxPawnTrapSpringChance", float.TryParse, ref this.gainFactorMaxPawnTrapSpringChance, ref UIUtility.NumericBuffer.gainFactorMaxPawnTrapSpringChance);
                SetHugsLibValue<float>("gainFactorMultCapacitySight", float.TryParse, ref this.gainFactorMultCapacitySight, ref UIUtility.NumericBuffer.gainFactorMultCapacitySight);
                SetHugsLibValue<float>("gainFactorMultCapacityMoving", float.TryParse, ref this.gainFactorMultCapacityMoving, ref UIUtility.NumericBuffer.gainFactorMultCapacityMoving);
                SetHugsLibValue<float>("gainFactorMultCapacityHearing", float.TryParse, ref this.gainFactorMultCapacityHearing, ref UIUtility.NumericBuffer.gainFactorMultCapacityHearing);
                SetHugsLibValue<float>("gainFactorMultCapacityManipulation", float.TryParse, ref this.gainFactorMultCapacityManipulation, ref UIUtility.NumericBuffer.gainFactorMultCapacityManipulation);
                SetHugsLibValue<float>("gainFactorMultCapacityMetabolism", float.TryParse, ref this.gainFactorMultCapacityMetabolism, ref UIUtility.NumericBuffer.gainFactorMultCapacityMetabolism);
                SetHugsLibValue<float>("gainFactorMultCapacityConsciousness", float.TryParse, ref this.gainFactorMultCapacityConsciousness, ref UIUtility.NumericBuffer.gainFactorMultCapacityConsciousness);
                SetHugsLibValue<float>("gainFactorMultCapacityBloodFiltration", float.TryParse, ref this.gainFactorMultCapacityBloodFiltration, ref UIUtility.NumericBuffer.gainFactorMultCapacityBloodFiltration);
                SetHugsLibValue<float>("gainFactorMultCapacityBloodPumping", float.TryParse, ref this.gainFactorMultCapacityBloodPumping, ref UIUtility.NumericBuffer.gainFactorMultCapacityBloodPumping);
                SetHugsLibValue<float>("gainFactorMultCapacityBreathing", float.TryParse, ref this.gainFactorMultCapacityBreathing, ref UIUtility.NumericBuffer.gainFactorMultCapacityBreathing);
                SetHugsLibValue<float>("gainFactorMaxCapacitySight", float.TryParse, ref this.gainFactorMaxCapacitySight, ref UIUtility.NumericBuffer.gainFactorMaxCapacitySight);
                SetHugsLibValue<float>("gainFactorMaxCapacityMoving", float.TryParse, ref this.gainFactorMaxCapacityMoving, ref UIUtility.NumericBuffer.gainFactorMaxCapacityMoving);
                SetHugsLibValue<float>("gainFactorMaxCapacityHearing", float.TryParse, ref this.gainFactorMaxCapacityHearing, ref UIUtility.NumericBuffer.gainFactorMaxCapacityHearing);
                SetHugsLibValue<float>("gainFactorMaxCapacityManipulation", float.TryParse, ref this.gainFactorMaxCapacityManipulation, ref UIUtility.NumericBuffer.gainFactorMaxCapacityManipulation);
                SetHugsLibValue<float>("gainFactorMaxCapacityMetabolism", float.TryParse, ref this.gainFactorMaxCapacityMetabolism, ref UIUtility.NumericBuffer.gainFactorMaxCapacityMetabolism);
                SetHugsLibValue<float>("gainFactorMaxCapacityConsciousness", float.TryParse, ref this.gainFactorMaxCapacityConsciousness, ref UIUtility.NumericBuffer.gainFactorMaxCapacityConsciousness);
                SetHugsLibValue<float>("gainFactorMaxCapacityBloodFiltration", float.TryParse, ref this.gainFactorMaxCapacityBloodFiltration, ref UIUtility.NumericBuffer.gainFactorMaxCapacityBloodFiltration);
                SetHugsLibValue<float>("gainFactorMaxCapacityBloodPumping", float.TryParse, ref this.gainFactorMaxCapacityBloodPumping, ref UIUtility.NumericBuffer.gainFactorMaxCapacityBloodPumping);
                SetHugsLibValue<float>("gainFactorMaxCapacityBreathing", float.TryParse, ref this.gainFactorMaxCapacityBreathing, ref UIUtility.NumericBuffer.gainFactorMaxCapacityBreathing);
                SetHugsLibValue<bool>("enableAddBionicsOption", bool.TryParse, ref this.enableAddBionicsOption);
                SetHugsLibValue<float>("addBionicsChanceFactor", float.TryParse, ref this.addBionicsChanceFactor, ref UIUtility.NumericBuffer.addBionicsChanceFactor);
                SetHugsLibValue<float>("addBionicsChanceMax", float.TryParse, ref this.addBionicsChanceMax, ref UIUtility.NumericBuffer.addBionicsChanceMax);
                SetHugsLibValue<float>("addBionicsChanceNegativeCurve", float.TryParse, ref this.addBionicsChanceNegativeCurve, ref UIUtility.NumericBuffer.addBionicsChanceNegativeCurve);
                SetHugsLibValue<int>("addBionicsMaxNumber", int.TryParse, ref this.addBionicsMaxNumber, ref UIUtility.NumericBuffer.addBionicsMaxNumber);
                SetHugsLibValue<int>("allowedByGTTechLevel", int.TryParse, ref this.allowedByGTTechLevel, ref UIUtility.NumericBuffer.allowedByGTTechLevel);
                SetHugsLibValue<bool>("enableRefineGearOption", bool.TryParse, ref this.enableRefineGearOption);
                SetHugsLibValue<float>("refineGearChanceFactor", float.TryParse, ref this.refineGearChanceFactor, ref UIUtility.NumericBuffer.refineGearChanceFactor);
                SetHugsLibValue<float>("refineGearChanceMax", float.TryParse, ref this.refineGearChanceMax, ref UIUtility.NumericBuffer.refineGearChanceMax);
                SetHugsLibValue<float>("refineGearChanceNegativeCurve", float.TryParse, ref this.refineGearChanceNegativeCurve, ref UIUtility.NumericBuffer.refineGearChanceNegativeCurve);
                SetHugsLibValue<int>("qualityUpMaxNum", int.TryParse, ref this.qualityUpMaxNum, ref UIUtility.NumericBuffer.qualityUpMaxNum);
                SetHugsLibValue<bool>("optionSetBiocode", bool.TryParse, ref this.optionSetBiocode);
                SetHugsLibValue<bool>("optionAddDeathAcidifier", bool.TryParse, ref this.optionAddDeathAcidifier);
                SetHugsLibValue<bool>("enableAddDrugOption", bool.TryParse, ref this.enableAddDrugOption);
                SetHugsLibValue<float>("addDrugChanceFactor", float.TryParse, ref this.addDrugChanceFactor, ref UIUtility.NumericBuffer.addDrugChanceFactor);
                SetHugsLibValue<float>("addDrugChanceMax", float.TryParse, ref this.addDrugChanceMax, ref UIUtility.NumericBuffer.addDrugChanceMax);
                SetHugsLibValue<float>("addDrugChanceNegativeCurve", float.TryParse, ref this.addDrugChanceNegativeCurve, ref UIUtility.NumericBuffer.addDrugChanceNegativeCurve);
                SetHugsLibValue<int>("addDrugMaxNumber", int.TryParse, ref this.addDrugMaxNumber, ref UIUtility.NumericBuffer.addDrugMaxNumber);
                SetHugsLibValue<bool>("giveOnlyActiveIngredients", bool.TryParse, ref this.giveOnlyActiveIngredients);
                SetHugsLibValue<bool>("ignoreNonVolatilityDrugs", bool.TryParse, ref this.ignoreNonVolatilityDrugs);
                SetHugsLibValue<bool>("withoutSocialDrug", bool.TryParse, ref this.withoutSocialDrug);
                SetHugsLibValue<bool>("withoutHardDrug", bool.TryParse, ref this.withoutHardDrug);
                SetHugsLibValue<bool>("withoutMedicalDrug", bool.TryParse, ref this.withoutMedicalDrug);
                SetHugsLibValue<bool>("withoutNonVolatilityDrug", bool.TryParse, ref this.withoutNonVolatilityDrug);
                SetHugsLibValue<bool>("limitationsByTechLevel", bool.TryParse, ref this.limitationsByTechLevel);
                SetHugsLibValue<int>("techLevelUpperRange", int.TryParse, ref this.techLevelUpperRange, ref UIUtility.NumericBuffer.techLevelUpperRange);
                SetHugsLibValue<int>("techLevelLowerRange", int.TryParse, ref this.techLevelLowerRange, ref UIUtility.NumericBuffer.techLevelLowerRange);

            }
            else
            {
                SetDefaultValueWithHugsLib<float>("chanceOfCompression", float.TryParse, ref this.chanceOfCompression, ref UIUtility.NumericBuffer.chanceOfCompression);
                SetDefaultValueWithHugsLib<bool>("allowMechanoids", bool.TryParse, ref this.allowMechanoids);
                SetDefaultValueWithHugsLib<bool>("allowInsectoids", bool.TryParse, ref this.allowInsectoids);
                SetDefaultValueWithHugsLib<int>("maxRaidPawnsCount", int.TryParse, ref this.maxRaidPawnsCount, ref UIUtility.NumericBuffer.maxRaidPawnsCount);
                SetDefaultValueWithHugsLib<float>("gainFactorMult", float.TryParse, ref this.gainFactorMult, ref UIUtility.NumericBuffer.gainFactorMult);
                SetDefaultValueWithHugsLib<float>("chanceOfEnhancement", float.TryParse, ref this.chanceOfEnhancement, ref UIUtility.NumericBuffer.chanceOfEnhancement);
                SetDefaultValueWithHugsLib<bool>("calcBaseMultByEnhanced", bool.TryParse, ref this.calcBaseMultByEnhanced);
                SetDefaultValueWithHugsLib<bool>("allowRaidFriendly", bool.TryParse, ref this.allowRaidFriendly);
                SetDefaultValueWithHugsLib<bool>("allowManhunters", bool.TryParse, ref this.allowManhunters);
                SetDefaultValueWithHugsLib<bool>("displayMessage", bool.TryParse, ref this.displayMessage);
                SetDefaultValueWithHugsLib<float>("gainFactorMultRaidFriendly", float.TryParse, ref this.gainFactorMultRaidFriendly, ref UIUtility.NumericBuffer.gainFactorMultRaidFriendly);
                SetDefaultValueWithHugsLib<bool>("enableMainEnhance", bool.TryParse, ref this.enableMainEnhance);
                SetDefaultValueWithHugsLib<float>("gainFactorMultPain", float.TryParse, ref this.gainFactorMultPain, ref UIUtility.NumericBuffer.gainFactorMultPain);
                SetDefaultValueWithHugsLib<float>("gainFactorMultArmorRating_Blunt", float.TryParse, ref this.gainFactorMultArmorRating_Blunt, ref UIUtility.NumericBuffer.gainFactorMultArmorRating_Blunt);
                SetDefaultValueWithHugsLib<float>("gainFactorMultArmorRating_Sharp", float.TryParse, ref this.gainFactorMultArmorRating_Sharp, ref UIUtility.NumericBuffer.gainFactorMultArmorRating_Sharp);
                SetDefaultValueWithHugsLib<float>("gainFactorMultArmorRating_Heat", float.TryParse, ref this.gainFactorMultArmorRating_Heat, ref UIUtility.NumericBuffer.gainFactorMultArmorRating_Heat);
                SetDefaultValueWithHugsLib<float>("gainFactorMultMeleeDodgeChance", float.TryParse, ref this.gainFactorMultMeleeDodgeChance, ref UIUtility.NumericBuffer.gainFactorMultMeleeDodgeChance);
                SetDefaultValueWithHugsLib<float>("gainFactorMultMeleeHitChance", float.TryParse, ref this.gainFactorMultMeleeHitChance, ref UIUtility.NumericBuffer.gainFactorMultMeleeHitChance);
                SetDefaultValueWithHugsLib<float>("gainFactorMultMoveSpeed", float.TryParse, ref this.gainFactorMultMoveSpeed, ref UIUtility.NumericBuffer.gainFactorMultMoveSpeed);
                SetDefaultValueWithHugsLib<float>("gainFactorMultShootingAccuracyPawn", float.TryParse, ref this.gainFactorMultShootingAccuracyPawn, ref UIUtility.NumericBuffer.gainFactorMultShootingAccuracyPawn);
                SetDefaultValueWithHugsLib<float>("gainFactorMultPawnTrapSpringChance", float.TryParse, ref this.gainFactorMultPawnTrapSpringChance, ref UIUtility.NumericBuffer.gainFactorMultPawnTrapSpringChance);
                SetDefaultValueWithHugsLib<float>("gainFactorMaxPain", float.TryParse, ref this.gainFactorMaxPain, ref UIUtility.NumericBuffer.gainFactorMaxPain);
                SetDefaultValueWithHugsLib<float>("gainFactorMaxArmorRating_Blunt", float.TryParse, ref this.gainFactorMaxArmorRating_Blunt, ref UIUtility.NumericBuffer.gainFactorMaxArmorRating_Blunt);
                SetDefaultValueWithHugsLib<float>("gainFactorMaxArmorRating_Sharp", float.TryParse, ref this.gainFactorMaxArmorRating_Sharp, ref UIUtility.NumericBuffer.gainFactorMaxArmorRating_Sharp);
                SetDefaultValueWithHugsLib<float>("gainFactorMaxArmorRating_Heat", float.TryParse, ref this.gainFactorMaxArmorRating_Heat, ref UIUtility.NumericBuffer.gainFactorMaxArmorRating_Heat);
                SetDefaultValueWithHugsLib<float>("gainFactorMaxMeleeDodgeChance", float.TryParse, ref this.gainFactorMaxMeleeDodgeChance, ref UIUtility.NumericBuffer.gainFactorMaxMeleeDodgeChance);
                SetDefaultValueWithHugsLib<float>("gainFactorMaxMeleeHitChance", float.TryParse, ref this.gainFactorMaxMeleeHitChance, ref UIUtility.NumericBuffer.gainFactorMaxMeleeHitChance);
                SetDefaultValueWithHugsLib<float>("gainFactorMaxMoveSpeed", float.TryParse, ref this.gainFactorMaxMoveSpeed, ref UIUtility.NumericBuffer.gainFactorMaxMoveSpeed);
                SetDefaultValueWithHugsLib<float>("gainFactorMaxShootingAccuracyPawn", float.TryParse, ref this.gainFactorMaxShootingAccuracyPawn, ref UIUtility.NumericBuffer.gainFactorMaxShootingAccuracyPawn);
                SetDefaultValueWithHugsLib<float>("gainFactorMaxPawnTrapSpringChance", float.TryParse, ref this.gainFactorMaxPawnTrapSpringChance, ref UIUtility.NumericBuffer.gainFactorMaxPawnTrapSpringChance);
                SetDefaultValueWithHugsLib<float>("gainFactorMultCapacitySight", float.TryParse, ref this.gainFactorMultCapacitySight, ref UIUtility.NumericBuffer.gainFactorMultCapacitySight);
                SetDefaultValueWithHugsLib<float>("gainFactorMultCapacityMoving", float.TryParse, ref this.gainFactorMultCapacityMoving, ref UIUtility.NumericBuffer.gainFactorMultCapacityMoving);
                SetDefaultValueWithHugsLib<float>("gainFactorMultCapacityHearing", float.TryParse, ref this.gainFactorMultCapacityHearing, ref UIUtility.NumericBuffer.gainFactorMultCapacityHearing);
                SetDefaultValueWithHugsLib<float>("gainFactorMultCapacityManipulation", float.TryParse, ref this.gainFactorMultCapacityManipulation, ref UIUtility.NumericBuffer.gainFactorMultCapacityManipulation);
                SetDefaultValueWithHugsLib<float>("gainFactorMultCapacityMetabolism", float.TryParse, ref this.gainFactorMultCapacityMetabolism, ref UIUtility.NumericBuffer.gainFactorMultCapacityMetabolism);
                SetDefaultValueWithHugsLib<float>("gainFactorMultCapacityConsciousness", float.TryParse, ref this.gainFactorMultCapacityConsciousness, ref UIUtility.NumericBuffer.gainFactorMultCapacityConsciousness);
                SetDefaultValueWithHugsLib<float>("gainFactorMultCapacityBloodFiltration", float.TryParse, ref this.gainFactorMultCapacityBloodFiltration, ref UIUtility.NumericBuffer.gainFactorMultCapacityBloodFiltration);
                SetDefaultValueWithHugsLib<float>("gainFactorMultCapacityBloodPumping", float.TryParse, ref this.gainFactorMultCapacityBloodPumping, ref UIUtility.NumericBuffer.gainFactorMultCapacityBloodPumping);
                SetDefaultValueWithHugsLib<float>("gainFactorMultCapacityBreathing", float.TryParse, ref this.gainFactorMultCapacityBreathing, ref UIUtility.NumericBuffer.gainFactorMultCapacityBreathing);
                SetDefaultValueWithHugsLib<float>("gainFactorMaxCapacitySight", float.TryParse, ref this.gainFactorMaxCapacitySight, ref UIUtility.NumericBuffer.gainFactorMaxCapacitySight);
                SetDefaultValueWithHugsLib<float>("gainFactorMaxCapacityMoving", float.TryParse, ref this.gainFactorMaxCapacityMoving, ref UIUtility.NumericBuffer.gainFactorMaxCapacityMoving);
                SetDefaultValueWithHugsLib<float>("gainFactorMaxCapacityHearing", float.TryParse, ref this.gainFactorMaxCapacityHearing, ref UIUtility.NumericBuffer.gainFactorMaxCapacityHearing);
                SetDefaultValueWithHugsLib<float>("gainFactorMaxCapacityManipulation", float.TryParse, ref this.gainFactorMaxCapacityManipulation, ref UIUtility.NumericBuffer.gainFactorMaxCapacityManipulation);
                SetDefaultValueWithHugsLib<float>("gainFactorMaxCapacityMetabolism", float.TryParse, ref this.gainFactorMaxCapacityMetabolism, ref UIUtility.NumericBuffer.gainFactorMaxCapacityMetabolism);
                SetDefaultValueWithHugsLib<float>("gainFactorMaxCapacityConsciousness", float.TryParse, ref this.gainFactorMaxCapacityConsciousness, ref UIUtility.NumericBuffer.gainFactorMaxCapacityConsciousness);
                SetDefaultValueWithHugsLib<float>("gainFactorMaxCapacityBloodFiltration", float.TryParse, ref this.gainFactorMaxCapacityBloodFiltration, ref UIUtility.NumericBuffer.gainFactorMaxCapacityBloodFiltration);
                SetDefaultValueWithHugsLib<float>("gainFactorMaxCapacityBloodPumping", float.TryParse, ref this.gainFactorMaxCapacityBloodPumping, ref UIUtility.NumericBuffer.gainFactorMaxCapacityBloodPumping);
                SetDefaultValueWithHugsLib<float>("gainFactorMaxCapacityBreathing", float.TryParse, ref this.gainFactorMaxCapacityBreathing, ref UIUtility.NumericBuffer.gainFactorMaxCapacityBreathing);
                SetDefaultValueWithHugsLib<bool>("enableAddBionicsOption", bool.TryParse, ref this.enableAddBionicsOption);
                SetDefaultValueWithHugsLib<float>("addBionicsChanceFactor", float.TryParse, ref this.addBionicsChanceFactor, ref UIUtility.NumericBuffer.addBionicsChanceFactor);
                SetDefaultValueWithHugsLib<float>("addBionicsChanceMax", float.TryParse, ref this.addBionicsChanceMax, ref UIUtility.NumericBuffer.addBionicsChanceMax);
                SetDefaultValueWithHugsLib<float>("addBionicsChanceNegativeCurve", float.TryParse, ref this.addBionicsChanceNegativeCurve, ref UIUtility.NumericBuffer.addBionicsChanceNegativeCurve);
                SetDefaultValueWithHugsLib<int>("addBionicsMaxNumber", int.TryParse, ref this.addBionicsMaxNumber, ref UIUtility.NumericBuffer.addBionicsMaxNumber);
                SetDefaultValueWithHugsLib<int>("allowedByGTTechLevel", int.TryParse, ref this.allowedByGTTechLevel, ref UIUtility.NumericBuffer.allowedByGTTechLevel);
                SetDefaultValueWithHugsLib<bool>("enableRefineGearOption", bool.TryParse, ref this.enableRefineGearOption);
                SetDefaultValueWithHugsLib<float>("refineGearChanceFactor", float.TryParse, ref this.refineGearChanceFactor, ref UIUtility.NumericBuffer.refineGearChanceFactor);
                SetDefaultValueWithHugsLib<float>("refineGearChanceMax", float.TryParse, ref this.refineGearChanceMax, ref UIUtility.NumericBuffer.refineGearChanceMax);
                SetDefaultValueWithHugsLib<float>("refineGearChanceNegativeCurve", float.TryParse, ref this.refineGearChanceNegativeCurve, ref UIUtility.NumericBuffer.refineGearChanceNegativeCurve);
                SetDefaultValueWithHugsLib<int>("qualityUpMaxNum", int.TryParse, ref this.qualityUpMaxNum, ref UIUtility.NumericBuffer.qualityUpMaxNum);
                SetDefaultValueWithHugsLib<bool>("optionSetBiocode", bool.TryParse, ref this.optionSetBiocode);
                SetDefaultValueWithHugsLib<bool>("optionAddDeathAcidifier", bool.TryParse, ref this.optionAddDeathAcidifier);
                SetDefaultValueWithHugsLib<bool>("enableAddDrugOption", bool.TryParse, ref this.enableAddDrugOption);
                SetDefaultValueWithHugsLib<float>("addDrugChanceFactor", float.TryParse, ref this.addDrugChanceFactor, ref UIUtility.NumericBuffer.addDrugChanceFactor);
                SetDefaultValueWithHugsLib<float>("addDrugChanceMax", float.TryParse, ref this.addDrugChanceMax, ref UIUtility.NumericBuffer.addDrugChanceMax);
                SetDefaultValueWithHugsLib<float>("addDrugChanceNegativeCurve", float.TryParse, ref this.addDrugChanceNegativeCurve, ref UIUtility.NumericBuffer.addDrugChanceNegativeCurve);
                SetDefaultValueWithHugsLib<int>("addDrugMaxNumber", int.TryParse, ref this.addDrugMaxNumber, ref UIUtility.NumericBuffer.addDrugMaxNumber);
                SetDefaultValueWithHugsLib<bool>("giveOnlyActiveIngredients", bool.TryParse, ref this.giveOnlyActiveIngredients);
                SetDefaultValueWithHugsLib<bool>("ignoreNonVolatilityDrugs", bool.TryParse, ref this.ignoreNonVolatilityDrugs);
                SetDefaultValueWithHugsLib<bool>("withoutSocialDrug", bool.TryParse, ref this.withoutSocialDrug);
                SetDefaultValueWithHugsLib<bool>("withoutHardDrug", bool.TryParse, ref this.withoutHardDrug);
                SetDefaultValueWithHugsLib<bool>("withoutMedicalDrug", bool.TryParse, ref this.withoutMedicalDrug);
                SetDefaultValueWithHugsLib<bool>("withoutNonVolatilityDrug", bool.TryParse, ref this.withoutNonVolatilityDrug);
                SetDefaultValueWithHugsLib<bool>("limitationsByTechLevel", bool.TryParse, ref this.limitationsByTechLevel);
                SetDefaultValueWithHugsLib<int>("techLevelUpperRange", int.TryParse, ref this.techLevelUpperRange, ref UIUtility.NumericBuffer.techLevelUpperRange);
                SetDefaultValueWithHugsLib<int>("techLevelLowerRange", int.TryParse, ref this.techLevelLowerRange, ref UIUtility.NumericBuffer.techLevelLowerRange);
            }
            this.Write();
        }

        public delegate bool TryParse<T>(string text, out T value);
        public static void SetHugsLibValue<T>(string key, TryParse<T> parse, ref T variable)
        {
            string text = HugsLibTransitionHelper.GetValue(key);
            if (text != null)
            {
                if (parse(text, out T value))
                {
                    variable = value;
                }
            }
        }
        public static void SetHugsLibValue<T>(string key, TryParse<T> parse, ref T variable, ref string buf)
        {
            string text = HugsLibTransitionHelper.GetValue(key);
            if (text != null)
            {
                if (parse(text, out T value))
                {
                    variable = value;
                    buf = value.ToString();
                }
            }
        }
        public static void SetDefaultValueWithHugsLib<T>(string key, TryParse<T> parse, ref T variable)
            where T : struct
        {
            T defaultValue = StaticVariables.GetValue<T>(key);
            string text = HugsLibTransitionHelper.GetValue(key);
            if (parse(text, out T value))
            {
                variable = value;
            }
            else
            {
                variable = defaultValue;
            }
        }
        public static void SetDefaultValueWithHugsLib<T>(string key, TryParse<T> parse, ref T variable, ref string buf)
            where T : struct
        {
            T defaultValue = StaticVariables.GetValue<T>(key);
            string text = HugsLibTransitionHelper.GetValue(key);
            if (parse(text, out T value))
            {
                variable = value;
                buf = value.ToString();
            }
            else
            {
                variable = defaultValue;
                buf = defaultValue.ToString();
            }
        }

        #region InitializeValues
        public void InitializeValues(Action postAction, params CompressedRaidMod.FunctionType[] functionTypes)
        {
            foreach (CompressedRaidMod.FunctionType functionType in functionTypes)
            {
                switch (functionType)
                {
                    case CompressedRaidMod.FunctionType.Common:
                        chanceOfCompression = StaticVariables.GetValue<float>(nameof(chanceOfCompression));
                        UIUtility.NumericBuffer.chanceOfCompression = chanceOfCompression.ToString();
                        chanceOfCompression = StaticVariables.GetValue<float>(nameof(chanceOfCompression));
                        UIUtility.NumericBuffer.chanceOfCompression = chanceOfCompression.ToString();
                        allowMechanoids = StaticVariables.GetValue<bool>(nameof(allowMechanoids));
                        allowInsectoids = StaticVariables.GetValue<bool>(nameof(allowInsectoids));
                        allowManhunters = StaticVariables.GetValue<bool>(nameof(allowManhunters));
                        maxRaidPawnsCount = StaticVariables.GetValue<int>(nameof(maxRaidPawnsCount));
                        UIUtility.NumericBuffer.maxRaidPawnsCount = maxRaidPawnsCount.ToString();
                        gainFactorMult = StaticVariables.GetValue<float>(nameof(gainFactorMult));
                        UIUtility.NumericBuffer.gainFactorMult = gainFactorMult.ToString();
                        chanceOfEnhancement = StaticVariables.GetValue<float>(nameof(chanceOfEnhancement));
                        UIUtility.NumericBuffer.chanceOfEnhancement = chanceOfEnhancement.ToString();
                        calcBaseMultByEnhanced = StaticVariables.GetValue<bool>(nameof(calcBaseMultByEnhanced));
                        allowRaidFriendly = StaticVariables.GetValue<bool>(nameof(allowRaidFriendly));
                        gainFactorMultRaidFriendly = StaticVariables.GetValue<float>(nameof(gainFactorMultRaidFriendly));
                        UIUtility.NumericBuffer.gainFactorMultRaidFriendly = gainFactorMultRaidFriendly.ToString();
                        displayMessage = StaticVariables.GetValue<bool>(nameof(displayMessage));
                        break;
                    case CompressedRaidMod.FunctionType.Main:
                        enableMainEnhance = StaticVariables.GetValue<bool>(nameof(enableMainEnhance));
                        gainFactorMultPain = StaticVariables.GetValue<float>(nameof(gainFactorMultPain));
                        UIUtility.NumericBuffer.gainFactorMultPain = gainFactorMultPain.ToString();
                        gainFactorMultArmorRating_Blunt = StaticVariables.GetValue<float>(nameof(gainFactorMultArmorRating_Blunt));
                        UIUtility.NumericBuffer.gainFactorMultArmorRating_Blunt = gainFactorMultArmorRating_Blunt.ToString();
                        gainFactorMultArmorRating_Sharp = StaticVariables.GetValue<float>(nameof(gainFactorMultArmorRating_Sharp));
                        UIUtility.NumericBuffer.gainFactorMultArmorRating_Sharp = gainFactorMultArmorRating_Sharp.ToString();
                        gainFactorMultArmorRating_Heat = StaticVariables.GetValue<float>(nameof(gainFactorMultArmorRating_Heat));
                        UIUtility.NumericBuffer.gainFactorMultArmorRating_Heat = gainFactorMultArmorRating_Heat.ToString();
                        gainFactorMultMeleeDodgeChance = StaticVariables.GetValue<float>(nameof(gainFactorMultMeleeDodgeChance));
                        UIUtility.NumericBuffer.gainFactorMultMeleeDodgeChance = gainFactorMultMeleeDodgeChance.ToString();
                        gainFactorMultMeleeHitChance = StaticVariables.GetValue<float>(nameof(gainFactorMultMeleeHitChance));
                        UIUtility.NumericBuffer.gainFactorMultMeleeHitChance = gainFactorMultMeleeHitChance.ToString();
                        gainFactorMultMoveSpeed = StaticVariables.GetValue<float>(nameof(gainFactorMultMoveSpeed));
                        UIUtility.NumericBuffer.gainFactorMultMoveSpeed = gainFactorMultMoveSpeed.ToString();
                        gainFactorMultShootingAccuracyPawn = StaticVariables.GetValue<float>(nameof(gainFactorMultShootingAccuracyPawn));
                        UIUtility.NumericBuffer.gainFactorMultShootingAccuracyPawn = gainFactorMultShootingAccuracyPawn.ToString();
                        gainFactorMultPawnTrapSpringChance = StaticVariables.GetValue<float>(nameof(gainFactorMultPawnTrapSpringChance));
                        UIUtility.NumericBuffer.gainFactorMultPawnTrapSpringChance = gainFactorMultPawnTrapSpringChance.ToString();
                        gainFactorMaxPain = StaticVariables.GetValue<float>(nameof(gainFactorMaxPain));
                        UIUtility.NumericBuffer.gainFactorMaxPain = gainFactorMaxPain.ToString();
                        gainFactorMaxArmorRating_Blunt = StaticVariables.GetValue<float>(nameof(gainFactorMaxArmorRating_Blunt));
                        UIUtility.NumericBuffer.gainFactorMaxArmorRating_Blunt = gainFactorMaxArmorRating_Blunt.ToString();
                        gainFactorMaxArmorRating_Sharp = StaticVariables.GetValue<float>(nameof(gainFactorMaxArmorRating_Sharp));
                        UIUtility.NumericBuffer.gainFactorMaxArmorRating_Sharp = gainFactorMaxArmorRating_Sharp.ToString();
                        gainFactorMaxArmorRating_Heat = StaticVariables.GetValue<float>(nameof(gainFactorMaxArmorRating_Heat));
                        UIUtility.NumericBuffer.gainFactorMaxArmorRating_Heat = gainFactorMaxArmorRating_Heat.ToString();
                        gainFactorMaxMeleeDodgeChance = StaticVariables.GetValue<float>(nameof(gainFactorMaxMeleeDodgeChance));
                        UIUtility.NumericBuffer.gainFactorMaxMeleeDodgeChance = gainFactorMaxMeleeDodgeChance.ToString();
                        gainFactorMaxMeleeHitChance = StaticVariables.GetValue<float>(nameof(gainFactorMaxMeleeHitChance));
                        UIUtility.NumericBuffer.gainFactorMaxMeleeHitChance = gainFactorMaxMeleeHitChance.ToString();
                        gainFactorMaxMoveSpeed = StaticVariables.GetValue<float>(nameof(gainFactorMaxMoveSpeed));
                        UIUtility.NumericBuffer.gainFactorMaxMoveSpeed = gainFactorMaxMoveSpeed.ToString();
                        gainFactorMaxShootingAccuracyPawn = StaticVariables.GetValue<float>(nameof(gainFactorMaxShootingAccuracyPawn));
                        UIUtility.NumericBuffer.gainFactorMaxShootingAccuracyPawn = gainFactorMaxShootingAccuracyPawn.ToString();
                        gainFactorMaxPawnTrapSpringChance = StaticVariables.GetValue<float>(nameof(gainFactorMaxPawnTrapSpringChance));
                        UIUtility.NumericBuffer.gainFactorMaxPawnTrapSpringChance = gainFactorMaxPawnTrapSpringChance.ToString();
                        gainFactorMultCapacitySight = StaticVariables.GetValue<float>(nameof(gainFactorMultCapacitySight));
                        UIUtility.NumericBuffer.gainFactorMultCapacitySight = gainFactorMultCapacitySight.ToString();
                        gainFactorMultCapacityMoving = StaticVariables.GetValue<float>(nameof(gainFactorMultCapacityMoving));
                        UIUtility.NumericBuffer.gainFactorMultCapacityMoving = gainFactorMultCapacityMoving.ToString();
                        gainFactorMultCapacityHearing = StaticVariables.GetValue<float>(nameof(gainFactorMultCapacityHearing));
                        UIUtility.NumericBuffer.gainFactorMultCapacityHearing = gainFactorMultCapacityHearing.ToString();
                        gainFactorMultCapacityManipulation = StaticVariables.GetValue<float>(nameof(gainFactorMultCapacityManipulation));
                        UIUtility.NumericBuffer.gainFactorMultCapacityManipulation = gainFactorMultCapacityManipulation.ToString();
                        gainFactorMultCapacityMetabolism = StaticVariables.GetValue<float>(nameof(gainFactorMultCapacityMetabolism));
                        UIUtility.NumericBuffer.gainFactorMultCapacityMetabolism = gainFactorMultCapacityMetabolism.ToString();
                        gainFactorMultCapacityConsciousness = StaticVariables.GetValue<float>(nameof(gainFactorMultCapacityConsciousness));
                        UIUtility.NumericBuffer.gainFactorMultCapacityConsciousness = gainFactorMultCapacityConsciousness.ToString();
                        gainFactorMultCapacityBloodFiltration = StaticVariables.GetValue<float>(nameof(gainFactorMultCapacityBloodFiltration));
                        UIUtility.NumericBuffer.gainFactorMultCapacityBloodFiltration = gainFactorMultCapacityBloodFiltration.ToString();
                        gainFactorMultCapacityBloodPumping = StaticVariables.GetValue<float>(nameof(gainFactorMultCapacityBloodPumping));
                        UIUtility.NumericBuffer.gainFactorMultCapacityBloodPumping = gainFactorMultCapacityBloodPumping.ToString();
                        gainFactorMultCapacityBreathing = StaticVariables.GetValue<float>(nameof(gainFactorMultCapacityBreathing));
                        UIUtility.NumericBuffer.gainFactorMultCapacityBreathing = gainFactorMultCapacityBreathing.ToString();
                        gainFactorMaxCapacitySight = StaticVariables.GetValue<float>(nameof(gainFactorMaxCapacitySight));
                        UIUtility.NumericBuffer.gainFactorMaxCapacitySight = gainFactorMaxCapacitySight.ToString();
                        gainFactorMaxCapacityMoving = StaticVariables.GetValue<float>(nameof(gainFactorMaxCapacityMoving));
                        UIUtility.NumericBuffer.gainFactorMaxCapacityMoving = gainFactorMaxCapacityMoving.ToString();
                        gainFactorMaxCapacityHearing = StaticVariables.GetValue<float>(nameof(gainFactorMaxCapacityHearing));
                        UIUtility.NumericBuffer.gainFactorMaxCapacityHearing = gainFactorMaxCapacityHearing.ToString();
                        gainFactorMaxCapacityManipulation = StaticVariables.GetValue<float>(nameof(gainFactorMaxCapacityManipulation));
                        UIUtility.NumericBuffer.gainFactorMaxCapacityManipulation = gainFactorMaxCapacityManipulation.ToString();
                        gainFactorMaxCapacityMetabolism = StaticVariables.GetValue<float>(nameof(gainFactorMaxCapacityMetabolism));
                        UIUtility.NumericBuffer.gainFactorMaxCapacityMetabolism = gainFactorMaxCapacityMetabolism.ToString();
                        gainFactorMaxCapacityConsciousness = StaticVariables.GetValue<float>(nameof(gainFactorMaxCapacityConsciousness));
                        UIUtility.NumericBuffer.gainFactorMaxCapacityConsciousness = gainFactorMaxCapacityConsciousness.ToString();
                        gainFactorMaxCapacityBloodFiltration = StaticVariables.GetValue<float>(nameof(gainFactorMaxCapacityBloodFiltration));
                        UIUtility.NumericBuffer.gainFactorMaxCapacityBloodFiltration = gainFactorMaxCapacityBloodFiltration.ToString();
                        gainFactorMaxCapacityBloodPumping = StaticVariables.GetValue<float>(nameof(gainFactorMaxCapacityBloodPumping));
                        UIUtility.NumericBuffer.gainFactorMaxCapacityBloodPumping = gainFactorMaxCapacityBloodPumping.ToString();
                        gainFactorMaxCapacityBreathing = StaticVariables.GetValue<float>(nameof(gainFactorMaxCapacityBreathing));
                        UIUtility.NumericBuffer.gainFactorMaxCapacityBreathing = gainFactorMaxCapacityBreathing.ToString();
                        break;
                    case CompressedRaidMod.FunctionType.AddBionics:
                        enableAddBionicsOption = StaticVariables.GetValue<bool>(nameof(enableAddBionicsOption));
                        addBionicsChanceFactor = StaticVariables.GetValue<float>(nameof(addBionicsChanceFactor));
                        UIUtility.NumericBuffer.addBionicsChanceFactor = addBionicsChanceFactor.ToString();
                        addBionicsChanceMax = StaticVariables.GetValue<float>(nameof(addBionicsChanceMax));
                        UIUtility.NumericBuffer.addBionicsChanceMax = addBionicsChanceMax.ToString();
                        addBionicsChanceNegativeCurve = StaticVariables.GetValue<float>(nameof(addBionicsChanceNegativeCurve));
                        UIUtility.NumericBuffer.addBionicsChanceNegativeCurve = addBionicsChanceNegativeCurve.ToString();
                        addBionicsMaxNumber = StaticVariables.GetValue<int>(nameof(addBionicsMaxNumber));
                        UIUtility.NumericBuffer.addBionicsMaxNumber = addBionicsMaxNumber.ToString();
                        allowedByGTTechLevel = StaticVariables.GetValue<int>(nameof(allowedByGTTechLevel));
                        UIUtility.NumericBuffer.allowedByGTTechLevel = allowedByGTTechLevel.ToString();
                        break;
                    case CompressedRaidMod.FunctionType.RefineGear:
                        enableRefineGearOption = StaticVariables.GetValue<bool>(nameof(enableRefineGearOption));
                        refineGearChanceFactor = StaticVariables.GetValue<float>(nameof(refineGearChanceFactor));
                        UIUtility.NumericBuffer.refineGearChanceFactor = refineGearChanceFactor.ToString();
                        refineGearChanceMax = StaticVariables.GetValue<float>(nameof(refineGearChanceMax));
                        UIUtility.NumericBuffer.refineGearChanceMax = refineGearChanceMax.ToString();
                        refineGearChanceNegativeCurve = StaticVariables.GetValue<float>(nameof(refineGearChanceNegativeCurve));
                        UIUtility.NumericBuffer.refineGearChanceNegativeCurve = refineGearChanceNegativeCurve.ToString();
                        qualityUpMaxNum = StaticVariables.GetValue<int>(nameof(qualityUpMaxNum));
                        UIUtility.NumericBuffer.qualityUpMaxNum = qualityUpMaxNum.ToString();
                        optionSetBiocode = StaticVariables.GetValue<bool>(nameof(optionSetBiocode));
                        optionAddDeathAcidifier = StaticVariables.GetValue<bool>(nameof(optionAddDeathAcidifier));
                        break;
                    case CompressedRaidMod.FunctionType.AddDrugEffects:
                        enableAddDrugOption = StaticVariables.GetValue<bool>(nameof(enableAddDrugOption));
                        addDrugChanceFactor = StaticVariables.GetValue<float>(nameof(addDrugChanceFactor));
                        UIUtility.NumericBuffer.addDrugChanceFactor = addDrugChanceFactor.ToString();
                        addDrugChanceMax = StaticVariables.GetValue<float>(nameof(addDrugChanceMax));
                        UIUtility.NumericBuffer.addDrugChanceMax = addDrugChanceMax.ToString();
                        addDrugChanceNegativeCurve = StaticVariables.GetValue<float>(nameof(addDrugChanceNegativeCurve));
                        UIUtility.NumericBuffer.addDrugChanceNegativeCurve = addDrugChanceNegativeCurve.ToString();
                        addDrugMaxNumber = StaticVariables.GetValue<int>(nameof(addDrugMaxNumber));
                        UIUtility.NumericBuffer.addDrugMaxNumber = addDrugMaxNumber.ToString();
                        giveOnlyActiveIngredients = StaticVariables.GetValue<bool>(nameof(giveOnlyActiveIngredients));
                        ignoreNonVolatilityDrugs = StaticVariables.GetValue<bool>(nameof(ignoreNonVolatilityDrugs));
                        withoutSocialDrug = StaticVariables.GetValue<bool>(nameof(withoutSocialDrug));
                        withoutHardDrug = StaticVariables.GetValue<bool>(nameof(withoutHardDrug));
                        withoutMedicalDrug = StaticVariables.GetValue<bool>(nameof(withoutMedicalDrug));
                        withoutNonVolatilityDrug = StaticVariables.GetValue<bool>(nameof(withoutNonVolatilityDrug));
                        limitationsByTechLevel = StaticVariables.GetValue<bool>(nameof(limitationsByTechLevel));
                        techLevelUpperRange = StaticVariables.GetValue<int>(nameof(techLevelUpperRange));
                        UIUtility.NumericBuffer.techLevelUpperRange = techLevelUpperRange.ToString();
                        techLevelLowerRange = StaticVariables.GetValue<int>(nameof(techLevelLowerRange));
                        UIUtility.NumericBuffer.techLevelLowerRange = techLevelLowerRange.ToString();
                        break;
                }
                //SettingValues();
                postAction();
            }
        }
        #endregion


        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look<bool>(ref this.radioTextInput, "radioTextInput", true, false);
            Scribe_Values.Look<bool>(ref this.radioSliderInput, "radioSliderInput", false, false);

            Scribe_Values.Look<float>(ref this.chanceOfCompression, "chanceOfCompression", StaticVariables.CHANCE_OF_COMPRESSION_DEFAULT, false);
            Scribe_Values.Look<bool>(ref this.allowMechanoids, "allowMechanoids", StaticVariables.ALLOW_MECHANOIDS_DEFAULT, false);
            Scribe_Values.Look<bool>(ref this.allowInsectoids, "allowInsectoids", StaticVariables.ALLOW_INSECTOIDS_DEFAULT, false);
            Scribe_Values.Look<int>(ref this.maxRaidPawnsCount, "maxRaidPawnsCount", StaticVariables.MAX_RAID_PAWNS_COUNT_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMult, "gainFactorMult", StaticVariables.GAIN_FACTOR_MULT_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.chanceOfEnhancement, "chanceOfEnhancement", StaticVariables.CHANCE_OF_ENHANCEMENT_DEFAULT, false);
            Scribe_Values.Look<bool>(ref this.calcBaseMultByEnhanced, "calcBaseMultByEnhanced", StaticVariables.CALC_BASE_MULT_BY_ENHANCED_DEFAULT, false);
            Scribe_Values.Look<bool>(ref this.allowRaidFriendly, "allowRaidFriendly", StaticVariables.ALLOW_RAID_FRIENDLY_DEFAULT, false);
            Scribe_Values.Look<bool>(ref this.allowManhunters, "allowManhunters", StaticVariables.ALLOW_MANHUNTERS_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMultRaidFriendly, "gainFactorMultRaidFriendly", StaticVariables.GAIN_FACTOR_MULT_RAID_FRIENDLY_DEFAULT, false);
            Scribe_Values.Look<bool>(ref this.displayMessage, "displayMessage", StaticVariables.DISPLAY_MESSAGE_DEFAULT, false);

            Scribe_Values.Look<bool>(ref this.enableMainEnhance, "enableMainEnhance", StaticVariables.ENABLE_MAIN_ENHANCE_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMultPain, "gainFactorMultPain", StaticVariables.GAIN_FACTOR_MULT_PAIN_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMultArmorRating_Blunt, "gainFactorMultArmorRating_Blunt", StaticVariables.GAIN_FACTOR_MULT_ARMOR_RATING_BLUNT_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMultArmorRating_Sharp, "gainFactorMultArmorRating_Sharp", StaticVariables.GAIN_FACTOR_MULT_ARMOR_RATING_SHARP_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMultArmorRating_Heat, "gainFactorMultArmorRating_Heat", StaticVariables.GAIN_FACTOR_MULT_ARMOR_RATING_HEAT_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMultMeleeDodgeChance, "gainFactorMultMeleeDodgeChance", StaticVariables.GAIN_FACTOR_MULT_MELEE_DODGE_CHANCE_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMultMeleeHitChance, "gainFactorMultMeleeHitChance", StaticVariables.GAIN_FACTOR_MULT_MELEE_HIT_CHANCE_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMultMoveSpeed, "gainFactorMultMoveSpeed", StaticVariables.GAIN_FACTOR_MULT_MOVE_SPEED_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMultShootingAccuracyPawn, "gainFactorMultShootingAccuracyPawn", StaticVariables.GAIN_FACTOR_MULT_SHOOTING_ACCURACY_PAWN_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMultPawnTrapSpringChance, "gainFactorMultPawnTrapSpringChance", StaticVariables.GAIN_FACTOR_MULT_PAWN_TRAP_SPRING_CHANCE_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMaxPain, "gainFactorMaxPain", StaticVariables.GAIN_FACTOR_MAX_PAIN_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMaxArmorRating_Blunt, "gainFactorMaxArmorRating_Blunt", StaticVariables.GAIN_FACTOR_MAX_ARMOR_RATING_BLUNT_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMaxArmorRating_Sharp, "gainFactorMaxArmorRating_Sharp", StaticVariables.GAIN_FACTOR_MAX_ARMOR_RATING_SHARP_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMaxArmorRating_Heat, "gainFactorMaxArmorRating_Heat", StaticVariables.GAIN_FACTOR_MAX_ARMOR_RATING_HEAT_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMaxMeleeDodgeChance, "gainFactorMaxMeleeDodgeChance", StaticVariables.GAIN_FACTOR_MAX_MELEE_DODGE_CHANCE_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMaxMeleeHitChance, "gainFactorMaxMeleeHitChance", StaticVariables.GAIN_FACTOR_MAX_MELEE_HIT_CHANCE_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMaxMoveSpeed, "gainFactorMaxMoveSpeed", StaticVariables.GAIN_FACTOR_MAX_MOVE_SPEED_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMaxShootingAccuracyPawn, "gainFactorMaxShootingAccuracyPawn", StaticVariables.GAIN_FACTOR_MAX_SHOOTING_ACCURACY_PAWN_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMaxPawnTrapSpringChance, "gainFactorMaxPawnTrapSpringChance", StaticVariables.GAIN_FACTOR_MAX_PAWN_TRAP_SPRING_CHANCE_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMultCapacitySight, "gainFactorMultCapacitySight", StaticVariables.GAIN_FACTOR_MULT_CAPACITY_SIGHT_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMultCapacityMoving, "gainFactorMultCapacityMoving", StaticVariables.GAIN_FACTOR_MULT_CAPACITY_MOVING_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMultCapacityHearing, "gainFactorMultCapacityHearing", StaticVariables.GAIN_FACTOR_MULT_CAPACITY_HEARING_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMultCapacityManipulation, "gainFactorMultCapacityManipulation", StaticVariables.GAIN_FACTOR_MULT_CAPACITY_MANIPULATION_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMultCapacityMetabolism, "gainFactorMultCapacityMetabolism", StaticVariables.GAIN_FACTOR_MULT_CAPACITY_METABOLISM_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMultCapacityConsciousness, "gainFactorMultCapacityConsciousness", StaticVariables.GAIN_FACTOR_MULT_CAPACITY_CONSCIOUSNESS_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMultCapacityBloodFiltration, "gainFactorMultCapacityBloodFiltration", StaticVariables.GAIN_FACTOR_MULT_CAPACITY_BLOOD_FILTRATION_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMultCapacityBloodPumping, "gainFactorMultCapacityBloodPumping", StaticVariables.GAIN_FACTOR_MULT_CAPACITY_BLOOD_PUMPING_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMultCapacityBreathing, "gainFactorMultCapacityBreathing", StaticVariables.GAIN_FACTOR_MULT_CAPACITY_BREATHING_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMaxCapacitySight, "gainFactorMaxCapacitySight", StaticVariables.GAIN_FACTOR_MAX_CAPACITY_SIGHT_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMaxCapacityMoving, "gainFactorMaxCapacityMoving", StaticVariables.GAIN_FACTOR_MAX_CAPACITY_MOVING_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMaxCapacityHearing, "gainFactorMaxCapacityHearing", StaticVariables.GAIN_FACTOR_MAX_CAPACITY_HEARING_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMaxCapacityManipulation, "gainFactorMaxCapacityManipulation", StaticVariables.GAIN_FACTOR_MAX_CAPACITY_MANIPULATION_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMaxCapacityMetabolism, "gainFactorMaxCapacityMetabolism", StaticVariables.GAIN_FACTOR_MAX_CAPACITY_METABOLISM_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMaxCapacityConsciousness, "gainFactorMaxCapacityConsciousness", StaticVariables.GAIN_FACTOR_MAX_CAPACITY_CONSCIOUSNESS_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMaxCapacityBloodFiltration, "gainFactorMaxCapacityBloodFiltration", StaticVariables.GAIN_FACTOR_MAX_CAPACITY_BLOOD_FILTRATION_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMaxCapacityBloodPumping, "gainFactorMaxCapacityBloodPumping", StaticVariables.GAIN_FACTOR_MAX_CAPACITY_BLOOD_PUMPING_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.gainFactorMaxCapacityBreathing, "gainFactorMaxCapacityBreathing", StaticVariables.GAIN_FACTOR_MAX_CAPACITY_BREATHING_DEFAULT, false);

            Scribe_Values.Look<bool>(ref this.enableAddBionicsOption, "enableAddBionicsOption", StaticVariables.ENABLE_ADD_BIONICS_OPTION_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.addBionicsChanceFactor, "addBionicsChanceFactor", StaticVariables.ADD_BIONICS_CHANCE_FACTOR_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.addBionicsChanceMax, "addBionicsChanceMax", StaticVariables.ADD_BIONICS_CHANCE_MAX_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.addBionicsChanceNegativeCurve, "addBionicsChanceNegativeCurve", StaticVariables.ADD_BIONICS_CHANCE_NEGATIVE_CURVE_DEFAULT, false);
            Scribe_Values.Look<int>(ref this.addBionicsMaxNumber, "addBionicsMaxNumber", StaticVariables.ADD_BIONICS_MAX_NUMBER_DEFAULT, false);
            Scribe_Values.Look<int>(ref this.allowedByGTTechLevel, "allowedByGTTechLevel", StaticVariables.ALLOWED_BY_G_T_TECH_LEVEL_DEFAULT, false);

            Scribe_Values.Look<bool>(ref this.enableRefineGearOption, "enableRefineGearOption", StaticVariables.ENABLE_REFINE_GEAR_OPTION_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.refineGearChanceFactor, "refineGearChanceFactor", StaticVariables.REFINE_GEAR_CHANCE_FACTOR_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.refineGearChanceMax, "refineGearChanceMax", StaticVariables.REFINE_GEAR_CHANCE_MAX_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.refineGearChanceNegativeCurve, "refineGearChanceNegativeCurve", StaticVariables.REFINE_GEAR_CHANCE_NEGATIVE_CURVE_DEFAULT, false);
            Scribe_Values.Look<int>(ref this.qualityUpMaxNum, "qualityUpMaxNum", StaticVariables.QUALITY_UP_MAX_NUM_DEFAULT, false);
            Scribe_Values.Look<bool>(ref this.optionSetBiocode, "optionSetBiocode", StaticVariables.OPTION_SET_BIOCODE_DEFAULT, false);
            Scribe_Values.Look<bool>(ref this.optionAddDeathAcidifier, "optionAddDeathAcidifier", StaticVariables.OPTION_ADD_DEATH_ACIDIFIER_DEFAULT, false);

            Scribe_Values.Look<bool>(ref this.enableAddDrugOption, "enableAddDrugOption", StaticVariables.ENABLE_ADD_DRUG_OPTION_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.addDrugChanceFactor, "addDrugChanceFactor", StaticVariables.ADD_DRUG_CHANCE_FACTOR_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.addDrugChanceMax, "addDrugChanceMax", StaticVariables.ADD_DRUG_CHANCE_MAX_DEFAULT, false);
            Scribe_Values.Look<float>(ref this.addDrugChanceNegativeCurve, "addDrugChanceNegativeCurve", StaticVariables.ADD_DRUG_CHANCE_NEGATIVE_CURVE_DEFAULT, false);
            Scribe_Values.Look<int>(ref this.addDrugMaxNumber, "addDrugMaxNumber", StaticVariables.ADD_DRUG_MAX_NUMBER_DEFAULT, false);
            Scribe_Values.Look<bool>(ref this.giveOnlyActiveIngredients, "giveOnlyActiveIngredients", StaticVariables.GIVE_ONLY_ACTIVE_INGREDIENTS_DEFAULT, false);
            Scribe_Values.Look<bool>(ref this.ignoreNonVolatilityDrugs, "ignoreNonVolatilityDrugs", StaticVariables.IGNORE_NON_VOLATILITY_DRUGS_DEFAULT, false);
            Scribe_Values.Look<bool>(ref this.withoutSocialDrug, "withoutSocialDrug", StaticVariables.WITHOUT_SOCIAL_DRUG_DEFAULT, false);
            Scribe_Values.Look<bool>(ref this.withoutHardDrug, "withoutHardDrug", StaticVariables.WITHOUT_HARD_DRUG_DEFAULT, false);
            Scribe_Values.Look<bool>(ref this.withoutMedicalDrug, "withoutMedicalDrug", StaticVariables.WITHOUT_MEDICAL_DRUG_DEFAULT, false);
            Scribe_Values.Look<bool>(ref this.withoutNonVolatilityDrug, "withoutNonVolatilityDrug", StaticVariables.WITHOUT_NON_VOLATILITY_DRUG_DEFAULT, false);
            Scribe_Values.Look<bool>(ref this.limitationsByTechLevel, "limitationsByTechLevel", StaticVariables.LIMITATIONS_BY_TECH_LEVEL_DEFAULT, false);
            Scribe_Values.Look<int>(ref this.techLevelUpperRange, "techLevelUpperRange", StaticVariables.TECH_LEVEL_UPPER_RANGE_DEFAULT, false);
            Scribe_Values.Look<int>(ref this.techLevelLowerRange, "techLevelLowerRange", StaticVariables.TECH_LEVEL_LOWER_RANGE_DEFAULT, false);
        }

        //操作モード
        public bool radioTextInput = true;
        public bool radioSliderInput = false;

        //共通設定 Common Settings
        public float chanceOfCompression = StaticVariables.CHANCE_OF_COMPRESSION_DEFAULT;
        public bool allowMechanoids = StaticVariables.ALLOW_MECHANOIDS_DEFAULT;
        public bool allowInsectoids = StaticVariables.ALLOW_INSECTOIDS_DEFAULT;
        public int maxRaidPawnsCount = StaticVariables.MAX_RAID_PAWNS_COUNT_DEFAULT;
        public float gainFactorMult = StaticVariables.GAIN_FACTOR_MULT_DEFAULT;
        public float chanceOfEnhancement = StaticVariables.CHANCE_OF_ENHANCEMENT_DEFAULT;
        public bool calcBaseMultByEnhanced = StaticVariables.CALC_BASE_MULT_BY_ENHANCED_DEFAULT;
        public bool allowRaidFriendly = StaticVariables.ALLOW_RAID_FRIENDLY_DEFAULT;
        public bool allowManhunters = StaticVariables.ALLOW_MANHUNTERS_DEFAULT;
        public float gainFactorMultRaidFriendly = StaticVariables.GAIN_FACTOR_MULT_RAID_FRIENDLY_DEFAULT;
        public bool displayMessage = StaticVariables.DISPLAY_MESSAGE_DEFAULT;

        //メイン強化機能 Main Enhance Function
        public bool enableMainEnhance = StaticVariables.ENABLE_MAIN_ENHANCE_DEFAULT;
        public float gainFactorMultPain = StaticVariables.GAIN_FACTOR_MULT_PAIN_DEFAULT;
        public float gainFactorMultArmorRating_Blunt = StaticVariables.GAIN_FACTOR_MULT_ARMOR_RATING_BLUNT_DEFAULT;
        public float gainFactorMultArmorRating_Sharp = StaticVariables.GAIN_FACTOR_MULT_ARMOR_RATING_SHARP_DEFAULT;
        public float gainFactorMultArmorRating_Heat = StaticVariables.GAIN_FACTOR_MULT_ARMOR_RATING_HEAT_DEFAULT;
        public float gainFactorMultMeleeDodgeChance = StaticVariables.GAIN_FACTOR_MULT_MELEE_DODGE_CHANCE_DEFAULT;
        public float gainFactorMultMeleeHitChance = StaticVariables.GAIN_FACTOR_MULT_MELEE_HIT_CHANCE_DEFAULT;
        public float gainFactorMultMoveSpeed = StaticVariables.GAIN_FACTOR_MULT_MOVE_SPEED_DEFAULT;
        public float gainFactorMultShootingAccuracyPawn = StaticVariables.GAIN_FACTOR_MULT_SHOOTING_ACCURACY_PAWN_DEFAULT;
        public float gainFactorMultPawnTrapSpringChance = StaticVariables.GAIN_FACTOR_MULT_PAWN_TRAP_SPRING_CHANCE_DEFAULT;
        public float gainFactorMaxPain = StaticVariables.GAIN_FACTOR_MAX_PAIN_DEFAULT;
        public float gainFactorMaxArmorRating_Blunt = StaticVariables.GAIN_FACTOR_MAX_ARMOR_RATING_BLUNT_DEFAULT;
        public float gainFactorMaxArmorRating_Sharp = StaticVariables.GAIN_FACTOR_MAX_ARMOR_RATING_SHARP_DEFAULT;
        public float gainFactorMaxArmorRating_Heat = StaticVariables.GAIN_FACTOR_MAX_ARMOR_RATING_HEAT_DEFAULT;
        public float gainFactorMaxMeleeDodgeChance = StaticVariables.GAIN_FACTOR_MAX_MELEE_DODGE_CHANCE_DEFAULT;
        public float gainFactorMaxMeleeHitChance = StaticVariables.GAIN_FACTOR_MAX_MELEE_HIT_CHANCE_DEFAULT;
        public float gainFactorMaxMoveSpeed = StaticVariables.GAIN_FACTOR_MAX_MOVE_SPEED_DEFAULT;
        public float gainFactorMaxShootingAccuracyPawn = StaticVariables.GAIN_FACTOR_MAX_SHOOTING_ACCURACY_PAWN_DEFAULT;
        public float gainFactorMaxPawnTrapSpringChance = StaticVariables.GAIN_FACTOR_MAX_PAWN_TRAP_SPRING_CHANCE_DEFAULT;
        public float gainFactorMultCapacitySight = StaticVariables.GAIN_FACTOR_MULT_CAPACITY_SIGHT_DEFAULT;
        public float gainFactorMultCapacityMoving = StaticVariables.GAIN_FACTOR_MULT_CAPACITY_MOVING_DEFAULT;
        public float gainFactorMultCapacityHearing = StaticVariables.GAIN_FACTOR_MULT_CAPACITY_HEARING_DEFAULT;
        public float gainFactorMultCapacityManipulation = StaticVariables.GAIN_FACTOR_MULT_CAPACITY_MANIPULATION_DEFAULT;
        public float gainFactorMultCapacityMetabolism = StaticVariables.GAIN_FACTOR_MULT_CAPACITY_METABOLISM_DEFAULT;
        public float gainFactorMultCapacityConsciousness = StaticVariables.GAIN_FACTOR_MULT_CAPACITY_CONSCIOUSNESS_DEFAULT;
        public float gainFactorMultCapacityBloodFiltration = StaticVariables.GAIN_FACTOR_MULT_CAPACITY_BLOOD_FILTRATION_DEFAULT;
        public float gainFactorMultCapacityBloodPumping = StaticVariables.GAIN_FACTOR_MULT_CAPACITY_BLOOD_PUMPING_DEFAULT;
        public float gainFactorMultCapacityBreathing = StaticVariables.GAIN_FACTOR_MULT_CAPACITY_BREATHING_DEFAULT;
        public float gainFactorMaxCapacitySight = StaticVariables.GAIN_FACTOR_MAX_CAPACITY_SIGHT_DEFAULT;
        public float gainFactorMaxCapacityMoving = StaticVariables.GAIN_FACTOR_MAX_CAPACITY_MOVING_DEFAULT;
        public float gainFactorMaxCapacityHearing = StaticVariables.GAIN_FACTOR_MAX_CAPACITY_HEARING_DEFAULT;
        public float gainFactorMaxCapacityManipulation = StaticVariables.GAIN_FACTOR_MAX_CAPACITY_MANIPULATION_DEFAULT;
        public float gainFactorMaxCapacityMetabolism = StaticVariables.GAIN_FACTOR_MAX_CAPACITY_METABOLISM_DEFAULT;
        public float gainFactorMaxCapacityConsciousness = StaticVariables.GAIN_FACTOR_MAX_CAPACITY_CONSCIOUSNESS_DEFAULT;
        public float gainFactorMaxCapacityBloodFiltration = StaticVariables.GAIN_FACTOR_MAX_CAPACITY_BLOOD_FILTRATION_DEFAULT;
        public float gainFactorMaxCapacityBloodPumping = StaticVariables.GAIN_FACTOR_MAX_CAPACITY_BLOOD_PUMPING_DEFAULT;
        public float gainFactorMaxCapacityBreathing = StaticVariables.GAIN_FACTOR_MAX_CAPACITY_BREATHING_DEFAULT;

        // バイオニクス追加機能 Add Bionics Function
        public bool enableAddBionicsOption = StaticVariables.ENABLE_ADD_BIONICS_OPTION_DEFAULT;
        public float addBionicsChanceFactor = StaticVariables.ADD_BIONICS_CHANCE_FACTOR_DEFAULT;
        public float addBionicsChanceMax = StaticVariables.ADD_BIONICS_CHANCE_MAX_DEFAULT;
        public float addBionicsChanceNegativeCurve = StaticVariables.ADD_BIONICS_CHANCE_NEGATIVE_CURVE_DEFAULT;
        public int addBionicsMaxNumber = StaticVariables.ADD_BIONICS_MAX_NUMBER_DEFAULT;
        public int allowedByGTTechLevel = StaticVariables.ALLOWED_BY_G_T_TECH_LEVEL_DEFAULT;

        // 装備強化機能 Refine Gear Function
        public bool enableRefineGearOption = StaticVariables.ENABLE_REFINE_GEAR_OPTION_DEFAULT;
        public float refineGearChanceFactor = StaticVariables.REFINE_GEAR_CHANCE_FACTOR_DEFAULT;
        public float refineGearChanceMax = StaticVariables.REFINE_GEAR_CHANCE_MAX_DEFAULT;
        public float refineGearChanceNegativeCurve = StaticVariables.REFINE_GEAR_CHANCE_NEGATIVE_CURVE_DEFAULT;
        public int qualityUpMaxNum = StaticVariables.QUALITY_UP_MAX_NUM_DEFAULT;
        public bool optionSetBiocode = StaticVariables.OPTION_SET_BIOCODE_DEFAULT;
        public bool optionAddDeathAcidifier = StaticVariables.OPTION_ADD_DEATH_ACIDIFIER_DEFAULT;

        // 薬物効果追加機能 Add Drug Effects Function
        public bool enableAddDrugOption = StaticVariables.ENABLE_ADD_DRUG_OPTION_DEFAULT;
        public float addDrugChanceFactor = StaticVariables.ADD_DRUG_CHANCE_FACTOR_DEFAULT;
        public float addDrugChanceMax = StaticVariables.ADD_DRUG_CHANCE_MAX_DEFAULT;
        public float addDrugChanceNegativeCurve = StaticVariables.ADD_DRUG_CHANCE_NEGATIVE_CURVE_DEFAULT;
        public int addDrugMaxNumber = StaticVariables.ADD_DRUG_MAX_NUMBER_DEFAULT;
        public bool giveOnlyActiveIngredients = StaticVariables.GIVE_ONLY_ACTIVE_INGREDIENTS_DEFAULT;
        public bool ignoreNonVolatilityDrugs = StaticVariables.IGNORE_NON_VOLATILITY_DRUGS_DEFAULT;
        public bool withoutSocialDrug = StaticVariables.WITHOUT_SOCIAL_DRUG_DEFAULT;
        public bool withoutHardDrug = StaticVariables.WITHOUT_HARD_DRUG_DEFAULT;
        public bool withoutMedicalDrug = StaticVariables.WITHOUT_MEDICAL_DRUG_DEFAULT;
        public bool withoutNonVolatilityDrug = StaticVariables.WITHOUT_NON_VOLATILITY_DRUG_DEFAULT;
        public bool limitationsByTechLevel = StaticVariables.LIMITATIONS_BY_TECH_LEVEL_DEFAULT;
        public int techLevelUpperRange = StaticVariables.TECH_LEVEL_UPPER_RANGE_DEFAULT;
        public int techLevelLowerRange = StaticVariables.TECH_LEVEL_LOWER_RANGE_DEFAULT;
    }
}
