using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ResourceNode : MonoBehaviour
{

    public enum ResourceType { Crystal, Gold, Iron };
    public ResourceType nodeType;
    [SerializeField] Light hoverLight;
    float initialLightIntensity;
    float hoverLightIntensity;

    [MinMaxRange(100, 10000)]
    public RangedFloat startingResources;
    float currentResources;

    void Start()
    {
        currentResources = Random.Range(startingResources.minValue, startingResources.maxValue);
        initialLightIntensity = hoverLight.intensity;
        hoverLightIntensity = initialLightIntensity * 1.5f;
    }

    void OnMouseOver()
    {
        print("Mouse over Resource Node");
        hoverLight.intensity = hoverLightIntensity;
    }

    void OnMouseExit()
    {
        hoverLight.intensity = initialLightIntensity;
    }

}
