using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using System.Text.RegularExpressions;
using RimWorld;

namespace CompressedRaid
{
    [StaticConstructorOnStartup]
    public class UIUtility
    {
        public const float MARGIN_LEFT = 5f;
        public const float MARGIN_TOP = 5f;
        public const float MARGIN_BOTTOM = 5f;
        public const float HEIGHT_ROW = 30f;
        public const float HEIGHT_SECTION_TITLE = 30f;
        public const float HEIGHT_HEADER = 40f;
        public const float MARGIN_HEADER = 10f;
        public const float WIDTH_CHECKBOX = 30f;
        public const float WIDTH_BUTTON = 30f;

        public const float LABEL_WIDTH_MIN = 125;
        public const float INPUT_WIDTH_MIN = 125;

        public const float MARGIN_RADIO = 200f;

        public const float WIDTH_QUARTER = 187.5f;
        public const float WIDTH_THREE_QUARTERS = 562.5f;
        public const float WIDTH_THREE_QUARTERS_HALF = 281.25f;
        public const float WIDTH_TWO_QUARTERS = 375f;


        public const float LABEL_WIDTH_HALF = 375f;
        public const float INPUT_WIDTH_HALF = 375f;
        public static readonly Color SUB_SECTION_BACKGROUND_ACTIVE_COLOR = Color.HSVToRGB(0.675f, 0.75f, 0.05f);
        public static readonly Color SUB_SECTION_BACKGROUND_DEACTIVE_COLOR = Color.HSVToRGB(0f, 0f, 0.25f);
        private static readonly int[] FLOAT_MEN_OBJECTS = new int[] { 0 };
        private static readonly Texture2D INITIALIZE_VALUE_TEX = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/NewFile", true);
        public static readonly int TECH_LEVEL_MAX = Enum.GetValues(typeof(TechLevel)).Cast<byte>().Max();
        public static readonly int QUALITY_CATEGORY_MAX = Enum.GetValues(typeof(QualityCategory)).Cast<byte>().Max();

        public static void ScrollBlock(Rect rect, ref Vector2 scrollPosition, ref Rect viewRect, ref float viewHeight, ref float viewHeightCache, Action inBlock)
        {
            Rect baseRect = rect;
            //rect.height = 100000f;
            //rect.width -= 20f;
            Widgets.BeginScrollView(rect, ref scrollPosition, viewRect, true);

            inBlock();

            viewRect = new Rect(0f, 0f, baseRect.width, viewHeight);
            Widgets.EndScrollView();
            viewHeightCache = viewHeight;
        }

        public static void HighlightRect(Rect rect, params Rect[] linkRect)
        {
            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
                if (linkRect != null)
                {
                    foreach (Rect link in linkRect)
                    {
                        Widgets.DrawHighlight(link);
                    }
                }
            }
        }

        public static void ToolTipRow(Rect row, string tooltip, float marginRight = UIUtility.WIDTH_BUTTON - UIUtility.MARGIN_LEFT)
        {
            Rect tipPane = new Rect(row);
            tipPane.width -= marginRight;
            if (Mouse.IsOver(tipPane))
            {
                if (tooltip != null)
                {
                    TooltipHandler.TipRegion(tipPane, tooltip);
                }
            }

        }

        public static float WriteFixedSectionTitle(Rect inRect, string title)
        {
            Text.Font = GameFont.Medium;
            Widgets.Label(inRect, title);
            Text.Font = GameFont.Small;
            Widgets.DrawLineHorizontal(inRect.x, inRect.y + UIUtility.HEIGHT_SECTION_TITLE, inRect.width);
            return UIUtility.HEIGHT_SECTION_TITLE;
        }

        public static float WriteFloatedSectionTitle(Rect inRect, string title, ref bool checkOn, ref bool valueChanged, Action initializeAction, bool noInitButton = false)
        {
            Text.Font = GameFont.Medium;
            LabeledCheckBoxLeft(inRect, title, ref checkOn, ref valueChanged, initializeAction, noInitButton);
            Text.Font = GameFont.Small;
            Widgets.DrawLineHorizontal(inRect.x + MARGIN_LEFT, inRect.y + UIUtility.HEIGHT_SECTION_TITLE, inRect.width - MARGIN_LEFT);
            return UIUtility.HEIGHT_SECTION_TITLE;
        }

        public static void LabeledRadioButton(Rect inRect, string text, ref bool radio, ref bool valueChanged)
        {
            radio = Widgets.RadioButtonLabeled(inRect, text, !radio);
        }

        private static bool SingleCheckBox(float x, float y, ref bool checkOn, ref bool valueChanged)
        {
            bool bk = checkOn;

            Vector2 checkVec = new Vector2(x, y);
            Widgets.Checkbox(checkVec, ref checkOn);

            valueChanged |= bk != checkOn;
            return bk != checkOn;
        }

        public static bool SingleCheckBox(Rect inRect, ref bool checkOn, ref bool valueChanged)
        {
            float x = inRect.x + ((WIDTH_CHECKBOX - Widgets.CheckboxSize) * 0.5f);
            float y = inRect.y + ((HEIGHT_ROW - Widgets.CheckboxSize) * 0.5f);
            return SingleCheckBox(x, y, ref checkOn, ref valueChanged);
        }

        public static bool LabeledCheckBoxLeft(Rect inRect, string text, ref bool checkOn, ref bool valueChanged, Action initializeAction, bool noInitButton = false)
        {
            bool nowChanged = SingleCheckBox(inRect, ref checkOn, ref valueChanged);
            Rect labelRect = new Rect(inRect.x + WIDTH_CHECKBOX, inRect.y, inRect.width - WIDTH_CHECKBOX, inRect.height);
            Widgets.Label(labelRect, text);
            InitializeValueButtonEntry(noInitButton, inRect, initializeAction, text);
            return nowChanged;
        }

        public static bool LabeledCheckBoxRight(Rect inRect, float labelWidth, string text, ref bool checkOn, ref bool valueChanged, Action initializeAction, bool noInitButton = false)
        {
            Rect labelRect = new Rect(inRect.x, inRect.y, labelWidth, inRect.height);
            Widgets.Label(labelRect, text);

            InitializeValueButtonEntry(noInitButton, inRect, initializeAction, text);

            return SingleCheckBox(inRect.x + labelWidth, inRect.y, ref checkOn, ref valueChanged);
        }

        private static void InitializeValueButton(Rect inRect, string floatMenuText, Action floatMenuAction)
        {
            if (Widgets.ButtonImage(inRect, INITIALIZE_VALUE_TEX))
            {
                FloatMenuUtility.MakeMenu<int>(FLOAT_MEN_OBJECTS,
                    x => floatMenuText,
                    x => floatMenuAction);
            }
        }

        public static void InitializeValueButtonEntry(bool noInitButton, Rect inRect, Action initializeAction, string text)
        {
            if (!noInitButton)
            {
                Rect initializeValueRect = new Rect(inRect.x + inRect.width - Widgets.CheckboxSize, inRect.y, Widgets.CheckboxSize, HEIGHT_ROW);
                UIUtility.InitializeValueButton(initializeValueRect, "CR_InitializeValue".Translate(text), () => {
                    initializeAction();
                });
                ToolTipRow(initializeValueRect, "CR_InitializeValue".Translate(text), 0f);
            }

        }

        public static bool LabeledTextBoxPercentage(Rect inRect, float labelWidth, float textWidth, string text, ref float val, ref string buf, ref bool valueChanged, float min, float max, Action initializeAction, Func<float, string> labelFormatter, TextAnchor labelAnchor = TextAnchor.MiddleLeft, bool noInitButton = false)
        {
            float bk = val;
            TextAnchor bkAnchor = Text.Anchor;

            Text.Anchor = labelAnchor;
            Rect labelRect = new Rect(inRect.x, inRect.y, labelWidth, HEIGHT_ROW);
            Widgets.Label(labelRect, text);
            Text.Anchor = bkAnchor;

            Rect textRect = new Rect(inRect.x + labelWidth, inRect.y, textWidth, HEIGHT_ROW);
            Widgets.TextFieldNumericLabeled<float>(textRect, labelFormatter == null ? String.Format("[{0:F2}%]", val * 100f) : labelFormatter(val), ref val, ref buf, min, max);
            if (float.TryParse(buf, out float result))
            {
                val = result;
            }

            InitializeValueButtonEntry(noInitButton, inRect, initializeAction, text);

            valueChanged |= bk != val;
            return bk != val;
        }
        public static bool LabeledTextBoxPercentage(Rect inRect, float labelWidth, float textWidth, string text, ref float val, ref string buf, ref bool valueChanged, float min, float max, Action initializeAction, TextAnchor labelAnchor = TextAnchor.MiddleLeft, bool noInitButton = false)
        {
            return LabeledTextBoxPercentage(inRect, labelWidth, textWidth, text, ref val, ref buf, ref valueChanged, min, max, initializeAction, null, labelAnchor, noInitButton);
        }

        public static bool LabeledTextBoxInt(Rect inRect, float labelWidth, float textWidth, string text, ref int val, ref string buf, ref bool valueChanged, int min, int max, Action initializeAction, Func<int, string> labelFormatter, TextAnchor labelAnchor = TextAnchor.MiddleLeft, bool noInitButton = false)
        {
            int bk = val;
            TextAnchor bkAnchor = Text.Anchor;

            Text.Anchor = labelAnchor;
            Rect labelRect = new Rect(inRect.x, inRect.y, labelWidth, HEIGHT_ROW);
            Widgets.Label(labelRect, text);
            Text.Anchor = bkAnchor;

            Rect textRect = new Rect(inRect.x + labelWidth, inRect.y, textWidth, HEIGHT_ROW);
            Widgets.TextFieldNumericLabeled<int>(textRect, labelFormatter == null ? null : labelFormatter(val), ref val, ref buf, min, max);
            if (int.TryParse(buf, out int result))
            {
                val = result;
            }

            InitializeValueButtonEntry(noInitButton, inRect, initializeAction, text);

            valueChanged |= bk != val;
            return bk != val;
        }
        public static bool LabeledTextBoxInt(Rect inRect, float labelWidth, float textWidth, string text, ref int val, ref string buf, ref bool valueChanged, int min, int max, Action initializeAction, TextAnchor labelAnchor = TextAnchor.MiddleLeft, bool noInitButton = false)
        {
            return LabeledTextBoxInt(inRect, labelWidth, textWidth, text, ref val, ref buf, ref valueChanged, min, max, initializeAction, null, labelAnchor, noInitButton);
        }

        public static bool LabeledSliderPercentage(Rect inRect, float labelWidth, float textWidth, string text, ref float val, ref bool valueChanged, float min, float max, Action initializeAction, Func<float, string> labelFormatter, TextAnchor labelAnchor = TextAnchor.MiddleLeft, bool noInitButton = false)
        {
            float bk = val;
            TextAnchor bkAnchor = Text.Anchor;

            Text.Anchor = labelAnchor;
            Rect labelRect = new Rect(inRect.x, inRect.y, labelWidth, HEIGHT_ROW);
            Widgets.Label(labelRect, text);
            Text.Anchor = bkAnchor;

            Rect sliderRect = new Rect(inRect.x + labelWidth, inRect.y, textWidth, HEIGHT_ROW);
            val = Widgets.HorizontalSlider(sliderRect, val, min, max, false, labelFormatter == null ? String.Format("[{0:F2} : {1:F2}%]", val, val * 100f) : labelFormatter(val), min.ToString(), max.ToString(), 0.01f);

            InitializeValueButtonEntry(noInitButton, inRect, initializeAction, text);

            valueChanged |= bk != val;
            return bk != val;
        }
        public static bool LabeledSliderPercentage(Rect inRect, float labelWidth, float textWidth, string text, ref float val, ref bool valueChanged, float min, float max, Action initializeAction, TextAnchor labelAnchor = TextAnchor.MiddleLeft, bool noInitButton = false)
        {
            return LabeledSliderPercentage(inRect, labelWidth, textWidth, text, ref val, ref valueChanged, min, max, initializeAction, null, labelAnchor, noInitButton);
        }

        public static bool LabeledSliderInt(Rect inRect, float labelWidth, float textWidth, string text, ref int val, ref bool valueChanged, float min, float max, Action initializeAction, Func<int, string> labelFormatter, TextAnchor labelAnchor = TextAnchor.MiddleLeft, bool noInitButton = false)
        {
            int bk = val;
            TextAnchor bkAnchor = Text.Anchor;

            Text.Anchor = labelAnchor;
            Rect labelRect = new Rect(inRect.x, inRect.y, labelWidth, HEIGHT_ROW);
            Widgets.Label(labelRect, text);
            Text.Anchor = bkAnchor;

            Rect sliderRect = new Rect(inRect.x + labelWidth, inRect.y, textWidth, HEIGHT_ROW);
            val = (int)Widgets.HorizontalSlider(sliderRect, val * 1f, min, max, false, labelFormatter == null ? val.ToString() : labelFormatter(val), min.ToString(), max.ToString(), 1f);

            InitializeValueButtonEntry(noInitButton, inRect, initializeAction, text);

            valueChanged |= bk != val;
            return bk != val;
        }
        public static bool LabeledSliderInt(Rect inRect, float labelWidth, float textWidth, string text, ref int val, ref bool valueChanged, float min, float max, Action initializeAction, TextAnchor labelAnchor = TextAnchor.MiddleLeft, bool noInitButton = false)
        {
            return LabeledSliderInt(inRect, labelWidth, textWidth, text, ref val, ref valueChanged, min, max, initializeAction, null, labelAnchor, noInitButton);
        }

        public class NumericBuffer
        {
            ////共通設定 Common Settings
            public static string chanceOfCompression;
            public static string maxRaidPawnsCount;
            public static string gainFactorMult;
            public static string chanceOfEnhancement;
            public static string gainFactorMultRaidFriendly;

            ////メイン強化機能 Main Enhance Function
            public static string gainFactorMultPain;
            public static string gainFactorMultArmorRating_Blunt;
            public static string gainFactorMultArmorRating_Sharp;
            public static string gainFactorMultArmorRating_Heat;
            public static string gainFactorMultMeleeDodgeChance;
            public static string gainFactorMultMeleeHitChance;
            public static string gainFactorMultMoveSpeed;
            public static string gainFactorMultShootingAccuracyPawn;
            public static string gainFactorMultPawnTrapSpringChance;
            public static string gainFactorMaxPain;
            public static string gainFactorMaxArmorRating_Blunt;
            public static string gainFactorMaxArmorRating_Sharp;
            public static string gainFactorMaxArmorRating_Heat;
            public static string gainFactorMaxMeleeDodgeChance;
            public static string gainFactorMaxMeleeHitChance;
            public static string gainFactorMaxMoveSpeed;
            public static string gainFactorMaxShootingAccuracyPawn;
            public static string gainFactorMaxPawnTrapSpringChance;
            public static string gainFactorMultCapacitySight;
            public static string gainFactorMultCapacityMoving;
            public static string gainFactorMultCapacityHearing;
            public static string gainFactorMultCapacityManipulation;
            public static string gainFactorMultCapacityMetabolism;
            public static string gainFactorMultCapacityConsciousness;
            public static string gainFactorMultCapacityBloodFiltration;
            public static string gainFactorMultCapacityBloodPumping;
            public static string gainFactorMultCapacityBreathing;
            public static string gainFactorMaxCapacitySight;
            public static string gainFactorMaxCapacityMoving;
            public static string gainFactorMaxCapacityHearing;
            public static string gainFactorMaxCapacityManipulation;
            public static string gainFactorMaxCapacityMetabolism;
            public static string gainFactorMaxCapacityConsciousness;
            public static string gainFactorMaxCapacityBloodFiltration;
            public static string gainFactorMaxCapacityBloodPumping;
            public static string gainFactorMaxCapacityBreathing;

            //// バイオニクス追加機能 Add Bionics Function
            public static string addBionicsChanceFactor;
            public static string addBionicsChanceMax;
            public static string addBionicsChanceNegativeCurve;
            public static string addBionicsMaxNumber;
            public static string allowedByGTTechLevel;

            //// 装備強化機能 Refine Gear Function
            public static string refineGearChanceFactor;
            public static string refineGearChanceMax;
            public static string refineGearChanceNegativeCurve;
            public static string qualityUpMaxNum;

            //// 薬物効果追加機能 Add Drug Effects Function
            public static string addDrugChanceFactor;
            public static string addDrugChanceMax;
            public static string addDrugChanceNegativeCurve;
            public static string addDrugMaxNumber;
            public static string techLevelUpperRange;
            public static string techLevelLowerRange;
        }
    }
}
