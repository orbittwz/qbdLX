using System;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace QobuzDownloaderX.Shared
{
    public static class FontManager
    {
        private static readonly PrivateFontCollection FontCollection = new PrivateFontCollection();

        static FontManager()
        {
            // Add HKGrotesk font to PrivateFontCollection
            AddFont(Properties.Resources.HKGrotesk);
        }

        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);

        private static void AddFont(byte[] fontData)
        {
            IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
            Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            // We HAVE to do this to register the font to the system (Weird .NET bug !)
            uint cFonts = 0;
            AddFontMemResourceEx(fontPtr, (uint)fontData.Length, IntPtr.Zero, ref cFonts);
            FontCollection.AddMemoryFont(fontPtr, fontData.Length);
            Marshal.FreeCoTaskMem(fontPtr);
        }

        private static FontFamily GetFontFamily(string fontFamilyName)
        {
            return Array.Find(FontCollection.Families, f => f.Name == fontFamilyName);
        }

        public static Font CreateFont(string fontFamilyName, float size, FontStyle style = FontStyle.Regular)
        {
            var fontFamily = GetFontFamily(fontFamilyName);
            return fontFamily != null ? new Font(fontFamily, size, style) : null;
        }
    }
}