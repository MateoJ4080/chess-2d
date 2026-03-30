using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ClampWidthToParent : MonoBehaviour
{
    [SerializeField] private RectTransform _parent;
    [SerializeField] private float _preferredWidth = 500;
    [SerializeField] private float offset = 20;

    private RectTransform rt;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        float parentWidth = _parent.rect.width;
        float width = Mathf.Min(_preferredWidth, parentWidth) - offset;
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }
}