using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AE_CloneStartParams : MonoBehaviour
{

    public AE_ClonesCreator clonesCreator;
  
    public RuntimeAnimatorController controller;
    public Material GhostMaterial;
    public AE_ShaderProperties ShaderFloatProperty = AE_ShaderProperties._Cutout;
    public AnimationCurve FadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float FadeTime = 3.5f;
    public CloneStartInfoProp[] CloneStartInfo;

    [System.Serializable]
    public class CloneStartInfoProp
    {
        public Transform Transform;
        public float StartDelay;

        [HideInInspector] public float CurrentDelay = 0f;
        [HideInInspector] public GameObject Instance;
    }

    void OnEnable()
    {
        foreach (var info in CloneStartInfo)
        {
            info.CurrentDelay = 0;
            if(info.Instance != null) info.Instance.SetActive(false);
        }
    }

    void Update()
    {
        foreach (var info in CloneStartInfo)
        {
            info.CurrentDelay += Time.deltaTime;
            if (info.CurrentDelay >= info.StartDelay)
            {
                if (info.Instance != null) info.Instance.SetActive(true);
                else
                {
                    info.Instance = InstantiateClone(info.Transform);
                }
            }
        }
    }

    private GameObject InstantiateClone(Transform parent)
    {
        var instance = Instantiate(clonesCreator.Character, parent);
        instance.SetActive(false);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = new Quaternion();
        instance.GetComponent<Animator>().runtimeAnimatorController = controller;

        var renderers = instance.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            renderers[0].material = GhostMaterial;
            var shaderFloatCurve = renderers[0].transform.gameObject.AddComponent<AE_ShaderFloatCurve>();
            shaderFloatCurve.ShaderFloatProperty = ShaderFloatProperty;
            shaderFloatCurve.FloatCurve = FadeCurve;
            shaderFloatCurve.GraphTimeMultiplier = FadeTime;
            if (renderers.Length > 1)
            {
                var newRend = renderers.ToList();
                newRend.RemoveAt(0);
                shaderFloatCurve.UseSharedRenderers = newRend.ToArray();
            }
        }
        instance.SetActive(true);
        return instance;
    }
}
