using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace CompressedRaid
{
    public class Dialog_ResetAllConfirm : Window
    {
        public Action m_ResetAllAction;
        public Action m_PostAction;
        private bool m_Agree = false;

        protected override void SetInitialSizeAndPosition()
        {
            base.SetInitialSizeAndPosition();
            base.windowRect.height = 380f;
        }

        public Dialog_ResetAllConfirm()
        {
            base.closeOnClickedOutside = true;
            base.doCloseButton = true;
            base.resizeable = false;
            this.absorbInputAroundWindow = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            TextAnchor textAnchorBk = Text.Anchor;
            GameFont fontBk = Text.Font;


            Text.Anchor = TextAnchor.MiddleCenter;
            float marginTop = UIUtility.MARGIN_TOP;
            Rect labelRect = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - UIUtility.MARGIN_LEFT, UIUtility.HEIGHT_ROW);
            Widgets.Label(labelRect, "CR_InitializeValueAllReally".Translate());
            marginTop += (UIUtility.HEIGHT_ROW * 3f) + UIUtility.MARGIN_TOP;

            if (m_Agree)
            {
                Text.Font = GameFont.Medium;
                Rect labelRect2 = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - UIUtility.MARGIN_LEFT, UIUtility.HEIGHT_ROW);
                Widgets.Label(labelRect2, "CR_InitializeValueAllConfirm".Translate());
                Text.Font = fontBk;
            }
            else
            {
                Rect buttonRec = new Rect((inRect.width - Widgets.BackButtonWidth) / 2, inRect.y + marginTop, Widgets.BackButtonWidth, Widgets.BackButtonHeight);
                if (Widgets.ButtonText(buttonRec, "CR_InitializeValueAllConfirmAgree".Translate()))
                {
                    SoundDefOf.Click.PlayOneShotOnCamera(null);
                    m_Agree = true;
                }
            }
            marginTop += (UIUtility.HEIGHT_ROW * 3f) + UIUtility.MARGIN_TOP;

            Text.Anchor = textAnchorBk;

            if (m_Agree)
            {
                Rect executeButtonRect = new Rect((inRect.width - Widgets.BackButtonWidth) / 2, inRect.y + marginTop, Widgets.BackButtonWidth, Widgets.BackButtonHeight);
                if (Widgets.ButtonText(executeButtonRect, "CR_ButtonExecute".Translate()))
                {
                    // TODO HugsLibからの初期化処理
                    if (m_ResetAllAction != null)
                    {
                        m_ResetAllAction();
                    }
                    if (m_PostAction != null)
                    {
                        m_PostAction();
                    }
                    this.Close();
                }
            }
        }
    }
}
