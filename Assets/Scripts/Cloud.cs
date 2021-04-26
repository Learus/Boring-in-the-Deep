using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public float xBound = 0.7f;
    public float yOffset = 0;
    public bool left= false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.position.x >= xBound) left = true;
        if (this.transform.position.x <= -xBound) left = false;

        Vector3 transl = new Vector3(left ? -1 : 1, 0, 0);

        this.transform.Translate(transl * 0.4f * Time.deltaTime);
    }
}
