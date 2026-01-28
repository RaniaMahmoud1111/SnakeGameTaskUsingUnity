using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Snake : MonoBehaviour
{
    private Vector2 _direction = Vector2.right;
    private List<Transform> _segments = new List<Transform>();
    public Transform segmentPrefab;

    public int initialSize = 4;

    // UI Elements
    public Text gameOverText;
    public Text scoreText;
    public Slider timeBar;

    // Game Variables
    private bool isGameOver = false;
    private int score = 0;
    public float totalTime = 60f; // Total time for the game
    private float timeRemaining;

    private void Start()
    {
        ResetState();
        gameOverText.gameObject.SetActive(false); // Hide Game Over text at the start
        timeRemaining = totalTime;
        UpdateScoreText();
    }

    private void Update()
    {
        if (isGameOver) return; // Stop processing if the game is over

        // Handle Snake Movement
        if (Input.GetKeyDown(KeyCode.W) && _direction != Vector2.down)
        {
            _direction = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.S) && _direction != Vector2.up)
        {
            _direction = Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.A) && _direction != Vector2.right)
        {
            _direction = Vector2.left;
        }
        else if (Input.GetKeyDown(KeyCode.D) && _direction != Vector2.left)
        {
            _direction = Vector2.right;
        }

        // Handle Timer
        if (!isGameOver)
        {
            timeRemaining -= Time.deltaTime;
            timeBar.value = timeRemaining / totalTime;

            if (timeRemaining <= 0)
            {
                GameOver();
            }
        }
    }

    private void FixedUpdate()
    {
        if (isGameOver) return; // Stop movement if the game is over

        // Move each segment to the position of the one before it
        for (int i = _segments.Count - 1; i > 0; i--)
        {
            _segments[i].position = _segments[i - 1].position;
        }

        // Move the snake's head
        this.transform.position = new Vector3(
            Mathf.Round(this.transform.position.x) + _direction.x,
            Mathf.Round(this.transform.position.y) + _direction.y,
            0.0f
        );
    }

    private void Grow()
    {
        Transform segment = Instantiate(this.segmentPrefab);
        segment.position = _segments[_segments.Count - 1].position;
        _segments.Add(segment);

        // Increase Score
        score++;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    private void ResetState()
    {
        for (int i = 1; i < _segments.Count; i++)
        {
            Destroy(_segments[i].gameObject);
        }
        _segments.Clear();
        _segments.Add(this.transform);

        for (int i = 1; i < this.initialSize; i++)
        {
            _segments.Add(Instantiate(this.segmentPrefab));
        }

        this.transform.position = Vector3.zero;
        _direction = Vector2.right;

        isGameOver = false;
        gameOverText.gameObject.SetActive(false); // Hide Game Over text
        timeRemaining = totalTime;
        score = 0;
        UpdateScoreText();
        timeBar.value = 1; // Reset time bar
    }

    private void GameOver()
    {
        isGameOver = true;
        gameOverText.gameObject.SetActive(true); // Make Game Over text visible
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Food")
        {
            Grow();
        }
        else if (other.tag == "Obstacle" || other.tag == "SnakeBody")
        {
            GameOver();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("SnakeBody"))
        {
            GameOver();
        }
    }
}
