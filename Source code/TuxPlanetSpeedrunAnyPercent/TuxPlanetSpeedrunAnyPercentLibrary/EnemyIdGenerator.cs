
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;

	public class EnemyIdGenerator
	{
		private int id;

		public EnemyIdGenerator()
		{
			this.id = 1;
		}

		public string GetNewId()
		{
			string newId = "enemyId" + this.id.ToStringCultureInvariant();
			this.id++;
			return newId;
		}
	}
}
