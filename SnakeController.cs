

using JetBrains.Annotations;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SnakeController : MonoBehaviour
{
    public Transform food;
    public Transform minBorder;
    public Transform maxBorder;
    public GameObject bodyPrefab;
    public LayerMask snakeLayer;
    private float moveTimer = 0f;
    public float moveInterval = 0.2f;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverText;
    private Vector3 direction = Vector3.forward;
    private List<Transform> bodyParts = new List<Transform>();
    //private List<Vector3> emptyCells = new List<Vector3>();


    private void Start()
    {
        bodyParts.Add(this.transform);
    }
    private void OnMoveSnake(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        if (input.x != 0 || input.y != 0)
        {
           Vector3 newDirection = new Vector3(Mathf.Round(input.x), 0, Mathf.Round(input.y));
            if (newDirection != -direction)
            {
                direction = newDirection;
            }
        }
    }
    private void Update()
    {
        moveTimer+= Time.deltaTime;
        if (moveTimer >= moveInterval)
        {
            BodyMove();
            moveTimer = 0f;
        }

        if (Physics.CheckSphere(food.position, 0.5f, snakeLayer))
        {
            SpawnFood();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        { 
            Grow();
            ScoreText();
        }
        else if (other.CompareTag("Wall") || other.CompareTag("Body"))
        {
            if (other.transform != bodyParts[bodyParts.Count - 1])
            {
               GameOverText();
            }
        }
    }
    private void SpawnFood()
    {
        float randomX = Mathf.Round(Random.Range(minBorder.position.x, maxBorder.position.x));
        float randomZ = Mathf.Round(Random.Range(minBorder.position.z, maxBorder.position.z));
        food.position = new Vector3(randomX, 1.5f, randomZ);
    }
    private void BodyMove()
    {
        //Moves the tail segments to the position of the previous segment, from back to front.
        for (int i = bodyParts.Count - 1; i > 0; i--)
        {
            bodyParts[i].position = bodyParts[i - 1].position;
        }
        transform.position += direction.normalized;
    }
    private void Grow()
    {
        Vector3 spawnPos = bodyParts[bodyParts.Count - 1].position;
        GameObject newPart = Instantiate(bodyPrefab, spawnPos, Quaternion.identity);
        bodyParts.Add(newPart.transform);
    }
    private void ScoreText()
    {
        int score = 0;
        score++;
        scoreText.text = "Score: " + score.ToString();
    }
    private void GameOverText()
    {
        gameOverText.gameObject.SetActive(true);
        Time.timeScale = 0f; //Game is paused.
    }
}
