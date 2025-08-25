using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GameRoot.UI
{
    public class RootButtonBinder : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        void Awake()
        {
            text.outlineWidth = 0.2f;
            text.outlineColor = new Color32(0, 0, 0, 245);
        }
    }
}