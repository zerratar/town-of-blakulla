
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class AutoFitChild : MonoBehaviour
{
    private RectTransform child;
    private RectTransform self;
    private ContentSizeFitter fitter;

    public float PaddingTopBottom = 5f;
    public float PaddingLeftRight = 10f;    

    // Start is called before the first frame update
    void Start()
    {
        this.child = (RectTransform)this.transform.GetChild(0).transform;
        this.self = (RectTransform)this.transform;
    }


    void LateUpdate()
    {
        var size = child.rect;

        if (size.height <= 0)
        {
            this.self.sizeDelta = new Vector2(0, 0);
            return;
        }

        var width = size.width + PaddingLeftRight;
        var height = size.height + PaddingTopBottom;

        this.self.sizeDelta = new Vector2(width, height);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
