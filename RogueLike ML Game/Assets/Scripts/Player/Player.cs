using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Transform fireTransform;
    public GameObject bullet;

    public float movementSpeed = 0.15f;
    public float jumpForce = 10;

    private HP hp;
    private HealthMonitor healthMonitor;
    private ScoreMonitor scoreMonitor;
    private Collider mainCollider;
    private SkinnedMeshRenderer playerRenderer;
    private new Rigidbody rigidbody;

    private Vector3 m_Move = Vector3.zero;
    private float m_Look = 0;
    private bool m_Jump = false;
    private bool m_Shoot = false;

    private readonly float lookModifier = 3.0f;
    private readonly float shootingSpread = 0.5f;
    private float timeToShootAgain = 0;

    private readonly float hitInvuln = 1.5f;
    private float hitTimer = 1;


    void FixedUpdate()
    {
        if (FindObjectOfType<Pause>().Paused) return;

        if (m_Look != 0) Look();
        if (m_Move.magnitude > 0) Move();
        if (m_Shoot) Shoot();
        Jump();

        timeToShootAgain += Time.deltaTime;
        hitTimer += Time.deltaTime;

        if (hitTimer < hitInvuln) Invincible();
        else
        {
            mainCollider.enabled = true;

            Color color = playerRenderer.material.color;
            color.a = 1;
            playerRenderer.material.color = color;
        }

        CheckForIsLooking(Vector3.zero);
        CheckForIsLooking(transform.right * 0.1f);
        CheckForIsLooking(transform.right * -0.1f);

        GetComponent<Animator>().SetInteger("HorizontalDirection", Mathf.RoundToInt(m_Move.x));
        GetComponent<Animator>().SetInteger("VerticalDirection", Mathf.RoundToInt(m_Move.z));
        GetComponent<Animator>().SetInteger("TurnDirection", Mathf.RoundToInt(m_Look / 10));
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        hp = GetComponent<HP>();
        mainCollider = GetComponent<Collider>();
        playerRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        rigidbody = GetComponent<Rigidbody>();
        healthMonitor = GetComponent<HealthMonitor>();
        scoreMonitor = GetComponent<ScoreMonitor>();
        healthMonitor.hitValue = healthMonitor.healthLength / hp.maxHealth;
        healthMonitor.playerHealth = hp.maxHealth;
        hp.justDied = new HP.JustDied(Died);
        hp.justHit = new HP.JustHit(Hit);
    }

    /***************** Input Methods ********************/
    public void OnLook(InputValue value)
    {
        m_Look = value.Get<float>();
    }

    public void OnMove(InputValue value)
    {
        m_Move.x = value.Get<Vector2>().x;
        m_Move.z = value.Get<Vector2>().y;
    }

    public void OnShoot(InputValue value)
    {
        m_Shoot = value.isPressed;
    }

    public void OnJump(InputValue value)
    {
        m_Jump = value.isPressed;
    }

    private void Reset()
    {
        transform.position = new Vector3(0, 0.5f, 0);
        //transform.rotation = Quaternion.identity;
        hp.Reset();
    }

    /***************** Local Methods ********************/
    private void Died()
    {
        FindObjectOfType<Pause>().Paused = true;
        Hit();
        Reset();
        healthMonitor.increasing = true;
        foreach (EnemyBullet bullet in FindObjectsOfType<EnemyBullet>())
            Destroy(bullet.gameObject);
    }

    private void Hit()
    {
        hitTimer = 0;
        mainCollider.enabled = false;
        GetComponent<HealthMonitor>().decreasing = true;
    }

    /***************** Helper/Cleaner Methods ********************/
    private void Look()
    {
        transform.Rotate(0, m_Look * Time.deltaTime * lookModifier, 0);
    }

    private void Move()
    {
        Vector3 movementVector = transform.position + transform.localToWorldMatrix.MultiplyVector(m_Move * movementSpeed);

        if (m_Move == Vector3.back)
        {
            if (!Physics.Raycast(transform.position, Vector3.back, out RaycastHit hit, 1.5f, LayerMask.GetMask("Wall")))
                rigidbody.MovePosition(movementVector);
        }
        else rigidbody.MovePosition(movementVector);
    }

    private void Jump()
    {
        if (Physics.BoxCast(mainCollider.bounds.center + Vector3.up * 1.75f,
            mainCollider.bounds.extents,
            -transform.up,
            transform.rotation,
            mainCollider.bounds.size.y + 0.1f,
            LayerMask.GetMask("Ground", "Enemy")))
        {
            if (m_Jump)
            {
                rigidbody.AddForce(0, jumpForce, 0);
                m_Jump = false;
            }

            foreach (Transform trans in GetComponentsInChildren<Transform>())
            {
                trans.gameObject.layer = LayerMask.NameToLayer("Player");
            }
            //Debug.DrawRay(mainCollider.bounds.center + Vector3.up * 1.75f, -transform.up * (mainCollider.bounds.extents.y + 0.5f), Color.green);
        }
        else
        {
            foreach (Transform trans in GetComponentsInChildren<Transform>())
            {
                trans.gameObject.layer = LayerMask.NameToLayer("PlayerJumping");
            }
            //Debug.DrawRay(mainCollider.bounds.center + Vector3.up * 1.75f, -transform.up * (mainCollider.bounds.extents.y + 0.5f), Color.red);
        }
    }

    private void Shoot()
    {
        if (timeToShootAgain < shootingSpread) return;

        GameObject newBullet = Instantiate(bullet, fireTransform.position, fireTransform.rotation);
        newBullet.GetComponent<PlayerBullet>().parent = scoreMonitor;
        newBullet.GetComponent<PlayerBullet>().speed = 0.45f;
        newBullet.layer = LayerMask.NameToLayer("PlayerBullets");

        timeToShootAgain = 0;
        m_Shoot = false;
    }

    private void CheckForIsLooking(Vector3 direction)
    {
        Quaternion straightRotation = Quaternion.LookRotation(transform.forward, Vector3.up);
        fireTransform.parent.rotation = straightRotation;

        if (Physics.SphereCast(fireTransform.position, 1.25f, fireTransform.forward + direction, out RaycastHit hit, 40, LayerMask.GetMask("Enemy")))
        {
            Brain hitEnemy = hit.collider.GetComponent<Brain>();
            if (hitEnemy == null) hitEnemy = hit.collider.GetComponentInParent<Brain>();
            hitEnemy.playerLooking = true;
            //Debug.DrawRay(fireTransform.position, (fireTransform.forward + direction) * 40, Color.green);
        }
        //else
        //    Debug.DrawRay(fireTransform.position, (fireTransform.forward + direction) * 40, Color.red);
    }

    private void Invincible()
    {
        Color color = playerRenderer.material.color;
        float alternating = hitInvuln / 8;

        if (hitTimer < alternating ||
            (hitTimer >= alternating * 2 && hitTimer < alternating * 3) ||
            (hitTimer >= alternating * 4 && hitTimer < alternating * 5) ||
            (hitTimer >= alternating * 6 && hitTimer < alternating * 7))
        {
            color.b = Mathf.Lerp(color.b, 0, 0.2f);
            color.g = Mathf.Lerp(color.g, 0, 0.2f);
        }
        else
        {
            color.b = Mathf.Lerp(color.b, 1, 0.2f);
            color.g = Mathf.Lerp(color.g, 1, 0.2f);
        }

        playerRenderer.material.color = color;
    }
}
