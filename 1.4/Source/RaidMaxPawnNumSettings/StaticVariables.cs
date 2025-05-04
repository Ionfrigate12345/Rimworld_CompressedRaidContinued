using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CompressedRaid
{
    public static class StaticVariables
    {
        private static Dictionary<string, object> m_DefaultValues = new Dictionary<string, object>();

        public static T GetValue<T>(string key)
        {
            if (m_DefaultValues.TryGetValue(key, out object value))
            {
                return (T)value;
            }
            return default(T);
        }

        static StaticVariables()
        {
            m_DefaultValues["chanceOfCompression"] = CHANCE_OF_COMPRESSION_DEFAULT;
            m_DefaultValues["allowMechanoids"] = ALLOW_MECHANOIDS_DEFAULT;
            m_DefaultValues["allowInsectoids"] = ALLOW_INSECTOIDS_DEFAULT;
            m_DefaultValues["maxRaidPawnsCount"] = MAX_RAID_PAWNS_COUNT_DEFAULT;
            m_DefaultValues["gainFactorMult"] = GAIN_FACTOR_MULT_DEFAULT;
            m_DefaultValues["chanceOfEnhancement"] = CHANCE_OF_ENHANCEMENT_DEFAULT;
            m_DefaultValues["calcBaseMultByEnhanced"] = CALC_BASE_MULT_BY_ENHANCED_DEFAULT;
            m_DefaultValues["allowRaidFriendly"] = ALLOW_RAID_FRIENDLY_DEFAULT;
            m_DefaultValues["allowManhunters"] = ALLOW_MANHUNTERS_DEFAULT;
            m_DefaultValues["gainFactorMultRaidFriendly"] = GAIN_FACTOR_MULT_RAID_FRIENDLY_DEFAULT;
            m_DefaultValues["displayMessage"] = DISPLAY_MESSAGE_DEFAULT;

            m_DefaultValues["enableMainEnhance"] = ENABLE_MAIN_ENHANCE_DEFAULT;
            m_DefaultValues["gainFactorMultPain"] = GAIN_FACTOR_MULT_PAIN_DEFAULT;
            m_DefaultValues["gainFactorMultArmorRating_Blunt"] = GAIN_FACTOR_MULT_ARMOR_RATING_BLUNT_DEFAULT;
            m_DefaultValues["gainFactorMultArmorRating_Sharp"] = GAIN_FACTOR_MULT_ARMOR_RATING_SHARP_DEFAULT;
            m_DefaultValues["gainFactorMultArmorRating_Heat"] = GAIN_FACTOR_MULT_ARMOR_RATING_HEAT_DEFAULT;
            m_DefaultValues["gainFactorMultMeleeDodgeChance"] = GAIN_FACTOR_MULT_MELEE_DODGE_CHANCE_DEFAULT;
            m_DefaultValues["gainFactorMultMeleeHitChance"] = GAIN_FACTOR_MULT_MELEE_HIT_CHANCE_DEFAULT;
            m_DefaultValues["gainFactorMultMoveSpeed"] = GAIN_FACTOR_MULT_MOVE_SPEED_DEFAULT;
            m_DefaultValues["gainFactorMultShootingAccuracyPawn"] = GAIN_FACTOR_MULT_SHOOTING_ACCURACY_PAWN_DEFAULT;
            m_DefaultValues["gainFactorMultPawnTrapSpringChance"] = GAIN_FACTOR_MULT_PAWN_TRAP_SPRING_CHANCE_DEFAULT;
            m_DefaultValues["gainFactorMaxPain"] = GAIN_FACTOR_MAX_PAIN_DEFAULT;
            m_DefaultValues["gainFactorMaxArmorRating_Blunt"] = GAIN_FACTOR_MAX_ARMOR_RATING_BLUNT_DEFAULT;
            m_DefaultValues["gainFactorMaxArmorRating_Sharp"] = GAIN_FACTOR_MAX_ARMOR_RATING_SHARP_DEFAULT;
            m_DefaultValues["gainFactorMaxArmorRating_Heat"] = GAIN_FACTOR_MAX_ARMOR_RATING_HEAT_DEFAULT;
            m_DefaultValues["gainFactorMaxMeleeDodgeChance"] = GAIN_FACTOR_MAX_MELEE_DODGE_CHANCE_DEFAULT;
            m_DefaultValues["gainFactorMaxMeleeHitChance"] = GAIN_FACTOR_MAX_MELEE_HIT_CHANCE_DEFAULT;
            m_DefaultValues["gainFactorMaxMoveSpeed"] = GAIN_FACTOR_MAX_MOVE_SPEED_DEFAULT;
            m_DefaultValues["gainFactorMaxShootingAccuracyPawn"] = GAIN_FACTOR_MAX_SHOOTING_ACCURACY_PAWN_DEFAULT;
            m_DefaultValues["gainFactorMaxPawnTrapSpringChance"] = GAIN_FACTOR_MAX_PAWN_TRAP_SPRING_CHANCE_DEFAULT;
            m_DefaultValues["gainFactorMultCapacitySight"] = GAIN_FACTOR_MULT_CAPACITY_SIGHT_DEFAULT;
            m_DefaultValues["gainFactorMultCapacityMoving"] = GAIN_FACTOR_MULT_CAPACITY_MOVING_DEFAULT;
            m_DefaultValues["gainFactorMultCapacityHearing"] = GAIN_FACTOR_MULT_CAPACITY_HEARING_DEFAULT;
            m_DefaultValues["gainFactorMultCapacityManipulation"] = GAIN_FACTOR_MULT_CAPACITY_MANIPULATION_DEFAULT;
            m_DefaultValues["gainFactorMultCapacityMetabolism"] = GAIN_FACTOR_MULT_CAPACITY_METABOLISM_DEFAULT;
            m_DefaultValues["gainFactorMultCapacityConsciousness"] = GAIN_FACTOR_MULT_CAPACITY_CONSCIOUSNESS_DEFAULT;
            m_DefaultValues["gainFactorMultCapacityBloodFiltration"] = GAIN_FACTOR_MULT_CAPACITY_BLOOD_FILTRATION_DEFAULT;
            m_DefaultValues["gainFactorMultCapacityBloodPumping"] = GAIN_FACTOR_MULT_CAPACITY_BLOOD_PUMPING_DEFAULT;
            m_DefaultValues["gainFactorMultCapacityBreathing"] = GAIN_FACTOR_MULT_CAPACITY_BREATHING_DEFAULT;
            m_DefaultValues["gainFactorMaxCapacitySight"] = GAIN_FACTOR_MAX_CAPACITY_SIGHT_DEFAULT;
            m_DefaultValues["gainFactorMaxCapacityMoving"] = GAIN_FACTOR_MAX_CAPACITY_MOVING_DEFAULT;
            m_DefaultValues["gainFactorMaxCapacityHearing"] = GAIN_FACTOR_MAX_CAPACITY_HEARING_DEFAULT;
            m_DefaultValues["gainFactorMaxCapacityManipulation"] = GAIN_FACTOR_MAX_CAPACITY_MANIPULATION_DEFAULT;
            m_DefaultValues["gainFactorMaxCapacityMetabolism"] = GAIN_FACTOR_MAX_CAPACITY_METABOLISM_DEFAULT;
            m_DefaultValues["gainFactorMaxCapacityConsciousness"] = GAIN_FACTOR_MAX_CAPACITY_CONSCIOUSNESS_DEFAULT;
            m_DefaultValues["gainFactorMaxCapacityBloodFiltration"] = GAIN_FACTOR_MAX_CAPACITY_BLOOD_FILTRATION_DEFAULT;
            m_DefaultValues["gainFactorMaxCapacityBloodPumping"] = GAIN_FACTOR_MAX_CAPACITY_BLOOD_PUMPING_DEFAULT;
            m_DefaultValues["gainFactorMaxCapacityBreathing"] = GAIN_FACTOR_MAX_CAPACITY_BREATHING_DEFAULT;

            m_DefaultValues["enableAddBionicsOption"] = ENABLE_ADD_BIONICS_OPTION_DEFAULT;
            m_DefaultValues["addBionicsChanceFactor"] = ADD_BIONICS_CHANCE_FACTOR_DEFAULT;
            m_DefaultValues["addBionicsChanceMax"] = ADD_BIONICS_CHANCE_MAX_DEFAULT;
            m_DefaultValues["addBionicsChanceNegativeCurve"] = ADD_BIONICS_CHANCE_NEGATIVE_CURVE_DEFAULT;
            m_DefaultValues["addBionicsMaxNumber"] = ADD_BIONICS_MAX_NUMBER_DEFAULT;
            m_DefaultValues["allowedByGTTechLevel"] = ALLOWED_BY_G_T_TECH_LEVEL_DEFAULT;

            m_DefaultValues["enableRefineGearOption"] = ENABLE_REFINE_GEAR_OPTION_DEFAULT;
            m_DefaultValues["refineGearChanceFactor"] = REFINE_GEAR_CHANCE_FACTOR_DEFAULT;
            m_DefaultValues["refineGearChanceMax"] = REFINE_GEAR_CHANCE_MAX_DEFAULT;
            m_DefaultValues["refineGearChanceNegativeCurve"] = REFINE_GEAR_CHANCE_NEGATIVE_CURVE_DEFAULT;
            m_DefaultValues["qualityUpMaxNum"] = QUALITY_UP_MAX_NUM_DEFAULT;
            m_DefaultValues["optionSetBiocode"] = OPTION_SET_BIOCODE_DEFAULT;
            m_DefaultValues["optionAddDeathAcidifier"] = OPTION_ADD_DEATH_ACIDIFIER_DEFAULT;

            m_DefaultValues["enableAddDrugOption"] = ENABLE_ADD_DRUG_OPTION_DEFAULT;
            m_DefaultValues["addDrugChanceFactor"] = ADD_DRUG_CHANCE_FACTOR_DEFAULT;
            m_DefaultValues["addDrugChanceMax"] = ADD_DRUG_CHANCE_MAX_DEFAULT;
            m_DefaultValues["addDrugChanceNegativeCurve"] = ADD_DRUG_CHANCE_NEGATIVE_CURVE_DEFAULT;
            m_DefaultValues["addDrugMaxNumber"] = ADD_DRUG_MAX_NUMBER_DEFAULT;
            m_DefaultValues["giveOnlyActiveIngredients"] = GIVE_ONLY_ACTIVE_INGREDIENTS_DEFAULT;
            m_DefaultValues["ignoreNonVolatilityDrugs"] = IGNORE_NON_VOLATILITY_DRUGS_DEFAULT;
            m_DefaultValues["withoutSocialDrug"] = WITHOUT_SOCIAL_DRUG_DEFAULT;
            m_DefaultValues["withoutHardDrug"] = WITHOUT_HARD_DRUG_DEFAULT;
            m_DefaultValues["withoutMedicalDrug"] = WITHOUT_MEDICAL_DRUG_DEFAULT;
            m_DefaultValues["withoutNonVolatilityDrug"] = WITHOUT_NON_VOLATILITY_DRUG_DEFAULT;
            m_DefaultValues["limitationsByTechLevel"] = LIMITATIONS_BY_TECH_LEVEL_DEFAULT;
            m_DefaultValues["techLevelUpperRange"] = TECH_LEVEL_UPPER_RANGE_DEFAULT;
            m_DefaultValues["techLevelLowerRange"] = TECH_LEVEL_LOWER_RANGE_DEFAULT;
        }

        public const float CHANCE_OF_COMPRESSION_DEFAULT = 1f;
        public const bool ALLOW_MECHANOIDS_DEFAULT = true;
        public const bool ALLOW_INSECTOIDS_DEFAULT = true;
        public const int MAX_RAID_PAWNS_COUNT_DEFAULT = 100;
        public const float GAIN_FACTOR_MULT_DEFAULT = 1f;
        public const float CHANCE_OF_ENHANCEMENT_DEFAULT = 1f;
        public const bool CALC_BASE_MULT_BY_ENHANCED_DEFAULT = true;
        public const bool ALLOW_RAID_FRIENDLY_DEFAULT = false;
        public const bool ALLOW_MANHUNTERS_DEFAULT = true;
        public const float GAIN_FACTOR_MULT_RAID_FRIENDLY_DEFAULT = 1f;
        public const bool DISPLAY_MESSAGE_DEFAULT = true;

        public const bool ENABLE_MAIN_ENHANCE_DEFAULT = true;
        public const float GAIN_FACTOR_MULT_PAIN_DEFAULT = 0.25f;
        public const float GAIN_FACTOR_MULT_ARMOR_RATING_BLUNT_DEFAULT = 0.25f;
        public const float GAIN_FACTOR_MULT_ARMOR_RATING_SHARP_DEFAULT = 0.25f;
        public const float GAIN_FACTOR_MULT_ARMOR_RATING_HEAT_DEFAULT = 0.25f;
        public const float GAIN_FACTOR_MULT_MELEE_DODGE_CHANCE_DEFAULT = 1f;
        public const float GAIN_FACTOR_MULT_MELEE_HIT_CHANCE_DEFAULT = 1f;
        public const float GAIN_FACTOR_MULT_MOVE_SPEED_DEFAULT = 0.5f;
        public const float GAIN_FACTOR_MULT_SHOOTING_ACCURACY_PAWN_DEFAULT = 0.25f;
        public const float GAIN_FACTOR_MULT_PAWN_TRAP_SPRING_CHANCE_DEFAULT = 0.5f;
        public const float GAIN_FACTOR_MAX_PAIN_DEFAULT = 1f;
        public const float GAIN_FACTOR_MAX_ARMOR_RATING_BLUNT_DEFAULT = 1f;
        public const float GAIN_FACTOR_MAX_ARMOR_RATING_SHARP_DEFAULT = 1f;
        public const float GAIN_FACTOR_MAX_ARMOR_RATING_HEAT_DEFAULT = 1f;
        public const float GAIN_FACTOR_MAX_MELEE_DODGE_CHANCE_DEFAULT = 10f;
        public const float GAIN_FACTOR_MAX_MELEE_HIT_CHANCE_DEFAULT = 10f;
        public const float GAIN_FACTOR_MAX_MOVE_SPEED_DEFAULT = 10f;
        public const float GAIN_FACTOR_MAX_SHOOTING_ACCURACY_PAWN_DEFAULT = 10f;
        public const float GAIN_FACTOR_MAX_PAWN_TRAP_SPRING_CHANCE_DEFAULT = 1f;
        public const float GAIN_FACTOR_MULT_CAPACITY_SIGHT_DEFAULT = 0.5f;
        public const float GAIN_FACTOR_MULT_CAPACITY_MOVING_DEFAULT = 0.5f;
        public const float GAIN_FACTOR_MULT_CAPACITY_HEARING_DEFAULT = 0.25f;
        public const float GAIN_FACTOR_MULT_CAPACITY_MANIPULATION_DEFAULT = 0.5f;
        public const float GAIN_FACTOR_MULT_CAPACITY_METABOLISM_DEFAULT = 0.25f;
        public const float GAIN_FACTOR_MULT_CAPACITY_CONSCIOUSNESS_DEFAULT = 0.5f;
        public const float GAIN_FACTOR_MULT_CAPACITY_BLOOD_FILTRATION_DEFAULT = 0.25f;
        public const float GAIN_FACTOR_MULT_CAPACITY_BLOOD_PUMPING_DEFAULT = 0.25f;
        public const float GAIN_FACTOR_MULT_CAPACITY_BREATHING_DEFAULT = 0.25f;
        public const float GAIN_FACTOR_MAX_CAPACITY_SIGHT_DEFAULT = 5f;
        public const float GAIN_FACTOR_MAX_CAPACITY_MOVING_DEFAULT = 5f;
        public const float GAIN_FACTOR_MAX_CAPACITY_HEARING_DEFAULT = 5f;
        public const float GAIN_FACTOR_MAX_CAPACITY_MANIPULATION_DEFAULT = 5f;
        public const float GAIN_FACTOR_MAX_CAPACITY_METABOLISM_DEFAULT = 5f;
        public const float GAIN_FACTOR_MAX_CAPACITY_CONSCIOUSNESS_DEFAULT = 5f;
        public const float GAIN_FACTOR_MAX_CAPACITY_BLOOD_FILTRATION_DEFAULT = 5f;
        public const float GAIN_FACTOR_MAX_CAPACITY_BLOOD_PUMPING_DEFAULT = 5f;
        public const float GAIN_FACTOR_MAX_CAPACITY_BREATHING_DEFAULT = 5f;

        public const bool ENABLE_ADD_BIONICS_OPTION_DEFAULT = false;
        public const float ADD_BIONICS_CHANCE_FACTOR_DEFAULT = 2f;
        public const float ADD_BIONICS_CHANCE_MAX_DEFAULT = 0.9f;
        public const float ADD_BIONICS_CHANCE_NEGATIVE_CURVE_DEFAULT = 0.5f;
        public const int ADD_BIONICS_MAX_NUMBER_DEFAULT = 4;
        public const int ALLOWED_BY_G_T_TECH_LEVEL_DEFAULT = 0;

        public const bool ENABLE_REFINE_GEAR_OPTION_DEFAULT = false;
        public const float REFINE_GEAR_CHANCE_FACTOR_DEFAULT = 1f;
        public const float REFINE_GEAR_CHANCE_MAX_DEFAULT = 0.9f;
        public const float REFINE_GEAR_CHANCE_NEGATIVE_CURVE_DEFAULT = 0.5f;
        public const int QUALITY_UP_MAX_NUM_DEFAULT = 4;
        public const bool OPTION_SET_BIOCODE_DEFAULT = false;
        public const bool OPTION_ADD_DEATH_ACIDIFIER_DEFAULT = false;

        public const bool ENABLE_ADD_DRUG_OPTION_DEFAULT = false;
        public const float ADD_DRUG_CHANCE_FACTOR_DEFAULT = 1f;
        public const float ADD_DRUG_CHANCE_MAX_DEFAULT = 0.9f;
        public const float ADD_DRUG_CHANCE_NEGATIVE_CURVE_DEFAULT = 0.5f;
        public const int ADD_DRUG_MAX_NUMBER_DEFAULT = 1;
        public const bool GIVE_ONLY_ACTIVE_INGREDIENTS_DEFAULT = true;
        public const bool IGNORE_NON_VOLATILITY_DRUGS_DEFAULT = true;
        public const bool WITHOUT_SOCIAL_DRUG_DEFAULT = false;
        public const bool WITHOUT_HARD_DRUG_DEFAULT = false;
        public const bool WITHOUT_MEDICAL_DRUG_DEFAULT = false;
        public const bool WITHOUT_NON_VOLATILITY_DRUG_DEFAULT = false;
        public const bool LIMITATIONS_BY_TECH_LEVEL_DEFAULT = true;
        public const int TECH_LEVEL_UPPER_RANGE_DEFAULT = 2;
        public const int TECH_LEVEL_LOWER_RANGE_DEFAULT = 2;
    }
}
