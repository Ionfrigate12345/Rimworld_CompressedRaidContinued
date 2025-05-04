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
    public class Dialog_LoadOldConfigConfirm : Window
    {

        public Action<bool> m_LoadOldConfigAction;
        public Action m_PostAction;
        private bool m_RadioCleanLoad = false;
        private bool m_RadioMergeLoad = false;

        protected override void SetInitialSizeAndPosition()
        {
            base.SetInitialSizeAndPosition();
            base.windowRect.height = 400f;
        }

        public Dialog_LoadOldConfigConfirm()
        {
            base.closeOnClickedOutside = true;
            base.doCloseButton = true;
            base.resizeable = false;
            this.absorbInputAroundWindow = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            TextAnchor textAnchorBk = Text.Anchor;
            bool textWordWrapBk = Text.WordWrap;

            Text.Anchor = TextAnchor.MiddleCenter;
            float marginTop = UIUtility.MARGIN_TOP;
            Rect cleanLoadRect = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - UIUtility.MARGIN_LEFT, UIUtility.HEIGHT_ROW);
            bool radioCleanLoadBk = m_RadioCleanLoad;
            if (Widgets.RadioButtonLabeled(cleanLoadRect, "CR_CleanLoadLabel".Translate(), m_RadioCleanLoad))
            {
                if (!radioCleanLoadBk)
                {
                    m_RadioCleanLoad = true;
                    m_RadioMergeLoad = false;
                }
            }
            marginTop += UIUtility.HEIGHT_ROW;

            Text.Anchor = TextAnchor.UpperLeft;
            Text.WordWrap = true;

            Rect cleanLoadDescRect = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_BUTTON, inRect.y + marginTop, inRect.width - UIUtility.MARGIN_LEFT - (UIUtility.WIDTH_BUTTON * 2), UIUtility.HEIGHT_ROW * 2);
            Widgets.Label(cleanLoadDescRect, "CR_CleanLoadDesc".Translate());
            marginTop += (UIUtility.HEIGHT_ROW * 2) + UIUtility.MARGIN_TOP;

            Text.Anchor = textAnchorBk;
            Text.WordWrap = textWordWrapBk;

            Rect highLightCleanRect = new Rect(cleanLoadRect.x, cleanLoadRect.y, cleanLoadRect.width, cleanLoadRect.height + cleanLoadDescRect.height);
            UIUtility.HighlightRect(highLightCleanRect, cleanLoadRect);

            Text.Anchor = TextAnchor.MiddleCenter;
            Rect mergeLoadRect = new Rect(inRect.x + UIUtility.MARGIN_LEFT, inRect.y + marginTop, inRect.width - UIUtility.MARGIN_LEFT, UIUtility.HEIGHT_ROW);
            bool radioMergeLoadBk = m_RadioMergeLoad;
            if (Widgets.RadioButtonLabeled(mergeLoadRect, "CR_MergeLoadLabel".Translate(), m_RadioMergeLoad))
            {
                if (!radioMergeLoadBk)
                {
                    m_RadioMergeLoad = true;
                    m_RadioCleanLoad = false;
                }
            }
            marginTop += UIUtility.HEIGHT_ROW;

            Text.Anchor = TextAnchor.UpperLeft;
            Text.WordWrap = true;

            Rect mergeLoadDescRect = new Rect(inRect.x + UIUtility.MARGIN_LEFT + UIUtility.WIDTH_BUTTON, inRect.y + marginTop, inRect.width - UIUtility.MARGIN_LEFT - (UIUtility.WIDTH_BUTTON * 2), UIUtility.HEIGHT_ROW * 2);
            Widgets.Label(mergeLoadDescRect, "CR_MergeLoadDesc".Translate());
            marginTop += (UIUtility.HEIGHT_ROW * 3) + UIUtility.MARGIN_TOP;

            Text.Anchor = textAnchorBk;
            Text.WordWrap = textWordWrapBk;

            Rect highLightMergeRect = new Rect(mergeLoadRect.x, mergeLoadRect.y, mergeLoadRect.width, mergeLoadRect.height + mergeLoadDescRect.height);
            UIUtility.HighlightRect(highLightMergeRect, mergeLoadRect);

            if (m_RadioCleanLoad || m_RadioMergeLoad)
            {
                Rect executeButtonRect = new Rect((inRect.width - Widgets.BackButtonWidth) / 2, inRect.y + marginTop, Widgets.BackButtonWidth, Widgets.BackButtonHeight);
                if (Widgets.ButtonText(executeButtonRect, "CR_ButtonExecute".Translate()))
                {
                    // TODO HugsLibからの初期化処理
                    if (m_LoadOldConfigAction != null)
                    {
                        m_LoadOldConfigAction(m_RadioMergeLoad);
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
