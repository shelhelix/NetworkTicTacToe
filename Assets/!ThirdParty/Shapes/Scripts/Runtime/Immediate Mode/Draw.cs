using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
#if SHAPES_URP
using UnityEngine.Rendering.Universal;
#elif SHAPES_HDRP
using UnityEngine.Rendering.HighDefinition;
#else
using UnityEngine.Rendering;

#endif

// Shapes © Freya Holmér - https://twitter.com/FreyaHolmer/
// Website & Documentation - https://acegikmo.com/shapes/
namespace Shapes {

	public static partial class Draw {


		/// <summary>Creates a DrawCommand for drawing in immediate mode.
		/// Example usage:
		/// using(Draw.Command(Camera.main)){ Draw.Line( a, b, Color.red ); }</summary>
		/// <param name="cam">The camera to draw in</param>
		/// <param name="cameraEvent">When during the render this command should execute</param>
		#if SHAPES_URP
		public static DrawCommand Command( Camera cam, RenderPassEvent cameraEvent = RenderPassEvent.BeforeRenderingPostProcessing ) => new DrawCommand( cam, cameraEvent );
		#elif SHAPES_HDRP
		public static DrawCommand Command( Camera cam, CustomPassInjectionPoint cameraEvent = CustomPassInjectionPoint.BeforePostProcess ) => new DrawCommand( cam, cameraEvent );
		#else
		public static DrawCommand Command( Camera cam, CameraEvent cameraEvent = CameraEvent.BeforeImageEffects ) => new DrawCommand( cam, cameraEvent );
		#endif

		static MpbLine mpbLine = new MpbLine();

		[OvldGenCallTarget] static void Line( [OvldDefault( nameof(BlendMode) )] ShapesBlendMode blendMode,
											  [OvldDefault( nameof(LineGeometry) )] LineGeometry geometry,
											  [OvldDefault( nameof(LineEndCaps) )] LineEndCap endCaps,
											  [OvldDefault( nameof(LineThicknessSpace) )] ThicknessSpace thicknessSpace,
											  Vector3 start,
											  Vector3 end,
											  [OvldDefault( nameof(Color) )] Color colorStart,
											  [OvldDefault( nameof(Color) )] Color colorEnd,
											  [OvldDefault( nameof(LineThickness) )] float thickness,
											  [OvldDefault( nameof(LineDashStyle) )] DashStyle dashStyle = null ) {
			using( new IMDrawer(
				metaMpb: mpbLine,
				sourceMat: ShapesMaterialUtils.GetLineMat( geometry, endCaps )[blendMode],
				sourceMesh: ShapesMeshUtils.GetLineMesh( geometry, endCaps, DetailLevel ) ) ) {
				MetaMpb.ApplyDashSettings( mpbLine, dashStyle, thickness );
				mpbLine.color.Add( colorStart );
				mpbLine.colorEnd.Add( colorEnd );
				mpbLine.pointStart.Add( start );
				mpbLine.pointEnd.Add( end );
				mpbLine.thickness.Add( thickness );
				mpbLine.alignment.Add( (float)geometry );
				mpbLine.thicknessSpace.Add( (float)thicknessSpace );
				mpbLine.scaleMode.Add( (float)ScaleMode );
			}
		}

		static MpbPolyline mpbPolyline = new MpbPolyline(); // they can use the same mpb structure
		static MpbPolyline mpbPolylineJoins = new MpbPolyline();

		[OvldGenCallTarget] static void Polyline( [OvldDefault( nameof(BlendMode) )] ShapesBlendMode blendMode,
												  PolylinePath path,
												  [OvldDefault( "false" )] bool closed,
												  [OvldDefault( nameof(PolylineGeometry) )] PolylineGeometry geometry,
												  [OvldDefault( nameof(PolylineJoins) )] PolylineJoins joins,
												  [OvldDefault( nameof(LineThickness) )] float thickness,
												  [OvldDefault( nameof(LineThicknessSpace) )] ThicknessSpace thicknessSpace,
												  [OvldDefault( nameof(Color) )] Color color ) {
			if( path.EnsureMeshIsReadyToRender( closed, joins, out Mesh mesh ) == false )
				return; // no points defined in the mesh

			switch( path.Count ) {
				case 0:
					Debug.LogWarning( "Tried to draw polyline with no points" );
					return;
				case 1:
					Debug.LogWarning( "Tried to draw polyline with only one point" );
					return;
			}

			void ApplyToMpb( MpbPolyline mpb ) {
				mpb.thickness.Add( thickness );
				mpb.thicknessSpace.Add( (int)thicknessSpace );
				mpb.color.Add( color );
				mpb.alignment.Add( (int)geometry );
				mpb.scaleMode.Add( (int)ScaleMode );
			}

			using( new IMDrawer( mpbPolyline, ShapesMaterialUtils.GetPolylineMat( joins )[blendMode], mesh, 0 ) )
				ApplyToMpb( mpbPolyline );

			if( joins.HasJoinMesh() ) {
				using( new IMDrawer( mpbPolylineJoins, ShapesMaterialUtils.GetPolylineJoinsMat( joins )[blendMode], mesh, 1 ) )
					ApplyToMpb( mpbPolylineJoins );
			}
		}

		static MpbPolygon mpbPolygon = new MpbPolygon();

		[OvldGenCallTarget] static void Polygon( [OvldDefault( nameof(BlendMode) )] ShapesBlendMode blendMode,
												 PolygonPath path,
												 [OvldDefault( nameof(PolygonTriangulation) )] PolygonTriangulation triangulation,
												 [OvldDefault( nameof(Color) )] Color color,
												 [OvldDefault( nameof(PolygonShapeFill) )] ShapeFill fill ) {
			if( path.EnsureMeshIsReadyToRender( triangulation, out Mesh mesh ) == false )
				return; // no points defined in the mesh

			switch( path.Count ) {
				case 0:
					Debug.LogWarning( "Tried to draw polygon with no points" );
					return;
				case 1:
					Debug.LogWarning( "Tried to draw polygon with only one point" );
					return;
				case 2:
					Debug.LogWarning( "Tried to draw polygon with only two points" );
					return;
			}

			using( new IMDrawer( mpbPolygon, ShapesMaterialUtils.matPolygon[blendMode], mesh ) ) {
				MetaMpb.ApplyColorOrFill( mpbPolygon, fill, color );
			}
		}

		[OvldGenCallTarget] static void Disc( Vector3 pos,
											  [OvldDefault( "Quaternion.identity" )] Quaternion rot,
											  [OvldDefault( nameof(DiscRadius) )] float radius,
											  [OvldDefault( nameof(Color) )] Color colorInnerStart,
											  [OvldDefault( nameof(Color) )] Color colorOuterStart,
											  [OvldDefault( nameof(Color) )] Color colorInnerEnd,
											  [OvldDefault( nameof(Color) )] Color colorOuterEnd ) {
			DiscCore( BlendMode, DiscRadiusSpace, RingThicknessSpace, false, false, pos, rot, radius, 0f, colorInnerStart, colorOuterStart, colorInnerEnd, colorOuterEnd );
		}

		[OvldGenCallTarget] static void Ring( Vector3 pos,
											  [OvldDefault( "Quaternion.identity" )] Quaternion rot,
											  [OvldDefault( nameof(DiscRadius) )] float radius,
											  [OvldDefault( nameof(RingThickness) )] float thickness,
											  [OvldDefault( nameof(Color) )] Color colorInnerStart,
											  [OvldDefault( nameof(Color) )] Color colorOuterStart,
											  [OvldDefault( nameof(Color) )] Color colorInnerEnd,
											  [OvldDefault( nameof(Color) )] Color colorOuterEnd,
											  [OvldDefault( nameof(RingDashStyle) )] DashStyle dashStyle = null ) {
			DiscCore( BlendMode, DiscRadiusSpace, RingThicknessSpace, true, false, pos, rot, radius, thickness, colorInnerStart, colorOuterStart, colorInnerEnd, colorOuterEnd, dashStyle );
		}

		[OvldGenCallTarget] static void Pie( Vector3 pos,
											 [OvldDefault( "Quaternion.identity" )] Quaternion rot,
											 [OvldDefault( nameof(DiscRadius) )] float radius,
											 [OvldDefault( nameof(Color) )] Color colorInnerStart,
											 [OvldDefault( nameof(Color) )] Color colorOuterStart,
											 [OvldDefault( nameof(Color) )] Color colorInnerEnd,
											 [OvldDefault( nameof(Color) )] Color colorOuterEnd,
											 float angleRadStart,
											 float angleRadEnd ) {
			DiscCore( BlendMode, DiscRadiusSpace, RingThicknessSpace, false, true, pos, rot, radius, 0f, colorInnerStart, colorOuterStart, colorInnerEnd, colorOuterEnd, null, angleRadStart, angleRadEnd );
		}

		[OvldGenCallTarget] static void Arc( Vector3 pos,
											 [OvldDefault( "Quaternion.identity" )] Quaternion rot,
											 [OvldDefault( nameof(DiscRadius) )] float radius,
											 [OvldDefault( nameof(RingThickness) )] float thickness,
											 [OvldDefault( nameof(Color) )] Color colorInnerStart,
											 [OvldDefault( nameof(Color) )] Color colorOuterStart,
											 [OvldDefault( nameof(Color) )] Color colorInnerEnd,
											 [OvldDefault( nameof(Color) )] Color colorOuterEnd,
											 float angleRadStart,
											 float angleRadEnd,
											 [OvldDefault( nameof(ArcEndCap) + "." + nameof(ArcEndCap.None) )] ArcEndCap endCaps,
											 [OvldDefault( nameof(RingDashStyle) )] DashStyle dashStyle = null ) {
			DiscCore( BlendMode, DiscRadiusSpace, RingThicknessSpace, true, true, pos, rot, radius, thickness, colorInnerStart, colorOuterStart, colorInnerEnd, colorOuterEnd, dashStyle, angleRadStart, angleRadEnd, endCaps );
		}

		static MpbDisc mpbDisc = new MpbDisc();

		static void DiscCore( ShapesBlendMode blendMode, ThicknessSpace spaceRadius, ThicknessSpace spaceThickness, bool hollow, bool sector, Vector3 pos, Quaternion rot, float radius, float thickness, Color colorInnerStart, Color colorOuterStart, Color colorInnerEnd, Color colorOuterEnd, DashStyle dashStyle = null, float angleRadStart = 0f, float angleRadEnd = 0f, ArcEndCap arcEndCaps = ArcEndCap.None ) {
			if( sector && Mathf.Abs( angleRadEnd - angleRadStart ) < 0.0001f )
				return;

			using( new IMDrawer( mpbDisc, ShapesMaterialUtils.GetDiscMaterial( hollow, sector )[blendMode], ShapesMeshUtils.QuadMesh[0], pos, rot ) ) {
				MetaMpb.ApplyDashSettings( mpbDisc, dashStyle, thickness );
				mpbDisc.radius.Add( radius );
				mpbDisc.radiusSpace.Add( (int)spaceRadius );
				mpbDisc.alignment.Add( (int)Draw.DiscGeometry );
				mpbDisc.thicknessSpace.Add( (int)spaceThickness );
				mpbDisc.thickness.Add( thickness );
				mpbDisc.scaleMode.Add( (int)ScaleMode );
				mpbDisc.angStart.Add( angleRadStart );
				mpbDisc.angEnd.Add( angleRadEnd );
				mpbDisc.roundCaps.Add( (int)arcEndCaps );
				mpbDisc.color.Add( colorInnerStart );
				mpbDisc.colorOuterStart.Add( colorOuterStart );
				mpbDisc.colorInnerEnd.Add( colorInnerEnd );
				mpbDisc.colorOuterEnd.Add( colorOuterEnd );
			}
		}

		static MpbRegularPolygon mpbRegularPolygon = new MpbRegularPolygon();

		[OvldGenCallTarget] static void RegularPolygon( [OvldDefault( nameof(BlendMode) )] ShapesBlendMode blendMode,
														[OvldDefault( nameof(RegularPolygonRadiusSpace) )] ThicknessSpace spaceRadius,
														[OvldDefault( nameof(RegularPolygonThicknessSpace) )] ThicknessSpace spaceThickness,
														Vector3 pos,
														[OvldDefault( "Quaternion.identity" )] Quaternion rot,
														[OvldDefault( nameof(RegularPolygonSideCount) )] int sideCount,
														[OvldDefault( nameof(RegularPolygonRadius) )] float radius,
														[OvldDefault( nameof(RegularPolygonThickness) )] float thickness,
														[OvldDefault( nameof(Color) )] Color color,
														bool hollow,
														[OvldDefault( "0f" )] float roundness,
														[OvldDefault( "0f" )] float angle,
														[OvldDefault( nameof(PolygonShapeFill) )] ShapeFill fill ) {
			using( new IMDrawer( mpbRegularPolygon, ShapesMaterialUtils.matRegularPolygon[blendMode], ShapesMeshUtils.QuadMesh[0], pos, rot ) ) {
				MetaMpb.ApplyColorOrFill( mpbRegularPolygon, fill, color );
				mpbRegularPolygon.radius.Add( radius );
				mpbRegularPolygon.radiusSpace.Add( (int)spaceRadius );
				mpbRegularPolygon.geometry.Add( (int)Draw.RegularPolygonGeometry );
				mpbRegularPolygon.sides.Add( Mathf.Max( 3, sideCount ) );
				mpbRegularPolygon.angle.Add( angle );
				mpbRegularPolygon.roundness.Add( roundness );
				mpbRegularPolygon.hollow.Add( hollow.AsInt() );
				mpbRegularPolygon.thicknessSpace.Add( (int)spaceThickness );
				mpbRegularPolygon.thickness.Add( thickness );
				mpbRegularPolygon.scaleMode.Add( (int)ScaleMode );
			}
		}

		static MpbRectangle mpbRectangle = new MpbRectangle();

		[OvldGenCallTarget] static void Rectangle( [OvldDefault( nameof(BlendMode) )] ShapesBlendMode blendMode,
												   [OvldDefault( "false" )] bool hollow,
												   [OvldDefault( "Vector3.zero" )] Vector3 pos,
												   [OvldDefault( "Quaternion.identity" )] Quaternion rot,
												   Rect rect,
												   [OvldDefault( nameof(Color) )] Color color,
												   [OvldDefault( "0f" )] float thickness = 0f,
												   [OvldDefault( "default" )] Vector4 cornerRadii = default ) {
			bool rounded = ShapesMath.MaxComp( cornerRadii ) >= 0.0001f;

			// positive vibes only
			if( rect.width < 0 ) rect.x -= rect.width *= -1;
			if( rect.height < 0 ) rect.y -= rect.height *= -1;

			if( hollow && thickness * 2 >= Mathf.Min( rect.width, rect.height ) ) hollow = false;

			using( new IMDrawer( mpbRectangle, ShapesMaterialUtils.GetRectMaterial( hollow, rounded )[blendMode], ShapesMeshUtils.QuadMesh[0], pos, rot ) ) {
				mpbRectangle.color.Add( color );
				mpbRectangle.rect.Add( rect.ToVector4() );
				mpbRectangle.cornerRadii.Add( cornerRadii );
				mpbRectangle.thickness.Add( thickness );
				mpbRectangle.scaleMode.Add( (int)ScaleMode );
			}
		}

		static MpbTriangle mpbTriangle = new MpbTriangle();

		[OvldGenCallTarget] static void Triangle( [OvldDefault( nameof(BlendMode) )] ShapesBlendMode blendMode,
												  Vector3 a,
												  Vector3 b,
												  Vector3 c,
												  [OvldDefault( nameof(Color) )] Color colorA,
												  [OvldDefault( nameof(Color) )] Color colorB,
												  [OvldDefault( nameof(Color) )] Color colorC ) {
			using( new IMDrawer( mpbTriangle, ShapesMaterialUtils.matTriangle[blendMode], ShapesMeshUtils.TriangleMesh[0] ) ) {
				mpbTriangle.a.Add( a );
				mpbTriangle.b.Add( b );
				mpbTriangle.c.Add( c );
				mpbTriangle.color.Add( colorA );
				mpbTriangle.colorB.Add( colorB );
				mpbTriangle.colorC.Add( colorC );
			}
		}

		static MpbQuad mpbQuad = new MpbQuad();

		[OvldGenCallTarget] static void Quad( [OvldDefault( nameof(BlendMode) )] ShapesBlendMode blendMode,
											  Vector3 a,
											  Vector3 b,
											  Vector3 c,
											  [OvldDefault( "a + ( c - b )" )] Vector3 d,
											  [OvldDefault( nameof(Color) )] Color colorA,
											  [OvldDefault( nameof(Color) )] Color colorB,
											  [OvldDefault( nameof(Color) )] Color colorC,
											  [OvldDefault( nameof(Color) )] Color colorD ) {
			using( new IMDrawer( mpbQuad, ShapesMaterialUtils.matQuad[blendMode], ShapesMeshUtils.QuadMesh[0] ) ) {
				mpbQuad.a.Add( a );
				mpbQuad.b.Add( b );
				mpbQuad.c.Add( c );
				mpbQuad.d.Add( d );
				mpbQuad.color.Add( colorA );
				mpbQuad.colorB.Add( colorB );
				mpbQuad.colorC.Add( colorC );
				mpbQuad.colorD.Add( colorD );
			}
		}


		static readonly MpbSphere metaMpbSphere = new MpbSphere();

		[OvldGenCallTarget] static void Sphere( [OvldDefault( nameof(BlendMode) )] ShapesBlendMode blendMode,
												[OvldDefault( nameof(SphereRadiusSpace) )] ThicknessSpace spaceRadius,
												Vector3 pos,
												[OvldDefault( nameof(SphereRadius) )] float radius,
												[OvldDefault( nameof(Color) )] Color color ) {
			using( new IMDrawer( metaMpbSphere, ShapesMaterialUtils.matSphere[blendMode], ShapesMeshUtils.SphereMesh[(int)DetailLevel], pos, Quaternion.identity ) ) {
				metaMpbSphere.color.Add( color );
				metaMpbSphere.radius.Add( radius );
				metaMpbSphere.radiusSpace.Add( (float)spaceRadius );
			}
		}

		static readonly MpbCone mpbCone = new MpbCone();

		[OvldGenCallTarget] static void Cone( [OvldDefault( nameof(BlendMode) )] ShapesBlendMode blendMode,
											  [OvldDefault( nameof(ConeSizeSpace) )] ThicknessSpace sizeSpace,
											  Vector3 pos,
											  [OvldDefault( "Quaternion.identity" )] Quaternion rot,
											  float radius,
											  float length,
											  [OvldDefault( "true" )] bool fillCap,
											  [OvldDefault( nameof(Color) )] Color color ) {
			Mesh mesh = fillCap ? ShapesMeshUtils.ConeMesh[(int)DetailLevel] : ShapesMeshUtils.ConeMeshUncapped[(int)DetailLevel];
			using( new IMDrawer( mpbCone, ShapesMaterialUtils.matCone[blendMode], mesh, pos, rot ) ) {
				mpbCone.color.Add( color );
				mpbCone.radius.Add( radius );
				mpbCone.length.Add( length );
				mpbCone.sizeSpace.Add( (float)sizeSpace );
			}
		}

		static readonly MpbCuboid mpbCuboid = new MpbCuboid();

		[OvldGenCallTarget] static void Cuboid( [OvldDefault( nameof(BlendMode) )] ShapesBlendMode blendMode,
												[OvldDefault( nameof(CuboidSizeSpace) )] ThicknessSpace sizeSpace,
												Vector3 pos,
												[OvldDefault( "Quaternion.identity" )] Quaternion rot,
												Vector3 size,
												[OvldDefault( nameof(Color) )] Color color ) {
			using( new IMDrawer( mpbCuboid, ShapesMaterialUtils.matCuboid[blendMode], ShapesMeshUtils.CuboidMesh[0], pos, rot ) ) {
				mpbCuboid.color.Add( color );
				mpbCuboid.size.Add( size );
				mpbCuboid.sizeSpace.Add( (float)sizeSpace );
			}
		}

		static MpbTorus mpbTorus = new MpbTorus();

		[OvldGenCallTarget] static void Torus( [OvldDefault( nameof(BlendMode) )] ShapesBlendMode blendMode,
											   [OvldDefault( nameof(TorusRadiusSpace) )] ThicknessSpace spaceRadius,
											   [OvldDefault( nameof(TorusThicknessSpace) )] ThicknessSpace spaceThickness,
											   Vector3 pos,
											   [OvldDefault( "Quaternion.identity" )] Quaternion rot,
											   float radius,
											   float thickness,
											   [OvldDefault( nameof(Color) )] Color color ) {
			if( thickness < 0.0001f )
				return;
			if( radius < 0.00001f ) {
				Sphere( blendMode, spaceThickness, pos, thickness, color ); // todo: thickness/2 ?
				return;
			}

			using( new IMDrawer( mpbTorus, ShapesMaterialUtils.matTorus[blendMode], ShapesMeshUtils.TorusMesh[(int)DetailLevel], pos, rot ) ) {
				mpbTorus.color.Add( color );
				mpbTorus.radius.Add( radius );
				mpbTorus.thickness.Add( thickness );
				mpbTorus.spaceRadius.Add( (int)spaceRadius );
				mpbTorus.spaceThickness.Add( (int)spaceThickness );
				mpbTorus.scaleMode.Add( (int)ScaleMode );
			}
		}

		static MpbText mpbText = new MpbText();

		[OvldGenCallTarget] static void Text( Vector3 pos,
											  [OvldDefault( "Quaternion.identity" )] Quaternion rot,
											  string content,
											  [OvldDefault( nameof(Font) )] TMP_FontAsset font,
											  [OvldDefault( nameof(FontSize) )] float fontSize,
											  [OvldDefault( nameof(TextAlign) )] TextAlign align,
											  [OvldDefault( nameof(Color) )] Color color ) {
			TextMeshPro tmp = ShapesTextDrawer.Instance.tmp;
			// Statics
			tmp.font = font;
			tmp.color = color;
			tmp.fontSize = fontSize;

			// Per-instance
			tmp.text = content;
			tmp.alignment = align.GetTMPAlignment();
			tmp.rectTransform.pivot = align.GetPivot();
			tmp.transform.position = pos;
			tmp.rectTransform.rotation = rot;
			tmp.ForceMeshUpdate();

			using( new IMDrawer( mpbText, font.material, tmp.mesh, tmp.transform.position, tmp.transform.rotation, cachedTMP: true ) ) {
				// will draw on dispose
			}
		}


	}

	// these are used by CodegenDrawOverloads
	[AttributeUsage( AttributeTargets.Method )]
	public class OvldGenCallTarget : Attribute {
	}

	[AttributeUsage( AttributeTargets.Parameter )]
	public class OvldDefault : Attribute {
		public string @default;
		public OvldDefault( string @default ) => this.@default = @default;
	}

}