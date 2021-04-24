using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = Vector3.zero;
    public float offsetSpeed = 5f;
    
    // Update is called once per frame
    void Update()
    {
        Vector3 target = new Vector3(transform.position.x, player.position.y, transform.position.z) + offset;
        transform.position = Vector3.Lerp(transform.position, target, offsetSpeed * Time.deltaTime);
    }
}
