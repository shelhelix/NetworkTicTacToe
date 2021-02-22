using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

// Shapes © Freya Holmér - https://twitter.com/FreyaHolmer/
// Website & Documentation - https://acegikmo.com/shapes/
namespace Shapes {

	public static partial class Draw {

		// draw matrix states:
		static Matrix4x4 matrix = Matrix4x4.identity;
		static bool hasCustomMatrix = false;
		public static bool HasCustomMatrix => hasCustomMatrix;

		/// <summary>The current immediate-mode drawing matrix. Setting this to transform.localToWorldMatrix will make following draw calls be relative to that object</summary>
		public static Matrix4x4 Matrix {
			get => matrix;
			set {
				matrix = value;
				hasCustomMatrix = value != Matrix4x4.identity;
			}
		}

		// globally shared render state styles
		internal static RenderState renderState;
		/// <summary>Current depth buffer compare function. Default is CompareFunction.LessEqual</summary>
		public static CompareFunction ZTest {
			get => renderState.zTest;
			set => renderState.zTest = value;
		}
		/// <summary>This ZOffsetFactor scales the maximum Z slope, with respect to X or Y of the polygon, while the other ZOffsetUnits, scales the minimum resolvable depth buffer value. This allows you to force one polygon to be drawn on top of another although they are actually in the same position. For example, if ZOffsetFactor = 0 & ZOffsetUnits = -1, it pulls the polygon closer to the camera, ignoring the polygon’s slope, whereas if ZOffsetFactor = -1 & ZOffsetUnits = -1, it will pull the polygon even closer when looking at a grazing angle.</summary>
		public static float ZOffsetFactor {
			get => renderState.zOffsetFactor;
			set => renderState.zOffsetFactor = value;
		}
		/// <summary>this ZOffsetUnits value scales the minimum resolvable depth buffer value, while the other ZOffsetFactor scales the maximum Z slope, with respect to X or Y of the polygon. This allows you to force one polygon to be drawn on top of another although they are actually in the same position. For example, if ZOffsetFactor = 0 & ZOffsetUnits = -1, it pulls the polygon closer to the camera, ignoring the polygon’s slope, whereas if ZOffsetFactor = -1 & ZOffsetUnits = -1, it will pull the polygon even closer when looking at a grazing angle.</summary>
		public static int ZOffsetUnits {
			get => renderState.zOffsetUnits;
			set => renderState.zOffsetUnits = value;
		}
		/// <summary> The stencil buffer function used to compare the reference value to the current contents of the buffer. Default: always </summary>
		public static CompareFunction StencilComp {
			get => renderState.stencilComp;
			set => renderState.stencilComp = value;
		}
		/// <summary>What to do with the contents of the stencil buffer if the stencil test (and the depth test) passes. Default: keep</summary>
		public static StencilOp StencilOpPass {
			get => renderState.stencilOpPass;
			set => renderState.stencilOpPass = value;
		}
		/// <summary>The stencil buffer id/reference value to be compared against</summary>
		public static byte StencilRefID {
			get => renderState.stencilRefID;
			set => renderState.stencilRefID = value;
		}
		/// <summary>A stencil buffer 8 bit mask as an 0–255 integer, used when comparing the reference value with the contents of the buffer. Default: 255</summary>
		public static byte StencilReadMask {
			get => renderState.stencilReadMask;
			set => renderState.stencilReadMask = value;
		}
		/// <summary>A stencil buffer 8 bit mask as an 0–255 integer, used when writing to the buffer. Note that, like other write masks, it specifies which bits of stencil buffer will be affected by write (i.e. WriteMask 0 means that no bits are affected and not that 0 will be written). Default: 255</summary>
		public static byte StencilWriteMask {
			get => renderState.stencilWriteMask;
			set => renderState.stencilWriteMask = value;
		}


		// common shared style states
		public static Color Color { get; set; }

		/// <summary>What blending mode to use</summary>
		public static ShapesBlendMode BlendMode { get; set; } // technically a render state, but we swap shaders here instead

		/// <summary>Sets how shapes should behave when scaled</summary>
		public static ScaleMode ScaleMode { get; set; }

		/// <summary>What detail level to use for 3D primitives (3D Lines/Sphere/Torus/Cone)</summary>
		public static DetailLevel DetailLevel { get; set; }

		// shared line & polyline states
		/// <summary>Thickness of lines and polylines</summary>
		public static float LineThickness { get; set; }

		/// <summary>Thickness space of lines and polylines</summary>
		public static ThicknessSpace LineThicknessSpace { get; set; }

		// line states
		/// <summary>End caps of lines</summary>
		public static LineEndCap LineEndCaps { get; set; }

		/// <summary>Type of geometry for lines</summary>
		public static LineGeometry LineGeometry { get; set; }

		// polygon states
		/// <summary>The triangulation method to use. Some of these are computationally faster than others, but only works for certain shapes</summary>
		public static PolygonTriangulation PolygonTriangulation { get; set; }

		/// <summary>The color fill style to use on polygons</summary>
		public static ShapeFill PolygonShapeFill { get; set; }

		// line dashes
		/// <summary>What dash style to use for lines</summary>
		public static DashStyle LineDashStyle { get; set; }

		/// <summary>What dash style to use for rings and arcs</summary>
		public static DashStyle RingDashStyle { get; set; }

		[System.Obsolete( "please use Draw.LineDashStyle.UniformSize or Draw.LineDashStyle.size instead", false )]
		public static float LineDashSize {
			get => LineDashStyle.UniformSize;
			set => LineDashStyle.UniformSize = value;
		}

		// polyline states
		/// <summary>Type of geometry for polylines</summary>
		public static PolylineGeometry PolylineGeometry { get; set; }

		/// <summary>The joins to use for polylines</summary>
		public static PolylineJoins PolylineJoins { get; set; }

		// disc & ring states
		/// <summary>Radius of discs, rings, pies & arcs</summary>
		public static float DiscRadius { get; set; }

		/// <summary>Whether or not discs, rings, pies & arcs should be billboarded</summary>
		public static DiscGeometry DiscGeometry { get; set; }

		/// <summary>Thickness of rings & arcs</summary>
		public static float RingThickness { get; set; }

		/// <summary>Thickness space of rings & arcs</summary>
		public static ThicknessSpace RingThicknessSpace { get; set; }

		/// <summary>Radius space of discs, rings, pies & arcs</summary>
		public static ThicknessSpace DiscRadiusSpace { get; set; }

		// regular polygon states
		/// <summary>Vertex radius of regular polygons</summary>
		public static float RegularPolygonRadius { get; set; }

		/// <summary>The number of sides on regular polygons</summary>
		public static int RegularPolygonSideCount { get; set; }

		/// <summary>Whether or not regular polygons should be billboarded</summary>
		public static RegularPolygonGeometry RegularPolygonGeometry { get; set; }

		/// <summary>Thickness of hollow regular polygons</summary>
		public static float RegularPolygonThickness { get; set; }

		/// <summary>Thickness space of hollow regular polygons</summary>
		public static ThicknessSpace RegularPolygonThicknessSpace { get; set; }

		/// <summary>Radius space of regular polygons</summary>
		public static ThicknessSpace RegularPolygonRadiusSpace { get; set; }

		/// <summary>The color fill style to use on regular polygons</summary>
		public static ShapeFill RegularPolygonShapeFill { get; set; }

		// 3D shape states
		/// <summary>Radius of spheres</summary>
		public static float SphereRadius { get; set; }

		/// <summary>Radius space of spheres</summary>
		public static ThicknessSpace SphereRadiusSpace { get; set; }

		/// <summary>Size space of cuboids</summary>
		public static ThicknessSpace CuboidSizeSpace { get; set; }

		/// <summary>Thickness space of tori (toruses)</summary>
		public static ThicknessSpace TorusThicknessSpace { get; set; }

		/// <summary>Radius space of tori (toruses)</summary>
		public static ThicknessSpace TorusRadiusSpace { get; set; }

		/// <summary>Size space of cones</summary>
		public static ThicknessSpace ConeSizeSpace { get; set; }

		// text states
		/// <summary>The TMP font to use when drawing text</summary>
		public static TMP_FontAsset Font { get; set; }

		/// <summary>The font size to use when drawing text</summary>
		public static float FontSize { get; set; }

		/// <summary>The text alignment to use when drawing text</summary>
		public static TextAlign TextAlign { get; set; }

		// initialize all default values
		static Draw() => ResetAllDrawStates();

		/// <summary>Resets all static states - both style & matrix</summary>
		public static void ResetAllDrawStates() {
			ResetMatrix();
			ResetStyle();
		}

		/// <summary>Resets the matrix to Matrix4x4.identity</summary>
		public static void ResetMatrix() {
			matrix = Matrix4x4.identity;
			hasCustomMatrix = false;
		}

		/// <summary>Resets style states, but not the drawing matrix</summary>
		/// See <see cref="Draw.ResetAllDrawStates()"/> to reset everything
		public static void ResetStyle() {
			Color = Color.white;
			ZTest = ShapeRenderer.DEFAULT_ZTEST;
			ZOffsetFactor = ShapeRenderer.DEFAULT_ZOFS_FACTOR;
			ZOffsetUnits = ShapeRenderer.DEFAULT_ZOFS_UNITS;
			StencilComp = ShapeRenderer.DEFAULT_STENCIL_COMP;
			StencilOpPass = ShapeRenderer.DEFAULT_STENCIL_OP;
			StencilRefID = ShapeRenderer.DEFAULT_STENCIL_REF_ID;
			StencilReadMask = ShapeRenderer.DEFAULT_STENCIL_MASK;
			StencilWriteMask = ShapeRenderer.DEFAULT_STENCIL_MASK;
			BlendMode = ShapesBlendMode.Transparent;
			ScaleMode = ScaleMode.Uniform;
			DetailLevel = DetailLevel.Medium;
			LineThickness = 0.05f;
			LineThicknessSpace = ThicknessSpace.Meters;
			LineDashStyle = DashStyle.DefaultDashStyleLine;
			LineEndCaps = LineEndCap.Round;
			LineGeometry = LineGeometry.Billboard;
			PolygonTriangulation = PolygonTriangulation.EarClipping;
			PolygonShapeFill = new ShapeFill();
			PolylineGeometry = PolylineGeometry.Billboard;
			PolylineJoins = PolylineJoins.Round;

			// disc
			DiscGeometry = DiscGeometry.Flat2D;
			DiscRadius = 1f;
			RingThickness = 0.05f;
			RingThicknessSpace = ThicknessSpace.Meters;
			DiscRadiusSpace = ThicknessSpace.Meters;
			RingDashStyle = DashStyle.DefaultDashStyleRing;

			// regular polygon
			RegularPolygonRadius = 1f;
			RegularPolygonSideCount = 6;
			RegularPolygonGeometry = RegularPolygonGeometry.Flat2D;
			RegularPolygonThickness = 0.05f;
			RegularPolygonThicknessSpace = ThicknessSpace.Meters;
			RegularPolygonRadiusSpace = ThicknessSpace.Meters;
			RegularPolygonShapeFill = new ShapeFill();

			SphereRadius = 1f;
			SphereRadiusSpace = ThicknessSpace.Meters;
			CuboidSizeSpace = ThicknessSpace.Meters;
			TorusThicknessSpace = ThicknessSpace.Meters;
			TorusRadiusSpace = ThicknessSpace.Meters;
			ConeSizeSpace = ThicknessSpace.Meters;
			Font = ShapesAssets.Instance.defaultFont;
			FontSize = 1f;
			TextAlign = TextAlign.Center;
		}


	}

}