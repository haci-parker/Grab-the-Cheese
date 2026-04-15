using UnityEngine;
using UnityEngine.InputSystem;


public class MouseMovementController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    
    // Objenin üzerinde Rigidbody2D olması zorunludur.
    private Rigidbody2D rb;
    private StoryDialogueManager manager;
    private bool canMove = true;
    private Animator anim;

    // Stoper pozisyon kontrolü
    private bool stoperTriggered = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        manager = FindObjectOfType<StoryDialogueManager>();
        anim = GetComponent<Animator>();

    }

    void Update()
    {
        // Manager yoksa, level 21 değilse, hareket kapalıysa veya Rigidbody eklenmemişse çalışmaz
        if (manager == null || manager.CurrentLevel < 21 || !canMove || rb == null) return;

        var kb = Keyboard.current;
        if (kb == null) return;

        float moveX = 0f;

        // Sağa ve sola hareket (A/D ve Yön Tuşları)
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) moveX = 1f;
        else if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) moveX = -1f;

        // Sağa-sola dönüş (Rotasyon ayarı)
        if (moveX > 0)
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        else if (moveX < 0)
            transform.localRotation = Quaternion.Euler(0, 180, 0);

        // Hızı Rigidbody'ye uygula (fiziksel hareket)
        rb.linearVelocity = new Vector2(moveX * speed, rb.linearVelocity.y);

        // Animasyon oynatma
        if (anim != null)
        {
            anim.speed = 1f; 
            
            // Kullanıcı isMoving parametresini ve geçişleri doğru şekilde kurduğu için
            // artık en sağlıklı yöntem olan SetBool'u kullanıyoruz:
            anim.SetBool("isMoving", moveX != 0);
        }

        // Zıplama Kontrolü (W ve Yukarı Yön tuşu)
        // Y eksenindeki hız sıfıra çok yakınsa(yerdeyse) zıplamaya izin ver
        if ((kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame) && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // Stoper pozisyon kontrolü — mouse Pos X >= 80 olduğunda durdur
        RectTransform rt = transform as RectTransform;
        float posX = rt != null ? rt.anchoredPosition.x : transform.position.x;
        if (!stoperTriggered && posX >= 80f)
        {
            stoperTriggered = true;
            canMove = false;
            rb.linearVelocity = Vector2.zero;

            // Animasyonu idle'a döndür
            if (anim != null)
                anim.SetBool("isMoving", false);

            // Arrow mouse location'ı kapat ve paneli aç
            if (manager != null)
                manager.DismissArrowMouseLocation();

            Debug.Log("[MouseMovementController] mouse X=80'e ulaştı — hareket durduruldu!");
        }
    }
}
