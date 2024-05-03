using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Transform playerSpawnPoint;
    public float playerHealth;
    public float maxPlayerHealth;
    public float playerScore;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        playerSpawnPoint = GameObject.FindWithTag("Start").transform;
        GameObject player = GameObject.FindWithTag("Player");
        player.transform.position = playerSpawnPoint.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddScore(float score)
    {
        playerScore += score;
    }
    public void TakeDamage(float damage)
    {
        playerHealth-= damage;
    }
    public void UpdateSpawnPoint(Transform newSpawnPoint)
    {
        playerSpawnPoint = newSpawnPoint;
    }
    public void RespawnPlayer()
    {
       // playerHealth = maxPlayerHealth;
        GameObject player = GameObject.FindWithTag("Player");
        player.transform.position = playerSpawnPoint.position;
    }
    public void DamagePlayer(float damage)
    {
        playerHealth -= damage;
        if (playerHealth <= 0)
        {
            playerHealth = maxPlayerHealth;
            RespawnPlayer();
            
        }
    }
}
