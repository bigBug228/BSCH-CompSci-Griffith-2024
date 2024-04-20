using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAIScript : MonoBehaviour
{
    public int health;
    public int damage;
    public float speed;
    public float aggroSpeed;
    public bool aggro;
    public bool playerDetected;
    public float aggroTime;
    public Transform playerLocation;
    private float initialSpeed;
    public float detectTime;

    public enum AIBehavior
    {
        Idle,
        Patrol,
        DetectPlayer,
        ChasePlayer,
        AggroIdle

    }
    public AIBehavior behavior;
    // Start is called before the first frame update
    void Start()
    {
        speed = initialSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        switch (behavior)
            {
            case AIBehavior.Idle:
                speed = 0;
                break;
                case AIBehavior.Patrol:
                speed = initialSpeed;
                break;
            case AIBehavior.DetectPlayer:
                speed = 0;
                break;
            case AIBehavior.ChasePlayer:

                speed = aggroSpeed;
                break;
            case AIBehavior.AggroIdle:
                speed =0;
                break;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")&& aggro ==false)
            {
            StopCoroutine("AggroTimer");
            StartCoroutine("DetectTime");
        }
        if (collision.gameObject.CompareTag("Player") && aggro == true)
        {
            StopCoroutine("AggroTimer");
            behavior = AIBehavior.ChasePlayer;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && aggro == true)
        {
            StopCoroutine("AggroTimer");
            behavior = AIBehavior.ChasePlayer;
        }
    }
    IEnumerator DetectTime()
    {
        behavior = AIBehavior.DetectPlayer;
        yield return new WaitForSeconds(detectTime);
        aggro = true;
        behavior = AIBehavior.ChasePlayer;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StopCoroutine("AggroTimer");
            StartCoroutine("AggroTimer");
        }
        
    }
    IEnumerator AggroTimer()
    {
        behavior = AIBehavior.AggroIdle;
        yield return new WaitForSeconds(aggroTime);
        if (behavior != AIBehavior.ChasePlayer)
        {
            behavior = AIBehavior.Idle;
            aggro = false;
        }    
    }
}
