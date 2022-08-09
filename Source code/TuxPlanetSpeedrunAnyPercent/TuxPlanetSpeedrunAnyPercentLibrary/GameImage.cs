
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using System;

	public enum GameImage
	{
		SoundOn_Black,
		SoundOff_Black,
		MusicOn_Black,
		MusicOff_Black,
		SoundOn_White,
		SoundOff_White,
		MusicOn_White,
		MusicOff_White,

		TilemapSnow,
		Tux,
		TuxMirrored,
		Konqi,
		KonqiMirrored,
		Blazeborn,
		BlazebornMirrored,
		Smartcap,
		SmartcapMirrored,
		BossHealth,
		C4,
		Coin,
		EarthShell,
		Igloo,
		Actors,
		Solid,
		Spikes,

		Signpost,

		PathDirt,
		Snow,
		LevelIcons,
		TuxOverworld,

		OceanBackground
	}

	public static class GameImageUtil
	{
		public static string GetImageFilename(this GameImage image)
		{
			switch (image)
			{
				case GameImage.SoundOn_Black: return "Kenney/SoundOn_Black.png";
				case GameImage.SoundOff_Black: return "Kenney/SoundOff_Black.png";
				case GameImage.MusicOn_Black: return "Kenney/MusicOn_Black.png";
				case GameImage.MusicOff_Black: return "Kenney/MusicOff_Black.png";
				case GameImage.SoundOn_White: return "Kenney/SoundOn_White.png";
				case GameImage.SoundOff_White: return "Kenney/SoundOff_White.png";
				case GameImage.MusicOn_White: return "Kenney/MusicOn_White.png";
				case GameImage.MusicOff_White: return "Kenney/MusicOff_White.png";

				case GameImage.TilemapSnow: return "KelvinShadewing/tssnow.png";
				case GameImage.Tux: return "KelvinShadewing/tux.png";
				case GameImage.TuxMirrored: return "KelvinShadewing/tux_mirrored.png";
				case GameImage.Konqi: return "KelvinShadewing/konqi.png";
				case GameImage.KonqiMirrored: return "KelvinShadewing/konqi_mirrored.png";
				case GameImage.Blazeborn: return "FrostC/Blazeborn.png";
				case GameImage.BlazebornMirrored: return "FrostC/Blazeborn_mirrored.png";
				case GameImage.Smartcap: return "KelvinShadewing/smartcap.png";
				case GameImage.SmartcapMirrored: return "KelvinShadewing/smartcap_mirrored.png";
				case GameImage.BossHealth: return "KelvinShadewing/boss-health.png";
				case GameImage.C4: return "KelvinShadewing/c4.png";
				case GameImage.Coin: return "KelvinShadewing/coin.png";
				case GameImage.EarthShell: return "KelvinShadewing/earthshell.png";
				case GameImage.Igloo: return "KelvinShadewing/igloo.png";
				case GameImage.Actors: return "KelvinShadewing/actors.png";
				case GameImage.Solid: return "KelvinShadewing/solid.png";
				case GameImage.Spikes: return "FrostC/spikes.png";

				case GameImage.Signpost: return "Nemisys/signpost.png";

				case GameImage.PathDirt: return "BenCreating/PathDirt.png";
				case GameImage.Snow: return "BenCreating/Snow/Snow.png";
				case GameImage.LevelIcons: return "KelvinShadewing/level-icons.png";
				case GameImage.TuxOverworld: return "KelvinShadewing/tuxO.png";

				case GameImage.OceanBackground: return "KnoblePersona/ocean.png";

				default: throw new Exception();
			}
		}
	}
}
