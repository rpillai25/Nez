namespace Nez
{
	/// <summary>
	/// Pure helper that converts wall-clock frame time into a whole number of fixed simulation steps.
	/// Used by <see cref="Core"/> when <see cref="Core.UseFixedTimeStep"/> is enabled. Kept free of any
	/// engine state so it can be unit tested headlessly.
	/// </summary>
	public static class FixedStepScheduler
	{
		/// <summary>
		/// Accumulates <paramref name="wallDt"/> scaled by <paramref name="speed"/> into <paramref name="accumulator"/>
		/// and returns how many whole steps of <paramref name="stepSeconds"/> should run this frame, capped at
		/// <paramref name="maxSteps"/>. Any backlog beyond one step is discarded after the cap so a long hitch or an
		/// occluded window never produces a catch-up spiral. A speed of zero (or less) runs no steps and leaves the
		/// accumulator untouched.
		/// </summary>
		public static int ComputeSteps(ref float accumulator, float wallDt, float speed, float stepSeconds, int maxSteps)
		{
			if (speed <= 0f || stepSeconds <= 0f || maxSteps <= 0)
				return 0;

			if (wallDt < 0f)
				wallDt = 0f;

			accumulator += wallDt * speed;

			var steps = (int)(accumulator / stepSeconds);
			if (steps > maxSteps)
				steps = maxSteps;

			accumulator -= steps * stepSeconds;

			// drop any backlog we could not service this frame: at most one partial step carries over
			if (accumulator > stepSeconds)
				accumulator = stepSeconds;

			return steps;
		}
	}
}
