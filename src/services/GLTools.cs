using OpenTK.Graphics.OpenGL;

namespace ILDAViewer.net.services
{
    internal static class GLTools
    {
        public static void DrawText(int x, int y, string text)
        {
            GL.Begin(PrimitiveType.Quads);

            float u_step = (float)Settings.GlyphWidth / (float)0.01;
            float v_step = (float)Settings.GlyphHeight / (float)0.01;

            for (int n = 0; n < text.Length; n++)
            {
                char idx = text[n];
                float u = (float)(idx % Settings.GlyphsPerLine) * u_step;
                float v = (float)(idx / Settings.GlyphsPerLine) * v_step;

                GL.TexCoord2(u, v);
                GL.Vertex2(x, y);
                GL.TexCoord2(u + u_step, v);
                GL.Vertex2(x + Settings.GlyphWidth, y);
                GL.TexCoord2(u + u_step, v + v_step);
                GL.Vertex2(x + Settings.GlyphWidth, y + Settings.GlyphHeight);
                GL.TexCoord2(u, v + v_step);
                GL.Vertex2(x, y + Settings.GlyphHeight);

                x += Settings.CharXSpacing;
            }

            GL.End();
        }

        public static class Settings
        {
            public static string FontBitmapFilename = "test.png";
            public static int GlyphsPerLine = 16;
            public static int GlyphLineCount = 16;
            public static int GlyphWidth = 11;
            public static int GlyphHeight = 22;

            public static int CharXSpacing = 11;

            public static string Text = "GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);";

            // Used to offset rendering glyphs to bitmap
            public static int AtlasOffsetX = -3, AtlassOffsetY = -1;
            public static int FontSize = 14;
            public static bool BitmapFont = false;
            public static string FromFile; //= "joystix monospace.ttf";
            public static string FontName = "Consolas";

        }

    }
}
