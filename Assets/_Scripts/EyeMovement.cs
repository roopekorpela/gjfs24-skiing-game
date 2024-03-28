using UnityEngine;

public class EyesScript : MonoBehaviour
{
    public float maxDistance = 1f;  // max distance the eyes can move

    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;

        Vector3 localMousePosition = transform.InverseTransformPoint(mousePosition);
        localMousePosition.z = 0f;

        if (localMousePosition.magnitude > maxDistance)
        {
            localMousePosition = localMousePosition.normalized * maxDistance;
        }

        transform.localPosition = new Vector3(localMousePosition.x, 0,0);
    }
}
