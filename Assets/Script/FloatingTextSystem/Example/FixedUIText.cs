using FlyRabbit.FloatingTextSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FixedUIText : MonoBehaviour
{
    public string Text = "";
    public TMP_FontAsset TMPFontAsset;
    public bool Follow;
    public Vector3 Offset;
    void Start()
    {
        if (Follow)
        {
            FloatingTextSystem.Instance.CreateFixedUIText(Text, 40, Offset, transform, TMPFontAsset);
        }
        else
        {
            FloatingTextSystem.Instance.CreateFixedUIText(Text, 40, transform.position, null, TMPFontAsset);
        }
    }
}
