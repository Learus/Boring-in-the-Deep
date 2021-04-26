using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = Vector3.zero;
    public float offsetSpeed = 3f;

    [Header("Bounds")]
    public float leftBound = -7.63f;
    public float rightBound = 7.64f;
    
    // Update is called once per frame
    void Update()
    {
        float x = transform.position.x;
        if (x < leftBound) x = leftBound;
        if (x > rightBound) x = rightBound;

        Vector3 target = new Vector3(x, player.position.y, transform.position.z) + offset;

        transform.position = Vector3.Lerp(transform.position, target, offsetSpeed * Time.deltaTime);
    }
}
