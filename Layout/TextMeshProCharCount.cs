using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextMeshProCharCount : MonoBehaviour
{
    private TMP_Text _textRenderer;

    private TMP_Text textRenderer {
        get => _textRenderer;
        set => _textRenderer = value;
    }

    [SerializeField]
    private int _maxWordCount;

    public int maxWordCount {
        get => _maxWordCount;
        set => _maxWordCount = value;
    }

    [SerializeField]
    private int _maxCharCount;

    public int maxCharCount {
        get => _maxCharCount;
        set => _maxCharCount = value;
    }


    private void Awake()
    {
        StoreTextRenderer();
    }

    private void StoreTextRenderer()
    {
        if (textRenderer == null) {
            textRenderer = gameObject.GetComponent<TMP_Text>();
        }
    }

    void Start()
    {
        textRenderer.maxVisibleWords = maxWordCount;
        textRenderer.maxVisibleCharacters = maxCharCount;
    }

    public void SetMaxVisibleWordCount(int maxWordCount)
    {
        this.maxWordCount = maxWordCount;
        textRenderer.maxVisibleWords = maxWordCount;
    }

    public void SetMaxVisibleCharCount(int maxCharCount)
    {
        this.maxCharCount = maxCharCount;
        textRenderer.maxVisibleCharacters = maxCharCount;
    }
}
