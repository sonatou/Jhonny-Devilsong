using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraScaler : MonoBehaviour
{
    [SerializeField] private float targetAspect = 16f / 9f;
    [SerializeField] private float referenceOrthographicSize = 5f;

    private Camera _cam;

    void Awake()
    {
        _cam = GetComponent<Camera>();
        AdjustSize();
    }

    void AdjustSize()
    {
        float currentAspect = (float)Screen.width / Screen.height;
        float scaleHeight = currentAspect / targetAspect;

        if (scaleHeight < 1f)
            _cam.orthographicSize = referenceOrthographicSize / scaleHeight;
        else
            _cam.orthographicSize = referenceOrthographicSize;
    }
}