/**
 * @file DistortionQuad.shader
 *
 * @author Geoffrey Marhuenda
 *
 * @brief Create distortion on the shader base on strength and speed.
 */
Shader "Lynx/Grab"
{
    Properties
    {
        _Strength("Distortion strength", Range(0.0, 0.5)) = 0.0
        _Speed("Speed", Range(0.0, 1000.0)) = 1.0
        _Noise("Noise Texture", 2D) = "white" {}
        _StrengthFilter("Strength Filter", 2D) = "white" {}


    }
    SubShader
    {
        // Draw after all opaque geometry
        Tags { "Queue" = "Transparent+10" }
        //Blend One Zero, Zero One

        // Grab the screen behind the object into _BackgroundTexture
        GrabPass
        {
            "_BackgroundTexture"
        }

        // Render the object with the texture generated above, and invert the colors
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata_base v) {
                v2f o;
                // use UnityObjectToClipPos from UnityCG.cginc to calculate 
                // the clip-space of the vertex
                o.pos = UnityObjectToClipPos(v.vertex);

                // use ComputeGrabScreenPos function from UnityCG.cginc
                // to get the correct texture coordinate
                o.grabPos = ComputeGrabScreenPos(o.pos);
                return o;
            }

            sampler2D _BackgroundTexture;

            half4 frag(v2f i) : SV_Target
            {
                half4 bgcolor = tex2Dproj(_BackgroundTexture, i.grabPos);
                return 1 - bgcolor;
            }
            ENDCG
        }

    }
}

