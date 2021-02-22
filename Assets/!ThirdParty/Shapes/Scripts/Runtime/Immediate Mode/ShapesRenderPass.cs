using UnityEngine;
using UnityEngine.Rendering;
#if SHAPES_URP
using UnityEngine.Rendering.Universal;
#elif SHAPES_HDRP
using System.Collections.Generic;
using UnityEngine.Rendering.HighDefinition;

#endif

// Shapes © Freya Holmér - https://twitter.com/FreyaHolmer/
// Website & Documentation - https://acegikmo.com/shapes/
namespace Shapes {

	#if SHAPES_URP
	internal class ShapesRenderPass : ScriptableRenderPass {
		DrawCommand drawCommand;

		public ShapesRenderPass( DrawCommand drawCommand ) {
			this.drawCommand = drawCommand;
			this.renderPassEvent = drawCommand.camEvt;
		}

		public override void Execute( ScriptableRenderContext context, ref RenderingData renderingData ) {
			CommandBuffer cmdBuf = new CommandBuffer { name = drawCommand.name }; // this feels a lil wasteful
			drawCommand.AppendToBuffer( cmdBuf );
			context.ExecuteCommandBuffer( cmdBuf );
			cmdBuf.Release();
		}

		public override void FrameCleanup( CommandBuffer cmd ) => DrawCommand.OnCommandRendered( drawCommand );
	}
	#elif SHAPES_HDRP
	public class ShapesRenderPass : CustomPass {
		// HDRP doesn't have ScriptableRenderPass stuff, so we use *one* custom pass per injection point, but branch inside of it instead
		// this does mean there will be redundancy/overhead in the way this is done, but, can't do much about it for now I think
		protected override void Execute( ScriptableRenderContext context, CommandBuffer cmd, HDCamera hdCamera, CullingResults cullingResult ) {

			List<DrawCommand> executingCommands = null;
			if( DrawCommand.cBuffersRendering.TryGetValue( hdCamera.camera, out List<DrawCommand> cmds ) ) {
				for( int i = 0; i < cmds.Count; i++ ) {
					if( cmds[i].camEvt == injectionPoint ) {
						if( executingCommands == null )
							executingCommands = new List<DrawCommand>();
						executingCommands.Add( cmds[i] );
						cmds[i].AppendToBuffer( cmd );
					}
				}
			}

			// if we added commands, execute them immediately
			if( executingCommands?.Count > 0 ) {
				context.ExecuteCommandBuffer( cmd ); // we have to execute it because OnCommandRendered might want to destroy used materials
				cmd.Clear();
				foreach( DrawCommand drawCommand in executingCommands )
					DrawCommand.OnCommandRendered( drawCommand ); // deletes cached assets
			}
		}
	}
	#endif

}