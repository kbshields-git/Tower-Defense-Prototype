using UnityEngine;

public class BuildGrid : MonoBehaviour
{
    [SerializeField]
    private float size = 1f;

    public Vector3 GetNearestPointOnGrid(Vector3 position)
    {
        position -= transform.position;

        int xCount = Mathf.RoundToInt(position.x / size);
        int yCount = Mathf.RoundToInt(position.y / size);
        int zCount = Mathf.RoundToInt(position.z / size);

        Vector3 result = new Vector3(
            (float)xCount * size,
            (float)yCount * size,
            (float)zCount * size);

        result += transform.position;

        return result;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if (GameManager.instance.alwaysDrawGizmos && GameManager.instance.drawGridGizmos)
            {
                Gizmos.color = Color.yellow;
                for (float x = -40; x < 40; x += size)
                {
                    for (float z = -40; z < 40; z += size)
                    {
                        var point = GetNearestPointOnGrid(new Vector3(x, 0f, z));
                        Gizmos.DrawSphere(point, 0.1f);
                    }

                }
            }
        }
    }
#endif
}
