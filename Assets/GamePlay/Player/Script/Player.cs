using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator ani;
    public Rigidbody rb;
    public bool jump = false;
    float speed;
    public bool stop = false;
    [SerializeField] private float walkSpeed = 0.2f;
    [SerializeField] private float Runspeed = 2f;
    void Start()
    {
        // Gán Rigidbody và Animator nếu chúng chưa được gán trong Inspector
        if (ani == null) ani = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    // Sử dụng FixedUpdate cho các lệnh vật lý (như Rigidbody.MovePosition)
    void FixedUpdate()
    {
        
    }
    private void Update()
    {
        Run();
        
        
            StartCoroutine(Dropping());
        
        
    }
    public void Run()
    {
        if (stop == true) return;
        float ipHorizontal = Input.GetAxis(axisName: "Horizontal");
        float ipVertical = Input.GetAxis(axisName: "Vertical");

        // 1. Tính toán Vector Di chuyển Thô dựa trên Input
        Vector3 movementInput = new Vector3(ipHorizontal, y: 0, ipVertical);

        if (movementInput.magnitude > 1)
        {
            movementInput.Normalize();
        }

        // --- Bắt đầu phần thay đổi quan trọng ---

        // 2. Chuyển đổi Vector di chuyển từ World Space sang Local Space
        // Lấy hướng Tiến (Forward) và hướng Phải (Right) của chính Player
        Vector3 forwardMovement = transform.forward * movementInput.z;
        Vector3 rightMovement = transform.right * movementInput.x;

        // Cộng hướng di chuyển lại và bỏ qua trục Y
        Vector3 finalMovement = forwardMovement + rightMovement;
        finalMovement.y = 0; // Giữ nguyên trục Y (chỉ di chuyển theo mặt phẳng)

        // 3. Áp dụng di chuyển vật lý
        // Sử dụng Time.deltaTime thay vì Time.fixedDeltaTime nếu Run() được gọi trong Update(), 
        // nhưng vì MovePosition cần FixedUpdate nên ta giữ nguyên FixedDeltaTime
        rb.MovePosition(rb.position + finalMovement * speed * Time.fixedDeltaTime);

        // --- Kết thúc phần thay đổi quan trọng ---

        // Logic Animation và Tốc độ
        ani.SetBool("Walk", ipHorizontal != 0 || ipVertical != 0);

        if (Input.GetKeyDown(KeyCode.Space) && jump == true)
        {
            ani.SetTrigger("jump");
            rb.AddForce(new Vector3(0, 9, 0) * 5, ForceMode.Impulse);
            jump = false;
        }

        if (Input.GetKey(KeyCode.LeftShift) && (ipHorizontal != 0 || ipVertical != 0))
        {
            ani.SetBool("Run", true);
            speed = Runspeed;
        }
        else
        {
            speed = walkSpeed;
            ani.SetBool("Run", false);
        }
    }
   

    // Logic Nhảy/Tiếp đất
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jump = true;
        }
    }
    public IEnumerator Dropping()
    {
        if (Input.GetKeyDown(KeyCode.F) && stop == false)
        {
            ani.SetTrigger("Drop");
            stop = true;
            yield return new WaitForSeconds(0.3f);
            stop = false;
        }
    }
}