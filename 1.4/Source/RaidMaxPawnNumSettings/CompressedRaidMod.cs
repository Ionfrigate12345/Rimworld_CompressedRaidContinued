using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using Verse.Sound;

namespace CompressedRaid
{
    [StaticConstructorOnStartup]
    public class CompressedRaidMod : Mod
    {
        public enum FunctionType
        {
            Common,
            Main,
            AddBionics,
            RefineGear,
            AddDrugEffects,
        }

        private const string MOD_SETTING_CATEGORY = "Compressed Raid";
        public override string SettingsCategory() => MOD_SETTING_CATEGORY;

        public CompressedRaidMod(ModContentPack content) : base(content)
        {
            m_Settings = base.GetSettings<CompressedRaidModSettings>();
            HugsLibTransitionHelper.LoadHugsLibModSettings();
            SettingValues();
        }

        private void ResetAll()
        {
            m_Settings.InitializeValues(SettingValues, CompressedRaidMod.FunctionType.Common, CompressedRaidMod.FunctionType.Main, CompressedRaidMod.FunctionType.AddBionics, CompressedRaidMod.FunctionType.RefineGear, CompressedRaidMod.FunctionType.AddDrugEffects);
        }

        private Vector2 m_ScrollbarPosition = Vector2.zero;
        private float m_ScrollHeightBk = 0f;

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);

            bool valueChanged = false;

            Rect viewRect = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + UIUtility.MARGIN_TOP, 0.95f * inRect.width, m_ScrollHeightBk);

            float rectBaseX = viewRect.x;
            float rectBaseY = viewRect.y;

            //valueChanged = true;
            //m_Settings.maxRaidPawnsCount = 25;
            float viewHeightSum = 0f;

            GameFont fontBk = Text.Font;
            TextAnchor anchorBk = Text.Anchor;
            bool wordWrapBk = Text.WordWrap;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            Text.WordWrap = false;

            Rect textInputRect = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + UIUtility.MARGIN_TOP, UIUtility.INPUT_WIDTH_MIN, UIUtility.HEIGHT_HEADER);
            UIUtility.HighlightRect(textInputRect);
            bool radioTextInputBk = m_Settings.radioTextInput;
            if (Widgets.RadioButtonLabeled(textInputRect, "CR_TextInput".Translate(), m_Settings.radioTextInput))
            {
                if (!radioTextInputBk)
                {
                    m_Settings.radioSliderInput = false;
                    m_Settings.radioTextInput = true;
                    valueChanged = true;
                }
            }

            Rect sliderInputRect = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.MARGIN_RADIO, inRect.y + UIUtility.MARGIN_TOP, UIUtility.INPUT_WIDTH_MIN, UIUtility.HEIGHT_HEADER);
            UIUtility.HighlightRect(sliderInputRect);
            bool radioSliderInputBk = m_Settings.radioSliderInput;
            if (Widgets.RadioButtonLabeled(sliderInputRect, "CR_SliderInput".Translate(), m_Settings.radioSliderInput))
            {
                if (!radioSliderInputBk)
                {
                    m_Settings.radioSliderInput = true;
                    m_Settings.radioTextInput = false;
                    valueChanged = true;
                }
            }

            if (HugsLibTransitionHelper.HugsLibReady())
            {
                const float LOAD_OLD_SETTINGS_BUTTON_WIDTH = 300f;
                float loadOldSettingsButtonX = inRect.x + viewRect.width - Widgets.CheckboxSize - LOAD_OLD_SETTINGS_BUTTON_WIDTH - UIUtility.WIDTH_BUTTON - UIUtility.MARGIN_LEFT;
                Rect loadOldSettingsButtonRect = new Rect(loadOldSettingsButtonX, inRect.y + UIUtility.MARGIN_TOP, LOAD_OLD_SETTINGS_BUTTON_WIDTH, UIUtility.HEIGHT_ROW);
                if (Widgets.ButtonText(loadOldSettingsButtonRect, "CR_LoadOldSettingsButton".Translate()))
                {
                    SoundDefOf.Click.PlayOneShotOnCamera(null);
                    Dialog_LoadOldConfigConfirm dialog = new Dialog_LoadOldConfigConfirm()
                    {
                        m_LoadOldConfigAction = m_Settings.InitVariablesFromHugsLib,
                        m_PostAction = SettingValues,
                    };
                    Find.WindowStack.Add(dialog);
                }
            }

            Rect initializeValueAllRect = new Rect(inRect.x + viewRect.width - Widgets.CheckboxSize, inRect.y + UIUtility.MARGIN_TOP, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
            UIUtility.InitializeValueButtonEntry(false, initializeValueAllRect, () => {
                SoundDefOf.Click.PlayOneShotOnCamera(null);
                Dialog_ResetAllConfirm dialog = new Dialog_ResetAllConfirm()
                {
                    m_ResetAllAction = ResetAll,
                    m_PostAction = SettingValues,
                };
                Find.WindowStack.Add(dialog);
            }, "CR_InitializeValueAll".Translate());

            //Rect topLine = new Rect();
            Widgets.DrawLineHorizontal(viewRect.x, viewRect.y + UIUtility.HEIGHT_HEADER, viewRect.width);

            Rect outRect = inRect;
            outRect.y += UIUtility.HEIGHT_HEADER + UIUtility.MARGIN_HEADER;
            outRect.height -= (UIUtility.HEIGHT_HEADER + UIUtility.MARGIN_HEADER);
            UIUtility.ScrollBlock(outRect, ref m_ScrollbarPosition, ref viewRect, ref viewHeightSum, ref m_ScrollHeightBk, () =>
            {
                float rectX = rectBaseX + UIUtility.MARGIN_LEFT;
                float rectY;
                float rectWidth = viewRect.width;
                float rectHeight;

                //共通設定
                rectY = rectBaseY + UIUtility.MARGIN_TOP + viewHeightSum;
                rectHeight = (UIUtility.HEIGHT_ROW * CalcCommonSectionRowNum()) + UIUtility.MARGIN_TOP + UIUtility.MARGIN_BOTTOM;
                Rect rectSecCommon = new Rect(rectX, rectY, rectWidth, rectHeight);
                EditCommonSection(rectSecCommon, ref valueChanged);
                viewHeightSum += rectSecCommon.height + UIUtility.MARGIN_BOTTOM;

                Rect initializeValueCommonRect = new Rect(inRect.x + viewRect.width - Widgets.CheckboxSize + UIUtility.MARGIN_LEFT, inRect.y + UIUtility.MARGIN_TOP, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                UIUtility.InitializeValueButtonEntry(false, initializeValueCommonRect, () => m_Settings.InitializeValues(SettingValues, CompressedRaidMod.FunctionType.Common), "CR_CommonSectionTitle".Translate());

                //メイン強化機能
                //rectY += viewHeightSum;
                rectY = rectBaseY + UIUtility.MARGIN_TOP + viewHeightSum;
                rectHeight = (UIUtility.HEIGHT_ROW * CalcMainSectionRowNum()) + UIUtility.MARGIN_TOP + UIUtility.MARGIN_BOTTOM;
                Rect rectSecMain = new Rect(rectX, rectY, rectWidth, rectHeight);
                EditMainSection(rectSecMain, ref valueChanged);
                viewHeightSum += rectSecMain.height + UIUtility.MARGIN_BOTTOM;

                //バイオニクス追加機能
                //rectY += viewHeightSum;
                rectY = rectBaseY + UIUtility.MARGIN_TOP + viewHeightSum + UIUtility.MARGIN_BOTTOM;
                rectHeight = (UIUtility.HEIGHT_ROW * CalcAddBionicsSectionRowNum()) + UIUtility.MARGIN_TOP + UIUtility.MARGIN_BOTTOM;
                Rect rectSecAddBionics = new Rect(rectX, rectY, rectWidth, rectHeight);
                EditAddBionicsSection(rectSecAddBionics, ref valueChanged);
                viewHeightSum += UIUtility.MARGIN_TOP + rectSecAddBionics.height + UIUtility.MARGIN_BOTTOM;

                //装備強化機能
                //rectY += viewHeightSum;
                rectY = rectBaseY + UIUtility.MARGIN_TOP + viewHeightSum + UIUtility.MARGIN_BOTTOM;
                rectHeight = (UIUtility.HEIGHT_ROW * CalcRefineGearSectionRowNum()) + UIUtility.MARGIN_TOP + UIUtility.MARGIN_BOTTOM;
                Rect rectSecRefineGear = new Rect(rectX, rectY, rectWidth, rectHeight);
                EditRefineGearSection(rectSecRefineGear, ref valueChanged);
                viewHeightSum += UIUtility.MARGIN_TOP + rectSecRefineGear.height + UIUtility.MARGIN_BOTTOM;

                //薬物効果追加機能
                //rectY += viewHeightSum;
                rectY = rectBaseY + UIUtility.MARGIN_TOP + viewHeightSum + UIUtility.MARGIN_BOTTOM;
                rectHeight = (UIUtility.HEIGHT_ROW * CalcAddDrugEffectsSectionRowNum()) + UIUtility.MARGIN_TOP + UIUtility.MARGIN_BOTTOM;
                Rect rectAddDrugEffects = new Rect(rectX, rectY, rectWidth, rectHeight);
                EditAddDrugEffectsSection(rectAddDrugEffects, ref valueChanged);
                viewHeightSum += UIUtility.MARGIN_TOP + rectAddDrugEffects.height + UIUtility.MARGIN_BOTTOM;

                viewRect.height = viewHeightSum;
            });
            Text.Anchor = anchorBk;
            Text.WordWrap = wordWrapBk;
            Text.Font = fontBk;


            if (valueChanged)
            {
                ValueChanged(SettingValues);
            }
        }

        public void ValueChanged(Action settingValues)
        {
            m_Settings.Write();
            settingValues();
        }

        #region EditCommonSection
        private int CalcCommonSectionRowNum()
        {
            int num = 1 + 7;
            if (m_Settings.allowRaidFriendly)
            {
                num++;
            }
            return num;
        }
        private void EditCommonSection(Rect inRect, ref bool valueChanged)
        {
            float marginTop = 0f;
            Rect titleRect = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_SECTION_TITLE);
            marginTop += UIUtility.WriteFixedSectionTitle(titleRect, "CR_CommonSectionTitle".Translate()) + UIUtility.MARGIN_BOTTOM;

            if (m_Settings.radioTextInput)
            {
                Rect chanceOfCompressionRect = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                UIUtility.HighlightRect(chanceOfCompressionRect);
                UIUtility.ToolTipRow(chanceOfCompressionRect, "CR_ChanceOfCompressionDesc".Translate());
                UIUtility.LabeledTextBoxPercentage(chanceOfCompressionRect, UIUtility.LABEL_WIDTH_HALF, UIUtility.INPUT_WIDTH_HALF, "CR_ChanceOfCompressionLabel".Translate(), ref m_Settings.chanceOfCompression, ref UIUtility.NumericBuffer.chanceOfCompression, ref valueChanged, 0f, 1f, () => {
                    float defaultValue = StaticVariables.GetValue<float>(nameof(m_Settings.chanceOfCompression));
                    m_Settings.chanceOfCompression = defaultValue;
                    UIUtility.NumericBuffer.chanceOfCompression = defaultValue.ToString();
                    SettingValues();
                });
                marginTop += UIUtility.HEIGHT_ROW;

                Rect chanceOfEnhancement = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                UIUtility.HighlightRect(chanceOfEnhancement);
                UIUtility.ToolTipRow(chanceOfEnhancement, "CR_ChanceOfEnhancementDesc".Translate());
                UIUtility.LabeledTextBoxPercentage(chanceOfEnhancement, UIUtility.LABEL_WIDTH_HALF, UIUtility.INPUT_WIDTH_HALF, "CR_ChanceOfEnhancementLabel".Translate(), ref m_Settings.chanceOfEnhancement, ref UIUtility.NumericBuffer.chanceOfEnhancement, ref valueChanged, 0f, 1f, () => {
                    float defaultValue = StaticVariables.GetValue<float>(nameof(m_Settings.chanceOfEnhancement));
                    m_Settings.chanceOfEnhancement = defaultValue;
                    UIUtility.NumericBuffer.chanceOfEnhancement = defaultValue.ToString();
                    SettingValues();
                });
                marginTop += UIUtility.HEIGHT_ROW;

                Rect calcBaseMultByEnhanced = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                UIUtility.HighlightRect(calcBaseMultByEnhanced);
                UIUtility.ToolTipRow(calcBaseMultByEnhanced, "CR_CalcBaseMultByEnhancedDesc".Translate());
                UIUtility.LabeledCheckBoxRight(calcBaseMultByEnhanced, UIUtility.LABEL_WIDTH_HALF, "CR_CalcBaseMultByEnhancedLabel".Translate(), ref m_Settings.calcBaseMultByEnhanced, ref valueChanged, () => {
                    bool defaultValue = StaticVariables.GetValue<bool>(nameof(m_Settings.calcBaseMultByEnhanced));
                    m_Settings.calcBaseMultByEnhanced = defaultValue;
                    SettingValues();
                });
                marginTop += UIUtility.HEIGHT_ROW;

                Rect maxRaidPawnsCount = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                UIUtility.HighlightRect(maxRaidPawnsCount);
                UIUtility.ToolTipRow(maxRaidPawnsCount, "CR_MaxRaidPawnsCountDesc".Translate());
                UIUtility.LabeledTextBoxInt(maxRaidPawnsCount, UIUtility.LABEL_WIDTH_HALF, UIUtility.INPUT_WIDTH_HALF, "CR_MaxRaidPawnsCountLabel".Translate(), ref m_Settings.maxRaidPawnsCount, ref UIUtility.NumericBuffer.maxRaidPawnsCount, ref valueChanged, 0, 1000, () => {
                    int defaultValue = StaticVariables.GetValue<int>(nameof(m_Settings.maxRaidPawnsCount));
                    m_Settings.maxRaidPawnsCount = defaultValue;
                    UIUtility.NumericBuffer.maxRaidPawnsCount = defaultValue.ToString();
                    SettingValues();
                });
                marginTop += UIUtility.HEIGHT_ROW;

                Rect gainFactorMult = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                UIUtility.HighlightRect(gainFactorMult);
                UIUtility.ToolTipRow(gainFactorMult, "CR_GainFactorMultDesc".Translate());
                UIUtility.LabeledTextBoxPercentage(gainFactorMult, UIUtility.LABEL_WIDTH_HALF, UIUtility.INPUT_WIDTH_HALF, "CR_GainFactorMultLabel".Translate(), ref m_Settings.gainFactorMult, ref UIUtility.NumericBuffer.gainFactorMult, ref valueChanged, 0f, 100f, () => {
                    float defaultValue = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMult));
                    m_Settings.gainFactorMult = defaultValue;
                    UIUtility.NumericBuffer.gainFactorMult = defaultValue.ToString();
                    SettingValues();
                });
                marginTop += UIUtility.HEIGHT_ROW;

                Rect allowCompressTargetTypes = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                UIUtility.HighlightRect(allowCompressTargetTypes);
                Rect allowCompressTargetTypesLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                Widgets.Label(allowCompressTargetTypesLabel, "CR_AllowTypesLabel".Translate());

                Rect allowRaidFriendly = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.INPUT_WIDTH_MIN, UIUtility.HEIGHT_ROW);
                UIUtility.HighlightRect(allowRaidFriendly);
                UIUtility.ToolTipRow(allowRaidFriendly, "CR_AllowRaidFriendlyDesc".Translate(), 0f);
                UIUtility.LabeledCheckBoxLeft(allowRaidFriendly, "CR_AllowRaidFriendlyLabel".Translate(), ref m_Settings.allowRaidFriendly, ref valueChanged, () => { }, true);

                Rect allowMechanoids = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_THREE_QUARTERS_HALF + UIUtility.INPUT_WIDTH_MIN, inRect.y + marginTop, UIUtility.INPUT_WIDTH_MIN, UIUtility.HEIGHT_ROW);
                UIUtility.HighlightRect(allowMechanoids);
                UIUtility.ToolTipRow(allowMechanoids, "CR_AllowMechanoidsDesc".Translate(), 0f);
                UIUtility.LabeledCheckBoxLeft(allowMechanoids, "CR_AllowMechanoidsLabel".Translate(), ref m_Settings.allowMechanoids, ref valueChanged, () => { }, true);

                Rect allowInsectoids = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_THREE_QUARTERS_HALF + (UIUtility.INPUT_WIDTH_MIN * 2f), inRect.y + marginTop, UIUtility.INPUT_WIDTH_MIN, UIUtility.HEIGHT_ROW);
                UIUtility.HighlightRect(allowInsectoids);
                UIUtility.ToolTipRow(allowInsectoids, "CR_AllowInsectoidsDesc".Translate(), 0f);
                UIUtility.LabeledCheckBoxLeft(allowInsectoids, "CR_AllowInsectoidsLabel".Translate(), ref m_Settings.allowInsectoids, ref valueChanged, () => { }, true);

                Rect allowManhunters = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_THREE_QUARTERS_HALF + (UIUtility.INPUT_WIDTH_MIN * 3f), inRect.y + marginTop, UIUtility.INPUT_WIDTH_MIN, UIUtility.HEIGHT_ROW);
                UIUtility.HighlightRect(allowManhunters);
                UIUtility.ToolTipRow(allowManhunters, "CR_AllowManhuntersDesc".Translate(), 0f);
                UIUtility.LabeledCheckBoxLeft(allowManhunters, "CR_AllowManhuntersLabel".Translate(), ref m_Settings.allowManhunters, ref valueChanged, () => { }, true);

                Rect initializeValueAllowCompressTargetTypes = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                UIUtility.InitializeValueButtonEntry(false, initializeValueAllowCompressTargetTypes, () => {
                    m_Settings.allowRaidFriendly = StaticVariables.GetValue<bool>(nameof(m_Settings.allowRaidFriendly));
                    m_Settings.allowMechanoids = StaticVariables.GetValue<bool>(nameof(m_Settings.allowMechanoids));
                    m_Settings.allowInsectoids = StaticVariables.GetValue<bool>(nameof(m_Settings.allowInsectoids));
                    m_Settings.allowManhunters = StaticVariables.GetValue<bool>(nameof(m_Settings.allowManhunters));
                    SettingValues();
                }, "CR_AllowTypesLabel".Translate());
                marginTop += UIUtility.HEIGHT_ROW;

                if (m_Settings.allowRaidFriendly)
                {
                    Rect gainFactorMultRaidFriendly = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultRaidFriendly);
                    UIUtility.ToolTipRow(gainFactorMultRaidFriendly, "CR_GainFactorMultRaidFriendlyDesc".Translate());
                    UIUtility.LabeledTextBoxPercentage(gainFactorMultRaidFriendly, UIUtility.LABEL_WIDTH_HALF, UIUtility.INPUT_WIDTH_HALF, "CR_GainFactorMultRaidFriendlyLabel".Translate(), ref m_Settings.gainFactorMultRaidFriendly, ref UIUtility.NumericBuffer.gainFactorMultRaidFriendly, ref valueChanged, 0f, 100f, () => {
                        float defaultValue = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultRaidFriendly));
                        m_Settings.gainFactorMultRaidFriendly = defaultValue;
                        UIUtility.NumericBuffer.gainFactorMultRaidFriendly = defaultValue.ToString();
                        SettingValues();
                    });
                    marginTop += UIUtility.HEIGHT_ROW;
                }

                Rect displayMessage = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                UIUtility.HighlightRect(displayMessage);
                UIUtility.ToolTipRow(displayMessage, "CR_DisplayMessageDesc".Translate());
                UIUtility.LabeledCheckBoxRight(displayMessage, UIUtility.LABEL_WIDTH_HALF, "CR_DisplayMessageLabel".Translate(), ref m_Settings.displayMessage, ref valueChanged, () => {
                    bool defaultValue = StaticVariables.GetValue<bool>(nameof(m_Settings.displayMessage));
                    m_Settings.displayMessage = defaultValue;
                    SettingValues();
                });
                marginTop += UIUtility.HEIGHT_ROW;
            }
            else
            {
                Rect chanceOfCompressionRect = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                UIUtility.HighlightRect(chanceOfCompressionRect);
                UIUtility.ToolTipRow(chanceOfCompressionRect, "CR_ChanceOfCompressionDesc".Translate());
                if (UIUtility.LabeledSliderPercentage(chanceOfCompressionRect, UIUtility.LABEL_WIDTH_HALF, UIUtility.INPUT_WIDTH_HALF, "CR_ChanceOfCompressionLabel".Translate(), ref m_Settings.chanceOfCompression, ref valueChanged, 0f, 1f, () => {
                    float defaultValue = StaticVariables.GetValue<float>(nameof(m_Settings.chanceOfCompression));
                    m_Settings.chanceOfCompression = defaultValue;
                    UIUtility.NumericBuffer.chanceOfCompression = defaultValue.ToString();
                    SettingValues();
                }))
                {
                    UIUtility.NumericBuffer.chanceOfCompression = m_Settings.chanceOfCompression.ToString();
                }
                marginTop += UIUtility.HEIGHT_ROW;

                Rect chanceOfEnhancement = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                UIUtility.HighlightRect(chanceOfEnhancement);
                UIUtility.ToolTipRow(chanceOfEnhancement, "CR_ChanceOfEnhancementDesc".Translate());
                if (UIUtility.LabeledSliderPercentage(chanceOfEnhancement, UIUtility.LABEL_WIDTH_HALF, UIUtility.INPUT_WIDTH_HALF, "CR_ChanceOfEnhancementLabel".Translate(), ref m_Settings.chanceOfEnhancement, ref valueChanged, 0f, 1f, () => {
                    float defaultValue = StaticVariables.GetValue<float>(nameof(m_Settings.chanceOfEnhancement));
                    m_Settings.chanceOfEnhancement = defaultValue;
                    UIUtility.NumericBuffer.chanceOfEnhancement = defaultValue.ToString();
                    SettingValues();
                }))
                {
                    UIUtility.NumericBuffer.chanceOfEnhancement = m_Settings.chanceOfEnhancement.ToString();
                }
                marginTop += UIUtility.HEIGHT_ROW;

                Rect calcBaseMultByEnhanced = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                UIUtility.HighlightRect(calcBaseMultByEnhanced);
                UIUtility.ToolTipRow(calcBaseMultByEnhanced, "CR_CalcBaseMultByEnhancedDesc".Translate());
                UIUtility.LabeledCheckBoxRight(calcBaseMultByEnhanced, UIUtility.LABEL_WIDTH_HALF, "CR_CalcBaseMultByEnhancedLabel".Translate(), ref m_Settings.calcBaseMultByEnhanced, ref valueChanged, () => {
                    bool defaultValue = StaticVariables.GetValue<bool>(nameof(m_Settings.calcBaseMultByEnhanced));
                    m_Settings.calcBaseMultByEnhanced = defaultValue;
                    SettingValues();
                });
                marginTop += UIUtility.HEIGHT_ROW;

                Rect maxRaidPawnsCount = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                UIUtility.HighlightRect(maxRaidPawnsCount);
                UIUtility.ToolTipRow(maxRaidPawnsCount, "CR_MaxRaidPawnsCountDesc".Translate());
                if (UIUtility.LabeledSliderInt(maxRaidPawnsCount, UIUtility.LABEL_WIDTH_HALF, UIUtility.INPUT_WIDTH_HALF, "CR_MaxRaidPawnsCountLabel".Translate(), ref m_Settings.maxRaidPawnsCount, ref valueChanged, 0, 1000, () => {
                    int defaultValue = StaticVariables.GetValue<int>(nameof(m_Settings.maxRaidPawnsCount));
                    m_Settings.maxRaidPawnsCount = defaultValue;
                    UIUtility.NumericBuffer.maxRaidPawnsCount = defaultValue.ToString();
                    SettingValues();
                }))
                {
                    UIUtility.NumericBuffer.maxRaidPawnsCount = m_Settings.maxRaidPawnsCount.ToString();
                }
                marginTop += UIUtility.HEIGHT_ROW;

                Rect gainFactorMult = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                UIUtility.HighlightRect(gainFactorMult);
                UIUtility.ToolTipRow(gainFactorMult, "CR_GainFactorMultDesc".Translate());
                if (UIUtility.LabeledSliderPercentage(gainFactorMult, UIUtility.LABEL_WIDTH_HALF, UIUtility.INPUT_WIDTH_HALF, "CR_GainFactorMultLabel".Translate(), ref m_Settings.gainFactorMult, ref valueChanged, 0f, 100f, () => {
                    float defaultValue = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMult));
                    m_Settings.gainFactorMult = defaultValue;
                    UIUtility.NumericBuffer.gainFactorMult = defaultValue.ToString();
                    SettingValues();
                }))
                {
                    UIUtility.NumericBuffer.gainFactorMult = m_Settings.gainFactorMult.ToString();
                }
                marginTop += UIUtility.HEIGHT_ROW;

                Rect allowCompressTargetTypes = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                UIUtility.HighlightRect(allowCompressTargetTypes);
                Rect allowCompressTargetTypesLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                Widgets.Label(allowCompressTargetTypesLabel, "CR_AllowTypesLabel".Translate());

                Rect allowRaidFriendly = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.INPUT_WIDTH_MIN, UIUtility.HEIGHT_ROW);
                UIUtility.HighlightRect(allowRaidFriendly);
                UIUtility.ToolTipRow(allowRaidFriendly, "CR_AllowRaidFriendlyDesc".Translate(), 0);
                UIUtility.LabeledCheckBoxLeft(allowRaidFriendly, "CR_AllowRaidFriendlyLabel".Translate(), ref m_Settings.allowRaidFriendly, ref valueChanged, () => { }, true);

                Rect allowMechanoids = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_THREE_QUARTERS_HALF + UIUtility.INPUT_WIDTH_MIN, inRect.y + marginTop, UIUtility.INPUT_WIDTH_MIN, UIUtility.HEIGHT_ROW);
                UIUtility.HighlightRect(allowMechanoids);
                UIUtility.ToolTipRow(allowMechanoids, "CR_AllowMechanoidsDesc".Translate(), 0);
                UIUtility.LabeledCheckBoxLeft(allowMechanoids, "CR_AllowMechanoidsLabel".Translate(), ref m_Settings.allowMechanoids, ref valueChanged, () => { }, true);

                Rect allowInsectoids = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_THREE_QUARTERS_HALF + (UIUtility.INPUT_WIDTH_MIN * 2f), inRect.y + marginTop, UIUtility.INPUT_WIDTH_MIN, UIUtility.HEIGHT_ROW);
                UIUtility.HighlightRect(allowInsectoids);
                UIUtility.ToolTipRow(allowInsectoids, "CR_AllowInsectoidsDesc".Translate(), 0);
                UIUtility.LabeledCheckBoxLeft(allowInsectoids, "CR_AllowInsectoidsLabel".Translate(), ref m_Settings.allowInsectoids, ref valueChanged, () => { }, true);

                Rect allowManhunters = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_THREE_QUARTERS_HALF + (UIUtility.INPUT_WIDTH_MIN * 3f), inRect.y + marginTop, UIUtility.INPUT_WIDTH_MIN, UIUtility.HEIGHT_ROW);
                UIUtility.HighlightRect(allowManhunters);
                UIUtility.ToolTipRow(allowManhunters, "CR_AllowManhuntersDesc".Translate(), 0f);
                UIUtility.LabeledCheckBoxLeft(allowManhunters, "CR_AllowManhuntersLabel".Translate(), ref m_Settings.allowManhunters, ref valueChanged, () => { }, true);

                Rect initializeValueAllowCompressTargetTypes = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                UIUtility.InitializeValueButtonEntry(false, initializeValueAllowCompressTargetTypes, () => {
                    m_Settings.allowRaidFriendly = StaticVariables.GetValue<bool>(nameof(m_Settings.allowRaidFriendly));
                    m_Settings.allowMechanoids = StaticVariables.GetValue<bool>(nameof(m_Settings.allowMechanoids));
                    m_Settings.allowInsectoids = StaticVariables.GetValue<bool>(nameof(m_Settings.allowInsectoids));
                    m_Settings.allowManhunters = StaticVariables.GetValue<bool>(nameof(m_Settings.allowManhunters));
                    SettingValues();
                }, "CR_AllowTypesLabel".Translate());
                marginTop += UIUtility.HEIGHT_ROW;

                if (m_Settings.allowRaidFriendly)
                {
                    Rect gainFactorMultRaidFriendly = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultRaidFriendly);
                    UIUtility.ToolTipRow(gainFactorMultRaidFriendly, "CR_GainFactorMultRaidFriendlyDesc".Translate());
                    if (UIUtility.LabeledSliderPercentage(gainFactorMultRaidFriendly, UIUtility.LABEL_WIDTH_HALF, UIUtility.INPUT_WIDTH_HALF, "CR_GainFactorMultRaidFriendlyLabel".Translate(), ref m_Settings.gainFactorMultRaidFriendly, ref valueChanged, 0f, 100f, () => {
                        float defaultValue = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultRaidFriendly));
                        m_Settings.gainFactorMultRaidFriendly = defaultValue;
                        UIUtility.NumericBuffer.gainFactorMultRaidFriendly = defaultValue.ToString();
                        SettingValues();
                    }))
                    {
                        UIUtility.NumericBuffer.gainFactorMultRaidFriendly = m_Settings.gainFactorMultRaidFriendly.ToString();
                    }
                    marginTop += UIUtility.HEIGHT_ROW;
                }

                Rect displayMessage = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                UIUtility.HighlightRect(displayMessage);
                UIUtility.ToolTipRow(displayMessage, "CR_DisplayMessageDesc".Translate());
                UIUtility.LabeledCheckBoxRight(displayMessage, UIUtility.LABEL_WIDTH_HALF, "CR_DisplayMessageLabel".Translate(), ref m_Settings.displayMessage, ref valueChanged, () => {
                    bool defaultValue = StaticVariables.GetValue<bool>(nameof(m_Settings.displayMessage));
                    m_Settings.displayMessage = defaultValue;
                    SettingValues();
                });
                marginTop += UIUtility.HEIGHT_ROW;
            }
        }
        #endregion

        #region EditMainSection
        private int CalcMainSectionRowNum()
        {
            int num = 1;
            if (m_Settings.enableMainEnhance)
            {
                num += 18;
            }
            else
            {
                num += 1;
            }
            return num;
        }
        private void EditMainSection(Rect inRect, ref bool valueChanged)
        {
            float marginTop = 0f;
            //Widgets.DrawBoxSolid(inRect, m_Settings.enableMainEnhance ? UIUtility.SUB_SECTION_BACKGROUND_ACTIVE_COLOR : UIUtility.SUB_SECTION_BACKGROUND_DEACTIVE_COLOR);
            if (!m_Settings.enableMainEnhance)
            {
                Widgets.DrawBoxSolid(inRect, UIUtility.SUB_SECTION_BACKGROUND_DEACTIVE_COLOR);
            }
            Widgets.DrawBox(inRect);
            Rect titleRect = new Rect(inRect.x, inRect.y, inRect.width - (UIUtility.MARGIN_LEFT), UIUtility.HEIGHT_SECTION_TITLE);
            marginTop += UIUtility.WriteFloatedSectionTitle(titleRect, "CR_MainSectionTitle".Translate(), ref m_Settings.enableMainEnhance, ref valueChanged, () => m_Settings.InitializeValues(SettingValues, CompressedRaidMod.FunctionType.Main)) + UIUtility.MARGIN_BOTTOM;

            if (m_Settings.enableMainEnhance)
            {
                float labelWidth = 75f;
                float textWidth = 206.25f;

                if (m_Settings.radioTextInput)
                {
                    //Pain
                    Rect gainEnhancementPain = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementPain);
                    Rect gainEnhancementPainLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementPainLabel, "CR_PainLabel".Translate());
                    Rect gainFactorMultPain = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultPain);
                    UIUtility.ToolTipRow(gainFactorMultPain, "CR_GainFactorMultPainDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMultPain, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultPain, ref UIUtility.NumericBuffer.gainFactorMultPain, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect gainFactorMaxPain = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxPain);
                    UIUtility.ToolTipRow(gainFactorMaxPain, "CR_GainFactorMaxPainDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMaxPain, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxPain, ref UIUtility.NumericBuffer.gainFactorMaxPain, ref valueChanged, 0f, 1f, () => { }, TextAnchor.MiddleRight, true);
                    Rect initializeValueGainEnhancementPain = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementPain, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultPain));
                        m_Settings.gainFactorMultPain = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultPain = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxPain));
                        m_Settings.gainFactorMaxPain = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxPain = defaultMax.ToString();
                        SettingValues();
                    }, "CR_PainLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //ArmorRating_Blunt
                    Rect gainEnhancementArmorRating_Blunt = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementArmorRating_Blunt);
                    Rect gainEnhancementArmorRating_BluntLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementArmorRating_BluntLabel, "CR_ArmorRating_BluntLabel".Translate());
                    Rect gainFactorMultArmorRating_Blunt = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultArmorRating_Blunt);
                    UIUtility.ToolTipRow(gainFactorMultArmorRating_Blunt, "CR_GainFactorMultArmorRating_BluntDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMultArmorRating_Blunt, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultArmorRating_Blunt, ref UIUtility.NumericBuffer.gainFactorMultArmorRating_Blunt, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect gainFactorMaxArmorRating_Blunt = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxArmorRating_Blunt);
                    UIUtility.ToolTipRow(gainFactorMaxArmorRating_Blunt, "CR_GainFactorMaxArmorRating_BluntDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMaxArmorRating_Blunt, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxArmorRating_Blunt, ref UIUtility.NumericBuffer.gainFactorMaxArmorRating_Blunt, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect initializeValueGainEnhancementArmorRating_Blunt = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementArmorRating_Blunt, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultArmorRating_Blunt));
                        m_Settings.gainFactorMultArmorRating_Blunt = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultArmorRating_Blunt = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxArmorRating_Blunt));
                        m_Settings.gainFactorMaxArmorRating_Blunt = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxArmorRating_Blunt = defaultMax.ToString();
                        SettingValues();
                    }, "CR_ArmorRating_BluntLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //ArmorRating_Sharp
                    Rect gainEnhancementArmorRating_Sharp = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementArmorRating_Sharp);
                    Rect gainEnhancementArmorRating_SharpLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementArmorRating_SharpLabel, "CR_ArmorRating_SharpLabel".Translate());
                    Rect gainFactorMultArmorRating_Sharp = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultArmorRating_Sharp);
                    UIUtility.ToolTipRow(gainFactorMultArmorRating_Sharp, "CR_GainFactorMultArmorRating_SharpDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMultArmorRating_Sharp, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultArmorRating_Sharp, ref UIUtility.NumericBuffer.gainFactorMultArmorRating_Sharp, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect gainFactorMaxArmorRating_Sharp = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxArmorRating_Sharp);
                    UIUtility.ToolTipRow(gainFactorMaxArmorRating_Sharp, "CR_GainFactorMaxArmorRating_SharpDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMaxArmorRating_Sharp, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxArmorRating_Sharp, ref UIUtility.NumericBuffer.gainFactorMaxArmorRating_Sharp, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect initializeValueGainEnhancementArmorRating_Sharp = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementArmorRating_Sharp, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultArmorRating_Sharp));
                        m_Settings.gainFactorMultArmorRating_Sharp = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultArmorRating_Sharp = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxArmorRating_Sharp));
                        m_Settings.gainFactorMaxArmorRating_Sharp = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxArmorRating_Sharp = defaultMax.ToString();
                        SettingValues();
                    }, "CR_ArmorRating_SharpLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //ArmorRating_Heat
                    Rect gainEnhancementArmorRating_Heat = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementArmorRating_Heat);
                    Rect gainEnhancementArmorRating_HeatLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementArmorRating_HeatLabel, "CR_ArmorRating_HeatLabel".Translate());
                    Rect gainFactorMultArmorRating_Heat = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultArmorRating_Heat);
                    UIUtility.ToolTipRow(gainFactorMultArmorRating_Heat, "CR_GainFactorMultArmorRating_HeatDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMultArmorRating_Heat, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultArmorRating_Heat, ref UIUtility.NumericBuffer.gainFactorMultArmorRating_Heat, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect gainFactorMaxArmorRating_Heat = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxArmorRating_Heat);
                    UIUtility.ToolTipRow(gainFactorMaxArmorRating_Heat, "CR_GainFactorMaxArmorRating_HeatDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMaxArmorRating_Heat, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxArmorRating_Heat, ref UIUtility.NumericBuffer.gainFactorMaxArmorRating_Heat, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect initializeValueGainEnhancementArmorRating_Heat = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementArmorRating_Heat, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultArmorRating_Heat));
                        m_Settings.gainFactorMultArmorRating_Heat = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultArmorRating_Heat = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxArmorRating_Heat));
                        m_Settings.gainFactorMaxArmorRating_Heat = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxArmorRating_Heat = defaultMax.ToString();
                        SettingValues();
                    }, "CR_ArmorRating_HeatLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //MeleeDodgeChance
                    Rect gainEnhancementMeleeDodgeChance = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementMeleeDodgeChance);
                    Rect gainEnhancementMeleeDodgeChanceLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementMeleeDodgeChanceLabel, "CR_MeleeDodgeChanceLabel".Translate());
                    Rect gainFactorMultMeleeDodgeChance = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultMeleeDodgeChance);
                    UIUtility.ToolTipRow(gainFactorMultMeleeDodgeChance, "CR_GainFactorMultMeleeDodgeChanceDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMultMeleeDodgeChance, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultMeleeDodgeChance, ref UIUtility.NumericBuffer.gainFactorMultMeleeDodgeChance, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect gainFactorMaxMeleeDodgeChance = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxMeleeDodgeChance);
                    UIUtility.ToolTipRow(gainFactorMaxMeleeDodgeChance, "CR_GainFactorMaxMeleeDodgeChanceDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMaxMeleeDodgeChance, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxMeleeDodgeChance, ref UIUtility.NumericBuffer.gainFactorMaxMeleeDodgeChance, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect initializeValueGainEnhancementMeleeDodgeChance = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementMeleeDodgeChance, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultMeleeDodgeChance));
                        m_Settings.gainFactorMultMeleeDodgeChance = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultMeleeDodgeChance = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxMeleeDodgeChance));
                        m_Settings.gainFactorMaxMeleeDodgeChance = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxMeleeDodgeChance = defaultMax.ToString();
                        SettingValues();
                    }, "CR_MeleeDodgeChanceLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //MeleeHitChance
                    Rect gainEnhancementMeleeHitChance = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementMeleeHitChance);
                    Rect gainEnhancementMeleeHitChanceLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementMeleeHitChanceLabel, "CR_MeleeHitChanceLabel".Translate());
                    Rect gainFactorMultMeleeHitChance = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultMeleeHitChance);
                    UIUtility.ToolTipRow(gainFactorMultMeleeHitChance, "CR_GainFactorMultMeleeHitChanceDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMultMeleeHitChance, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultMeleeHitChance, ref UIUtility.NumericBuffer.gainFactorMultMeleeHitChance, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect gainFactorMaxMeleeHitChance = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxMeleeHitChance);
                    UIUtility.ToolTipRow(gainFactorMaxMeleeHitChance, "CR_GainFactorMaxMeleeHitChanceDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMaxMeleeHitChance, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxMeleeHitChance, ref UIUtility.NumericBuffer.gainFactorMaxMeleeHitChance, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect initializeValueGainEnhancementMeleeHitChance = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementMeleeHitChance, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultMeleeHitChance));
                        m_Settings.gainFactorMultMeleeHitChance = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultMeleeHitChance = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxMeleeHitChance));
                        m_Settings.gainFactorMaxMeleeHitChance = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxMeleeHitChance = defaultMax.ToString();
                        SettingValues();
                    }, "CR_MeleeHitChanceLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //MoveSpeed
                    Rect gainEnhancementMoveSpeed = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementMoveSpeed);
                    Rect gainEnhancementMoveSpeedLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementMoveSpeedLabel, "CR_MoveSpeedLabel".Translate());
                    Rect gainFactorMultMoveSpeed = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultMoveSpeed);
                    UIUtility.ToolTipRow(gainFactorMultMoveSpeed, "CR_GainFactorMultMoveSpeedDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMultMoveSpeed, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultMoveSpeed, ref UIUtility.NumericBuffer.gainFactorMultMoveSpeed, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect gainFactorMaxMoveSpeed = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxMoveSpeed);
                    UIUtility.ToolTipRow(gainFactorMaxMoveSpeed, "CR_GainFactorMaxMoveSpeedDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMaxMoveSpeed, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxMoveSpeed, ref UIUtility.NumericBuffer.gainFactorMaxMoveSpeed, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect initializeValueGainEnhancementMoveSpeed = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementMoveSpeed, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultMoveSpeed));
                        m_Settings.gainFactorMultMoveSpeed = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultMoveSpeed = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxMoveSpeed));
                        m_Settings.gainFactorMaxMoveSpeed = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxMoveSpeed = defaultMax.ToString();
                        SettingValues();
                    }, "CR_MoveSpeedLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //ShootingAccuracyPawn
                    Rect gainEnhancementShootingAccuracyPawn = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementShootingAccuracyPawn);
                    Rect gainEnhancementShootingAccuracyPawnLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementShootingAccuracyPawnLabel, "CR_ShootingAccuracyPawnLabel".Translate());
                    Rect gainFactorMultShootingAccuracyPawn = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultShootingAccuracyPawn);
                    UIUtility.ToolTipRow(gainFactorMultShootingAccuracyPawn, "CR_GainFactorMultShootingAccuracyPawnDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMultShootingAccuracyPawn, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultShootingAccuracyPawn, ref UIUtility.NumericBuffer.gainFactorMultShootingAccuracyPawn, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect gainFactorMaxShootingAccuracyPawn = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxShootingAccuracyPawn);
                    UIUtility.ToolTipRow(gainFactorMaxShootingAccuracyPawn, "CR_GainFactorMaxShootingAccuracyPawnDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMaxShootingAccuracyPawn, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxShootingAccuracyPawn, ref UIUtility.NumericBuffer.gainFactorMaxShootingAccuracyPawn, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect initializeValueGainEnhancementShootingAccuracyPawn = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementShootingAccuracyPawn, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultShootingAccuracyPawn));
                        m_Settings.gainFactorMultShootingAccuracyPawn = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultShootingAccuracyPawn = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxShootingAccuracyPawn));
                        m_Settings.gainFactorMaxShootingAccuracyPawn = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxShootingAccuracyPawn = defaultMax.ToString();
                        SettingValues();
                    }, "CR_ShootingAccuracyPawnLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //PawnTrapSpringChance
                    Rect gainEnhancementPawnTrapSpringChance = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementPawnTrapSpringChance);
                    Rect gainEnhancementPawnTrapSpringChanceLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementPawnTrapSpringChanceLabel, "CR_PawnTrapSpringChanceLabel".Translate());
                    Rect gainFactorMultPawnTrapSpringChance = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultPawnTrapSpringChance);
                    UIUtility.ToolTipRow(gainFactorMultPawnTrapSpringChance, "CR_GainFactorMultPawnTrapSpringChanceDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMultPawnTrapSpringChance, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultPawnTrapSpringChance, ref UIUtility.NumericBuffer.gainFactorMultPawnTrapSpringChance, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect gainFactorMaxPawnTrapSpringChance = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxPawnTrapSpringChance);
                    UIUtility.ToolTipRow(gainFactorMaxPawnTrapSpringChance, "CR_GainFactorMaxPawnTrapSpringChanceDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMaxPawnTrapSpringChance, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxPawnTrapSpringChance, ref UIUtility.NumericBuffer.gainFactorMaxPawnTrapSpringChance, ref valueChanged, 0f, 1f, () => { }, TextAnchor.MiddleRight, true);
                    Rect initializeValueGainEnhancementPawnTrapSpringChance = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementPawnTrapSpringChance, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultPawnTrapSpringChance));
                        m_Settings.gainFactorMultPawnTrapSpringChance = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultPawnTrapSpringChance = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxPawnTrapSpringChance));
                        m_Settings.gainFactorMaxPawnTrapSpringChance = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxPawnTrapSpringChance = defaultMax.ToString();
                        SettingValues();
                    }, "CR_PawnTrapSpringChanceLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //CapacitySight
                    Rect gainEnhancementCapacitySight = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementCapacitySight);
                    Rect gainEnhancementCapacitySightLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementCapacitySightLabel, "CR_CapacitySightLabel".Translate());
                    Rect gainFactorMultCapacitySight = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultCapacitySight);
                    UIUtility.ToolTipRow(gainFactorMultCapacitySight, "CR_GainFactorMultCapacitySightDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMultCapacitySight, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultCapacitySight, ref UIUtility.NumericBuffer.gainFactorMultCapacitySight, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect gainFactorMaxCapacitySight = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxCapacitySight);
                    UIUtility.ToolTipRow(gainFactorMaxCapacitySight, "CR_GainFactorMaxCapacitySightDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMaxCapacitySight, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxCapacitySight, ref UIUtility.NumericBuffer.gainFactorMaxCapacitySight, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect initializeValueGainEnhancementCapacitySight = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementCapacitySight, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultCapacitySight));
                        m_Settings.gainFactorMultCapacitySight = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultCapacitySight = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxCapacitySight));
                        m_Settings.gainFactorMaxCapacitySight = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxCapacitySight = defaultMax.ToString();
                        SettingValues();
                    }, "CR_CapacitySightLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //CapacityMoving
                    Rect gainEnhancementCapacityMoving = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementCapacityMoving);
                    Rect gainEnhancementCapacityMovingLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementCapacityMovingLabel, "CR_CapacityMovingLabel".Translate());
                    Rect gainFactorMultCapacityMoving = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultCapacityMoving);
                    UIUtility.ToolTipRow(gainFactorMultCapacityMoving, "CR_GainFactorMultCapacityMovingDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMultCapacityMoving, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultCapacityMoving, ref UIUtility.NumericBuffer.gainFactorMultCapacityMoving, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect gainFactorMaxCapacityMoving = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxCapacityMoving);
                    UIUtility.ToolTipRow(gainFactorMaxCapacityMoving, "CR_GainFactorMaxCapacityMovingDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMaxCapacityMoving, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxCapacityMoving, ref UIUtility.NumericBuffer.gainFactorMaxCapacityMoving, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect initializeValueGainEnhancementCapacityMoving = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementCapacityMoving, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultCapacityMoving));
                        m_Settings.gainFactorMultCapacityMoving = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultCapacityMoving = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxCapacityMoving));
                        m_Settings.gainFactorMaxCapacityMoving = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxCapacityMoving = defaultMax.ToString();
                        SettingValues();
                    }, "CR_CapacityMovingLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //CapacityHearing
                    Rect gainEnhancementCapacityHearing = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementCapacityHearing);
                    Rect gainEnhancementCapacityHearingLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementCapacityHearingLabel, "CR_CapacityHearingLabel".Translate());
                    Rect gainFactorMultCapacityHearing = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultCapacityHearing);
                    UIUtility.ToolTipRow(gainFactorMultCapacityHearing, "CR_GainFactorMultCapacityHearingDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMultCapacityHearing, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultCapacityHearing, ref UIUtility.NumericBuffer.gainFactorMultCapacityHearing, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect gainFactorMaxCapacityHearing = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxCapacityHearing);
                    UIUtility.ToolTipRow(gainFactorMaxCapacityHearing, "CR_GainFactorMaxCapacityHearingDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMaxCapacityHearing, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxCapacityHearing, ref UIUtility.NumericBuffer.gainFactorMaxCapacityHearing, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect initializeValueGainEnhancementCapacityHearing = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementCapacityHearing, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultCapacityHearing));
                        m_Settings.gainFactorMultCapacityHearing = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultCapacityHearing = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxCapacityHearing));
                        m_Settings.gainFactorMaxCapacityHearing = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxCapacityHearing = defaultMax.ToString();
                        SettingValues();
                    }, "CR_CapacityHearingLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //CapacityManipulation
                    Rect gainEnhancementCapacityManipulation = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementCapacityManipulation);
                    Rect gainEnhancementCapacityManipulationLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementCapacityManipulationLabel, "CR_CapacityManipulationLabel".Translate());
                    Rect gainFactorMultCapacityManipulation = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultCapacityManipulation);
                    UIUtility.ToolTipRow(gainFactorMultCapacityManipulation, "CR_GainFactorMultCapacityManipulationDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMultCapacityManipulation, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultCapacityManipulation, ref UIUtility.NumericBuffer.gainFactorMultCapacityManipulation, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect gainFactorMaxCapacityManipulation = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxCapacityManipulation);
                    UIUtility.ToolTipRow(gainFactorMaxCapacityManipulation, "CR_GainFactorMaxCapacityManipulationDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMaxCapacityManipulation, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxCapacityManipulation, ref UIUtility.NumericBuffer.gainFactorMaxCapacityManipulation, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect initializeValueGainEnhancementCapacityManipulation = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementCapacityManipulation, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultCapacityManipulation));
                        m_Settings.gainFactorMultCapacityManipulation = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultCapacityManipulation = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxCapacityManipulation));
                        m_Settings.gainFactorMaxCapacityManipulation = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxCapacityManipulation = defaultMax.ToString();
                        SettingValues();
                    }, "CR_CapacityManipulationLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //CapacityMetabolism
                    Rect gainEnhancementCapacityMetabolism = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementCapacityMetabolism);
                    Rect gainEnhancementCapacityMetabolismLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementCapacityMetabolismLabel, "CR_CapacityMetabolismLabel".Translate());
                    Rect gainFactorMultCapacityMetabolism = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultCapacityMetabolism);
                    UIUtility.ToolTipRow(gainFactorMultCapacityMetabolism, "CR_GainFactorMultCapacityMetabolismDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMultCapacityMetabolism, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultCapacityMetabolism, ref UIUtility.NumericBuffer.gainFactorMultCapacityMetabolism, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect gainFactorMaxCapacityMetabolism = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxCapacityMetabolism);
                    UIUtility.ToolTipRow(gainFactorMaxCapacityMetabolism, "CR_GainFactorMaxCapacityMetabolismDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMaxCapacityMetabolism, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxCapacityMetabolism, ref UIUtility.NumericBuffer.gainFactorMaxCapacityMetabolism, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect initializeValueGainEnhancementCapacityMetabolism = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementCapacityMetabolism, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultCapacityMetabolism));
                        m_Settings.gainFactorMultCapacityMetabolism = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultCapacityMetabolism = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxCapacityMetabolism));
                        m_Settings.gainFactorMaxCapacityMetabolism = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxCapacityMetabolism = defaultMax.ToString();
                        SettingValues();
                    }, "CR_CapacityMetabolismLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //CapacityConsciousness
                    Rect gainEnhancementCapacityConsciousness = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementCapacityConsciousness);
                    Rect gainEnhancementCapacityConsciousnessLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementCapacityConsciousnessLabel, "CR_CapacityConsciousnessLabel".Translate());
                    Rect gainFactorMultCapacityConsciousness = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultCapacityConsciousness);
                    UIUtility.ToolTipRow(gainFactorMultCapacityConsciousness, "CR_GainFactorMultCapacityConsciousnessDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMultCapacityConsciousness, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultCapacityConsciousness, ref UIUtility.NumericBuffer.gainFactorMultCapacityConsciousness, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect gainFactorMaxCapacityConsciousness = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxCapacityConsciousness);
                    UIUtility.ToolTipRow(gainFactorMaxCapacityConsciousness, "CR_GainFactorMaxCapacityConsciousnessDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMaxCapacityConsciousness, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxCapacityConsciousness, ref UIUtility.NumericBuffer.gainFactorMaxCapacityConsciousness, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect initializeValueGainEnhancementCapacityConsciousness = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementCapacityConsciousness, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultCapacityConsciousness));
                        m_Settings.gainFactorMultCapacityConsciousness = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultCapacityConsciousness = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxCapacityConsciousness));
                        m_Settings.gainFactorMaxCapacityConsciousness = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxCapacityConsciousness = defaultMax.ToString();
                        SettingValues();
                    }, "CR_CapacityConsciousnessLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //CapacityBloodFiltration
                    Rect gainEnhancementCapacityBloodFiltration = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementCapacityBloodFiltration);
                    Rect gainEnhancementCapacityBloodFiltrationLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementCapacityBloodFiltrationLabel, "CR_CapacityBloodFiltrationLabel".Translate());
                    Rect gainFactorMultCapacityBloodFiltration = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultCapacityBloodFiltration);
                    UIUtility.ToolTipRow(gainFactorMultCapacityBloodFiltration, "CR_GainFactorMultCapacityBloodFiltrationDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMultCapacityBloodFiltration, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultCapacityBloodFiltration, ref UIUtility.NumericBuffer.gainFactorMultCapacityBloodFiltration, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect gainFactorMaxCapacityBloodFiltration = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxCapacityBloodFiltration);
                    UIUtility.ToolTipRow(gainFactorMaxCapacityBloodFiltration, "CR_GainFactorMaxCapacityBloodFiltrationDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMaxCapacityBloodFiltration, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxCapacityBloodFiltration, ref UIUtility.NumericBuffer.gainFactorMaxCapacityBloodFiltration, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect initializeValueGainEnhancementCapacityBloodFiltration = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementCapacityBloodFiltration, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultCapacityBloodFiltration));
                        m_Settings.gainFactorMultCapacityBloodFiltration = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultCapacityBloodFiltration = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxCapacityBloodFiltration));
                        m_Settings.gainFactorMaxCapacityBloodFiltration = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxCapacityBloodFiltration = defaultMax.ToString();
                        SettingValues();
                    }, "CR_CapacityBloodFiltrationLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //CapacityBloodPumping
                    Rect gainEnhancementCapacityBloodPumping = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementCapacityBloodPumping);
                    Rect gainEnhancementCapacityBloodPumpingLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementCapacityBloodPumpingLabel, "CR_CapacityBloodPumpingLabel".Translate());
                    Rect gainFactorMultCapacityBloodPumping = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultCapacityBloodPumping);
                    UIUtility.ToolTipRow(gainFactorMultCapacityBloodPumping, "CR_GainFactorMultCapacityBloodPumpingDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMultCapacityBloodPumping, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultCapacityBloodPumping, ref UIUtility.NumericBuffer.gainFactorMultCapacityBloodPumping, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect gainFactorMaxCapacityBloodPumping = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxCapacityBloodPumping);
                    UIUtility.ToolTipRow(gainFactorMaxCapacityBloodPumping, "CR_GainFactorMaxCapacityBloodPumpingDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMaxCapacityBloodPumping, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxCapacityBloodPumping, ref UIUtility.NumericBuffer.gainFactorMaxCapacityBloodPumping, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect initializeValueGainEnhancementCapacityBloodPumping = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementCapacityBloodPumping, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultCapacityBloodPumping));
                        m_Settings.gainFactorMultCapacityBloodPumping = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultCapacityBloodPumping = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxCapacityBloodPumping));
                        m_Settings.gainFactorMaxCapacityBloodPumping = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxCapacityBloodPumping = defaultMax.ToString();
                        SettingValues();
                    }, "CR_CapacityBloodPumpingLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //CapacityBreathing
                    Rect gainEnhancementCapacityBreathing = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementCapacityBreathing);
                    Rect gainEnhancementCapacityBreathingLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementCapacityBreathingLabel, "CR_CapacityBreathingLabel".Translate());
                    Rect gainFactorMultCapacityBreathing = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultCapacityBreathing);
                    UIUtility.ToolTipRow(gainFactorMultCapacityBreathing, "CR_GainFactorMultCapacityBreathingDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMultCapacityBreathing, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultCapacityBreathing, ref UIUtility.NumericBuffer.gainFactorMultCapacityBreathing, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect gainFactorMaxCapacityBreathing = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxCapacityBreathing);
                    UIUtility.ToolTipRow(gainFactorMaxCapacityBreathing, "CR_GainFactorMaxCapacityBreathingDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(gainFactorMaxCapacityBreathing, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxCapacityBreathing, ref UIUtility.NumericBuffer.gainFactorMaxCapacityBreathing, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);
                    Rect initializeValueGainEnhancementCapacityBreathing = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementCapacityBreathing, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultCapacityBreathing));
                        m_Settings.gainFactorMultCapacityBreathing = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultCapacityBreathing = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxCapacityBreathing));
                        m_Settings.gainFactorMaxCapacityBreathing = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxCapacityBreathing = defaultMax.ToString();
                        SettingValues();
                    }, "CR_CapacityBreathingLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;
                }
                else
                {
                    //Pain
                    Rect gainEnhancementPain = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementPain);
                    Rect gainEnhancementPainLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementPainLabel, "CR_PainLabel".Translate());
                    Rect gainFactorMultPain = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultPain);
                    UIUtility.ToolTipRow(gainFactorMultPain, "CR_GainFactorMultPainDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMultPain, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultPain, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMultPain = m_Settings.gainFactorMultPain.ToString();
                    }
                    Rect gainFactorMaxPain = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxPain);
                    UIUtility.ToolTipRow(gainFactorMaxPain, "CR_GainFactorMaxPainDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMaxPain, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxPain, ref valueChanged, 0f, 1f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMaxPain = m_Settings.gainFactorMaxPain.ToString();
                    }
                    Rect initializeValueGainEnhancementPain = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementPain, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultPain));
                        m_Settings.gainFactorMultPain = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultPain = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxPain));
                        m_Settings.gainFactorMaxPain = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxPain = defaultMax.ToString();
                        SettingValues();
                    }, "CR_PainLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //ArmorRating_Blunt
                    Rect gainEnhancementArmorRating_Blunt = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementArmorRating_Blunt);
                    Rect gainEnhancementArmorRating_BluntLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementArmorRating_BluntLabel, "CR_ArmorRating_BluntLabel".Translate());
                    Rect gainFactorMultArmorRating_Blunt = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultArmorRating_Blunt);
                    UIUtility.ToolTipRow(gainFactorMultArmorRating_Blunt, "CR_GainFactorMultArmorRating_BluntDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMultArmorRating_Blunt, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultArmorRating_Blunt, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMultArmorRating_Blunt = m_Settings.gainFactorMultArmorRating_Blunt.ToString();
                    }
                    Rect gainFactorMaxArmorRating_Blunt = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxArmorRating_Blunt);
                    UIUtility.ToolTipRow(gainFactorMaxArmorRating_Blunt, "CR_GainFactorMaxArmorRating_BluntDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMaxArmorRating_Blunt, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxArmorRating_Blunt, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMaxArmorRating_Blunt = m_Settings.gainFactorMaxArmorRating_Blunt.ToString();
                    }
                    Rect initializeValueGainEnhancementArmorRating_Blunt = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementArmorRating_Blunt, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultArmorRating_Blunt));
                        m_Settings.gainFactorMultArmorRating_Blunt = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultArmorRating_Blunt = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxArmorRating_Blunt));
                        m_Settings.gainFactorMaxArmorRating_Blunt = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxArmorRating_Blunt = defaultMax.ToString();
                        SettingValues();
                    }, "CR_ArmorRating_BluntLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //ArmorRating_Sharp
                    Rect gainEnhancementArmorRating_Sharp = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementArmorRating_Sharp);
                    Rect gainEnhancementArmorRating_SharpLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementArmorRating_SharpLabel, "CR_ArmorRating_SharpLabel".Translate());
                    Rect gainFactorMultArmorRating_Sharp = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultArmorRating_Sharp);
                    UIUtility.ToolTipRow(gainFactorMultArmorRating_Sharp, "CR_GainFactorMultArmorRating_SharpDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMultArmorRating_Sharp, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultArmorRating_Sharp, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMultArmorRating_Sharp = m_Settings.gainFactorMultArmorRating_Sharp.ToString();
                    }
                    Rect gainFactorMaxArmorRating_Sharp = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxArmorRating_Sharp);
                    UIUtility.ToolTipRow(gainFactorMaxArmorRating_Sharp, "CR_GainFactorMaxArmorRating_SharpDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMaxArmorRating_Sharp, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxArmorRating_Sharp, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMaxArmorRating_Sharp = m_Settings.gainFactorMaxArmorRating_Sharp.ToString();
                    }
                    Rect initializeValueGainEnhancementArmorRating_Sharp = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementArmorRating_Sharp, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultArmorRating_Sharp));
                        m_Settings.gainFactorMultArmorRating_Sharp = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultArmorRating_Sharp = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxArmorRating_Sharp));
                        m_Settings.gainFactorMaxArmorRating_Sharp = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxArmorRating_Sharp = defaultMax.ToString();
                        SettingValues();
                    }, "CR_ArmorRating_SharpLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //ArmorRating_Heat
                    Rect gainEnhancementArmorRating_Heat = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementArmorRating_Heat);
                    Rect gainEnhancementArmorRating_HeatLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementArmorRating_HeatLabel, "CR_ArmorRating_HeatLabel".Translate());
                    Rect gainFactorMultArmorRating_Heat = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultArmorRating_Heat);
                    UIUtility.ToolTipRow(gainFactorMultArmorRating_Heat, "CR_GainFactorMultArmorRating_HeatDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMultArmorRating_Heat, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultArmorRating_Heat, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMultArmorRating_Heat = m_Settings.gainFactorMultArmorRating_Heat.ToString();
                    }
                    Rect gainFactorMaxArmorRating_Heat = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxArmorRating_Heat);
                    UIUtility.ToolTipRow(gainFactorMaxArmorRating_Heat, "CR_GainFactorMaxArmorRating_HeatDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMaxArmorRating_Heat, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxArmorRating_Heat, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMaxArmorRating_Heat = m_Settings.gainFactorMaxArmorRating_Heat.ToString();
                    }
                    Rect initializeValueGainEnhancementArmorRating_Heat = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementArmorRating_Heat, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultArmorRating_Heat));
                        m_Settings.gainFactorMultArmorRating_Heat = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultArmorRating_Heat = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxArmorRating_Heat));
                        m_Settings.gainFactorMaxArmorRating_Heat = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxArmorRating_Heat = defaultMax.ToString();
                        SettingValues();
                    }, "CR_ArmorRating_HeatLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //MeleeDodgeChance
                    Rect gainEnhancementMeleeDodgeChance = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementMeleeDodgeChance);
                    Rect gainEnhancementMeleeDodgeChanceLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementMeleeDodgeChanceLabel, "CR_MeleeDodgeChanceLabel".Translate());
                    Rect gainFactorMultMeleeDodgeChance = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultMeleeDodgeChance);
                    UIUtility.ToolTipRow(gainFactorMultMeleeDodgeChance, "CR_GainFactorMultMeleeDodgeChanceDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMultMeleeDodgeChance, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultMeleeDodgeChance, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMultMeleeDodgeChance = m_Settings.gainFactorMultMeleeDodgeChance.ToString();
                    }
                    Rect gainFactorMaxMeleeDodgeChance = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxMeleeDodgeChance);
                    UIUtility.ToolTipRow(gainFactorMaxMeleeDodgeChance, "CR_GainFactorMaxMeleeDodgeChanceDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMaxMeleeDodgeChance, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxMeleeDodgeChance, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMaxMeleeDodgeChance = m_Settings.gainFactorMaxMeleeDodgeChance.ToString();
                    }
                    Rect initializeValueGainEnhancementMeleeDodgeChance = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementMeleeDodgeChance, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultMeleeDodgeChance));
                        m_Settings.gainFactorMultMeleeDodgeChance = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultMeleeDodgeChance = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxMeleeDodgeChance));
                        m_Settings.gainFactorMaxMeleeDodgeChance = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxMeleeDodgeChance = defaultMax.ToString();
                        SettingValues();
                    }, "CR_MeleeDodgeChanceLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //MeleeHitChance
                    Rect gainEnhancementMeleeHitChance = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementMeleeHitChance);
                    Rect gainEnhancementMeleeHitChanceLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementMeleeHitChanceLabel, "CR_MeleeHitChanceLabel".Translate());
                    Rect gainFactorMultMeleeHitChance = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultMeleeHitChance);
                    UIUtility.ToolTipRow(gainFactorMultMeleeHitChance, "CR_GainFactorMultMeleeHitChanceDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMultMeleeHitChance, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultMeleeHitChance, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMultMeleeHitChance = m_Settings.gainFactorMultMeleeHitChance.ToString();
                    }
                    Rect gainFactorMaxMeleeHitChance = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxMeleeHitChance);
                    UIUtility.ToolTipRow(gainFactorMaxMeleeHitChance, "CR_GainFactorMaxMeleeHitChanceDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMaxMeleeHitChance, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxMeleeHitChance, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMaxMeleeHitChance = m_Settings.gainFactorMaxMeleeHitChance.ToString();
                    }
                    Rect initializeValueGainEnhancementMeleeHitChance = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementMeleeHitChance, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultMeleeHitChance));
                        m_Settings.gainFactorMultMeleeHitChance = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultMeleeHitChance = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxMeleeHitChance));
                        m_Settings.gainFactorMaxMeleeHitChance = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxMeleeHitChance = defaultMax.ToString();
                        SettingValues();
                    }, "CR_MeleeHitChanceLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //MoveSpeed
                    Rect gainEnhancementMoveSpeed = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementMoveSpeed);
                    Rect gainEnhancementMoveSpeedLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementMoveSpeedLabel, "CR_MoveSpeedLabel".Translate());
                    Rect gainFactorMultMoveSpeed = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultMoveSpeed);
                    UIUtility.ToolTipRow(gainFactorMultMoveSpeed, "CR_GainFactorMultMoveSpeedDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMultMoveSpeed, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultMoveSpeed, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMultMoveSpeed = m_Settings.gainFactorMultMoveSpeed.ToString();
                    }
                    Rect gainFactorMaxMoveSpeed = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxMoveSpeed);
                    UIUtility.ToolTipRow(gainFactorMaxMoveSpeed, "CR_GainFactorMaxMoveSpeedDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMaxMoveSpeed, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxMoveSpeed, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMaxMoveSpeed = m_Settings.gainFactorMaxMoveSpeed.ToString();
                    }
                    Rect initializeValueGainEnhancementMoveSpeed = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementMoveSpeed, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultMoveSpeed));
                        m_Settings.gainFactorMultMoveSpeed = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultMoveSpeed = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxMoveSpeed));
                        m_Settings.gainFactorMaxMoveSpeed = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxMoveSpeed = defaultMax.ToString();
                        SettingValues();
                    }, "CR_MoveSpeedLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //ShootingAccuracyPawn
                    Rect gainEnhancementShootingAccuracyPawn = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementShootingAccuracyPawn);
                    Rect gainEnhancementShootingAccuracyPawnLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementShootingAccuracyPawnLabel, "CR_ShootingAccuracyPawnLabel".Translate());
                    Rect gainFactorMultShootingAccuracyPawn = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultShootingAccuracyPawn);
                    UIUtility.ToolTipRow(gainFactorMultShootingAccuracyPawn, "CR_GainFactorMultShootingAccuracyPawnDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMultShootingAccuracyPawn, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultShootingAccuracyPawn, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMultShootingAccuracyPawn = m_Settings.gainFactorMultShootingAccuracyPawn.ToString();
                    }
                    Rect gainFactorMaxShootingAccuracyPawn = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxShootingAccuracyPawn);
                    UIUtility.ToolTipRow(gainFactorMaxShootingAccuracyPawn, "CR_GainFactorMaxShootingAccuracyPawnDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMaxShootingAccuracyPawn, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxShootingAccuracyPawn, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMaxShootingAccuracyPawn = m_Settings.gainFactorMaxShootingAccuracyPawn.ToString();
                    }
                    Rect initializeValueGainEnhancementShootingAccuracyPawn = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementShootingAccuracyPawn, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultShootingAccuracyPawn));
                        m_Settings.gainFactorMultShootingAccuracyPawn = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultShootingAccuracyPawn = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxShootingAccuracyPawn));
                        m_Settings.gainFactorMaxShootingAccuracyPawn = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxShootingAccuracyPawn = defaultMax.ToString();
                        SettingValues();
                    }, "CR_ShootingAccuracyPawnLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //PawnTrapSpringChance
                    Rect gainEnhancementPawnTrapSpringChance = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementPawnTrapSpringChance);
                    Rect gainEnhancementPawnTrapSpringChanceLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementPawnTrapSpringChanceLabel, "CR_PawnTrapSpringChanceLabel".Translate());
                    Rect gainFactorMultPawnTrapSpringChance = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultPawnTrapSpringChance);
                    UIUtility.ToolTipRow(gainFactorMultPawnTrapSpringChance, "CR_GainFactorMultPawnTrapSpringChanceDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMultPawnTrapSpringChance, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultPawnTrapSpringChance, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMultPawnTrapSpringChance = m_Settings.gainFactorMultPawnTrapSpringChance.ToString();
                    }
                    Rect gainFactorMaxPawnTrapSpringChance = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxPawnTrapSpringChance);
                    UIUtility.ToolTipRow(gainFactorMaxPawnTrapSpringChance, "CR_GainFactorMaxPawnTrapSpringChanceDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMaxPawnTrapSpringChance, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxPawnTrapSpringChance, ref valueChanged, 0f, 1f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMaxPawnTrapSpringChance = m_Settings.gainFactorMaxPawnTrapSpringChance.ToString();
                    }
                    Rect initializeValueGainEnhancementPawnTrapSpringChance = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementPawnTrapSpringChance, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultPawnTrapSpringChance));
                        m_Settings.gainFactorMultPawnTrapSpringChance = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultPawnTrapSpringChance = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxPawnTrapSpringChance));
                        m_Settings.gainFactorMaxPawnTrapSpringChance = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxPawnTrapSpringChance = defaultMax.ToString();
                        SettingValues();
                    }, "CR_PawnTrapSpringChanceLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //CapacitySight
                    Rect gainEnhancementCapacitySight = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementCapacitySight);
                    Rect gainEnhancementCapacitySightLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementCapacitySightLabel, "CR_CapacitySightLabel".Translate());
                    Rect gainFactorMultCapacitySight = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultCapacitySight);
                    UIUtility.ToolTipRow(gainFactorMultCapacitySight, "CR_GainFactorMultCapacitySightDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMultCapacitySight, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultCapacitySight, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMultCapacitySight = m_Settings.gainFactorMultCapacitySight.ToString();
                    }
                    Rect gainFactorMaxCapacitySight = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxCapacitySight);
                    UIUtility.ToolTipRow(gainFactorMaxCapacitySight, "CR_GainFactorMaxCapacitySightDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMaxCapacitySight, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxCapacitySight, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMaxCapacitySight = m_Settings.gainFactorMaxCapacitySight.ToString();
                    }
                    Rect initializeValueGainEnhancementCapacitySight = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementCapacitySight, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultCapacitySight));
                        m_Settings.gainFactorMultCapacitySight = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultCapacitySight = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxCapacitySight));
                        m_Settings.gainFactorMaxCapacitySight = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxCapacitySight = defaultMax.ToString();
                        SettingValues();
                    }, "CR_CapacitySightLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //CapacityMoving
                    Rect gainEnhancementCapacityMoving = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementCapacityMoving);
                    Rect gainEnhancementCapacityMovingLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementCapacityMovingLabel, "CR_CapacityMovingLabel".Translate());
                    Rect gainFactorMultCapacityMoving = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultCapacityMoving);
                    UIUtility.ToolTipRow(gainFactorMultCapacityMoving, "CR_GainFactorMultCapacityMovingDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMultCapacityMoving, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultCapacityMoving, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMultCapacityMoving = m_Settings.gainFactorMultCapacityMoving.ToString();
                    }
                    Rect gainFactorMaxCapacityMoving = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxCapacityMoving);
                    UIUtility.ToolTipRow(gainFactorMaxCapacityMoving, "CR_GainFactorMaxCapacityMovingDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMaxCapacityMoving, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxCapacityMoving, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMaxCapacityMoving = m_Settings.gainFactorMaxCapacityMoving.ToString();
                    }
                    Rect initializeValueGainEnhancementCapacityMoving = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementCapacityMoving, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultCapacityMoving));
                        m_Settings.gainFactorMultCapacityMoving = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultCapacityMoving = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxCapacityMoving));
                        m_Settings.gainFactorMaxCapacityMoving = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxCapacityMoving = defaultMax.ToString();
                        SettingValues();
                    }, "CR_CapacityMovingLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //CapacityHearing
                    Rect gainEnhancementCapacityHearing = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementCapacityHearing);
                    Rect gainEnhancementCapacityHearingLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementCapacityHearingLabel, "CR_CapacityHearingLabel".Translate());
                    Rect gainFactorMultCapacityHearing = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultCapacityHearing);
                    UIUtility.ToolTipRow(gainFactorMultCapacityHearing, "CR_GainFactorMultCapacityHearingDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMultCapacityHearing, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultCapacityHearing, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMultCapacityHearing = m_Settings.gainFactorMultCapacityHearing.ToString();
                    }
                    Rect gainFactorMaxCapacityHearing = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxCapacityHearing);
                    UIUtility.ToolTipRow(gainFactorMaxCapacityHearing, "CR_GainFactorMaxCapacityHearingDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMaxCapacityHearing, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxCapacityHearing, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMaxCapacityHearing = m_Settings.gainFactorMaxCapacityHearing.ToString();
                    }
                    Rect initializeValueGainEnhancementCapacityHearing = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementCapacityHearing, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultCapacityHearing));
                        m_Settings.gainFactorMultCapacityHearing = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultCapacityHearing = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxCapacityHearing));
                        m_Settings.gainFactorMaxCapacityHearing = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxCapacityHearing = defaultMax.ToString();
                        SettingValues();
                    }, "CR_CapacityHearingLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //CapacityManipulation
                    Rect gainEnhancementCapacityManipulation = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementCapacityManipulation);
                    Rect gainEnhancementCapacityManipulationLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementCapacityManipulationLabel, "CR_CapacityManipulationLabel".Translate());
                    Rect gainFactorMultCapacityManipulation = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultCapacityManipulation);
                    UIUtility.ToolTipRow(gainFactorMultCapacityManipulation, "CR_GainFactorMultCapacityManipulationDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMultCapacityManipulation, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultCapacityManipulation, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMultCapacityManipulation = m_Settings.gainFactorMultCapacityManipulation.ToString();
                    }
                    Rect gainFactorMaxCapacityManipulation = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxCapacityManipulation);
                    UIUtility.ToolTipRow(gainFactorMaxCapacityManipulation, "CR_GainFactorMaxCapacityManipulationDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMaxCapacityManipulation, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxCapacityManipulation, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMaxCapacityManipulation = m_Settings.gainFactorMaxCapacityManipulation.ToString();
                    }
                    Rect initializeValueGainEnhancementCapacityManipulation = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementCapacityManipulation, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultCapacityManipulation));
                        m_Settings.gainFactorMultCapacityManipulation = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultCapacityManipulation = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxCapacityManipulation));
                        m_Settings.gainFactorMaxCapacityManipulation = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxCapacityManipulation = defaultMax.ToString();
                        SettingValues();
                    }, "CR_CapacityManipulationLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //CapacityMetabolism
                    Rect gainEnhancementCapacityMetabolism = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementCapacityMetabolism);
                    Rect gainEnhancementCapacityMetabolismLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementCapacityMetabolismLabel, "CR_CapacityMetabolismLabel".Translate());
                    Rect gainFactorMultCapacityMetabolism = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultCapacityMetabolism);
                    UIUtility.ToolTipRow(gainFactorMultCapacityMetabolism, "CR_GainFactorMultCapacityMetabolismDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMultCapacityMetabolism, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultCapacityMetabolism, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMultCapacityMetabolism = m_Settings.gainFactorMultCapacityMetabolism.ToString();
                    }
                    Rect gainFactorMaxCapacityMetabolism = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxCapacityMetabolism);
                    UIUtility.ToolTipRow(gainFactorMaxCapacityMetabolism, "CR_GainFactorMaxCapacityMetabolismDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMaxCapacityMetabolism, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxCapacityMetabolism, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMaxCapacityMetabolism = m_Settings.gainFactorMaxCapacityMetabolism.ToString();
                    }
                    Rect initializeValueGainEnhancementCapacityMetabolism = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementCapacityMetabolism, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultCapacityMetabolism));
                        m_Settings.gainFactorMultCapacityMetabolism = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultCapacityMetabolism = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxCapacityMetabolism));
                        m_Settings.gainFactorMaxCapacityMetabolism = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxCapacityMetabolism = defaultMax.ToString();
                        SettingValues();
                    }, "CR_CapacityMetabolismLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //CapacityConsciousness
                    Rect gainEnhancementCapacityConsciousness = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementCapacityConsciousness);
                    Rect gainEnhancementCapacityConsciousnessLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementCapacityConsciousnessLabel, "CR_CapacityConsciousnessLabel".Translate());
                    Rect gainFactorMultCapacityConsciousness = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultCapacityConsciousness);
                    UIUtility.ToolTipRow(gainFactorMultCapacityConsciousness, "CR_GainFactorMultCapacityConsciousnessDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMultCapacityConsciousness, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultCapacityConsciousness, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMultCapacityConsciousness = m_Settings.gainFactorMultCapacityConsciousness.ToString();
                    }
                    Rect gainFactorMaxCapacityConsciousness = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxCapacityConsciousness);
                    UIUtility.ToolTipRow(gainFactorMaxCapacityConsciousness, "CR_GainFactorMaxCapacityConsciousnessDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMaxCapacityConsciousness, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxCapacityConsciousness, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMaxCapacityConsciousness = m_Settings.gainFactorMaxCapacityConsciousness.ToString();
                    }
                    Rect initializeValueGainEnhancementCapacityConsciousness = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementCapacityConsciousness, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultCapacityConsciousness));
                        m_Settings.gainFactorMultCapacityConsciousness = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultCapacityConsciousness = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxCapacityConsciousness));
                        m_Settings.gainFactorMaxCapacityConsciousness = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxCapacityConsciousness = defaultMax.ToString();
                        SettingValues();
                    }, "CR_CapacityConsciousnessLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //CapacityBloodFiltration
                    Rect gainEnhancementCapacityBloodFiltration = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementCapacityBloodFiltration);
                    Rect gainEnhancementCapacityBloodFiltrationLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementCapacityBloodFiltrationLabel, "CR_CapacityBloodFiltrationLabel".Translate());
                    Rect gainFactorMultCapacityBloodFiltration = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultCapacityBloodFiltration);
                    UIUtility.ToolTipRow(gainFactorMultCapacityBloodFiltration, "CR_GainFactorMultCapacityBloodFiltrationDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMultCapacityBloodFiltration, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultCapacityBloodFiltration, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMultCapacityBloodFiltration = m_Settings.gainFactorMultCapacityBloodFiltration.ToString();
                    }
                    Rect gainFactorMaxCapacityBloodFiltration = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxCapacityBloodFiltration);
                    UIUtility.ToolTipRow(gainFactorMaxCapacityBloodFiltration, "CR_GainFactorMaxCapacityBloodFiltrationDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMaxCapacityBloodFiltration, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxCapacityBloodFiltration, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMaxCapacityBloodFiltration = m_Settings.gainFactorMaxCapacityBloodFiltration.ToString();
                    }
                    Rect initializeValueGainEnhancementCapacityBloodFiltration = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementCapacityBloodFiltration, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultCapacityBloodFiltration));
                        m_Settings.gainFactorMultCapacityBloodFiltration = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultCapacityBloodFiltration = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxCapacityBloodFiltration));
                        m_Settings.gainFactorMaxCapacityBloodFiltration = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxCapacityBloodFiltration = defaultMax.ToString();
                        SettingValues();
                    }, "CR_CapacityBloodFiltrationLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //CapacityBloodPumping
                    Rect gainEnhancementCapacityBloodPumping = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementCapacityBloodPumping);
                    Rect gainEnhancementCapacityBloodPumpingLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementCapacityBloodPumpingLabel, "CR_CapacityBloodPumpingLabel".Translate());
                    Rect gainFactorMultCapacityBloodPumping = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultCapacityBloodPumping);
                    UIUtility.ToolTipRow(gainFactorMultCapacityBloodPumping, "CR_GainFactorMultCapacityBloodPumpingDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMultCapacityBloodPumping, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultCapacityBloodPumping, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMultCapacityBloodPumping = m_Settings.gainFactorMultCapacityBloodPumping.ToString();
                    }
                    Rect gainFactorMaxCapacityBloodPumping = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxCapacityBloodPumping);
                    UIUtility.ToolTipRow(gainFactorMaxCapacityBloodPumping, "CR_GainFactorMaxCapacityBloodPumpingDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMaxCapacityBloodPumping, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxCapacityBloodPumping, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMaxCapacityBloodPumping = m_Settings.gainFactorMaxCapacityBloodPumping.ToString();
                    }
                    Rect initializeValueGainEnhancementCapacityBloodPumping = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementCapacityBloodPumping, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultCapacityBloodPumping));
                        m_Settings.gainFactorMultCapacityBloodPumping = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultCapacityBloodPumping = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxCapacityBloodPumping));
                        m_Settings.gainFactorMaxCapacityBloodPumping = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxCapacityBloodPumping = defaultMax.ToString();
                        SettingValues();
                    }, "CR_CapacityBloodPumpingLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //CapacityBreathing
                    Rect gainEnhancementCapacityBreathing = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainEnhancementCapacityBreathing);
                    Rect gainEnhancementCapacityBreathingLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(gainEnhancementCapacityBreathingLabel, "CR_CapacityBreathingLabel".Translate());
                    Rect gainFactorMultCapacityBreathing = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMultCapacityBreathing);
                    UIUtility.ToolTipRow(gainFactorMultCapacityBreathing, "CR_GainFactorMultCapacityBreathingDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMultCapacityBreathing, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.gainFactorMultCapacityBreathing, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMultCapacityBreathing = m_Settings.gainFactorMultCapacityBreathing.ToString();
                    }
                    Rect gainFactorMaxCapacityBreathing = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(gainFactorMaxCapacityBreathing);
                    UIUtility.ToolTipRow(gainFactorMaxCapacityBreathing, "CR_GainFactorMaxCapacityBreathingDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(gainFactorMaxCapacityBreathing, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.gainFactorMaxCapacityBreathing, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.gainFactorMaxCapacityBreathing = m_Settings.gainFactorMaxCapacityBreathing.ToString();
                    }
                    Rect initializeValueGainEnhancementCapacityBreathing = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueGainEnhancementCapacityBreathing, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMultCapacityBreathing));
                        m_Settings.gainFactorMultCapacityBreathing = defaultMult;
                        UIUtility.NumericBuffer.gainFactorMultCapacityBreathing = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.gainFactorMaxCapacityBreathing));
                        m_Settings.gainFactorMaxCapacityBreathing = defaultMax;
                        UIUtility.NumericBuffer.gainFactorMaxCapacityBreathing = defaultMax.ToString();
                        SettingValues();
                    }, "CR_CapacityBreathingLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;


                }
            }
            else
            {
                Text.Font = GameFont.Medium;
                Rect deactiveRect = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_CHECKBOX, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                Widgets.Label(deactiveRect, "CR_SectionDisableMessage".Translate());
                Text.Font = GameFont.Small;
            }
        }
        #endregion

        #region EditAddBionicsSection
        private int CalcAddBionicsSectionRowNum()
        {
            int num = 1;
            if (m_Settings.enableAddBionicsOption)
            {
                num += 4;
            }
            else
            {
                num += 1;
            }
            return num;
        }
        private void EditAddBionicsSection(Rect inRect, ref bool valueChanged)
        {
            float marginTop = 0f;
            if (!m_Settings.enableAddBionicsOption)
            {
                Widgets.DrawBoxSolid(inRect, UIUtility.SUB_SECTION_BACKGROUND_DEACTIVE_COLOR);
            }
            Widgets.DrawBox(inRect);

            Rect titleRect = new Rect(inRect.x, inRect.y, inRect.width - (UIUtility.MARGIN_LEFT), UIUtility.HEIGHT_SECTION_TITLE);
            marginTop += UIUtility.WriteFloatedSectionTitle(titleRect, "CR_OptionSectionTitle".Translate("CR_OptionAddBionicsTitle".Translate()), ref m_Settings.enableAddBionicsOption, ref valueChanged, () => m_Settings.InitializeValues(SettingValues, CompressedRaidMod.FunctionType.AddBionics)) + UIUtility.MARGIN_BOTTOM;

            float labelWidth = 75f;
            float textWidth = 206.25f;

            if (m_Settings.enableAddBionicsOption)
            {
                if (m_Settings.radioTextInput)
                {
                    //ChanceFactor
                    Rect addBionicsChanceFactorRow = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(addBionicsChanceFactorRow);
                    Rect addBionicsChanceFactorLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(addBionicsChanceFactorLabel, "CR_AddBionicsChanceLabel".Translate());
                    Rect addBionicsChanceFactor = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(addBionicsChanceFactor);
                    UIUtility.ToolTipRow(addBionicsChanceFactor, "CR_AddBionicsChanceFactorDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(addBionicsChanceFactor, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.addBionicsChanceFactor, ref UIUtility.NumericBuffer.addBionicsChanceFactor, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);

                    //ChanceMax
                    Rect addBionicsChanceMax = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(addBionicsChanceMax);
                    UIUtility.ToolTipRow(addBionicsChanceMax, "CR_AddBionicsChanceMaxDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(addBionicsChanceMax, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.addBionicsChanceMax, ref UIUtility.NumericBuffer.addBionicsChanceMax, ref valueChanged, 0f, 1f, () => { }, TextAnchor.MiddleRight, true);
                    Rect initializeValueAddBionicsChanceMax = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueAddBionicsChanceMax, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.addBionicsChanceFactor));
                        m_Settings.addBionicsChanceFactor = defaultMult;
                        UIUtility.NumericBuffer.addBionicsChanceFactor = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.addBionicsChanceMax));
                        m_Settings.addBionicsChanceMax = defaultMax;
                        UIUtility.NumericBuffer.addBionicsChanceMax = defaultMax.ToString();
                        SettingValues();
                    }, "CR_AddBionicsChanceLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //addBionicsChanceNegativeCurve
                    Rect addBionicsChanceNegativeCurveRow = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(addBionicsChanceNegativeCurveRow);
                    UIUtility.ToolTipRow(addBionicsChanceNegativeCurveRow, "CR_AddBionicsChanceNegativeCurveDesc".Translate());
                    UIUtility.LabeledTextBoxPercentage(addBionicsChanceNegativeCurveRow, UIUtility.LABEL_WIDTH_HALF, UIUtility.INPUT_WIDTH_HALF, "CR_AddBionicsChanceNegativeCurveLabel".Translate(), ref m_Settings.addBionicsChanceNegativeCurve, ref UIUtility.NumericBuffer.addBionicsChanceNegativeCurve, ref valueChanged, 0f, 1f, () => {
                        float defaultValue = StaticVariables.GetValue<float>(nameof(m_Settings.addBionicsChanceNegativeCurve));
                        m_Settings.addBionicsChanceNegativeCurve = defaultValue;
                        UIUtility.NumericBuffer.addBionicsChanceNegativeCurve = defaultValue.ToString();
                        SettingValues();
                    });
                    marginTop += UIUtility.HEIGHT_ROW;

                    //addBionicsMaxNumber
                    Rect addBionicsMaxNumberRow = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(addBionicsMaxNumberRow);
                    UIUtility.ToolTipRow(addBionicsMaxNumberRow, "CR_AddBionicsMaxNumberDesc".Translate());
                    UIUtility.LabeledTextBoxInt(addBionicsMaxNumberRow, UIUtility.LABEL_WIDTH_HALF, UIUtility.INPUT_WIDTH_HALF, "CR_AddBionicsMaxNumberLabel".Translate(), ref m_Settings.addBionicsMaxNumber, ref UIUtility.NumericBuffer.addBionicsMaxNumber, ref valueChanged, 1, 100, () =>
                    {
                        int defaultValue = StaticVariables.GetValue<int>(nameof(m_Settings.addBionicsMaxNumber));
                        m_Settings.addBionicsMaxNumber = defaultValue;
                        UIUtility.NumericBuffer.addBionicsMaxNumber = defaultValue.ToString();
                        SettingValues();
                    });
                    marginTop += UIUtility.HEIGHT_ROW;

                    //allowedByGTTechLevel
                    Rect allowedByGTTechLevelRow = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(allowedByGTTechLevelRow);
                    UIUtility.ToolTipRow(allowedByGTTechLevelRow, "CR_AllowedByGTTechLevelDesc".Translate());
                    UIUtility.LabeledTextBoxInt(allowedByGTTechLevelRow, UIUtility.LABEL_WIDTH_HALF, UIUtility.INPUT_WIDTH_HALF, "CR_AllowedByGTTechLevelLabel".Translate(), ref m_Settings.allowedByGTTechLevel, ref UIUtility.NumericBuffer.allowedByGTTechLevel, ref valueChanged, 0x00, UIUtility.TECH_LEVEL_MAX, () => {
                        int defaultValue = StaticVariables.GetValue<int>(nameof(m_Settings.allowedByGTTechLevel));
                        m_Settings.allowedByGTTechLevel = defaultValue;
                        UIUtility.NumericBuffer.allowedByGTTechLevel = defaultValue.ToString();
                        SettingValues();
                    }, (int val) => {
                        if (val >= 0 && val <= UIUtility.TECH_LEVEL_MAX)
                        {
                            TechLevel techLevel = (TechLevel)Enum.ToObject(typeof(TechLevel), val);
                            if (techLevel == TechLevel.Undefined)
                            {
                                return String.Format("{0} ", "CR_NoTechLevelLimmitLabel".Translate());
                            }
                            else
                            {
                                return String.Format("{0} [{1}]", String.Format("TechLevel_{0}", techLevel).Translate(), techLevel);
                            }
                        }
                        else
                        {
                            return null;
                        }
                    });
                    marginTop += UIUtility.HEIGHT_ROW;
                }
                else
                {
                    //ChanceFactor
                    Rect addBionicsChanceFactorRow = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(addBionicsChanceFactorRow);
                    Rect addBionicsChanceFactorLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(addBionicsChanceFactorLabel, "CR_AddBionicsChanceLabel".Translate());
                    Rect addBionicsChanceFactor = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(addBionicsChanceFactor);
                    UIUtility.ToolTipRow(addBionicsChanceFactor, "CR_AddBionicsChanceFactorDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(addBionicsChanceFactor, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.addBionicsChanceFactor, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.addBionicsChanceFactor = m_Settings.addBionicsChanceFactor.ToString();
                    }
                    //, ref UIUtility.NumericBuffer.addBionicsChanceFactor
                    //ChanceMax
                    Rect addBionicsChanceMax = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(addBionicsChanceMax);
                    UIUtility.ToolTipRow(addBionicsChanceMax, "CR_AddBionicsChanceMaxDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(addBionicsChanceMax, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.addBionicsChanceMax, ref valueChanged, 0f, 1f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.addBionicsChanceMax = m_Settings.addBionicsChanceMax.ToString();
                    }
                    Rect initializeValueAddBionicsChanceMax = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueAddBionicsChanceMax, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.addBionicsChanceFactor));
                        m_Settings.addBionicsChanceFactor = defaultMult;
                        UIUtility.NumericBuffer.addBionicsChanceFactor = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.addBionicsChanceMax));
                        m_Settings.addBionicsChanceMax = defaultMax;
                        UIUtility.NumericBuffer.addBionicsChanceMax = defaultMax.ToString();
                        SettingValues();
                    }, "CR_AddBionicsChanceLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //addBionicsChanceNegativeCurve
                    Rect addBionicsChanceNegativeCurveRow = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(addBionicsChanceNegativeCurveRow);
                    UIUtility.ToolTipRow(addBionicsChanceNegativeCurveRow, "CR_AddBionicsChanceNegativeCurveDesc".Translate());
                    if (UIUtility.LabeledSliderPercentage(addBionicsChanceNegativeCurveRow, UIUtility.LABEL_WIDTH_HALF, UIUtility.INPUT_WIDTH_HALF, "CR_AddBionicsChanceNegativeCurveLabel".Translate(), ref m_Settings.addBionicsChanceNegativeCurve, ref valueChanged, 0f, 1f, () =>
                    {
                        float defaultValue = StaticVariables.GetValue<float>(nameof(m_Settings.addBionicsChanceNegativeCurve));
                        m_Settings.addBionicsChanceNegativeCurve = defaultValue;
                        UIUtility.NumericBuffer.addBionicsChanceNegativeCurve = defaultValue.ToString();
                        SettingValues();
                    }))
                    {
                        UIUtility.NumericBuffer.addBionicsChanceNegativeCurve = m_Settings.addBionicsChanceNegativeCurve.ToString();
                    };
                    marginTop += UIUtility.HEIGHT_ROW;

                    //addBionicsMaxNumber
                    Rect addBionicsMaxNumberRow = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(addBionicsMaxNumberRow);
                    UIUtility.ToolTipRow(addBionicsMaxNumberRow, "CR_AddBionicsMaxNumberDesc".Translate());
                    if (UIUtility.LabeledSliderInt(addBionicsMaxNumberRow, UIUtility.LABEL_WIDTH_HALF, UIUtility.INPUT_WIDTH_HALF, "CR_AddBionicsMaxNumberLabel".Translate(), ref m_Settings.addBionicsMaxNumber, ref valueChanged, 1, 100, () =>
                    {
                        int defaultValue = StaticVariables.GetValue<int>(nameof(m_Settings.addBionicsMaxNumber));
                        m_Settings.addBionicsMaxNumber = defaultValue;
                        UIUtility.NumericBuffer.addBionicsMaxNumber = defaultValue.ToString();
                        SettingValues();
                    }))
                    {
                        UIUtility.NumericBuffer.addBionicsMaxNumber = m_Settings.addBionicsMaxNumber.ToString();
                    };
                    marginTop += UIUtility.HEIGHT_ROW;

                    //allowedByGTTechLevel
                    Rect allowedByGTTechLevelRow = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(allowedByGTTechLevelRow);
                    UIUtility.ToolTipRow(allowedByGTTechLevelRow, "CR_AllowedByGTTechLevelDesc".Translate());
                    if (UIUtility.LabeledSliderInt(allowedByGTTechLevelRow, UIUtility.LABEL_WIDTH_HALF, UIUtility.INPUT_WIDTH_HALF, "CR_AllowedByGTTechLevelLabel".Translate(), ref m_Settings.allowedByGTTechLevel, ref valueChanged, 0x00, UIUtility.TECH_LEVEL_MAX, () =>
                    {
                        int defaultValue = StaticVariables.GetValue<int>(nameof(m_Settings.allowedByGTTechLevel));
                        m_Settings.allowedByGTTechLevel = defaultValue;
                        UIUtility.NumericBuffer.allowedByGTTechLevel = defaultValue.ToString();
                        SettingValues();
                    }, (int val) => {
                        if (val >= 0 && val <= UIUtility.TECH_LEVEL_MAX)
                        {
                            TechLevel techLevel = (TechLevel)Enum.ToObject(typeof(TechLevel), val);
                            if (techLevel == TechLevel.Undefined)
                            {
                                return String.Format("{0} : [{1}]", val, "CR_NoTechLevelLimmitLabel".Translate());
                            }
                            else
                            {
                                return String.Format("{0} : {1} [{2}]", val, String.Format("TechLevel_{0}", techLevel).Translate(), techLevel);
                            }
                        }
                        else
                        {
                            return val.ToString();
                        }
                    }))
                    {
                        UIUtility.NumericBuffer.allowedByGTTechLevel = m_Settings.allowedByGTTechLevel.ToString();
                    };
                    marginTop += UIUtility.HEIGHT_ROW;
                }
            }
            else
            {
                Text.Font = GameFont.Medium;
                Rect deactiveRect = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_CHECKBOX, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                Widgets.Label(deactiveRect, "CR_SectionDisableMessage".Translate());
                Text.Font = GameFont.Small;
            }

        }
        #endregion

        #region EditRefineGearSection
        private int CalcRefineGearSectionRowNum()
        {
            int num = 1;
            if (m_Settings.enableRefineGearOption)
            {
                num += 5;
            }
            else
            {
                num += 1;
            }
            return num;
        }
        private void EditRefineGearSection(Rect inRect, ref bool valueChanged)
        {
            float marginTop = 0f;
            if (!m_Settings.enableRefineGearOption)
            {
                Widgets.DrawBoxSolid(inRect, UIUtility.SUB_SECTION_BACKGROUND_DEACTIVE_COLOR);
            }
            Widgets.DrawBox(inRect);

            Rect titleRect = new Rect(inRect.x, inRect.y, inRect.width - (UIUtility.MARGIN_LEFT), UIUtility.HEIGHT_SECTION_TITLE);
            marginTop += UIUtility.WriteFloatedSectionTitle(titleRect, "CR_OptionSectionTitle".Translate("CR_OptionRefineGearTitle".Translate()), ref m_Settings.enableRefineGearOption, ref valueChanged, () => m_Settings.InitializeValues(SettingValues, CompressedRaidMod.FunctionType.RefineGear)) + UIUtility.MARGIN_BOTTOM;

            float labelWidth = 75f;
            float textWidth = 206.25f;

            if (m_Settings.enableRefineGearOption)
            {
                if (m_Settings.radioTextInput)
                {
                    //ChanceFactor
                    Rect refineGearChanceRow = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(refineGearChanceRow);
                    Rect refineGearChanceFactorLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(refineGearChanceFactorLabel, "CR_RefineGearChanceLabel".Translate());
                    Rect refineGearChanceFactor = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(refineGearChanceFactor);
                    UIUtility.ToolTipRow(refineGearChanceFactor, "CR_RefineGearChanceFactorDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(refineGearChanceFactor, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.refineGearChanceFactor, ref UIUtility.NumericBuffer.refineGearChanceFactor, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);

                    //ChanceMax
                    Rect refineGearChanceMax = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(refineGearChanceMax);
                    UIUtility.ToolTipRow(refineGearChanceMax, "CR_AddBionicsChanceMaxDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(refineGearChanceMax, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.refineGearChanceMax, ref UIUtility.NumericBuffer.refineGearChanceMax, ref valueChanged, 0f, 1f, () => { }, TextAnchor.MiddleRight, true);
                    Rect initializeValueRefineGearChanceMax = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueRefineGearChanceMax, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.refineGearChanceFactor));
                        m_Settings.refineGearChanceFactor = defaultMult;
                        UIUtility.NumericBuffer.refineGearChanceFactor = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.refineGearChanceMax));
                        m_Settings.refineGearChanceMax = defaultMax;
                        UIUtility.NumericBuffer.refineGearChanceMax = defaultMax.ToString();
                        SettingValues();
                    }, "CR_RefineGearChanceLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //refineGearChanceNegativeCurve
                    Rect refineGearChanceNegativeCurveRow = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(refineGearChanceNegativeCurveRow);
                    UIUtility.ToolTipRow(refineGearChanceNegativeCurveRow, "CR_RefineGearChanceNegativeCurveDesc".Translate());
                    UIUtility.LabeledTextBoxPercentage(refineGearChanceNegativeCurveRow, UIUtility.LABEL_WIDTH_HALF, UIUtility.INPUT_WIDTH_HALF, "CR_RefineGearChanceNegativeCurveLabel".Translate(), ref m_Settings.refineGearChanceNegativeCurve, ref UIUtility.NumericBuffer.refineGearChanceNegativeCurve, ref valueChanged, 0f, 1f, () => {
                        float defaultValue = StaticVariables.GetValue<float>(nameof(m_Settings.refineGearChanceNegativeCurve));
                        m_Settings.refineGearChanceNegativeCurve = defaultValue;
                        UIUtility.NumericBuffer.refineGearChanceNegativeCurve = defaultValue.ToString();
                        SettingValues();
                    });
                    marginTop += UIUtility.HEIGHT_ROW;

                    //qualityUpMaxNum
                    Rect qualityUpMaxNumRow = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(qualityUpMaxNumRow);
                    UIUtility.ToolTipRow(qualityUpMaxNumRow, "CR_QualityUpMaxNumDesc".Translate());
                    UIUtility.LabeledTextBoxInt(qualityUpMaxNumRow, UIUtility.LABEL_WIDTH_HALF, UIUtility.INPUT_WIDTH_HALF, "CR_QualityUpMaxNumLabel".Translate(), ref m_Settings.qualityUpMaxNum, ref UIUtility.NumericBuffer.qualityUpMaxNum, ref valueChanged, 0x01, UIUtility.QUALITY_CATEGORY_MAX, () =>
                    {
                        int defaultValue = StaticVariables.GetValue<int>(nameof(m_Settings.qualityUpMaxNum));
                        m_Settings.qualityUpMaxNum = defaultValue;
                        UIUtility.NumericBuffer.qualityUpMaxNum = defaultValue.ToString();
                        SettingValues();
                    });
                    marginTop += UIUtility.HEIGHT_ROW;

                    //optionSetBiocode
                    Rect optionSetBiocode = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(optionSetBiocode);
                    UIUtility.ToolTipRow(optionSetBiocode, "CR_OptionSetBiocodeDesc".Translate());
                    UIUtility.LabeledCheckBoxRight(optionSetBiocode, UIUtility.LABEL_WIDTH_HALF, "CR_OptionSetBiocodeLabel".Translate(), ref m_Settings.optionSetBiocode, ref valueChanged, () => {
                        bool defaultValue = StaticVariables.GetValue<bool>(nameof(m_Settings.optionSetBiocode));
                        m_Settings.optionSetBiocode = defaultValue;
                        SettingValues();
                    });
                    marginTop += UIUtility.HEIGHT_ROW;

                    //optionAddDeathAcidifier
                    Rect optionAddDeathAcidifier = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(optionAddDeathAcidifier);
                    UIUtility.ToolTipRow(optionAddDeathAcidifier, "CR_OptionAddDeathAcidifierDesc".Translate());
                    UIUtility.LabeledCheckBoxRight(optionAddDeathAcidifier, UIUtility.LABEL_WIDTH_HALF, "CR_OptionAddDeathAcidifierLabel".Translate(), ref m_Settings.optionAddDeathAcidifier, ref valueChanged, () => {
                        bool defaultValue = StaticVariables.GetValue<bool>(nameof(m_Settings.optionAddDeathAcidifier));
                        m_Settings.optionAddDeathAcidifier = defaultValue;
                        SettingValues();
                    });
                    marginTop += UIUtility.HEIGHT_ROW;
                }
                else
                {
                    //ChanceFactor
                    Rect refineGearChanceRow = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(refineGearChanceRow);
                    Rect refineGearChanceFactorLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(refineGearChanceFactorLabel, "CR_RefineGearChanceLabel".Translate());
                    Rect refineGearChanceFactor = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(refineGearChanceFactor);
                    UIUtility.ToolTipRow(refineGearChanceFactor, "CR_RefineGearChanceFactorDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(refineGearChanceFactor, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.refineGearChanceFactor, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.refineGearChanceFactor = m_Settings.refineGearChanceFactor.ToString();
                    }

                    //ChanceMax
                    Rect refineGearChanceMax = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(refineGearChanceMax);
                    UIUtility.ToolTipRow(refineGearChanceMax, "CR_AddBionicsChanceMaxDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(refineGearChanceMax, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.refineGearChanceMax, ref valueChanged, 0f, 1f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.refineGearChanceMax = m_Settings.refineGearChanceMax.ToString();
                    }
                    Rect initializeValueRefineGearChanceMax = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueRefineGearChanceMax, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.refineGearChanceFactor));
                        m_Settings.refineGearChanceFactor = defaultMult;
                        UIUtility.NumericBuffer.refineGearChanceFactor = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.refineGearChanceMax));
                        m_Settings.refineGearChanceMax = defaultMax;
                        UIUtility.NumericBuffer.refineGearChanceMax = defaultMax.ToString();
                        SettingValues();
                    }, "CR_RefineGearChanceLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //refineGearChanceNegativeCurve
                    Rect refineGearChanceNegativeCurveRow = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(refineGearChanceNegativeCurveRow);
                    UIUtility.ToolTipRow(refineGearChanceNegativeCurveRow, "CR_RefineGearChanceNegativeCurveDesc".Translate());
                    if (UIUtility.LabeledSliderPercentage(refineGearChanceNegativeCurveRow, UIUtility.LABEL_WIDTH_HALF, UIUtility.INPUT_WIDTH_HALF, "CR_RefineGearChanceNegativeCurveLabel".Translate(), ref m_Settings.refineGearChanceNegativeCurve, ref valueChanged, 0f, 1f, () => {
                        float defaultValue = StaticVariables.GetValue<float>(nameof(m_Settings.refineGearChanceNegativeCurve));
                        m_Settings.refineGearChanceNegativeCurve = defaultValue;
                        UIUtility.NumericBuffer.refineGearChanceNegativeCurve = defaultValue.ToString();
                        SettingValues();
                    }))
                    {
                        UIUtility.NumericBuffer.refineGearChanceNegativeCurve = m_Settings.refineGearChanceNegativeCurve.ToString();
                    }
                    marginTop += UIUtility.HEIGHT_ROW;

                    //qualityUpMaxNum
                    Rect qualityUpMaxNumRow = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(qualityUpMaxNumRow);
                    UIUtility.ToolTipRow(qualityUpMaxNumRow, "CR_QualityUpMaxNumDesc".Translate());
                    if (UIUtility.LabeledSliderInt(qualityUpMaxNumRow, UIUtility.LABEL_WIDTH_HALF, UIUtility.INPUT_WIDTH_HALF, "CR_QualityUpMaxNumLabel".Translate(), ref m_Settings.qualityUpMaxNum, ref valueChanged, 0x01, UIUtility.QUALITY_CATEGORY_MAX, () =>
                    {
                        int defaultValue = StaticVariables.GetValue<int>(nameof(m_Settings.qualityUpMaxNum));
                        m_Settings.qualityUpMaxNum = defaultValue;
                        UIUtility.NumericBuffer.qualityUpMaxNum = defaultValue.ToString();
                        SettingValues();
                    }))
                    {
                        UIUtility.NumericBuffer.qualityUpMaxNum = m_Settings.qualityUpMaxNum.ToString();
                    }
                    marginTop += UIUtility.HEIGHT_ROW;

                    //optionSetBiocode
                    Rect optionSetBiocode = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(optionSetBiocode);
                    UIUtility.ToolTipRow(optionSetBiocode, "CR_OptionSetBiocodeDesc".Translate());
                    UIUtility.LabeledCheckBoxRight(optionSetBiocode, UIUtility.LABEL_WIDTH_HALF, "CR_OptionSetBiocodeLabel".Translate(), ref m_Settings.optionSetBiocode, ref valueChanged, () => {
                        bool defaultValue = StaticVariables.GetValue<bool>(nameof(m_Settings.optionSetBiocode));
                        m_Settings.optionSetBiocode = defaultValue;
                        SettingValues();
                    });
                    marginTop += UIUtility.HEIGHT_ROW;

                    //optionAddDeathAcidifier
                    Rect optionAddDeathAcidifier = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(optionAddDeathAcidifier);
                    UIUtility.ToolTipRow(optionAddDeathAcidifier, "CR_OptionAddDeathAcidifierDesc".Translate());
                    UIUtility.LabeledCheckBoxRight(optionAddDeathAcidifier, UIUtility.LABEL_WIDTH_HALF, "CR_OptionAddDeathAcidifierLabel".Translate(), ref m_Settings.optionAddDeathAcidifier, ref valueChanged, () => {
                        bool defaultValue = StaticVariables.GetValue<bool>(nameof(m_Settings.optionAddDeathAcidifier));
                        m_Settings.optionAddDeathAcidifier = defaultValue;
                        SettingValues();
                    });
                    marginTop += UIUtility.HEIGHT_ROW;
                }
            }
            else
            {
                Text.Font = GameFont.Medium;
                Rect deactiveRect = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_CHECKBOX, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                Widgets.Label(deactiveRect, "CR_SectionDisableMessage".Translate());
                Text.Font = GameFont.Small;
            }

        }
        #endregion

        #region EditAddDrugEffectsSection
        private int CalcAddDrugEffectsSectionRowNum()
        {
            int num = 1;
            if (m_Settings.enableAddDrugOption)
            {
                num += 7;
                if (m_Settings.limitationsByTechLevel)
                {
                    num++;
                }
            }
            else
            {
                num += 1;
            }
            return num;
        }
        private void EditAddDrugEffectsSection(Rect inRect, ref bool valueChanged)
        {
            float marginTop = 0f;
            if (!m_Settings.enableAddDrugOption)
            {
                Widgets.DrawBoxSolid(inRect, UIUtility.SUB_SECTION_BACKGROUND_DEACTIVE_COLOR);
            }
            Widgets.DrawBox(inRect);

            //enableAddDrugOption
            Rect titleRect = new Rect(inRect.x, inRect.y, inRect.width - (UIUtility.MARGIN_LEFT), UIUtility.HEIGHT_SECTION_TITLE);
            marginTop += UIUtility.WriteFloatedSectionTitle(titleRect, "CR_OptionSectionTitle".Translate("CR_OptionAddDrugTitle".Translate()), ref m_Settings.enableAddDrugOption, ref valueChanged, () => m_Settings.InitializeValues(SettingValues, CompressedRaidMod.FunctionType.AddDrugEffects)) + UIUtility.MARGIN_BOTTOM;

            float labelWidth = 75f;
            float textWidth = 206.25f;

            if (m_Settings.enableAddDrugOption)
            {
                if (m_Settings.radioTextInput)
                {

                    //addDrugChanceFactor
                    //ChanceFactor
                    Rect addDrugChanceRow = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(addDrugChanceRow);
                    Rect addDrugChanceFactorLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(addDrugChanceFactorLabel, "CR_AddDrugChanceLabel".Translate());
                    Rect addDrugChanceFactor = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(addDrugChanceFactor);
                    UIUtility.ToolTipRow(addDrugChanceFactor, "CR_AddDrugChanceFactorDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(addDrugChanceFactor, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.addDrugChanceFactor, ref UIUtility.NumericBuffer.addDrugChanceFactor, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true);

                    //addDrugChanceMax
                    //ChanceMax
                    Rect addDrugChanceMax = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(addDrugChanceMax);
                    UIUtility.ToolTipRow(addDrugChanceMax, "CR_AddDrugChanceMaxDesc".Translate(), 0);
                    UIUtility.LabeledTextBoxPercentage(addDrugChanceMax, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.addDrugChanceMax, ref UIUtility.NumericBuffer.addDrugChanceMax, ref valueChanged, 0f, 1f, () => { }, TextAnchor.MiddleRight, true);
                    Rect initializeValueAddDrugChanceMax = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueAddDrugChanceMax, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.addDrugChanceFactor));
                        m_Settings.addDrugChanceFactor = defaultMult;
                        UIUtility.NumericBuffer.addDrugChanceFactor = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.addDrugChanceMax));
                        m_Settings.addDrugChanceMax = defaultMax;
                        UIUtility.NumericBuffer.addDrugChanceMax = defaultMax.ToString();
                        SettingValues();
                    }, "CR_AddDrugChanceLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //addDrugChanceNegativeCurve
                    Rect addDrugChanceNegativeCurveRow = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(addDrugChanceNegativeCurveRow);
                    UIUtility.ToolTipRow(addDrugChanceNegativeCurveRow, "CR_AddDrugChanceNegativeCurveDesc".Translate());
                    UIUtility.LabeledTextBoxPercentage(addDrugChanceNegativeCurveRow, UIUtility.LABEL_WIDTH_HALF, UIUtility.INPUT_WIDTH_HALF, "CR_AddDrugChanceNegativeCurveLabel".Translate(), ref m_Settings.addDrugChanceNegativeCurve, ref UIUtility.NumericBuffer.addDrugChanceNegativeCurve, ref valueChanged, 0f, 1f, () => {
                        float defaultValue = StaticVariables.GetValue<float>(nameof(m_Settings.addDrugChanceNegativeCurve));
                        m_Settings.addDrugChanceNegativeCurve = defaultValue;
                        UIUtility.NumericBuffer.addDrugChanceNegativeCurve = defaultValue.ToString();
                        SettingValues();
                    });
                    marginTop += UIUtility.HEIGHT_ROW;

                    //addDrugMaxNumber
                    Rect addDrugMaxNumberRow = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(addDrugMaxNumberRow);
                    UIUtility.ToolTipRow(addDrugMaxNumberRow, "CR_AddDrugMaxNumberDesc".Translate());
                    UIUtility.LabeledTextBoxInt(addDrugMaxNumberRow, UIUtility.LABEL_WIDTH_HALF, UIUtility.INPUT_WIDTH_HALF, "CR_AddDrugMaxNumberLabel".Translate(), ref m_Settings.addDrugMaxNumber, ref UIUtility.NumericBuffer.addDrugMaxNumber, ref valueChanged, 1, 100, () =>
                    {
                        int defaultValue = StaticVariables.GetValue<int>(nameof(m_Settings.addDrugMaxNumber));
                        m_Settings.addDrugMaxNumber = defaultValue;
                        UIUtility.NumericBuffer.addDrugMaxNumber = defaultValue.ToString();
                        SettingValues();
                    });
                    marginTop += UIUtility.HEIGHT_ROW;

                    //giveOnlyActiveIngredients
                    Rect giveOnlyActiveIngredients = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(giveOnlyActiveIngredients);
                    UIUtility.ToolTipRow(giveOnlyActiveIngredients, "CR_GiveOnlyActiveIngredientsDesc".Translate());
                    UIUtility.LabeledCheckBoxRight(giveOnlyActiveIngredients, UIUtility.LABEL_WIDTH_HALF, "CR_GiveOnlyActiveIngredientsLabel".Translate(), ref m_Settings.giveOnlyActiveIngredients, ref valueChanged, () => {
                        bool defaultValue = StaticVariables.GetValue<bool>(nameof(m_Settings.giveOnlyActiveIngredients));
                        m_Settings.giveOnlyActiveIngredients = defaultValue;
                        SettingValues();
                    });
                    marginTop += UIUtility.HEIGHT_ROW;

                    //ignoreNonVolatilityDrugs
                    Rect ignoreNonVolatilityDrugs = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(ignoreNonVolatilityDrugs);
                    UIUtility.ToolTipRow(ignoreNonVolatilityDrugs, "CR_IgnoreNonVolatilityDrugsDesc".Translate());
                    UIUtility.LabeledCheckBoxRight(ignoreNonVolatilityDrugs, UIUtility.LABEL_WIDTH_HALF, "CR_IgnoreNonVolatilityDrugsLabel".Translate(), ref m_Settings.ignoreNonVolatilityDrugs, ref valueChanged, () => {
                        bool defaultValue = StaticVariables.GetValue<bool>(nameof(m_Settings.ignoreNonVolatilityDrugs));
                        m_Settings.ignoreNonVolatilityDrugs = defaultValue;
                        SettingValues();
                    });
                    marginTop += UIUtility.HEIGHT_ROW;

                    //チェックボックス横に4つ並べる！(社会性ドラッグ、ハードドラッグ、医療ドラッグ、永続ドラッグ)
                    Rect withoutDrugCategories = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(withoutDrugCategories);
                    Rect withoutDrugCategoriesLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(withoutDrugCategoriesLabel, "CR_WithoutDrugLabel".Translate());

                    //withoutSocialDrug
                    Rect withoutSocialDrug = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_BUTTON, inRect.y + marginTop, UIUtility.INPUT_WIDTH_MIN, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(withoutSocialDrug);
                    UIUtility.ToolTipRow(withoutSocialDrug, "CR_WithoutSocialDrugDesc".Translate(), 0);
                    UIUtility.LabeledCheckBoxLeft(withoutSocialDrug, "CR_WithoutSocialDrugLabel".Translate(), ref m_Settings.withoutSocialDrug, ref valueChanged, () => { }, true);

                    //withoutHardDrug
                    Rect withoutHardDrug = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.INPUT_WIDTH_MIN + UIUtility.WIDTH_BUTTON, inRect.y + marginTop, UIUtility.INPUT_WIDTH_MIN, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(withoutHardDrug);
                    UIUtility.ToolTipRow(withoutHardDrug, "CR_WithoutHardDrugDesc".Translate(), 0);
                    UIUtility.LabeledCheckBoxLeft(withoutHardDrug, "CR_WithoutHardDrugLabel".Translate(), ref m_Settings.withoutHardDrug, ref valueChanged, () => { }, true);

                    //withoutMedicalDrug
                    Rect withoutMedicalDrug = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + (UIUtility.INPUT_WIDTH_MIN * 2f) + UIUtility.WIDTH_BUTTON, inRect.y + marginTop, UIUtility.INPUT_WIDTH_MIN, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(withoutMedicalDrug);
                    UIUtility.ToolTipRow(withoutMedicalDrug, "CR_WithoutMedicalDrugDesc".Translate(), 0);
                    UIUtility.LabeledCheckBoxLeft(withoutMedicalDrug, "CR_WithoutMedicalDrugLabel".Translate(), ref m_Settings.withoutMedicalDrug, ref valueChanged, () => { }, true);

                    //withoutNonVolatilityDrug
                    Rect withoutNonVolatilityDrug = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + (UIUtility.INPUT_WIDTH_MIN * 3f) + UIUtility.WIDTH_BUTTON, inRect.y + marginTop, UIUtility.INPUT_WIDTH_MIN, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(withoutNonVolatilityDrug);
                    UIUtility.ToolTipRow(withoutNonVolatilityDrug, "CR_WithoutNonVolatilityDrugDesc".Translate(), 0);
                    UIUtility.LabeledCheckBoxLeft(withoutNonVolatilityDrug, "CR_WithoutNonVolatilityDrugLabel".Translate(), ref m_Settings.withoutNonVolatilityDrug, ref valueChanged, () => { }, true);

                    Rect initializeValueAllowCompressTargetTypes = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueAllowCompressTargetTypes, () => {
                        m_Settings.withoutSocialDrug = StaticVariables.GetValue<bool>(nameof(m_Settings.withoutSocialDrug));
                        m_Settings.withoutHardDrug = StaticVariables.GetValue<bool>(nameof(m_Settings.withoutHardDrug));
                        m_Settings.withoutMedicalDrug = StaticVariables.GetValue<bool>(nameof(m_Settings.withoutMedicalDrug));
                        m_Settings.withoutNonVolatilityDrug = StaticVariables.GetValue<bool>(nameof(m_Settings.withoutNonVolatilityDrug));
                        SettingValues();
                    }, "CR_WithoutDrugLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //limitationsByTechLevel
                    Rect limitationsByTechLevel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(limitationsByTechLevel);
                    UIUtility.ToolTipRow(limitationsByTechLevel, "CR_LimitationsByTechLevelDesc".Translate());
                    UIUtility.LabeledCheckBoxRight(limitationsByTechLevel, UIUtility.LABEL_WIDTH_HALF, "CR_LimitationsByTechLevelLabel".Translate(), ref m_Settings.limitationsByTechLevel, ref valueChanged, () => {
                        bool defaultValue = StaticVariables.GetValue<bool>(nameof(m_Settings.limitationsByTechLevel));
                        m_Settings.limitationsByTechLevel = defaultValue;
                        SettingValues();
                    });
                    marginTop += UIUtility.HEIGHT_ROW;

                    if (m_Settings.limitationsByTechLevel)
                    {
                        //techLevelAllowableRange
                        Rect techLevelAllowableRange = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                        UIUtility.HighlightRect(techLevelAllowableRange);
                        Rect techLevelAllowableRangeLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                        Widgets.Label(techLevelAllowableRangeLabel, "CR_TechLevelAllowableRangeLabel".Translate());
                        //techLevelLowerRange
                        Rect techLevelLowerRange = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                        UIUtility.HighlightRect(techLevelLowerRange);
                        UIUtility.ToolTipRow(techLevelLowerRange, "CR_TechLevelLowerRangeDesc".Translate(), 0);
                        UIUtility.LabeledTextBoxInt(techLevelLowerRange, labelWidth, textWidth, null, ref m_Settings.techLevelLowerRange, ref UIUtility.NumericBuffer.techLevelLowerRange, ref valueChanged, 0x00, UIUtility.TECH_LEVEL_MAX, () => { }, x => "CR_TechLevelLowerRangeLabel".Translate(), TextAnchor.MiddleRight, true);
                        //techLevelUpperRange
                        Rect techLevelUpperRange = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                        UIUtility.HighlightRect(techLevelUpperRange);
                        UIUtility.ToolTipRow(techLevelUpperRange, "CR_TechLevelUpperRangeDesc".Translate(), 0);
                        UIUtility.LabeledTextBoxInt(techLevelUpperRange, labelWidth, textWidth, null, ref m_Settings.techLevelUpperRange, ref UIUtility.NumericBuffer.techLevelUpperRange, ref valueChanged, 0x00, UIUtility.TECH_LEVEL_MAX, () => { }, x => "CR_TechLevelUpperRangeLabel".Translate(), TextAnchor.MiddleRight, true);
                        Rect initializeValueTechLevelAllowableRange = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                        UIUtility.InitializeValueButtonEntry(false, initializeValueTechLevelAllowableRange, () =>
                        {
                            int defaultMult = StaticVariables.GetValue<int>(nameof(m_Settings.techLevelUpperRange));
                            m_Settings.techLevelUpperRange = defaultMult;
                            UIUtility.NumericBuffer.techLevelUpperRange = defaultMult.ToString();
                            int defaultMax = StaticVariables.GetValue<int>(nameof(m_Settings.techLevelLowerRange));
                            m_Settings.techLevelLowerRange = defaultMax;
                            UIUtility.NumericBuffer.techLevelLowerRange = defaultMax.ToString();
                            SettingValues();
                        }, "CR_TechLevelAllowableRangeLabel".Translate());
                        marginTop += UIUtility.HEIGHT_ROW;
                    }
                }
                else
                {
                    //addDrugChanceFactor
                    //ChanceFactor
                    Rect addDrugChanceRow = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(addDrugChanceRow);
                    Rect addDrugChanceFactorLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(addDrugChanceFactorLabel, "CR_AddDrugChanceLabel".Translate());
                    Rect addDrugChanceFactor = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(addDrugChanceFactor);
                    UIUtility.ToolTipRow(addDrugChanceFactor, "CR_AddDrugChanceFactorDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(addDrugChanceFactor, labelWidth, textWidth, "CR_Mult".Translate(), ref m_Settings.addDrugChanceFactor, ref valueChanged, 0f, 10f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.addDrugChanceFactor = m_Settings.addDrugChanceFactor.ToString();
                    }

                    //addDrugChanceMax
                    //ChanceMax
                    Rect addDrugChanceMax = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(addDrugChanceMax);
                    UIUtility.ToolTipRow(addDrugChanceMax, "CR_AddDrugChanceMaxDesc".Translate(), 0);
                    if (UIUtility.LabeledSliderPercentage(addDrugChanceMax, labelWidth, textWidth, "CR_Max".Translate(), ref m_Settings.addDrugChanceMax, ref valueChanged, 0f, 1f, () => { }, TextAnchor.MiddleRight, true))
                    {
                        UIUtility.NumericBuffer.addDrugChanceMax = m_Settings.addDrugChanceMax.ToString();
                    }
                    Rect initializeValueAddDrugChanceMax = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueAddDrugChanceMax, () =>
                    {
                        float defaultMult = StaticVariables.GetValue<float>(nameof(m_Settings.addDrugChanceFactor));
                        m_Settings.addDrugChanceFactor = defaultMult;
                        UIUtility.NumericBuffer.addDrugChanceFactor = defaultMult.ToString();
                        float defaultMax = StaticVariables.GetValue<float>(nameof(m_Settings.addDrugChanceMax));
                        m_Settings.addDrugChanceMax = defaultMax;
                        UIUtility.NumericBuffer.addDrugChanceMax = defaultMax.ToString();
                        SettingValues();
                    }, "CR_AddDrugChanceLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //addDrugChanceNegativeCurve
                    Rect addDrugChanceNegativeCurveRow = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(addDrugChanceNegativeCurveRow);
                    UIUtility.ToolTipRow(addDrugChanceNegativeCurveRow, "CR_AddDrugChanceNegativeCurveDesc".Translate());
                    if (UIUtility.LabeledSliderPercentage(addDrugChanceNegativeCurveRow, UIUtility.LABEL_WIDTH_HALF, UIUtility.INPUT_WIDTH_HALF, "CR_AddDrugChanceNegativeCurveLabel".Translate(), ref m_Settings.addDrugChanceNegativeCurve, ref valueChanged, 0f, 1f, () => {
                        float defaultValue = StaticVariables.GetValue<float>(nameof(m_Settings.addDrugChanceNegativeCurve));
                        m_Settings.addDrugChanceNegativeCurve = defaultValue;
                        UIUtility.NumericBuffer.addDrugChanceNegativeCurve = defaultValue.ToString();
                        SettingValues();
                    }))
                    {
                        UIUtility.NumericBuffer.addDrugChanceNegativeCurve = m_Settings.addDrugChanceNegativeCurve.ToString();
                    }
                    marginTop += UIUtility.HEIGHT_ROW;

                    //addDrugMaxNumber
                    Rect addDrugMaxNumberRow = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(addDrugMaxNumberRow);
                    UIUtility.ToolTipRow(addDrugMaxNumberRow, "CR_AddDrugMaxNumberDesc".Translate());
                    if (UIUtility.LabeledSliderInt(addDrugMaxNumberRow, UIUtility.LABEL_WIDTH_HALF, UIUtility.INPUT_WIDTH_HALF, "CR_AddDrugMaxNumberLabel".Translate(), ref m_Settings.addDrugMaxNumber, ref valueChanged, 1, 100, () =>
                    {
                        int defaultValue = StaticVariables.GetValue<int>(nameof(m_Settings.addDrugMaxNumber));
                        m_Settings.addDrugMaxNumber = defaultValue;
                        UIUtility.NumericBuffer.addDrugMaxNumber = defaultValue.ToString();
                        SettingValues();
                    }))
                    {
                        UIUtility.NumericBuffer.addDrugMaxNumber = m_Settings.addDrugMaxNumber.ToString();
                    }
                    marginTop += UIUtility.HEIGHT_ROW;

                    //giveOnlyActiveIngredients
                    Rect giveOnlyActiveIngredients = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(giveOnlyActiveIngredients);
                    UIUtility.ToolTipRow(giveOnlyActiveIngredients, "CR_GiveOnlyActiveIngredientsDesc".Translate());
                    UIUtility.LabeledCheckBoxRight(giveOnlyActiveIngredients, UIUtility.LABEL_WIDTH_HALF, "CR_GiveOnlyActiveIngredientsLabel".Translate(), ref m_Settings.giveOnlyActiveIngredients, ref valueChanged, () => {
                        bool defaultValue = StaticVariables.GetValue<bool>(nameof(m_Settings.giveOnlyActiveIngredients));
                        m_Settings.giveOnlyActiveIngredients = defaultValue;
                        SettingValues();
                    });
                    marginTop += UIUtility.HEIGHT_ROW;

                    //ignoreNonVolatilityDrugs
                    Rect ignoreNonVolatilityDrugs = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(ignoreNonVolatilityDrugs);
                    UIUtility.ToolTipRow(ignoreNonVolatilityDrugs, "CR_IgnoreNonVolatilityDrugsDesc".Translate());
                    UIUtility.LabeledCheckBoxRight(ignoreNonVolatilityDrugs, UIUtility.LABEL_WIDTH_HALF, "CR_IgnoreNonVolatilityDrugsLabel".Translate(), ref m_Settings.ignoreNonVolatilityDrugs, ref valueChanged, () => {
                        bool defaultValue = StaticVariables.GetValue<bool>(nameof(m_Settings.ignoreNonVolatilityDrugs));
                        m_Settings.ignoreNonVolatilityDrugs = defaultValue;
                        SettingValues();
                    });
                    marginTop += UIUtility.HEIGHT_ROW;

                    //チェックボックス横に4つ並べる！(社会性ドラッグ、ハードドラッグ、医療ドラッグ、永続ドラッグ)
                    Rect withoutDrugCategories = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(withoutDrugCategories);
                    Rect withoutDrugCategoriesLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                    Widgets.Label(withoutDrugCategoriesLabel, "CR_WithoutDrugLabel".Translate());

                    //withoutSocialDrug
                    Rect withoutSocialDrug = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_BUTTON, inRect.y + marginTop, UIUtility.INPUT_WIDTH_MIN, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(withoutSocialDrug);
                    UIUtility.ToolTipRow(withoutSocialDrug, "CR_WithoutSocialDrugDesc".Translate(), 0);
                    UIUtility.LabeledCheckBoxLeft(withoutSocialDrug, "CR_WithoutSocialDrugLabel".Translate(), ref m_Settings.withoutSocialDrug, ref valueChanged, () => { }, true);

                    //withoutHardDrug
                    Rect withoutHardDrug = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.INPUT_WIDTH_MIN + UIUtility.WIDTH_BUTTON, inRect.y + marginTop, UIUtility.INPUT_WIDTH_MIN, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(withoutHardDrug);
                    UIUtility.ToolTipRow(withoutHardDrug, "CR_WithoutHardDrugDesc".Translate(), 0);
                    UIUtility.LabeledCheckBoxLeft(withoutHardDrug, "CR_WithoutHardDrugLabel".Translate(), ref m_Settings.withoutHardDrug, ref valueChanged, () => { }, true);

                    //withoutMedicalDrug
                    Rect withoutMedicalDrug = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + (UIUtility.INPUT_WIDTH_MIN * 2f) + UIUtility.WIDTH_BUTTON, inRect.y + marginTop, UIUtility.INPUT_WIDTH_MIN, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(withoutMedicalDrug);
                    UIUtility.ToolTipRow(withoutMedicalDrug, "CR_WithoutMedicalDrugDesc".Translate(), 0);
                    UIUtility.LabeledCheckBoxLeft(withoutMedicalDrug, "CR_WithoutMedicalDrugLabel".Translate(), ref m_Settings.withoutMedicalDrug, ref valueChanged, () => { }, true);

                    //withoutNonVolatilityDrug
                    Rect withoutNonVolatilityDrug = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + (UIUtility.INPUT_WIDTH_MIN * 3f) + UIUtility.WIDTH_BUTTON, inRect.y + marginTop, UIUtility.INPUT_WIDTH_MIN, UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(withoutNonVolatilityDrug);
                    UIUtility.ToolTipRow(withoutNonVolatilityDrug, "CR_WithoutNonVolatilityDrugDesc".Translate(), 0);
                    UIUtility.LabeledCheckBoxLeft(withoutNonVolatilityDrug, "CR_WithoutNonVolatilityDrugLabel".Translate(), ref m_Settings.withoutNonVolatilityDrug, ref valueChanged, () => { }, true);

                    Rect initializeValueAllowCompressTargetTypes = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                    UIUtility.InitializeValueButtonEntry(false, initializeValueAllowCompressTargetTypes, () => {
                        m_Settings.withoutSocialDrug = StaticVariables.GetValue<bool>(nameof(m_Settings.withoutSocialDrug));
                        m_Settings.withoutHardDrug = StaticVariables.GetValue<bool>(nameof(m_Settings.withoutHardDrug));
                        m_Settings.withoutMedicalDrug = StaticVariables.GetValue<bool>(nameof(m_Settings.withoutMedicalDrug));
                        m_Settings.withoutNonVolatilityDrug = StaticVariables.GetValue<bool>(nameof(m_Settings.withoutNonVolatilityDrug));
                        SettingValues();
                    }, "CR_WithoutDrugLabel".Translate());
                    marginTop += UIUtility.HEIGHT_ROW;

                    //limitationsByTechLevel
                    Rect limitationsByTechLevel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    UIUtility.HighlightRect(limitationsByTechLevel);
                    UIUtility.ToolTipRow(limitationsByTechLevel, "CR_LimitationsByTechLevelDesc".Translate());
                    UIUtility.LabeledCheckBoxRight(limitationsByTechLevel, UIUtility.LABEL_WIDTH_HALF, "CR_LimitationsByTechLevelLabel".Translate(), ref m_Settings.limitationsByTechLevel, ref valueChanged, () => {
                        bool defaultValue = StaticVariables.GetValue<bool>(nameof(m_Settings.limitationsByTechLevel));
                        m_Settings.limitationsByTechLevel = defaultValue;
                        SettingValues();
                    });
                    marginTop += UIUtility.HEIGHT_ROW;

                    if (m_Settings.limitationsByTechLevel)
                    {
                        //techLevelAllowableRange
                        Rect techLevelAllowableRange = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                        UIUtility.HighlightRect(techLevelAllowableRange);
                        Rect techLevelAllowableRangeLabel = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, UIUtility.WIDTH_QUARTER, UIUtility.HEIGHT_ROW);
                        Widgets.Label(techLevelAllowableRangeLabel, "CR_TechLevelAllowableRangeLabel".Translate());
                        //techLevelLowerRange
                        Rect techLevelLowerRange = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                        UIUtility.HighlightRect(techLevelLowerRange);
                        UIUtility.ToolTipRow(techLevelLowerRange, "CR_TechLevelLowerRangeDesc".Translate(), 0);
                        if (UIUtility.LabeledSliderInt(techLevelLowerRange, labelWidth, textWidth, "CR_TechLevelLowerRangeLabel".Translate(), ref m_Settings.techLevelLowerRange, ref valueChanged, 0x00, UIUtility.TECH_LEVEL_MAX, () => { }, TextAnchor.MiddleRight, true))
                        {
                            UIUtility.NumericBuffer.techLevelLowerRange = m_Settings.techLevelLowerRange.ToString();
                        }
                        //techLevelUpperRange
                        Rect techLevelUpperRange = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_QUARTER + UIUtility.WIDTH_THREE_QUARTERS_HALF, inRect.y + marginTop, UIUtility.WIDTH_THREE_QUARTERS_HALF, UIUtility.HEIGHT_ROW);
                        UIUtility.HighlightRect(techLevelUpperRange);
                        UIUtility.ToolTipRow(techLevelUpperRange, "CR_TechLevelUpperRangeDesc".Translate(), 0);
                        if (UIUtility.LabeledSliderInt(techLevelUpperRange, labelWidth, textWidth, "CR_TechLevelUpperRangeLabel".Translate(), ref m_Settings.techLevelUpperRange, ref valueChanged, 0x00, UIUtility.TECH_LEVEL_MAX, () => { }, TextAnchor.MiddleRight, true))
                        {
                            UIUtility.NumericBuffer.techLevelUpperRange = m_Settings.techLevelUpperRange.ToString();
                        }
                        Rect initializeValueTechLevelAllowableRange = new Rect(inRect.x + inRect.width - UIUtility.MARGIN_LEFT - Widgets.CheckboxSize, inRect.y + marginTop, Widgets.CheckboxSize, UIUtility.HEIGHT_ROW);
                        UIUtility.InitializeValueButtonEntry(false, initializeValueTechLevelAllowableRange, () =>
                        {
                            int defaultMult = StaticVariables.GetValue<int>(nameof(m_Settings.techLevelUpperRange));
                            m_Settings.techLevelUpperRange = defaultMult;
                            UIUtility.NumericBuffer.techLevelUpperRange = defaultMult.ToString();
                            int defaultMax = StaticVariables.GetValue<int>(nameof(m_Settings.techLevelLowerRange));
                            m_Settings.techLevelLowerRange = defaultMax;
                            UIUtility.NumericBuffer.techLevelLowerRange = defaultMax.ToString();
                            SettingValues();
                        }, "CR_TechLevelAllowableRangeLabel".Translate());
                        marginTop += UIUtility.HEIGHT_ROW;
                    }


                    ////refineGearChanceNegativeCurve
                    //Rect refineGearChanceNegativeCurveRow = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                    //UIUtility.HighlightRect(refineGearChanceNegativeCurveRow);
                    //UIUtility.ToolTipRow(refineGearChanceNegativeCurveRow, "CR_RefineGearChanceNegativeCurveDesc".Translate());
                    //if (UIUtility.LabeledSliderPercentage(refineGearChanceNegativeCurveRow, UIUtility.LABEL_WIDTH_HALF, UIUtility.INPUT_WIDTH_HALF, "CR_RefineGearChanceNegativeCurveLabel".Translate(), ref m_Settings.refineGearChanceNegativeCurve, ref valueChanged, 0f, 1f, () => {
                    //    float defaultValue = StaticVariables.GetValue<float>(nameof(m_Settings.refineGearChanceNegativeCurve));
                    //    m_Settings.refineGearChanceNegativeCurve = defaultValue;
                    //    UIUtility.NumericBuffer.refineGearChanceNegativeCurve = defaultValue.ToString();
                    //    SettingValues();
                    //}))
                    //{
                    //    UIUtility.NumericBuffer.refineGearChanceNegativeCurve = m_Settings.refineGearChanceNegativeCurve.ToString();
                    //}
                    //marginTop += UIUtility.HEIGHT_ROW;
                }
            }
            else
            {
                Text.Font = GameFont.Medium;
                Rect deactiveRect = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_CHECKBOX, inRect.y + marginTop, inRect.width - (UIUtility.MARGIN_LEFT * 2), UIUtility.HEIGHT_ROW);
                Widgets.Label(deactiveRect, "CR_SectionDisableMessage".Translate());
                Text.Font = GameFont.Small;
            }

        }
        #endregion

        #region SettingValues
        public void SettingValues()
        {
            CompressedRaidMod.chanceOfCompressionValue = m_Settings.chanceOfCompression;
            CompressedRaidMod.maxRaidPawnsCountValue = m_Settings.maxRaidPawnsCount;
            CompressedRaidMod.gainFactorMultValue = m_Settings.gainFactorMult;
            CompressedRaidMod.chanceOfEnhancementValue = m_Settings.chanceOfEnhancement;
            CompressedRaidMod.calcBaseMultByEnhancedValue = m_Settings.calcBaseMultByEnhanced;
            CompressedRaidMod.allowRaidFriendlyValue = m_Settings.allowRaidFriendly;
            CompressedRaidMod.gainFactorMultRaidFriendlyValue = m_Settings.gainFactorMultRaidFriendly;
            CompressedRaidMod.allowMechanoidsValue = m_Settings.allowMechanoids;
            CompressedRaidMod.allowInsectoidsValue = m_Settings.allowInsectoids;
            CompressedRaidMod.allowManhuntersValue = m_Settings.allowManhunters;
            CompressedRaidMod.displayMessageValue = m_Settings.displayMessage;
            CompressedRaidMod.enableMainEnhanceValue = m_Settings.enableMainEnhance;
            CompressedRaidMod.gainFactorMultPainValue = m_Settings.gainFactorMultPain;
            CompressedRaidMod.gainFactorMultArmorRating_BluntValue = m_Settings.gainFactorMultArmorRating_Blunt;
            CompressedRaidMod.gainFactorMultArmorRating_SharpValue = m_Settings.gainFactorMultArmorRating_Sharp;
            CompressedRaidMod.gainFactorMultArmorRating_HeatValue = m_Settings.gainFactorMultArmorRating_Heat;
            CompressedRaidMod.gainFactorMultMeleeDodgeChanceValue = m_Settings.gainFactorMultMeleeDodgeChance;
            CompressedRaidMod.gainFactorMultMeleeHitChanceValue = m_Settings.gainFactorMultMeleeHitChance;
            CompressedRaidMod.gainFactorMultMoveSpeedValue = m_Settings.gainFactorMultMoveSpeed;
            CompressedRaidMod.gainFactorMultShootingAccuracyPawnValue = m_Settings.gainFactorMultShootingAccuracyPawn;
            CompressedRaidMod.gainFactorMultPawnTrapSpringChanceValue = m_Settings.gainFactorMultPawnTrapSpringChance;
            CompressedRaidMod.gainFactorMultCapacitySightValue = m_Settings.gainFactorMultCapacitySight;
            CompressedRaidMod.gainFactorMultCapacityMovingValue = m_Settings.gainFactorMultCapacityMoving;
            CompressedRaidMod.gainFactorMultCapacityHearingValue = m_Settings.gainFactorMultCapacityHearing;
            CompressedRaidMod.gainFactorMultCapacityManipulationValue = m_Settings.gainFactorMultCapacityManipulation;
            CompressedRaidMod.gainFactorMultCapacityMetabolismValue = m_Settings.gainFactorMultCapacityMetabolism;
            CompressedRaidMod.gainFactorMultCapacityConsciousnessValue = m_Settings.gainFactorMultCapacityConsciousness;
            CompressedRaidMod.gainFactorMultCapacityBloodFiltrationValue = m_Settings.gainFactorMultCapacityBloodFiltration;
            CompressedRaidMod.gainFactorMultCapacityBloodPumpingValue = m_Settings.gainFactorMultCapacityBloodPumping;
            CompressedRaidMod.gainFactorMultCapacityBreathingValue = m_Settings.gainFactorMultCapacityBreathing;
            CompressedRaidMod.gainFactorMaxPainValue = m_Settings.gainFactorMaxPain;
            CompressedRaidMod.gainFactorMaxArmorRating_BluntValue = m_Settings.gainFactorMaxArmorRating_Blunt;
            CompressedRaidMod.gainFactorMaxArmorRating_SharpValue = m_Settings.gainFactorMaxArmorRating_Sharp;
            CompressedRaidMod.gainFactorMaxArmorRating_HeatValue = m_Settings.gainFactorMaxArmorRating_Heat;
            CompressedRaidMod.gainFactorMaxMeleeDodgeChanceValue = m_Settings.gainFactorMaxMeleeDodgeChance;
            CompressedRaidMod.gainFactorMaxMeleeHitChanceValue = m_Settings.gainFactorMaxMeleeHitChance;
            CompressedRaidMod.gainFactorMaxMoveSpeedValue = m_Settings.gainFactorMaxMoveSpeed;
            CompressedRaidMod.gainFactorMaxShootingAccuracyPawnValue = m_Settings.gainFactorMaxShootingAccuracyPawn;
            CompressedRaidMod.gainFactorMaxPawnTrapSpringChanceValue = m_Settings.gainFactorMaxPawnTrapSpringChance;
            CompressedRaidMod.gainFactorMaxCapacitySightValue = m_Settings.gainFactorMaxCapacitySight;
            CompressedRaidMod.gainFactorMaxCapacityMovingValue = m_Settings.gainFactorMaxCapacityMoving;
            CompressedRaidMod.gainFactorMaxCapacityHearingValue = m_Settings.gainFactorMaxCapacityHearing;
            CompressedRaidMod.gainFactorMaxCapacityManipulationValue = m_Settings.gainFactorMaxCapacityManipulation;
            CompressedRaidMod.gainFactorMaxCapacityMetabolismValue = m_Settings.gainFactorMaxCapacityMetabolism;
            CompressedRaidMod.gainFactorMaxCapacityConsciousnessValue = m_Settings.gainFactorMaxCapacityConsciousness;
            CompressedRaidMod.gainFactorMaxCapacityBloodFiltrationValue = m_Settings.gainFactorMaxCapacityBloodFiltration;
            CompressedRaidMod.gainFactorMaxCapacityBloodPumpingValue = m_Settings.gainFactorMaxCapacityBloodPumping;
            CompressedRaidMod.gainFactorMaxCapacityBreathingValue = m_Settings.gainFactorMaxCapacityBreathing;
            CompressedRaidMod.enableAddBionicsOptionValue = m_Settings.enableAddBionicsOption;
            CompressedRaidMod.addBionicsChanceFactorValue = m_Settings.addBionicsChanceFactor;
            CompressedRaidMod.addBionicsChanceMaxValue = m_Settings.addBionicsChanceMax;
            CompressedRaidMod.addBionicsChanceNegativeCurveValue = m_Settings.addBionicsChanceNegativeCurve;
            CompressedRaidMod.addBionicsMaxNumberValue = m_Settings.addBionicsMaxNumber;
            CompressedRaidMod.allowedByGTTechLevelValue = m_Settings.allowedByGTTechLevel;
            CompressedRaidMod.enableRefineGearOptionValue = m_Settings.enableRefineGearOption;
            CompressedRaidMod.refineGearChanceFactorValue = m_Settings.refineGearChanceFactor;
            CompressedRaidMod.refineGearChanceMaxValue = m_Settings.refineGearChanceMax;
            CompressedRaidMod.refineGearChanceNegativeCurveValue = m_Settings.refineGearChanceNegativeCurve;
            CompressedRaidMod.qualityUpMaxNumValue = (byte)m_Settings.qualityUpMaxNum;
            CompressedRaidMod.optionSetBiocodeValue = m_Settings.optionSetBiocode;
            CompressedRaidMod.optionAddDeathAcidifierValue = m_Settings.optionAddDeathAcidifier;
            CompressedRaidMod.enableAddDrugOptionValue = m_Settings.enableAddDrugOption;
            CompressedRaidMod.addDrugChanceFactorValue = m_Settings.addDrugChanceFactor;
            CompressedRaidMod.addDrugChanceMaxValue = m_Settings.addDrugChanceMax;
            CompressedRaidMod.addDrugChanceNegativeCurveValue = m_Settings.addDrugChanceNegativeCurve;
            CompressedRaidMod.addDrugMaxNumberValue = m_Settings.addDrugMaxNumber;
            CompressedRaidMod.giveOnlyActiveIngredientsValue = m_Settings.giveOnlyActiveIngredients;
            CompressedRaidMod.ignoreNonVolatilityDrugsValue = m_Settings.ignoreNonVolatilityDrugs;
            CompressedRaidMod.withoutSocialDrugValue = m_Settings.withoutSocialDrug;
            CompressedRaidMod.withoutHardDrugValue = m_Settings.withoutHardDrug;
            CompressedRaidMod.withoutMedicalDrugValue = m_Settings.withoutMedicalDrug;
            CompressedRaidMod.withoutNonVolatilityDrugValue = m_Settings.withoutNonVolatilityDrug;
            CompressedRaidMod.limitationsByTechLevelValue = m_Settings.limitationsByTechLevel;
            CompressedRaidMod.techLevelUpperRangeValue = m_Settings.techLevelUpperRange;
            CompressedRaidMod.techLevelLowerRangeValue = m_Settings.techLevelLowerRange;
        }
        #endregion

        private CompressedRaidModSettings m_Settings;

        #region SettingHandledValues
        public static float chanceOfCompressionValue = StaticVariables.CHANCE_OF_COMPRESSION_DEFAULT;
        public static bool allowMechanoidsValue = StaticVariables.ALLOW_MECHANOIDS_DEFAULT;
        public static bool allowInsectoidsValue = StaticVariables.ALLOW_INSECTOIDS_DEFAULT;
        public static int maxRaidPawnsCountValue = StaticVariables.MAX_RAID_PAWNS_COUNT_DEFAULT;
        public static float gainFactorMultValue = StaticVariables.GAIN_FACTOR_MULT_DEFAULT;
        public static float chanceOfEnhancementValue = StaticVariables.CHANCE_OF_ENHANCEMENT_DEFAULT;
        public static bool calcBaseMultByEnhancedValue = StaticVariables.CALC_BASE_MULT_BY_ENHANCED_DEFAULT;
        public static bool allowRaidFriendlyValue = StaticVariables.ALLOW_RAID_FRIENDLY_DEFAULT;
        public static bool allowManhuntersValue = StaticVariables.ALLOW_MANHUNTERS_DEFAULT;
        public static float gainFactorMultRaidFriendlyValue = StaticVariables.GAIN_FACTOR_MULT_RAID_FRIENDLY_DEFAULT;
        public static bool displayMessageValue = StaticVariables.DISPLAY_MESSAGE_DEFAULT;
        public static bool enableMainEnhanceValue = StaticVariables.ENABLE_MAIN_ENHANCE_DEFAULT;
        public static float gainFactorMultPainValue = StaticVariables.GAIN_FACTOR_MULT_PAIN_DEFAULT;
        public static float gainFactorMultArmorRating_BluntValue = StaticVariables.GAIN_FACTOR_MULT_ARMOR_RATING_BLUNT_DEFAULT;
        public static float gainFactorMultArmorRating_SharpValue = StaticVariables.GAIN_FACTOR_MULT_ARMOR_RATING_SHARP_DEFAULT;
        public static float gainFactorMultArmorRating_HeatValue = StaticVariables.GAIN_FACTOR_MULT_ARMOR_RATING_HEAT_DEFAULT;
        public static float gainFactorMultMeleeDodgeChanceValue = StaticVariables.GAIN_FACTOR_MULT_MELEE_DODGE_CHANCE_DEFAULT;
        public static float gainFactorMultMeleeHitChanceValue = StaticVariables.GAIN_FACTOR_MULT_MELEE_HIT_CHANCE_DEFAULT;
        public static float gainFactorMultMoveSpeedValue = StaticVariables.GAIN_FACTOR_MULT_MOVE_SPEED_DEFAULT;
        public static float gainFactorMultShootingAccuracyPawnValue = StaticVariables.GAIN_FACTOR_MULT_SHOOTING_ACCURACY_PAWN_DEFAULT;
        public static float gainFactorMultPawnTrapSpringChanceValue = StaticVariables.GAIN_FACTOR_MULT_PAWN_TRAP_SPRING_CHANCE_DEFAULT;
        public static float gainFactorMaxPainValue = StaticVariables.GAIN_FACTOR_MAX_PAIN_DEFAULT;
        public static float gainFactorMaxArmorRating_BluntValue = StaticVariables.GAIN_FACTOR_MAX_ARMOR_RATING_BLUNT_DEFAULT;
        public static float gainFactorMaxArmorRating_SharpValue = StaticVariables.GAIN_FACTOR_MAX_ARMOR_RATING_SHARP_DEFAULT;
        public static float gainFactorMaxArmorRating_HeatValue = StaticVariables.GAIN_FACTOR_MAX_ARMOR_RATING_HEAT_DEFAULT;
        public static float gainFactorMaxMeleeDodgeChanceValue = StaticVariables.GAIN_FACTOR_MAX_MELEE_DODGE_CHANCE_DEFAULT;
        public static float gainFactorMaxMeleeHitChanceValue = StaticVariables.GAIN_FACTOR_MAX_MELEE_HIT_CHANCE_DEFAULT;
        public static float gainFactorMaxMoveSpeedValue = StaticVariables.GAIN_FACTOR_MAX_MOVE_SPEED_DEFAULT;
        public static float gainFactorMaxShootingAccuracyPawnValue = StaticVariables.GAIN_FACTOR_MAX_SHOOTING_ACCURACY_PAWN_DEFAULT;
        public static float gainFactorMaxPawnTrapSpringChanceValue = StaticVariables.GAIN_FACTOR_MAX_PAWN_TRAP_SPRING_CHANCE_DEFAULT;
        public static float gainFactorMultCapacitySightValue = StaticVariables.GAIN_FACTOR_MULT_CAPACITY_SIGHT_DEFAULT;
        public static float gainFactorMultCapacityMovingValue = StaticVariables.GAIN_FACTOR_MULT_CAPACITY_MOVING_DEFAULT;
        public static float gainFactorMultCapacityHearingValue = StaticVariables.GAIN_FACTOR_MULT_CAPACITY_HEARING_DEFAULT;
        public static float gainFactorMultCapacityManipulationValue = StaticVariables.GAIN_FACTOR_MULT_CAPACITY_MANIPULATION_DEFAULT;
        public static float gainFactorMultCapacityMetabolismValue = StaticVariables.GAIN_FACTOR_MULT_CAPACITY_METABOLISM_DEFAULT;
        public static float gainFactorMultCapacityConsciousnessValue = StaticVariables.GAIN_FACTOR_MULT_CAPACITY_CONSCIOUSNESS_DEFAULT;
        public static float gainFactorMultCapacityBloodFiltrationValue = StaticVariables.GAIN_FACTOR_MULT_CAPACITY_BLOOD_FILTRATION_DEFAULT;
        public static float gainFactorMultCapacityBloodPumpingValue = StaticVariables.GAIN_FACTOR_MULT_CAPACITY_BLOOD_PUMPING_DEFAULT;
        public static float gainFactorMultCapacityBreathingValue = StaticVariables.GAIN_FACTOR_MULT_CAPACITY_BREATHING_DEFAULT;
        public static float gainFactorMaxCapacitySightValue = StaticVariables.GAIN_FACTOR_MAX_CAPACITY_SIGHT_DEFAULT;
        public static float gainFactorMaxCapacityMovingValue = StaticVariables.GAIN_FACTOR_MAX_CAPACITY_MOVING_DEFAULT;
        public static float gainFactorMaxCapacityHearingValue = StaticVariables.GAIN_FACTOR_MAX_CAPACITY_HEARING_DEFAULT;
        public static float gainFactorMaxCapacityManipulationValue = StaticVariables.GAIN_FACTOR_MAX_CAPACITY_MANIPULATION_DEFAULT;
        public static float gainFactorMaxCapacityMetabolismValue = StaticVariables.GAIN_FACTOR_MAX_CAPACITY_METABOLISM_DEFAULT;
        public static float gainFactorMaxCapacityConsciousnessValue = StaticVariables.GAIN_FACTOR_MAX_CAPACITY_CONSCIOUSNESS_DEFAULT;
        public static float gainFactorMaxCapacityBloodFiltrationValue = StaticVariables.GAIN_FACTOR_MAX_CAPACITY_BLOOD_FILTRATION_DEFAULT;
        public static float gainFactorMaxCapacityBloodPumpingValue = StaticVariables.GAIN_FACTOR_MAX_CAPACITY_BLOOD_PUMPING_DEFAULT;
        public static float gainFactorMaxCapacityBreathingValue = StaticVariables.GAIN_FACTOR_MAX_CAPACITY_BREATHING_DEFAULT;
        public static bool enableAddBionicsOptionValue = StaticVariables.ENABLE_ADD_BIONICS_OPTION_DEFAULT;
        public static float addBionicsChanceFactorValue = StaticVariables.ADD_BIONICS_CHANCE_FACTOR_DEFAULT;
        public static float addBionicsChanceMaxValue = StaticVariables.ADD_BIONICS_CHANCE_MAX_DEFAULT;
        public static float addBionicsChanceNegativeCurveValue = StaticVariables.ADD_BIONICS_CHANCE_NEGATIVE_CURVE_DEFAULT;
        public static int addBionicsMaxNumberValue = StaticVariables.ADD_BIONICS_MAX_NUMBER_DEFAULT;
        public static int allowedByGTTechLevelValue = StaticVariables.ALLOWED_BY_G_T_TECH_LEVEL_DEFAULT;
        public static bool enableRefineGearOptionValue = StaticVariables.ENABLE_REFINE_GEAR_OPTION_DEFAULT;
        public static float refineGearChanceFactorValue = StaticVariables.REFINE_GEAR_CHANCE_FACTOR_DEFAULT;
        public static float refineGearChanceMaxValue = StaticVariables.REFINE_GEAR_CHANCE_MAX_DEFAULT;
        public static float refineGearChanceNegativeCurveValue = StaticVariables.REFINE_GEAR_CHANCE_NEGATIVE_CURVE_DEFAULT;
        public static byte qualityUpMaxNumValue = StaticVariables.QUALITY_UP_MAX_NUM_DEFAULT;
        public static bool optionSetBiocodeValue = StaticVariables.OPTION_SET_BIOCODE_DEFAULT;
        public static bool optionAddDeathAcidifierValue = StaticVariables.OPTION_ADD_DEATH_ACIDIFIER_DEFAULT;
        public static bool enableAddDrugOptionValue = StaticVariables.ENABLE_ADD_DRUG_OPTION_DEFAULT;
        public static float addDrugChanceFactorValue = StaticVariables.ADD_DRUG_CHANCE_FACTOR_DEFAULT;
        public static float addDrugChanceMaxValue = StaticVariables.ADD_DRUG_CHANCE_MAX_DEFAULT;
        public static float addDrugChanceNegativeCurveValue = StaticVariables.ADD_DRUG_CHANCE_NEGATIVE_CURVE_DEFAULT;
        public static int addDrugMaxNumberValue = StaticVariables.ADD_DRUG_MAX_NUMBER_DEFAULT;
        public static bool giveOnlyActiveIngredientsValue = StaticVariables.GIVE_ONLY_ACTIVE_INGREDIENTS_DEFAULT;
        public static bool ignoreNonVolatilityDrugsValue = StaticVariables.IGNORE_NON_VOLATILITY_DRUGS_DEFAULT;
        public static bool withoutSocialDrugValue = StaticVariables.WITHOUT_SOCIAL_DRUG_DEFAULT;
        public static bool withoutHardDrugValue = StaticVariables.WITHOUT_HARD_DRUG_DEFAULT;
        public static bool withoutMedicalDrugValue = StaticVariables.WITHOUT_MEDICAL_DRUG_DEFAULT;
        public static bool withoutNonVolatilityDrugValue = StaticVariables.WITHOUT_NON_VOLATILITY_DRUG_DEFAULT;
        public static bool limitationsByTechLevelValue = StaticVariables.LIMITATIONS_BY_TECH_LEVEL_DEFAULT;
        public static int techLevelUpperRangeValue = StaticVariables.TECH_LEVEL_UPPER_RANGE_DEFAULT;
        public static int techLevelLowerRangeValue = StaticVariables.TECH_LEVEL_LOWER_RANGE_DEFAULT;
        #endregion

        #region methods
        public static bool ModDisabled()
        {
            bool chance = true;
            if (chanceOfCompressionValue <= 0f)
            {
                return true;
            }
            else if (chanceOfCompressionValue < 1f)
            {
                chance = Rand.Chance(chanceOfCompressionValue);
            }
            return !chance || maxRaidPawnsCountValue <= 0;
        }
        public static bool EnhanceDisabled()
        {
            if (!enableMainEnhanceValue &&
                (enableAddBionicsOptionValue && (addBionicsChanceFactorValue <= 0f || addBionicsChanceMaxValue <= 0f)) ||
                (enableRefineGearOptionValue && (refineGearChanceFactorValue <= 0f || refineGearChanceMaxValue <= 0f)) ||
                (enableAddDrugOptionValue && (addDrugChanceFactorValue <= 0f || addDrugChanceMaxValue <= 0f)))
            {
                return true;
            }
            return gainFactorMultValue <= 0f;
        }
        public static bool CompressedEnabled()
        {
            return enableMainEnhanceValue && maxRaidPawnsCountValue > 0 && gainFactorMultValue > 0f;
        }
        public static bool OptionEnabled()
        {
            return enableAddBionicsOptionValue || enableRefineGearOptionValue || enableAddDrugOptionValue;
        }
        public static bool AllowCompress(Pawn pawn)
        {
            if (!allowMechanoidsValue)
            {
                bool isMechanoid = pawn?.RaceProps?.IsMechanoid ?? false;
                if (isMechanoid)
                {
                    return false;
                }
            }
            if (!allowInsectoidsValue)
            {
                bool isInsectoid = pawn?.RaceProps?.FleshType == FleshTypeDefOf.Insectoid;
                if (isInsectoid)
                {
                    return false;
                }
            }
            return true;
        }
        public static bool AllowCompress(IncidentParms parms, out bool raidFriendly)
        {
            raidFriendly = false;
            bool hostileRaid = FactionUtility.HostileTo(Faction.OfPlayer, parms.faction);
            if (hostileRaid)
            {
                return true;
            }
            else
            {
                if (!CompressedRaidMod.allowRaidFriendlyValue)
                {
                    return false;
                }
                bool isRaidFriendly = parms.traderKind == null && parms.generateFightersOnly;
                if (isRaidFriendly)
                {
                    raidFriendly = true;
                    return true;
                }
            }
            return false;
        }
        public static bool AllowCompress(PawnGroupMakerParms parms, out bool raidFriendly)
        {
            raidFriendly = false;
            bool hostileRaid = FactionUtility.HostileTo(Faction.OfPlayer, parms.faction);
            if (hostileRaid)
            {
                return true;
            }
            else
            {
                if (!CompressedRaidMod.allowRaidFriendlyValue)
                {
                    return false;
                }
                bool isRaidFriendly = parms.groupKind == PawnGroupKindDefOf.Combat;
                if (isRaidFriendly)
                {
                    raidFriendly = true;
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
