﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewProgressBar : MonoBehaviour
{
    private bool isInit = false;
    private Image progressBar;
    public void Awake()
    {
        progressBar = transform.GetComponent<Image>();
        progressBar.type = Image.Type.Filled;
        progressBar.fillMethod = Image.FillMethod.Horizontal;
        progressBar.fillOrigin = 0;
    }

    public void SetProgressValue(float value)
    {
        progressBar.fillAmount = value;
    }
}
