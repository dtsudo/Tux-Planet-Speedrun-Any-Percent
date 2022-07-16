
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using System;

	public enum GameFont
	{
		DTSimpleFont12Pt,
		DTSimpleFont14Pt,
		DTSimpleFont16Pt,
		DTSimpleFont18Pt,
		DTSimpleFont20Pt,
		DTSimpleFont32Pt
	}

	public static class GameFontUtil
	{
		public class FontInfo
		{
			public FontInfo(
				string ttfFontFilename,
				string woffFontFilename,
				int fontSize,
				string javascriptFontSize,
				string lineHeight,
				string monoGameSpriteFontName)
			{
				this.TtfFontFilename = ttfFontFilename;
				this.WoffFontFilename = woffFontFilename;
				this.FontSize = fontSize;
				this.JavascriptFontSize = javascriptFontSize;
				this.LineHeight = lineHeight;
				this.MonoGameSpriteFontName = monoGameSpriteFontName;
			}

			public string TtfFontFilename { get; private set; }
			public string WoffFontFilename { get; private set; }
			public int FontSize { get; private set; }
			public string JavascriptFontSize { get; private set; }
			public string LineHeight { get; private set; }
			public string MonoGameSpriteFontName { get; private set; }
		}

		public static FontInfo GetFontInfo(this GameFont font)
		{
			switch (font)
			{
				case GameFont.DTSimpleFont12Pt:
					return new FontInfo(
						ttfFontFilename: "Metaflop/dtsimplefont.ttf",
						woffFontFilename: "Metaflop/dtsimplefont.woff",
						fontSize: 12,
						javascriptFontSize: "15.86",
						lineHeight: "15.5",
						monoGameSpriteFontName: "dtsimplefont12");
				case GameFont.DTSimpleFont14Pt:
					return new FontInfo(
						ttfFontFilename: "Metaflop/dtsimplefont.ttf",
						woffFontFilename: "Metaflop/dtsimplefont.woff",
						fontSize: 14,
						javascriptFontSize: "19.31",
						lineHeight: "18.5",
						monoGameSpriteFontName: "dtsimplefont14");
				case GameFont.DTSimpleFont16Pt:
					return new FontInfo(
						ttfFontFilename: "Metaflop/dtsimplefont.ttf",
						woffFontFilename: "Metaflop/dtsimplefont.woff",
						fontSize: 16,
						javascriptFontSize: "21.85",
						lineHeight: "23",
						monoGameSpriteFontName: "dtsimplefont16");
				case GameFont.DTSimpleFont18Pt:
					return new FontInfo(
						ttfFontFilename: "Metaflop/dtsimplefont.ttf",
						woffFontFilename: "Metaflop/dtsimplefont.woff",
						fontSize: 18,
						javascriptFontSize: "24.19",
						lineHeight: "24",
						monoGameSpriteFontName: "dtsimplefont18");
				case GameFont.DTSimpleFont20Pt:
					return new FontInfo(
						ttfFontFilename: "Metaflop/dtsimplefont.ttf",
						woffFontFilename: "Metaflop/dtsimplefont.woff",
						fontSize: 20,
						javascriptFontSize: "26.76",
						lineHeight: "28.2",
						monoGameSpriteFontName: "dtsimplefont20");
				case GameFont.DTSimpleFont32Pt:
					return new FontInfo(
						ttfFontFilename: "Metaflop/dtsimplefont.ttf",
						woffFontFilename: "Metaflop/dtsimplefont.woff",
						fontSize: 32,
						javascriptFontSize: "42.95",
						lineHeight: "44",
						monoGameSpriteFontName: "dtsimplefont32");
				default: throw new Exception();
			}
		}
	}
}
