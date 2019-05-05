using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TextMeshProUGUI = TMPro.TextMeshProUGUI;

public class TextWithBackground : MonoBehaviour
{
    private TextMeshProUGUI label;

    public string text;

    public Color color = Color.white;
    public Color background = new Color(0, 0, 0, 0.5f);

    public RectTransform rectTransform;
    private Image image;

    // Start is called before the first frame update
    void Start()
    {
        label = this.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        image = this.gameObject.GetComponentInChildren<Image>();
        rectTransform = this.gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (label)
        {
            image.color = background;
            label.color = color;
            label.text = text;
        }
    }
}
