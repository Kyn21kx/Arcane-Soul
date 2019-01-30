using UnityEngine;
using System.Collections;

public class AE_ShaderFloatCurve : MonoBehaviour {

    public AE_ShaderProperties ShaderFloatProperty = AE_ShaderProperties._Cutoff;
    public AnimationCurve FloatCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float GraphTimeMultiplier = 1, GraphIntensityMultiplier = 1;
    public bool IsLoop;
    public int MaterialNumber = 0;
    public bool UseSharedMaterial;
    public Renderer[] UseSharedRenderers;

    private bool canUpdate;
    private float startTime;
    private Material mat;
    private float startFloat;
    private int propertyID;
    //private string shaderProperty;
    private bool isInitialized;
    

    private void Awake()
    {
        //mat = GetComponent<Renderer>().material;
        var rend = GetComponent<Renderer>();
        if (rend == null)
        {
            var projector = GetComponent<Projector>();
            if (projector != null)
            {
                if (!UseSharedMaterial)
                {
                    if (!projector.material.name.EndsWith("(Instance)"))
                        projector.material = new Material(projector.material) { name = projector.material.name + " (Instance)" };
                    mat = projector.material;
                }
                else
                {
                    mat = projector.material;
                }
            }
        }
        else
        {
            if (!UseSharedMaterial) mat = rend.materials[MaterialNumber];
            else mat = rend.sharedMaterials[MaterialNumber];
        }
      

        //shaderProperty = ShaderFloatProperty.ToString();
        if (mat.HasProperty(ShaderFloatProperty.ToString()))
        {
            //propertyID = Shader.PropertyToID(shaderProperty);
            startFloat = mat.GetFloat(ShaderFloatProperty.ToString());
            var eval = FloatCurve.Evaluate(0)*GraphIntensityMultiplier;
            mat.SetFloat(ShaderFloatProperty.ToString(), eval);
            isInitialized = true;
        }

        if (UseSharedRenderers != null)
            foreach (var sharedRend in UseSharedRenderers)
            {
                sharedRend.sharedMaterial = mat;
            }
    }
    
    private void OnEnable()
    {
        startTime = Time.time;
        canUpdate = true;
        if (isInitialized)
        {
            var eval = FloatCurve.Evaluate(0)*GraphIntensityMultiplier;
            mat.SetFloat(ShaderFloatProperty.ToString(), eval);
        }
    }

    private void Update()
    {
        var time = Time.time - startTime;
        if (canUpdate)
        {
            var eval = FloatCurve.Evaluate(time / GraphTimeMultiplier) * GraphIntensityMultiplier;
            mat.SetFloat(ShaderFloatProperty.ToString(), eval);
            if (UseSharedRenderers != null)
                foreach (var sharedRend in UseSharedRenderers)
                {
                    sharedRend.sharedMaterial = mat;
                }
        }
        if (time >= GraphTimeMultiplier)
        {
            if (IsLoop) startTime = Time.time;
            else canUpdate = false;
        }
    }
   
    void OnDisable()
    {
        if(UseSharedMaterial) mat.SetFloat(ShaderFloatProperty.ToString(), startFloat);

    }

    //void OnDestroy()
    //{
    //    if (!UseSharedMaterial)
    //    {
    //        if (mat != null)
    //            DestroyImmediate(mat);
    //        mat = null;
    //    }
    //}
}
