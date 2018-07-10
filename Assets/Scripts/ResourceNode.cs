using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ResourceNode : MonoBehaviour
{
    GameObject resourceNodeGO;
    public enum ResourceType { Crystal, Gold, Iron };
    public ResourceType nodeType;
    [SerializeField] Light hoverLight;
    public float initialLightIntensity;
    public float hoverLightMultiplier = 3.5f;
    public float glowSpeed;

    public GameObject resourceOverlay;



    float hoverLightIntensity;

    [MinMaxRange(100, 10000)]
    public RangedFloat startingResources;
    float currentResources;

    bool mouseOverNode;
    private Coroutine rollOffCoroutine;

    void Start()
    {
        resourceNodeGO = this.gameObject;
        currentResources = Random.Range(startingResources.minValue, startingResources.maxValue);
        initialLightIntensity = hoverLight.intensity;
        hoverLightIntensity = initialLightIntensity * hoverLightMultiplier;
        resourceOverlay.SetActive(false);
        mouseOverNode = false;
    }

    void OnMouseOver()
    {
        mouseOverNode = true;
        resourceOverlay.SetActive(true);
        //resourceOverlay.transform.position = Vector3.Lerp(this.transform.position, Camera.main.transform.position, .05f);
        resourceOverlay.transform.LookAt(Camera.main.transform);
        hoverLight.intensity = Mathf.Lerp(hoverLight.intensity, hoverLightIntensity, Time.deltaTime * glowSpeed);
    }

    void OnMouseDown()
    {
        
    }

    void OnMouseExit()
    {
        mouseOverNode = false;
        resourceOverlay.SetActive(false);
        rollOffCoroutine = StartCoroutine(RollOffGlow());

    }

    IEnumerator RollOffGlow()
    {
        while (!mouseOverNode && hoverLight.intensity > initialLightIntensity)
        {
            hoverLight.intensity = Mathf.Lerp(hoverLight.intensity, initialLightIntensity, Time.deltaTime * glowSpeed);
            yield return new WaitForEndOfFrame();
        }
    }

}
