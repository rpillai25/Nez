namespace Nez.Particles
{
	/// <summary>
	/// Random source for particle variance. Deliberately separate from <see cref="Nez.Random"/>: particles are
	/// purely visual, and games that seed the global stream for deterministic simulation (replays) must not have
	/// gameplay outcomes depend on how many particles happened to spawn.
	/// </summary>
	public static class ParticleRandom
	{
		/// <summary>the generator behind particle variance. Replace to make visuals reproducible if desired.</summary>
		public static System.Random Rng = new System.Random();

		/// <summary>returns a random float between -1 and 1</summary>
		public static float MinusOneToOne()
		{
			return (float)Rng.NextDouble() * 2f - 1f;
		}
	}
}
