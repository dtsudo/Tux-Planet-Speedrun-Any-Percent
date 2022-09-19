
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
		TilemapCastle,
		BossDoor,
		Tux,
		TuxMirrored,
		Konqi,
		KonqiMirrored,
		KonqiFire,
		KonqiFireMirrored,
		Yeti,
		YetiMirrored,
		Blazeborn,
		BlazebornMirrored,
		Smartcap,
		SmartcapMirrored,
		Bouncecap,
		BouncecapMirrored,
		Flyamanita,
		FlyamanitaMirrored,
		Snail,
		SnailMirrored,
		SnailBlue,
		SnailBlueMirrored,
		FishBlue,
		FishBlueMirrored,
		FishGreen,
		FishGreenMirrored,
		FishRed,
		FishRedMirrored,
		Orange,
		OrangeMirrored,
		Poof,
		BossHealth,
		C4,
		Coin,
		EarthShell,
		Igloo,
		Actors,
		Solid,
		Spikes,
		Flash,
		ExplodeF,
		Flame,
		FlameBlue,
		Lock,
		KeyCopper,
		KeySilver,
		KeyGold,
		KeyMythril,

		Signpost,

		PathDirt,
		ForestSnowy,
		RocksSnow,
		Snow,
		WaterCliffSnow,
		Mountains,
		Towns,
		LevelIcons,
		TuxOverworld,

		WaterSurface,

		OceanBackground,
		Arctis2,
		CaveBackground
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
				case GameImage.TilemapCastle: return "KelvinShadewing/tsCastle.png";
				case GameImage.BossDoor: return "KelvinShadewing/boss-door.png";
				case GameImage.Tux: return "KelvinShadewing/tux.png";
				case GameImage.TuxMirrored: return "KelvinShadewing/tux_mirrored.png";
				case GameImage.Konqi: return "KelvinShadewing/konqi.png";
				case GameImage.KonqiMirrored: return "KelvinShadewing/konqi_mirrored.png";
				case GameImage.KonqiFire: return "KelvinShadewing/konqifire.png";
				case GameImage.KonqiFireMirrored: return "KelvinShadewing/konqifire_mirrored.png";
				case GameImage.Yeti: return "KelvinShadewing/yeti.png";
				case GameImage.YetiMirrored: return "KelvinShadewing/yeti_mirrored.png";
				case GameImage.Blazeborn: return "FrostC/Blazeborn.png";
				case GameImage.BlazebornMirrored: return "FrostC/Blazeborn_mirrored.png";
				case GameImage.Smartcap: return "KelvinShadewing/smartcap.png";
				case GameImage.SmartcapMirrored: return "KelvinShadewing/smartcap_mirrored.png";
				case GameImage.Bouncecap: return "KelvinShadewing/bouncecap.png";
				case GameImage.BouncecapMirrored: return "KelvinShadewing/bouncecap_mirrored.png";
				case GameImage.Flyamanita: return "KelvinShadewing/flyamanita.png";
				case GameImage.FlyamanitaMirrored: return "KelvinShadewing/flyamanita_mirrored.png";
				case GameImage.Snail: return "KelvinShadewing/snail.png";
				case GameImage.SnailMirrored: return "KelvinShadewing/snail_mirrored.png";
				case GameImage.SnailBlue: return "KelvinShadewing/snail-blue.png";
				case GameImage.SnailBlueMirrored: return "KelvinShadewing/snail-blue_mirrored.png";
				case GameImage.FishBlue: return "KelvinShadewing/fishblue.png";
				case GameImage.FishBlueMirrored: return "KelvinShadewing/fishblue_mirrored.png";
				case GameImage.FishGreen: return "KelvinShadewing/fishgreen.png";
				case GameImage.FishGreenMirrored: return "KelvinShadewing/fishgreen_mirrored.png";
				case GameImage.FishRed: return "KelvinShadewing/fishred.png";
				case GameImage.FishRedMirrored: return "KelvinShadewing/fishred_mirrored.png";
				case GameImage.Orange: return "KelvinShadewing/orange.png";
				case GameImage.OrangeMirrored: return "KelvinShadewing/orange_mirrored.png";
				case GameImage.Poof: return "KelvinShadewing/poof.png";
				case GameImage.BossHealth: return "KelvinShadewing/boss-health.png";
				case GameImage.C4: return "KelvinShadewing/c4.png";
				case GameImage.Coin: return "KelvinShadewing/coin.png";
				case GameImage.EarthShell: return "KelvinShadewing/earthshell.png";
				case GameImage.Igloo: return "KelvinShadewing/igloo.png";
				case GameImage.Actors: return "KelvinShadewing/actors.png";
				case GameImage.Solid: return "KelvinShadewing/solid.png";
				case GameImage.Spikes: return "FrostC/spikes.png";
				case GameImage.Flash: return "KelvinShadewing/tfFlash.png";
				case GameImage.ExplodeF: return "KelvinShadewing/explodeF.png";
				case GameImage.Flame: return "KelvinShadewing/flame.png";
				case GameImage.FlameBlue: return "KelvinShadewing/flameBlue.png";
				case GameImage.Lock: return "KelvinShadewing/lock.png";
				case GameImage.KeyCopper: return "KelvinShadewing/key-copper.png";
				case GameImage.KeySilver: return "KelvinShadewing/key-silver.png";
				case GameImage.KeyGold: return "KelvinShadewing/key-gold.png";
				case GameImage.KeyMythril: return "KelvinShadewing/key-mythril.png";

				case GameImage.Signpost: return "Nemisys/signpost.png";

				case GameImage.PathDirt: return "BenCreating/PathDirt.png";
				case GameImage.ForestSnowy: return "BenCreating/Snow/ForestSnowy.png";
				case GameImage.RocksSnow: return "BenCreating/Snow/RocksSnow.png";
				case GameImage.Snow: return "BenCreating/Snow/Snow.png";
				case GameImage.WaterCliffSnow: return "BenCreating/Snow/WaterCliffSnow.png";
				case GameImage.Mountains: return "BenCreating/Mountains.png";
				case GameImage.Towns: return "BenCreating/Grass/Towns.png";
				case GameImage.LevelIcons: return "KelvinShadewing/level-icons.png";
				case GameImage.TuxOverworld: return "KelvinShadewing/tuxO.png";

				case GameImage.WaterSurface: return "KelvinShadewing/watersurface.png";

				case GameImage.OceanBackground: return "KnoblePersona/ocean.png";
				case GameImage.Arctis2: return "grumbel/arctis2.png";
				case GameImage.CaveBackground: return "KelvinShadewing/caveBackground.png";

				default: throw new Exception();
			}
		}
	}
}
