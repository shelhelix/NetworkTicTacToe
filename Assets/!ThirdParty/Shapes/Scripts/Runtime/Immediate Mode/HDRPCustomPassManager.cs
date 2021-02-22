#if SHAPES_HDRP
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
#endif

// Shapes © Freya Holmér - https://twitter.com/FreyaHolmer/
// Website & Documentation - https://acegikmo.com/shapes/
namespace Shapes {

	#if SHAPES_HDRP
	[ExecuteAlways]
	public class HDRPCustomPassManager : MonoBehaviour {

		static HDRPCustomPassManager instance;
		public static HDRPCustomPassManager Instance {
			get {
				if( instance == null ) {
					// no cached one, see if we can find it in the scene
					if( ( instance = FindObjectOfType<HDRPCustomPassManager>() ) == null ) {
						// nothing in the scene, create it
						GameObject go = new GameObject( "Shapes HDRP Manager" ) { hideFlags = HideFlags.DontSave };
						instance = go.AddComponent<HDRPCustomPassManager>();
					}
				}

				return instance;
			}
		}

		void OnEnable() => instance = this;
		void Awake() => instance = this;

		Dictionary<CustomPassInjectionPoint, CustomPassVolume> volumes = new Dictionary<CustomPassInjectionPoint, CustomPassVolume>();

		public void MakeSureVolumeExistsForInjectionPoint( CustomPassInjectionPoint injPt ) {
			if( volumes.TryGetValue( injPt, out CustomPassVolume volume ) == false ) {
				// not found in the dictionary - see if there is one on this object
				if( ( volume = GetComponents<CustomPassVolume>().FirstOrDefault( v => v.injectionPoint == injPt ) ) == null ) {
					// not found on this object, create it
					volume = gameObject.AddComponent<CustomPassVolume>();
					volume.injectionPoint = injPt;
					volume.hideFlags = HideFlags.DontSave; // don't serialize this pls
					volume.runInEditMode = true;
					volumes[injPt] = volume;
					volume.AddPassOfType( typeof(ShapesRenderPass) ); // this pass branches internally
				}
			}
		}

	}
	#endif

}