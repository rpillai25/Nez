using System.Runtime.CompilerServices;


namespace Nez
{
	/// <summary>
	/// provides frame timing information
	/// </summary>
	public static class Time
	{
		/// <summary>
		/// total time the game has been running
		/// </summary>
		public static float TotalTime;

		/// <summary>
		/// delta time from the previous frame to the current, scaled by timeScale
		/// </summary>
		public static float DeltaTime;

		/// <summary>
		/// unscaled version of deltaTime. Not affected by timeScale
		/// </summary>
		public static float UnscaledDeltaTime;

		/// <summary>
		/// secondary deltaTime for use when you need to scale two different deltas simultaneously
		/// </summary>
		public static float AltDeltaTime;

		/// <summary>
		/// total time since the Scene was loaded
		/// </summary>
		public static float TimeSinceSceneLoad;

		/// <summary>
		/// time scale of deltaTime
		/// </summary>
		public static float TimeScale = 1f;

		/// <summary>
		/// time scale of altDeltaTime
		/// </summary>
		public static float AltTimeScale = 1f;

		/// <summary>
		/// total number of frames that have passed. With <see cref="Core.UseFixedTimeStep"/> this counts rendered
		/// frames (presentation passes), not simulation steps — see <see cref="SimulationStep"/> for those.
		/// </summary>
		public static uint FrameCount;

		/// <summary>
		/// total number of fixed simulation steps that have run (only advances when <see cref="Core.UseFixedTimeStep"/>
		/// is enabled). Simulation code that needs a frame counter should use this instead of <see cref="FrameCount"/>.
		/// </summary>
		public static long SimulationStep;

		/// <summary>
		/// Maximum value that DeltaTime can be. This can be useful to prevent physics from breaking when dragging
		/// the game window or if your game hitches.
		/// </summary>
		public static float MaxDeltaTime = float.MaxValue;

		internal static void Update(float dt)
		{
			if(dt > MaxDeltaTime)
				dt = MaxDeltaTime;
			TotalTime += dt;
			DeltaTime = dt * TimeScale;
			AltDeltaTime = dt * AltTimeScale;
			UnscaledDeltaTime = dt;
			TimeSinceSceneLoad += dt;
			FrameCount++;
		}


		/// <summary>
		/// advances the clock for one fixed simulation step. DeltaTime is the constant step so every accumulator,
		/// coroutine wait and tween in the simulation advances identically regardless of the wall-clock frame rate.
		/// TotalTime and FrameCount are deliberately left alone — they belong to the presentation pass.
		/// </summary>
		internal static void SimulationStepUpdate(float stepSeconds)
		{
			DeltaTime = stepSeconds * TimeScale;
			AltDeltaTime = stepSeconds * AltTimeScale;
			UnscaledDeltaTime = stepSeconds;
			SimulationStep++;
		}

		/// <summary>
		/// advances the wall-clock side of the clock once per rendered frame (after all simulation steps ran).
		/// DeltaTime becomes the real frame delta so UI animations and camera code see smooth timing.
		/// </summary>
		internal static void PresentationUpdate(float wallDt)
		{
			TotalTime += wallDt;
			DeltaTime = wallDt * TimeScale;
			AltDeltaTime = wallDt * AltTimeScale;
			UnscaledDeltaTime = wallDt;
			TimeSinceSceneLoad += wallDt;
			FrameCount++;
		}

		internal static void SceneChanged()
		{
			TimeSinceSceneLoad = 0f;
		}


		/// <summary>
		/// Allows to check in intervals. Should only be used with interval values above deltaTime,
		/// otherwise it will always return true.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool CheckEvery(float interval)
		{
			// we subtract deltaTime since timeSinceSceneLoad already includes this update ticks deltaTime
			return (int) (TimeSinceSceneLoad / interval) > (int) ((TimeSinceSceneLoad - DeltaTime) / interval);
		}
	}
}
