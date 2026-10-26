using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerConfig config;
    [SerializeField] private Rigidbody2D rb;
    
    private IMoveInput input;
    private Vector2 velocity;
    private Vector2 facingDirection = Vector2.down; // Arah default menghadap ke bawah

    public void Construct(IMoveInput moveInput) => input = moveInput;

    // Properti publik agar bisa dibaca oleh PlayerAnimator
    public Vector2 Velocity => velocity;
    public Vector2 Facing => facingDirection;

    void Update()
    {
        if (input == null) return;

        // 1. Tentukan kecepatan (Walk / Run dari Config)
        float speed = input.IsRunning ? config.RunSpeed : config.WalkSpeed;
        velocity = input.Move * speed;

        // 2. Simpan arah terakhir saat player bergerak untuk animasi (Facing)
        if (input.Move.sqrMagnitude > 0.01f)
        {
            facingDirection = input.Move;
        }
    }

    void FixedUpdate() 
    {
        // Menggerakkan Rigidbody menggunakan linearVelocity (standar Unity 6)
        rb.linearVelocity = velocity;
    }
}