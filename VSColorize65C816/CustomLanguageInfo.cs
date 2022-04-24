using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Drawing;
using System.Runtime.InteropServices;


namespace VSColorize65C816
{
    [Guid("4DDB60E0-86FB-4CC1-B854-860401682F3E")]
    internal class CustomLanguageInfo : IVsLanguageInfo, IVsProvideColorableItems
    {
        public const string LanguageName = "64tass W65C816";
        private readonly ColorableItem[] _colorableItems;

        public CustomLanguageInfo()
        {
            // If we don't explicitly specify colorable items, VS will pick some default that doesn't include the dark grey like in C++ include.
            // This list is coupled to the Colors enum.

            _colorableItems = new[]
            {

                new ColorableItem("Text", "Text", COLORINDEX.CI_USERTEXT_FG, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                new ColorableItem("Keyword", "Keyword", COLORINDEX.CI_BLUE, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                new ColorableItem("Comment", "Comment", COLORINDEX.CI_DARKGREEN, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                new ColorableItem("Preprocessor", "Preprocessor", COLORINDEX.CI_DARKGRAY, COLORINDEX.CI_USERTEXT_BK, Color.Empty, Color.Empty, FONTFLAGS.FF_DEFAULT),
                new ColorableItem("String", "String", COLORINDEX.CI_RED, COLORINDEX.CI_USERTEXT_BK, Color.DarkRed, Color.Empty, FONTFLAGS.FF_DEFAULT),
            };
        }

        int IVsLanguageInfo.GetLanguageName(out string bstrName)
        {
            bstrName = LanguageName;
            return VSConstants.S_OK;
        }

        int IVsLanguageInfo.GetFileExtensions(out string pbstrExtensions)
        {
            pbstrExtensions = ".s;.asm"; // File extensions, including periods, separated by semi-colons
            return VSConstants.S_OK;
        }

        int IVsLanguageInfo.GetColorizer(IVsTextLines pBuffer, out IVsColorizer ppColorizer)
        {
            if (pBuffer == null)
            {
                ppColorizer = null;
                return VSConstants.E_INVALIDARG;
            }

            ppColorizer = new CustomColorizer(pBuffer);
            return VSConstants.S_OK;
        }

        int IVsLanguageInfo.GetCodeWindowManager(IVsCodeWindow pCodeWin, out IVsCodeWindowManager ppCodeWinMgr)
        {
            ppCodeWinMgr = null;
            return VSConstants.E_NOTIMPL;
        }

        int IVsProvideColorableItems.GetItemCount(out int count)
        {
            count = _colorableItems.Length;
            return VSConstants.S_OK;
        }

        int IVsProvideColorableItems.GetColorableItem(int index, out IVsColorableItem item)
        {
            if (index < 1 || index > _colorableItems.Length)
            {
                item = null;
                return VSConstants.E_INVALIDARG;
            }

            item = _colorableItems[index - 1];
            return VSConstants.S_OK;
        }
    }

}
