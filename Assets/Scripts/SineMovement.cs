using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineMovement : MonoBehaviour
{

    public float Speed = 0.5f;
    public float MaxMovement = 1.0f;
    
    private float StartHeight = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        StartHeight = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float height = Mathf.Sin(Time.time * Speed) * MaxMovement + StartHeight;
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
    }
}
