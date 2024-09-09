using System;
using System.Collections.Generic;
using Nez.Textures;


namespace Nez.Sprites
{
	/// <summary>
	/// SpriteAnimator handles the display and animation of a sprite
	/// </summary>
	public partial class SpriteAnimator : SpriteRenderer, IUpdatable
	{
		/// <summary>
		/// fired when an animation completes, includes the animation name;
		/// </summary>
		public event Action<string> OnAnimationCompletedEvent;

		/// <summary>
		/// animation playback speed
		/// </summary>
		public float Speed = 1;

		/// <summary>
		/// the current state of the animation
		/// </summary>
		public State AnimationState { get; private set; } = State.None;

		/// <summary>
		/// the current animation
		/// </summary>
		public SpriteAnimation CurrentAnimation { get; private set; }

		/// <summary>
		/// the name of the current animation
		/// </summary>
		public string CurrentAnimationName { get; private set; }

		/// <summary>
		/// index of the current frame in sprite array of the current animation
		/// </summary>
		public int CurrentFrame { get; set; }
		
		/// <summary>
		/// amount of frames in the current animation
		/// </summary>
		public int FrameCount { get; private set; }

		/// <summary>
		/// returns the total elapsed time of the animation.
		/// </summary>
		public float CurrentElapsedTime { get; private set; }
		
		/// <summary>
		/// Provides access to list of available animations
		/// </summary>
		public Dictionary<string, SpriteAnimation> Animations { get; private set; } 
			= new Dictionary<string, SpriteAnimation>();

		/// <summary>
		/// Mode of looping the animation.
		/// It can have 5 different values: Loop, Once, ClampForever, PingPong and PingPongOnce. Defaults to Loop.
		/// </summary>
		public LoopMode CurrentLoopMode { get; private set; }
		
		/// <summary>
		/// Loop Mode Controller responsible for getting the next frame and checking end of animation.
		/// </summary>
		public ILoopModeController LoopModeController { get; private set; }
		
		/// <summary>
		/// The amount of seconds remaining in the current frame
		/// </summary>
		public float FrameTimeLeft { get; private set; }

		public SpriteAnimator()
		{ }

		public SpriteAnimator(Sprite sprite) => SetSprite(sprite);

		public virtual void Update()
		{
			if (AnimationState != State.Running || CurrentAnimation == null)
				return;

			CurrentElapsedTime += Time.DeltaTime;
			FrameTimeLeft -= Time.DeltaTime;
			if (ShouldChangeFrame())
			{
				LoopModeController.NextFrame(this);
			}
		}

		/// <summary>
		/// adds all the animations from the SpriteAtlas
		/// </summary>
		public SpriteAnimator AddAnimationsFromAtlas(SpriteAtlas atlas)
		{
			for (var i = 0; i < atlas.AnimationNames.Length; i++)
				Animations.Add(atlas.AnimationNames[i], atlas.SpriteAnimations[i]);
			return this;
		}

		/// <summary>
		/// Adds a SpriteAnimation
		/// </summary>
		public SpriteAnimator AddAnimation(string name, SpriteAnimation animation)
		{
			// if we have no sprite use the first frame we find
			if (Sprite == null && animation.Sprites.Length > 0)
				SetSprite(animation.Sprites[0]);
			Animations[name] = animation;
			return this;
		}

		public SpriteAnimator AddAnimation(string name, Sprite[] sprites, float fps = 10) => AddAnimation(name, fps, sprites);

		public SpriteAnimator AddAnimation(string name, float fps, params Sprite[] sprites)
		{
			AddAnimation(name, new SpriteAnimation(sprites, fps));
			return this;
		}
		
		/// <summary>
		/// checks to see if the animation is playing (i.e. the animation is active. it may still be in the paused state)
		/// </summary>
		public bool IsAnimationActive(string name) => CurrentAnimation != null && CurrentAnimationName.Equals(name);
		
		/// <summary>
		/// checks to see if the CurrentAnimation is running
		/// </summary>
		public bool IsRunning => AnimationState == State.Running;

		/// <summary>
		/// plays the animation with the given name. If no loopMode is specified it is defaults to Loop
		/// </summary>
		public void Play(string name, LoopMode loopMode = LoopMode.Loop)
		{
			CurrentAnimation = Animations[name];
			CurrentAnimationName = name;
			FrameCount = CurrentAnimation.FrameRates.Length;
			
			SetFrame(0);

			CurrentLoopMode = loopMode;
			LoopModeController = GetNewController(loopMode);
			AnimationState = State.Running;
		}
		
		/// <summary>
		/// pauses the animator
		/// </summary>
		public void Pause() => AnimationState = State.Paused;

		/// <summary>
		/// unpauses the animator
		/// </summary>
		public void UnPause() => AnimationState = State.Running;

		/// <summary>
		/// stops the current animation and nulls it out
		/// </summary>
		public void Stop()
		{
			CurrentAnimation = null;
			CurrentAnimationName = null;
			CurrentFrame = 0;
			CurrentElapsedTime = 0;
			AnimationState = State.None;
		}

		/// <summary>
		/// Sets the current frame for the animation
		/// </summary>
		/// <param name="frameIndex">Index of the desired frame</param>
		public void SetFrame(int frameIndex)
		{
			CurrentFrame = frameIndex;
			Sprite = CurrentAnimation.Sprites[frameIndex];
			FrameTimeLeft = ConvertFrameRateToSeconds(CurrentAnimation.FrameRates[frameIndex]);
		}

		/// <summary>
		/// Sets the animation as completed
		/// </summary>
		/// <param name="returnToFirstFrame">If the animation should return to the first frame before finishing</param>
		public void SetCompleted(bool returnToFirstFrame = false)
		{
			if (returnToFirstFrame)
			{
				SetFrame(0);
			}
			
			CurrentElapsedTime = 0;
			AnimationState = State.Completed;
			OnAnimationCompletedEvent?.Invoke(CurrentAnimationName);
		}
		
		/// <summary>
		/// Checks if it needs to change the current animation frame
		/// </summary>
		/// <returns>True if it does need to change frame, false otherwise</returns>
		private bool ShouldChangeFrame()
		{
			return FrameTimeLeft <= 0;
		}
		
		/// <summary>
		/// Converts an animation frame rate (1/60s) to seconds
		/// </summary>
		/// <param name="frameRate"></param>
		/// <returns>The number of seconds as a float</returns>
		private float ConvertFrameRateToSeconds(float frameRate)
		{
			return 1 / (frameRate * Speed);
		}
	}
}