using UnityEngine;

public class KeepRotation : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, 0f);
    private Transform _parent;

    void Start()
    {
        _parent = transform.parent;
    }

    void LateUpdate()
    {
        transform.position = _parent.position + offset;
        transform.rotation = Quaternion.identity;
    }
}