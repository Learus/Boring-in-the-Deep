using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapTemplate : MonoBehaviour
{
    public List<int> PrevDependencies;
    public List<int> NextDependencies;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.name == "Player")
        {
            Game.Instance.EnteredNewTemplate(name);
        }
    }
}
