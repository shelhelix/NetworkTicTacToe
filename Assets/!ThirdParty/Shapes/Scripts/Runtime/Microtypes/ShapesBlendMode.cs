// Shapes © Freya Holmér - https://twitter.com/FreyaHolmer/
// Website & Documentation - https://acegikmo.com/shapes/

using System.ComponentModel;

namespace Shapes {

	public enum ShapesBlendMode {
		[Description( "Opaque" )] Opaque = 0,
		[Description( "Transparent_" )] Transparent = 1,
		[Description( "Linear Dodge (Additive)" )] Additive = 2,
		[Description( "Color Dodge" )] ColorDodge = 9,
		[Description( "Screen" )] Screen = 4,
		[Description( "Lighten_" )] Lighten = 7,
		[Description( "Linear Burn" )] LinearBurn = 6,
		[Description( "Color Burn" )] ColorBurn = 10,
		[Description( "Multiply" )] Multiplicative = 3,
		[Description( "Darken_" )] Darken = 8,
		[Description( "Subtract" )] Subtractive = 5,
	}

}