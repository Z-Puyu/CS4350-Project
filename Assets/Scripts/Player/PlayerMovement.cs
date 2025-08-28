using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    private Vector2 direction;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnMove(InputValue dir)
    {
        direction = dir.Get<Vector2>() * speed;
    }

    void Update()
    {
        rb.position = new Vector2(rb.position.x + direction.x, rb.position.y + direction.y);
    }
}
