using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBall : MonoBehaviour
{
    public GameManager gameManager;
    public AudioSource blip;
    public AudioSource blop;

    Vector3 ballStartPosition;
    Rigidbody2D rb2d;
    readonly float speed = 400;
    
	void Start ()
    {
        rb2d = GetComponent<Rigidbody2D>();
        ballStartPosition = transform.position;
        ResetBall();
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "backwall" || collision.gameObject.tag == "frontwall")
        {
            if (collision.gameObject.tag == "backwall")
                gameManager.Score(0);
            else gameManager.Score(1);

            blop.Play();
            ResetBall();
        }
        else
            blip.Play();
    }

    public void ResetBall()
    {
        transform.position = ballStartPosition;
        rb2d.velocity = Vector3.zero;
        int xDir = Random.Range(0, 2);

        xDir = (xDir == 0) ?
            Random.Range(100, 301)  :
            Random.Range(-300, -101);
        
        Vector3 dir = new Vector3(xDir, Random.Range(-100, 100), 0).normalized;

        rb2d.AddForce(dir * speed);
    }
}
