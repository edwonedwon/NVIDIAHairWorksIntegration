using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public struct hwHShader
{
    public static hwHShader NullHandle = new hwHShader(0xFFFFFFFF);

    public uint id;

    public hwHShader(uint v) { this.id = v; }
    public static implicit operator hwHShader(uint v) { return new hwHShader(v); }
    public static implicit operator uint (hwHShader v) { return v.id; }
    public static implicit operator bool (hwHShader v) { return v.id != 0xFFFFFFFF; }
}

[System.Serializable]
public struct hwHAsset
{
    public static hwHAsset NullHandle = new hwHAsset(0xFFFFFFFF);

    public uint id;

    public hwHAsset(uint v) { this.id = v; }
    public static implicit operator hwHAsset(uint v) { return new hwHAsset(v); }
    public static implicit operator uint (hwHAsset v) { return v.id; }
    public static implicit operator bool (hwHAsset v) { return v.id != 0xFFFFFFFF; }
}

[System.Serializable]
public struct hwHInstance
{
    public static hwHInstance NullHandle = new hwHInstance(0xFFFFFFFF);

    public uint id;

    public hwHInstance(uint v) { this.id = v; }
    public static implicit operator hwHInstance(uint v) { return new hwHInstance(v); }
    public static implicit operator uint (hwHInstance v) { return v.id; }
    public static implicit operator bool (hwHInstance v) { return v.id != 0xFFFFFFFF; }
}

// C# では bool は 1 byte なのに marshaling の際は 4 byte のデータに変換される。
// (WinAPI の BOOL = 4 byte への変換を意図しているためらしい)
// 一方 C++ の bool は、標準はサイズを規定していないものの、大抵 1 byte であり、VisualC++ でもそうなっている。
// この違いを埋めるため、bool は [MarshalAs(UnmanagedType.I1)] で 1 byte データとして marshaling させる必要がある。
// これを怠ると、bool が連続するフィールドがある struct は C# 側と C++ 側で構造がズレることになる。
// ( http://stackoverflow.com/questions/9766403/structlayout-pack-1-doesnt-work-with-bool via @Tan90909090 )
[System.Serializable]
public unsafe struct hwDescriptor
{
    // global controls
    [MarshalAs(UnmanagedType.I1)] public bool m_enable;                    //!< [true/false] whether to enable this hair. When disabled, hair will not cause any computation/rendering

    // hair geometry (density/width/length/clump) controls
	[Header("Width")]
	[Tooltip("hair width (thickness) [In Millimeters] ")]
    public float m_width;                  //!< [In Millimeters] hair width (thickness)
	[Tooltip(" [0 - 1.0] noise factor for hair width noise  ")]
	public float m_widthNoise;             //!< [0 - 1.0] noise factor for hair width noise 
	[Tooltip(" [0 - 1.0] scale factor for top side of the strand ")]
	public float m_widthRootScale;
	[Tooltip(" [0 - 1.0] scale factor for bottom side of the strand")]
	public float m_widthTipScale;

	[Header("Clump")]
	[Tooltip(" [0 - 1.0] probability of each hair gets clumped (0 = all hairs get clumped, 1 = clump scale is randomly distributed from 0 to 1)")]
	public float m_clumpNoise;
	[Tooltip(" [0 - 2.0] exponential factor to control roundness of clump shape  = 0 = linear cone, clump scale *= power(t, roundness; where t is normalized distance from the root)")]
	public float m_clumpRoundness;
	[Tooltip(" [0 - 1.0] how clumped each hair face is")]
	public float m_clumpScale;

	[Header("Density")]
	[Tooltip(" [0 -    ] hair density per face (1.0 = 64 hairs per face)")]
	public float m_density;
	[Tooltip(" [true/false] whether to use per-pixel sampling or per-vertex sampling for density map")]
	[MarshalAs(UnmanagedType.I1)] public bool m_usePixelDensity;

	[Header("Length")]
	[Tooltip(" [0 - 1.0] length variation noise")]
	public float m_lengthNoise;
	[Tooltip(" [0 - 1.0] length control for growing hair effect")]
	public float m_lengthScale;

	[Header("Wave")]
	[Tooltip(" [In Centimeters] size of waves for hair waviness ")]
	public float m_waveScale;
	[Tooltip(" [0 - 1.0] noise factor for the wave scale")]
	public float m_waveScaleNoise;
	[Tooltip("[0 - 1.0] waviness at clump level")]
	public float m_waveScaleClump;
	[Tooltip(" [0 - 1.0] waviness at strand level")]
	public float m_waveScaleStrand;
	[Tooltip(" [0 -    ] wave frequency (1.0 = one sine wave along hair length)")]
	public float m_waveFreq;
	[Tooltip(" [0 - 1.0] noise factor for the wave frequency ")]
	public float m_waveFreqNoise;
	[Tooltip(" [0 - 1.0] For some distance from the root, we atteunate waviness so that root itself does not move [0-1]")]
	public float m_waveRootStraighten;  

	/// shading controls
	[Header("Render Color")]
	[Tooltip(" [0 - 1.0] falloff factor for alpha transition from root ")]
	public float m_rootAlphaFalloff;
	[Tooltip(" [0 - 1.0] color of hair root (when hair textures are not used)")]
	public Color m_rootColor;
	[Tooltip(" [0 - 1.0] color of hair tip (when hair textures are not used)")]
	public Color m_tipColor;
	[Tooltip(" [0 - 1.0] blend factor between root and tip color in addition to hair length")]
	public float m_rootTipColorWeight;
	[Tooltip(" [0 - 1.0] falloff factor for root/tip color interpolation")]
	public float m_rootTipColorFalloff;      

	[Header("Render Diffuse")]
	[Tooltip(" [0 - 1.0] blend factor between Kajiya hair lighting vs normal skin lighting.")]
	public float m_diffuseBlend;
	[Tooltip(" [0 - 1.0] blend factor between mesh normal vs hair normal. Use higher value for longer (surface like) hair.")]
	public float m_hairNormalWeight;
	[Tooltip(" [0 - number of bones] index for the bone which we use as model center for diffuse shading purpose")]
	public int m_hairNormalBoneIndex;       

	[Header("Render Specular")]
	[Tooltip(" [0 - 1.0] specular color")]
	public Color m_specularColor;
	[Tooltip(" [0 - 1.0] amount of specular noise")]
	public float m_specularNoiseScale;
	[Tooltip(" [0 - 1.0] amount of specular scale from env probe")]
	public float m_specularEnvScale;
	[Tooltip(" [0 - 1.0] primary specular factor")]
	public float m_specularPrimary;
	[Tooltip(" [0 - ] primary specular power exponent")]
	public float m_specularPowerPrimary;
	[Tooltip(" [0 - 1.0] shift factor to make specular highlight move with noise")]
	public float m_specularPrimaryBreakup;
	[Tooltip(" [0 - 1.0] secondary specular factor")]
	public float m_specularSecondary;
	[Tooltip(" [0 - 1.0] secondary highlight shift offset along tangents")]
	public float m_specularSecondaryOffset;
	[Tooltip(" [0 - ] secondary specular power exponent		")]
	public float m_specularPowerSecondary;
	[Tooltip(" [0 - 1.0] strength of the glint noise")]
	public float m_glintStrength;
	[Tooltip(" [0 - 1024] number of glint sparklets along each hair")]
	public float m_glintCount;
	[Tooltip(" [0 - ] glint power exponent")]
	public float m_glintExponent;        

	[Header("Shadows")]
	[Tooltip(" [true/false] this hair cast shadows onto the scene")]
	[MarshalAs(UnmanagedType.I1)] public bool m_castShadows;
	[Tooltip(" [true/false] this hair receives shadows from the scene")]
	[MarshalAs(UnmanagedType.I1)] public bool m_receiveShadows;
	[Tooltip(" [In Centimeters] distance through hair volume beyond which hairs get completely shadowed.")]
	public float m_shadowSigma;             

	[Header("Blending")]
	[Tooltip(" [GFSDK_HAIR_STRAND_BLEND_MODE] blend mode when strand texture is used. Supported mode are defined in GFSDK_HAIR_STRAND_BLEND_MODE.")]
	public int m_strandBlendMode;
	[Tooltip(" [0 - 1.0] scale strand texture before blend")]
	public float m_strandBlendScale;          

	// simulation control
	[Header("Simulation")]
	[Tooltip(" [0 - 1.0] radius of backstop collision (normalized along hair length)")]
	public float m_backStopRadius;
	[Tooltip(" [0 - 1.0] stiffness for bending, useful for long hair")]
	public float m_bendStiffness;
	[Tooltip(" [0 - ] damping to slow down hair motion")]
	public float m_damping;
	[Tooltip(" [0 - 1.0] gravity force direction (unit vector)")]
	public Vector3 m_gravityDir;
	[Tooltip(" [0 - 1.0] friction when capsule collision is used")]
	public float m_friction;
	[Tooltip(" [In Meters] mass scale for this hair")]
	public float m_massScale;
	[Tooltip(" [0 - 1.0] inertia control. (0: no inertia, 1: full intertia)")]
	public float m_inertiaScale;
	[Tooltip(" [In Meters] speed limit where everything gets locked (for teleport etc.)")]
	public float m_inertiaLimit;               

	[Header("Simulation Stiffness")]
	[Tooltip(" [0 - 1.0] how strong the hair interaction force is")]
	public float m_interactionStiffness;
	[Tooltip(" [0 - 1.0] attenuation of stiffness away from the root (stiffer at root, weaker toward tip)")]
	public float m_rootStiffness;
	[Tooltip(" [0 - 1.0] stiffness for pin constraints")]
	public float m_pinStiffness;
	[Tooltip(" [true/false] whether to turn on/off simulation")]
	[MarshalAs(UnmanagedType.I1)] public bool m_simulate;
	[Tooltip(" [0 - 1.0] how close hairs try to stay within skinned position")]
	public float m_stiffness;
	[Tooltip(" [0 - 1.0] how strongly hairs move toward the stiffness target")]
	public float m_stiffnessStrength;
	[Tooltip(" [0 - 1.0] how fast hair stiffness gerneated motion decays over time")]
	public float m_stiffnessDamping;
	[Tooltip(" [0 - 1.0] attenuation of stiffness away from the tip (stiffer at tip, weaker toward root)")]
	public float m_tipStiffness;
	[Tooltip(" [true/false] whether to use the sphere/capsule collision or not for hair/body collision")]
	[MarshalAs(UnmanagedType.I1)] public bool m_useCollision;
	[Tooltip(" [In Meters] vector force for main wind direction")]
	public Vector3 m_wind;
	[Tooltip(" [0 - 1.0] strength of wind noise")]
	public float m_windNoise;             

	public Vector4 m_stiffnessCurve;          //! [0 - 1.0] curve values for stiffness 
    public Vector4 m_stiffnessStrengthCurve;  //! [0 - 1.0] curve values for stiffness strength
    public Vector4 m_stiffnessDampingCurve;   //! [0 - 1.0] curve values for stiffness damping
    public Vector4 m_bendStiffnessCurve;      //! [0 - 1.0] curve values for bend stiffness
    public Vector4 m_interactionStiffnessCurve;//! [0 - 1.0] curve values for interaction stiffness

    // lod controls
	[Header("LOD")]
	[Tooltip(" [true/false] whether to enable/disable entire lod scheme")]
	[MarshalAs(UnmanagedType.I1)] public bool m_enableLOD;
	[Tooltip(" [true/false] whether to enable lod for far away object (distance LOD)")]
	[MarshalAs(UnmanagedType.I1)] public bool m_enableDistanceLOD;
	[Tooltip(" [In Meters] distance (in scene unit) to camera where fur will start fading out (by reducing density)")]
	public float m_distanceLODStart;
	[Tooltip(" [In Meters] distance (in scene unit) to camera where fur will completely disappear (and stop simulating)")]
	public float m_distanceLODEnd;
	[Tooltip(" [In Meters] distance (in scene unit) to camera where fur will fade with alpha from 1 (this distance) to 0 (DistanceLODEnd)")]
	public float m_distanceLODFadeStart;
	[Tooltip(" [0 - ] density when distance LOD is in action.  hairDensity gets scaled based on LOD factor.")]
	public float m_distanceLODDensity;
	[Tooltip(" [In Millimeters] hair width that can change when close up density is triggered by closeup lod mechanism")]
	public float m_distanceLODWidth;

	[Tooltip(" [true/false] whether to enable lod for close object (detail LOD)")]
	[MarshalAs(UnmanagedType.I1)] public bool m_enableDetailLOD;
	[Tooltip(" [In Meters] distance (in scene unit) to camera where fur will start getting denser toward closeup density")]
	public float m_detailLODStart;
	[Tooltip(" [In Meters] distance (in scene unit) to camera where fur will get full closeup density value")]
	public float m_detailLODEnd;
	[Tooltip(" [0 - ] density scale when closeup LOD is in action.  hairDensity gets scaled based on LOD factor.")]
	public float m_detailLODDensity;
	[Tooltip(" [In Millimeters] hair width that can change when close up density is triggered by closeup lod mechanism")]
	public float m_detailLODWidth;

	[Tooltip(" [0 - 1] density scale factor to reduce hair density for shadow map rendering")]
	public float m_shadowDensityScale;     

	[Header("Culling")]
	[Tooltip(" [true/false] when this is on, density for hairs outside view are set to 0. Use this option when fur is in a closeup.")]
	[MarshalAs(UnmanagedType.I1)] public bool m_useViewfrustrumCulling;
	[Tooltip(" [true/false] when this is on, density for hairs growing from backfacing faces will be set to 0")]
	[MarshalAs(UnmanagedType.I1)] public bool m_useBackfaceCulling;
	[Tooltip(" [-1 - 1.0] threshold to determine backface, note that this value should be slightly smaller 0 to avoid hairs at the silhouette from disappearing")]
	public float m_backfaceCullingThreshold;

	[Tooltip(" [true/false] when this is on, hairs get culled when their root points are inside the sphere")]
	[MarshalAs(UnmanagedType.I1)] public bool m_useCullSphere;
	[Tooltip(" inverse of general affine transform (scale, rotation, translation..) applied to a unit sphere centered at origin")]
	public Matrix4x4 m_cullSphereInvTransform;
	[Tooltip(" how many vertices are generated per each control hair segments in spline curves")]
	public int m_splineMultiplier;      

	// drawing option
	[Header("Visualize Options")]
	[Tooltip(" [true/false] draw render hair")]
	[MarshalAs(UnmanagedType.I1)] public bool m_drawRenderHairs;
	[Tooltip(" [true/false] visualize skinning bones")]
	[MarshalAs(UnmanagedType.I1)] public bool m_visualizeBones;
	[Tooltip(" [true/false] draw bounding box of hairs")]
	[MarshalAs(UnmanagedType.I1)] public bool m_visualizeBoundingBox;
	[Tooltip(" [true/false] visualize collision capsules")]
	[MarshalAs(UnmanagedType.I1)] public bool m_visualizeCapsules;
	[Tooltip(" [true/false] draw control vertices of guide hairs")]
	[MarshalAs(UnmanagedType.I1)] public bool m_visualizeControlVertices;
	[Tooltip(" [true/false] draw cull sphere")]
	[MarshalAs(UnmanagedType.I1)] public bool m_visualizeCullSphere;
	[Tooltip(" [true/false] visualize coordinate frames")]
	[MarshalAs(UnmanagedType.I1)] public bool m_visualizeFrames;
	[Tooltip(" [true/false] draw growth mesh")]
	[MarshalAs(UnmanagedType.I1)] public bool m_visualizeGrowthMesh;
	[Tooltip(" [true/false] draw guide hairs")]
	[MarshalAs(UnmanagedType.I1)] public bool m_visualizeGuideHairs;
	[Tooltip(" [true/false] draw hair interaction lines")]
	[MarshalAs(UnmanagedType.I1)] public bool m_visualizeHairInteractions;
	[Tooltip(" [0 - ] for per hair visualization, how many hairs to skip?")]
	public int m_visualizeHairSkips;
	[Tooltip(" [true/false] visualize target pose for bending")]
	[MarshalAs(UnmanagedType.I1)] public bool m_visualizeLocalPos;
	[Tooltip(" [true/false] whether to visualize pin constraint spheres")]
	[MarshalAs(UnmanagedType.I1)] public bool m_visualizePinConstraints;
	[Tooltip(" [true/false] visualize normals used for hair shading")]
	[MarshalAs(UnmanagedType.I1)] public bool m_visualizeShadingNormals;
	[Tooltip(" [true/false] visualize bone used as shading normal center")]
	[MarshalAs(UnmanagedType.I1)] public bool m_visualizeShadingNormalBone;
	[Tooltip(" [true/false] draw skinned positions for guide hairs")]
	[MarshalAs(UnmanagedType.I1)] public bool m_visualizeSkinnedGuideHairs; 

	[Header("Colorize Mode")]
	[Tooltip(" [GFSDK_HAIR_COLORIZE_MODE] colorize hair based on various terms. See GFSDK_HAIR_COLORIZE_MODE.")]
	public int m_colorizeMode;

	// texture control
	[Tooltip(" texture chanel for each control textures.  ")]
	public fixed int m_textureChannels[(int)hwTextureType.NUM_TEXTURES]; 

	// model to world transform
	[Tooltip("render time transformation to offset hair from its simulated position ")]
	public Matrix4x4 m_modelToWorld;             


    public static hwDescriptor default_value
    {
        get
        {
            hwDescriptor ret = new hwDescriptor();
            ret.initialize();
            return ret;
        }
    }

    public void initialize()
    {
        m_enable = true;

        // default geometry parameters
        m_width = 1.0f;
        m_widthNoise = 0.0f;
        m_widthRootScale = 1.0f;
        m_widthTipScale = 0.1f;

        m_clumpNoise = 0.0f;
        m_clumpRoundness = 1.0f;
        m_clumpScale = 0.0f;

        m_density = 1.0f;
        m_lengthNoise = 1.0f;
        m_lengthScale = 1.0f;

        m_usePixelDensity = false;
        m_waveScale = 0.0f;
        m_waveScaleClump = 0.0f;
        m_waveScaleStrand = 1.0f;
        m_waveScaleNoise = 0.5f;
        m_waveFreq = 3.0f;
        m_waveFreqNoise = 0.5f;
        m_waveRootStraighten = 0.0f;

        // default shading parameters
        m_diffuseBlend = 0.5f;
        m_hairNormalWeight = 0.0f;
        m_hairNormalBoneIndex = -1;

        m_glintStrength = 0.0f;
        m_glintCount = 256.0f;
        m_glintExponent = 2.0f;
        m_rootTipColorWeight = 0.5f;
        m_rootTipColorFalloff = 1.0f;

        m_specularNoiseScale = 0.0f;
        m_specularEnvScale = 0.25f;
        m_specularPrimary = 0.1f;
        m_specularPrimaryBreakup = 0.0f;
        m_specularSecondary = 0.05f;
        m_specularSecondaryOffset = 0.1f;
        m_specularPowerPrimary = 100.0f;
        m_specularPowerSecondary = 20.0f;

        m_rootAlphaFalloff = 0.0f;

        m_shadowSigma = 0.2f;
        //m_castShadows = true;
        //m_receiveShadows = true;

        m_strandBlendMode = 0;
        m_strandBlendScale = 1.0f;

        // default simulation parameters
        m_simulate = true;
        m_backStopRadius = 0.0f;
        m_bendStiffness = 0.0f;
        m_damping = 0.0f;
        m_friction = 0.0f;
        m_inertiaScale = 1.0f;
        m_inertiaLimit = 1000.0f;
        m_interactionStiffness = 0.0f;
        m_massScale = 10.0f;
        m_windNoise = 0.0f;
        m_stiffness = 0.5f;
        m_stiffnessStrength = 1.0f;
        m_stiffnessDamping = 0.0f;
        m_rootStiffness = 0.5f;
        m_pinStiffness = 1.0f;
        m_tipStiffness = 0.0f;
        m_useCollision = false;

        // default LOD parameters
        m_enableLOD = false;

        m_enableDistanceLOD = true;
        m_distanceLODStart = 5.0f; 
        m_distanceLODEnd = 10.0f; 
        m_distanceLODFadeStart = 1000.0f;
        m_distanceLODDensity = 0.0f;
        m_distanceLODWidth = 1.0f;

        m_enableDetailLOD = true;
        m_detailLODStart = 2.0f; 
        m_detailLODEnd = 1.0f; 
        m_detailLODDensity = 1.0f;
        m_detailLODWidth = 1.0f;

        m_shadowDensityScale = 0.5f;

        m_useViewfrustrumCulling = true;
        m_useBackfaceCulling = false;
        m_backfaceCullingThreshold = -0.2f;
        m_useCullSphere = false;

        m_splineMultiplier = 4;

        // visualization options
        m_drawRenderHairs = true;		
        m_visualizeBones = false;
        m_visualizePinConstraints = false;
        m_visualizeCapsules = false;
        m_visualizeGrowthMesh = false;	
        m_visualizeGuideHairs = false;	
        m_visualizeControlVertices = false;
        m_visualizeBoundingBox = false;
        m_visualizeCullSphere = false;
        m_visualizeShadingNormalBone = false;
        m_visualizeHairInteractions = false;
        m_visualizeSkinnedGuideHairs = false;	
        m_visualizeFrames = false;
        m_visualizeLocalPos = false;
        m_visualizeShadingNormals = false;
        m_visualizeHairSkips = 0;

        m_colorizeMode = 0;

        {
            m_gravityDir.x = 0.0f;
            m_gravityDir.y = 0.0f;
            m_gravityDir.z = -1.0f;

            m_wind.x = 0.0f;
            m_wind.y = 0.0f;
            m_wind.z = 0.0f;

            m_rootColor.r = 1.0f; m_rootColor.g = 1.0f; m_rootColor.b = 1.0f; m_rootColor.a = 1.0f;
            m_tipColor.r = 1.0f; m_tipColor.g = 1.0f; m_tipColor.b = 1.0f; m_tipColor.a = 1.0f;
            m_specularColor.r = 1.0f; m_specularColor.g = 1.0f; m_specularColor.b = 1.0f; m_specularColor.a = 1.0f;

            m_modelToWorld = Matrix4x4.identity;
            m_cullSphereInvTransform = Matrix4x4.identity;

            m_stiffnessCurve = Vector4.one;
            m_stiffnessStrengthCurve = Vector4.one;
            m_stiffnessDampingCurve = Vector4.one;
            m_bendStiffnessCurve = Vector4.one;
            m_interactionStiffnessCurve = Vector4.one;

            fixed(int *p = m_textureChannels)
            {
                for (int i = 0; i < (int)hwTextureType.NUM_TEXTURES; i++)
                {
                    p[i] = 0;
                }
            }

        }
    }
}

[System.Serializable]
public struct hwConversionSettings
{
    public hwUpAxis up_axis;
    public hwHandedness handedness;
    IntPtr matrix_ptr;
    public float unit;


    public static hwConversionSettings default_value
    {
        get
        {
            hwConversionSettings ret = new hwConversionSettings();
            ret.initialize();
            return ret;
        }
    }

    void initialize()
    {
        up_axis = hwUpAxis.YUp;
        handedness = hwHandedness.RightHanded;
        unit = 0.0f;
    }
}

[System.Serializable]
public struct hwLightData
{
    public const int MaxLights = 8;

    public int type;
    int pad0, pad2, pad3;
    public Vector3 position;
    public float range;
    public Vector3 direction;
    float pad4;
    public Color color;
	public int angle;
	int pad5, pad6, pad7;
}


public enum hwUpAxis
{
    Unknown,
    YUp,
    ZUp,
}

public enum hwHandedness
{
    Unknown,
    RightHanded,
    LeftHanded,
}

public enum hwTextureType
{
    DENSITY, //<! hair density map [ shape control ]
    ROOT_COLOR, //<! color at the hair root [ shading ]
    TIP_COLOR, //<! color at the hair tip [ shading ]
    WIDTH,  //<! width  [ shape control ]
    STIFFNESS,  //<! stiffness control [ simulation ]
    ROOT_STIFFNESS, //<! stiffness control for root stiffness [simulation]
    CLUMP_SCALE,  //<! clumpiness control [ shape control]
    CLUMP_ROUNDNESS, //<! clumping noise [ shape control]
    WAVE_SCALE, //<! waviness scale [ shape control ]
    WAVE_FREQ, //<! waviness frequency [ shape control ]
    STRAND, //<! texture along hair strand [ shading ]
    LENGTH, //<! length control [shape control] 
    SPECULAR, //<! specularity control [shading ] 
    WEIGHTS, //!< weight texture for multiple material blending [control for all other textures]

    NUM_TEXTURES
};

[System.Serializable]
public struct hwDQuaternion
{
    public Quaternion q1;
    public Quaternion q2;

    public hwDQuaternion(Quaternion q, Vector3 t)
    {
        q1 = q;
        q2 = new Quaternion(
            0.5f * ( t.x * q.w + t.y * q.z - t.z * q.y),
            0.5f * (-t.x * q.z + t.y * q.w + t.z * q.x),
            0.5f * ( t.x * q.y - t.y * q.x + t.z * q.w),
            0.5f * (-t.x * q.x - t.y * q.y - t.z * q.z) );
    }

    static public hwDQuaternion identity { get { return new hwDQuaternion(Quaternion.identity, Vector3.zero); } }
}


public static class HairWorksIntegration
{
    public delegate void hwLogCallback(System.IntPtr cstr);
    [DllImport ("HairWorksIntegration")] public static extern IntPtr        hwGetRenderEventFunc();
    [DllImport ("HairWorksIntegration")] public static extern void          hwSetLogCallback(hwLogCallback cb);

    [DllImport ("HairWorksIntegration")] public static extern hwHShader     hwShaderLoadFromFile(string path);
    [DllImport ("HairWorksIntegration")] public static extern bool          hwShaderRelease(hwHShader sid);
    [DllImport ("HairWorksIntegration")] public static extern bool          hwShaderReload(hwHShader sid);

    [DllImport ("HairWorksIntegration")] public static extern hwHAsset      hwAssetLoadFromFile(string path);
    [DllImport ("HairWorksIntegration")] public static extern bool          hwAssetRelease(hwHAsset aid);
    [DllImport ("HairWorksIntegration")] public static extern bool          hwAssetReload(hwHAsset aid);
    [DllImport ("HairWorksIntegration")] public static extern int           hwAssetGetNumBones(hwHAsset aid);
    [DllImport ("HairWorksIntegration")] private static extern IntPtr       hwAssetGetBoneName(hwHAsset aid, int nth);
    public static string hwAssetGetBoneNameString(hwHAsset aid, int nth) { return Marshal.PtrToStringAnsi(hwAssetGetBoneName(aid, nth)); }

    [DllImport ("HairWorksIntegration")] public static extern void          hwAssetGetBoneIndices(hwHAsset aid, ref Vector4 o_indices);
    [DllImport ("HairWorksIntegration")] public static extern void          hwAssetGetBoneWeights(hwHAsset aid, ref Vector4 o_weight);
    [DllImport ("HairWorksIntegration")] public static extern void          hwAssetGetBindPose(hwHAsset aid, int nth, ref Matrix4x4 o_bindpose);

    [DllImport ("HairWorksIntegration")] public static extern void          hwAssetGetDefaultDescriptor(hwHAsset aid, ref hwDescriptor o_desc);


    [DllImport ("HairWorksIntegration")] public static extern hwHInstance   hwInstanceCreate(hwHAsset aid);
    [DllImport ("HairWorksIntegration")] public static extern bool          hwInstanceRelease(hwHInstance iid);
    [DllImport ("HairWorksIntegration")] public static extern void          hwInstanceGetBounds(hwHInstance iid, ref Vector3 o_min, ref Vector3 o_max);
    [DllImport ("HairWorksIntegration")] public static extern void          hwInstanceGetDescriptor(hwHInstance iid, ref hwDescriptor desc);
    [DllImport ("HairWorksIntegration")] public static extern void          hwInstanceSetDescriptor(hwHInstance iid, ref hwDescriptor desc);
    [DllImport ("HairWorksIntegration")] public static extern void          hwInstanceSetTexture(hwHInstance iid, hwTextureType type, IntPtr tex);
    [DllImport ("HairWorksIntegration")] public static extern void          hwInstanceUpdateSkinningMatrices(hwHInstance iid, int num_bones, IntPtr matrices);
    [DllImport ("HairWorksIntegration")] public static extern void          hwInstanceUpdateSkinningDQs(hwHInstance iid, int num_bones, IntPtr dqs);

    [DllImport ("HairWorksIntegration")] public static extern void          hwBeginScene();
    [DllImport ("HairWorksIntegration")] public static extern void          hwEndScene();
    [DllImport ("HairWorksIntegration")] public static extern void          hwSetViewProjection(ref Matrix4x4 view, ref Matrix4x4 proj, float fov);
    [DllImport ("HairWorksIntegration")] public static extern void          hwSetRenderTarget(IntPtr framebuffer, IntPtr depthbuffer);
    [DllImport ("HairWorksIntegration")] public static extern void          hwSetShader(hwHShader sid);
    [DllImport ("HairWorksIntegration")] public static extern void          hwSetLights(int num_lights, IntPtr lights);
    [DllImport ("HairWorksIntegration")] public static extern void          hwRender(hwHInstance iid);
    [DllImport ("HairWorksIntegration")] public static extern void          hwRenderShadow(hwHInstance iid);
    [DllImport ("HairWorksIntegration")] public static extern void          hwStepSimulation(float dt);
	[DllImport ("HairWorksIntegration")] public static extern void          hwSetSphericalHarmonics(ref Vector4 Ar, ref Vector4 Ag, ref Vector4 Ab, ref Vector4 Br, ref Vector4 Bg, ref Vector4 Bb, ref Vector4 C);
    static void LogCallback(System.IntPtr cstr)
    {
        Debug.Log(Marshal.PtrToStringAnsi(cstr));
    }

    public static void hwSetLogCallback()
    {
        //// for debug
        //if(Application.isEditor)
        //{
          //hwSetLogCallback(LogCallback);
        //}
    }


}
