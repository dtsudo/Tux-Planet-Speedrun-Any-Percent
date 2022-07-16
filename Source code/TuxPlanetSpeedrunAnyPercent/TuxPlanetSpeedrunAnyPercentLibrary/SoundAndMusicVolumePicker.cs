
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;

	public class SoundAndMusicVolumePicker
	{
		public enum Color
		{
			Black,
			White
		}

		private SoundVolumePicker soundVolumePicker;
		private MusicVolumePicker musicVolumePicker;

		public SoundAndMusicVolumePicker(int xPos, int yPos, int initialSoundVolume, int initialMusicVolume, int elapsedMicrosPerFrame, Color color)
		{
			this.soundVolumePicker = new SoundVolumePicker(xPos: xPos, yPos: yPos + 50, initialVolume: initialSoundVolume, color: color);
			this.musicVolumePicker = new MusicVolumePicker(xPos: xPos, yPos: yPos, initialVolume: initialMusicVolume, color: color);
		}

		public void ProcessFrame(
			IMouse mouseInput,
			IMouse previousMouseInput)
		{
			this.soundVolumePicker.ProcessFrame(mouseInput: mouseInput, previousMouseInput: previousMouseInput);
			this.musicVolumePicker.ProcessFrame(mouseInput: mouseInput, previousMouseInput: previousMouseInput);
		}
		
		/// <summary>
		/// Returns a number from 0 to 100 (both inclusive)
		/// </summary>
		public int GetCurrentSoundVolume()
		{
			return this.soundVolumePicker.GetCurrentSoundVolume();
		}

		/// <summary>
		/// Returns a number from 0 to 100 (both inclusive)
		/// </summary>
		public int GetCurrentMusicVolume()
		{
			return this.musicVolumePicker.GetCurrentMusicVolume();
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			this.soundVolumePicker.Render(displayOutput: displayOutput);
			this.musicVolumePicker.Render(displayOutput: displayOutput);
		}
	}
}
