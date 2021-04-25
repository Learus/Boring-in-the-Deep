using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapTemplate : MonoBehaviour
{
    public List<int> PrevDependencies;
    public List<int> NextDependencies;
    public int WinIndex;
    public enum TemplateType
    {
        Normal,
        Transition,
        Boss
    };

    public TemplateType type;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.name == "Player")
        {
            Game.Instance.EnteredNewTemplate(this);
        }
    }
}
