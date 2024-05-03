using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIScript : MonoBehaviour
{
    public enum State
    {
        Idle,
        Patrol,
        DetectPlayer,
        Chasing,
        AggroIdle,
    }

    public State enemyAIState;
    public Transform playerTransform; 
    public List<Transform> patrolPoints; 
    public float moveSpeed; 
    public float chaseSpeed; 
    public float detectedPlayerTime; 
    public float aggroTime; 
    public float damageInterval = 1f; 
    public float acceleration;

    private Animator _animator;
    private int currentPatrolPoint = 0;
    private float speed;
    private Rigidbody2D _myRb;
    private Coroutine damageCoroutine;
    private bool facingRight = false;

    void Start()
    {
        enemyAIState = State.Idle;
        _myRb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector2 velocity = _myRb.velocity;
        velocity.x = speed;
        _myRb.velocity = velocity;

        switch (enemyAIState)
        {
            case State.Idle:
                speed = 0;
                ChangeAnimationState(0);
                break;
            case State.Patrol:
                Patrol();
                ChangeAnimationState(1);
                break;
            case State.Chasing:
                ChasePlayer();
                ChangeAnimationState(2);
                break;
            case State.AggroIdle:
                speed = 0;
                ChangeAnimationState(0);
                break;
        }
    }

    private void ChangeAnimationState(int state)
    {
        _animator.SetInteger("State", state);
    }

    private void Patrol()
    {
        if (patrolPoints.Count == 0) return;

        speed = moveSpeed;
        Transform targetPoint = patrolPoints[currentPatrolPoint];
        float step = speed * Time.deltaTime;
        transform.position = new Vector2(
            Vector2.MoveTowards(transform.position, targetPoint.position, step).x,
            transform.position.y
        );

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Count;
        }
    }

    private void ChasePlayer()
    {
        if (playerTransform != null)
        {
            float step = chaseSpeed * Time.deltaTime;
            transform.position = new Vector2(
                Vector2.MoveTowards(transform.position, playerTransform.position, step).x,
                transform.position.y
            );
        }
    }
    private void FixedUpdate()
    {
        Vector2 targetPosition = Vector2.zero;
        float step = speed * Time.fixedDeltaTime; 

        switch (enemyAIState)
        {
            case State.Patrol:
                if (patrolPoints.Count == 0) return;
                targetPosition = patrolPoints[currentPatrolPoint].position;
                if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
                {
                    currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Count;
                }
                break;
            case State.Chasing:
                if (playerTransform != null)
                {
                    targetPosition = playerTransform.position;
                }
                break;
        }
        Vector2 newPosition = Vector2.MoveTowards(transform.position, targetPosition, step);
        Vector2 moveDirection = newPosition - new Vector2(transform.position.x, transform.position.y);
        _myRb.MovePosition(new Vector3(newPosition.x, newPosition.y, 0));

 
        if (moveDirection.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveDirection.x < 0 && facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        facingRight = !facingRight; 
        transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            enemyAIState = State.Chasing;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            enemyAIState = State.Patrol;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (damageCoroutine != null) StopCoroutine(damageCoroutine);
            damageCoroutine = StartCoroutine(DamagePlayerOverTime());
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    IEnumerator DamagePlayerOverTime()
    {
        while (true)
        {
            GameManager.Instance.DamagePlayer(1); 
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
