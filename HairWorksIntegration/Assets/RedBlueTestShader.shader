Shader "Custom/RedBlueTestShader"{
	Properties{
	}

	SubShader{
		Pass{
		CGPROGRAM
	#pragma vertex vert_img
	#pragma fragment frag

	#include "UnityCG.cginc"

	/// which eye we are rendering. 0 == left, 1 == right
	uniform int RenderingEye;


	float4 frag(v2f_img i) : COLOR
	{
		return RenderingEye == 0 ? float4(1,0,0,1) : float4(0,0,1,1);
	}

		ENDCG
	}
	}
}


