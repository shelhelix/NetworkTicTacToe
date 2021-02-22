using UnityEngine;
using UnityEngine.Rendering;

// Shapes © Freya Holmér - https://twitter.com/FreyaHolmer/
// Website & Documentation - https://acegikmo.com/shapes/
namespace Shapes {
	
	struct ShapeDrawState {
		public Mesh mesh;
		public Material mat;
		public int submesh;

		internal bool CompatibleWith( ShapeDrawState other ) => mesh == other.mesh && submesh == other.submesh && mat == other.mat;
	}
	
	// this is the type that is submitted to the render call
	abstract class ShapeDrawCall {
		public ShapeDrawState drawState;
		public MaterialPropertyBlock mpb;
		public int count;
		public abstract void AddToCommandBuffer( CommandBuffer cmd );
	}

	class ShapeDrawCallSingle : ShapeDrawCall {
		public Matrix4x4 matrix;
		public ShapeDrawCallSingle( Matrix4x4 matrix ) => this.matrix = matrix;
		public override void AddToCommandBuffer( CommandBuffer cmd ) => cmd.DrawMesh( drawState.mesh, matrix, drawState.mat, drawState.submesh, 0, mpb );
	}

	class ShapeDrawCallInstanced : ShapeDrawCall {
		public Matrix4x4[] matrices;
		public ShapeDrawCallInstanced( Matrix4x4[] matrices ) => this.matrices = matrices;
		public override void AddToCommandBuffer( CommandBuffer cmd ) => cmd.DrawMeshInstanced( drawState.mesh, drawState.submesh, drawState.mat, 0, matrices, count, mpb );
	}

	


}