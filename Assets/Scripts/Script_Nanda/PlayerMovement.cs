using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerConfig config;
    [SerializeField] private Rigidbody2D rb;
    private IMoveInput input;
    private Vector2 velocity;

    // Baru di v5: dibaca PlayerAnimator (nanti) dan PlayerInteractor (sekarang)
    public Vector2 Velocity => velocity;
    public Vector2 Facing { get; private set; } = Vector2.down;

    public void Construct(IMoveInput moveInput) => input = moveInput;

    void Update()
    {
        if (input == null) return;
        Vector2 move = input.Move;
        float speed = input.IsRunning ? config.RunSpeed : config.WalkSpeed;
        velocity = move * speed;
        if (move.sqrMagnitude > 0.01f) Facing = ToCardinal(move);
    }

    void FixedUpdate() => rb.linearVelocity = velocity;

    private static Vector2 ToCardinal(Vector2 v) =>
        Mathf.Abs(v.x) > Mathf.Abs(v.y)
            ? new Vector2(Mathf.Sign(v.x), 0f)
            : new Vector2(0f, Mathf.Sign(v.y));
}