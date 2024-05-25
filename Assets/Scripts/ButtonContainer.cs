using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonContainer : MonoBehaviour
{
    public Button buttonPrefab;

    public float upPadding;

    public float padding;
    
    private RectTransform m_ButtonRectTransform;

    private float m_CurrentPosY;

    void Awake()
    {
        m_ButtonRectTransform = buttonPrefab.gameObject.GetComponent<RectTransform>();
        m_CurrentPosY = - m_ButtonRectTransform.rect.height / 2 - upPadding;
    }

    void Start()
    {
        Debug.Log(m_CurrentPosY);

    }
    
    void Update()
    {
        
    }

    public Button AddButton(string text)
    {
        var newButton = Instantiate(buttonPrefab.gameObject, Vector3.zero, Quaternion.identity, gameObject.transform);
        var textRectTransform = newButton.GetComponent<RectTransform>();
        var anchoredPosition = textRectTransform.anchoredPosition;
        anchoredPosition.x = 0;
        anchoredPosition.y = m_CurrentPosY;
        textRectTransform.anchorMin = new Vector2(0.5f, 1.0f);
        textRectTransform.anchorMax = new Vector2(0.5f, 1.0f);
        textRectTransform.pivot = new Vector2(0.5f, 1.0f);
        textRectTransform.anchoredPosition = anchoredPosition;
        m_CurrentPosY -= buttonPrefab.gameObject.GetComponent<RectTransform>().rect.height + padding;
        newButton.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = text;
        return newButton.GetComponent<Button>();
    }

    public void Clear()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        m_CurrentPosY = - buttonPrefab.gameObject.GetComponent<RectTransform>().rect.height / 2 - upPadding;
    }
}
