namespace Nez
{
	public class DefaultRenderer : Renderer
	{
		/// <summary>
		/// renders all renderLayers
		/// </summary>
		/// <param name="renderOrder">Render order.</param>
		/// <param name="camera">Camera.</param>
		public DefaultRenderer(int renderOrder = 0, Camera camera = null) : base(renderOrder, camera)
		{
		}

		public override void Render(Scene scene)
		{
			var cam = Camera ?? scene.Camera;
			BeginRender(cam);

			var filter = scene.RenderableFilter;
			for (var i = 0; i < scene.RenderableComponents.Count; i++)
			{
				var renderable = scene.RenderableComponents[i];
				if (renderable.Enabled && (filter == null || filter(renderable)) && renderable.IsVisibleFromCamera(cam))
					RenderAfterStateCheck(renderable, cam);
			}

			if (ShouldDebugRender && Core.DebugRenderEnabled)
				DebugRender(scene, cam);

			EndRender();
		}
	}
}