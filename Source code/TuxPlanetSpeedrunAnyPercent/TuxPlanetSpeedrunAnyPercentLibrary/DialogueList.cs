
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class DialogueList
	{
		private IReadOnlyList<Dialogue> dialogues;

		public DialogueList(IReadOnlyList<Dialogue> dialogues)
		{
			this.dialogues = new List<Dialogue>(dialogues);
		}

		public class Result
		{
			public Result(
				DialogueList dialogueList,
				bool isDone)
			{
				this.DialogueList = dialogueList;
				this.IsDone = isDone;
			}

			public DialogueList DialogueList { get; private set; }
			public bool IsDone { get; private set; }
		}

		public Result ProcessFrame(
			Move move,
			int elapsedMicrosPerFrame)
		{
			Dialogue.Result dialogueResult = this.dialogues[0].ProcessFrame(
				move: move,
				elapsedMicrosPerFrame: elapsedMicrosPerFrame);

			List<Dialogue> newDialogues;
			bool isDone = false;

			if (dialogueResult.IsDone)
			{
				if (this.dialogues.Count == 1)
				{
					newDialogues = new List<Dialogue>() { dialogueResult.Dialogue };
					isDone = true;
				}
				else
				{
					newDialogues = new List<Dialogue>();
					for (int i = 1; i < this.dialogues.Count; i++)
						newDialogues.Add(this.dialogues[i]);
				}
			}
			else
			{
				newDialogues = new List<Dialogue>();
				newDialogues.Add(dialogueResult.Dialogue);
				for (int i = 1; i < this.dialogues.Count; i++)
					newDialogues.Add(this.dialogues[i]);
			}

			return new Result(
				dialogueList: new DialogueList(dialogues: newDialogues),
				isDone: isDone);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput, int windowWidth, int windowHeight)
		{
			this.dialogues[0].Render(
				displayOutput: displayOutput, 
				windowWidth: windowWidth, 
				windowHeight: windowHeight);
		}
	}
}
