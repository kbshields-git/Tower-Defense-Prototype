using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ResourceNode : MonoBehaviour
{

    public enum ResourceType { Crystal, Gold, Iron };
    public ResourceType nodeType;
    [SerializeField] Light hoverLight;
    public float initialLightIntensity;
    public float glowSpeed;
    float hoverLightIntensity;

    [MinMaxRange(100, 10000)]
    public RangedFloat startingResources;
    float currentResources;

    bool mouseOverNode;

    void Start()
    {
        currentResources = Random.Range(startingResources.minValue, startingResources.maxValue);
        initialLightIntensity = hoverLight.intensity;
        hoverLightIntensity = initialLightIntensity * 1.5f;
        mouseOverNode = false;
    }

    void Update()
    {
        if (!mouseOverNode && hoverLight.intensity > initialLightIntensity)
        {
			hoverLight.intensity = Mathf.Lerp(hoverLight.intensity, initialLightIntensity, Time.deltaTime * glowSpeed);
        }
    }
    void OnMouseOver()
    {
        mouseOverNode = true;
        hoverLight.intensity = Mathf.Lerp(hoverLight.intensity, hoverLightIntensity, Time.deltaTime * glowSpeed);
    }

    void OnMouseExit()
    {

        mouseOverNode = false;
    }

}
