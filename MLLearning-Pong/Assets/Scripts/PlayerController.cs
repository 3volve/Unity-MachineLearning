using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public readonly float maxSpeed = 15f;
    public readonly float paddleYClamp = 4.4f;

    Rigidbody2D rb2d;
    float posX;
    float movingValue = 0;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        posX = transform.position.x;
    }
    
    void Update()
    {
        float posy = Mathf.Clamp(transform.position.y +
                                movingValue * Time.deltaTime * maxSpeed,
                                -paddleYClamp, paddleYClamp);

        transform.position = new Vector3(posX, posy, 0);
    }

    void OnUpDown(InputValue value)
    {
        movingValue = value.Get<float>();
    }
}
