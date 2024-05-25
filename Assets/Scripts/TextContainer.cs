using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextContainer : MonoBehaviour
{
    public TMP_Text tmpText;

    private RectTransform m_TextRectTransform;

    private float m_CurrentPosY;

    void Start()
    {
        m_TextRectTransform = tmpText.gameObject.GetComponent<RectTransform>();
        m_CurrentPosY = - m_TextRectTransform.rect.height / 2;

    }
    
    void Update()
    {
        
    }

    public void AddText(string text)
    {
        var newText = Instantiate(tmpText.gameObject, Vector3.zero, Quaternion.identity, gameObject.transform);
        var textRectTransform = newText.GetComponent<RectTransform>();
        var anchoredPosition = textRectTransform.anchoredPosition;
        anchoredPosition.x = 0;
        anchoredPosition.y = m_CurrentPosY;
        textRectTransform.anchorMin = new Vector2(0.5f, 1.0f);
        textRectTransform.anchorMax = new Vector2(0.5f, 1.0f);
        textRectTransform.pivot = new Vector2(0.5f, 1.0f);
        textRectTransform.anchoredPosition = anchoredPosition;
        m_CurrentPosY -= tmpText.gameObject.GetComponent<RectTransform>().rect.height;
        newText.GetComponent<TMP_Text>().text = text;
    }

    public void Clear()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        m_CurrentPosY = - tmpText.gameObject.GetComponent<RectTransform>().rect.height / 2;
    }
}
