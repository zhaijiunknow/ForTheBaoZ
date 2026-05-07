using FlyRabbit.FloatingTextSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FixedWorldText : MonoBehaviour
{
    public string Text = "";
    public TMP_FontAsset TMPFontAsset;
    public bool Follow;
    public Vector3 Offset;
    void Start()
    {
        if (Follow)
        {
            FloatingTextSystem.Instance.CreateFixedWorldText(Text, 2, Offset, transform, TMPFontAsset);
        }
        else
        {
            FloatingTextSystem.Instance.CreateFixedWorldText(Text, 2, transform.position, null, TMPFontAsset);
        }
        
    }
}
