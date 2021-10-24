using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : SoldierController
{
    [SerializeField]
    private Camera cam;
    private Rigidbody rb;
    
    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float rotateSpeed = 3f;
    
    private Vector3 _rotation = Vector3.zero;
    private Vector3 _cameraRotation = Vector3.zero;
    private Vector3 _velocity = Vector3.zero;

    //private bool gunIsEnabled = true;

    [SerializeField]
    private GameObject gun;

    [SerializeField]
    private GameObject gunTarget;

    //Health
    // public int health = 100;
    // public int damage = 10;
    public float gotShotForce = 500f;

    public GameObject handGun;

    // Animation
    private Animator shootingAnimator;

    // Sound
    public AudioSource stepSound;
    public AudioSource ShootSound;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        handGun.SetActive(false);
        shootingAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 1 && health > 0)
        {
            if (gotShot)
                TakeDamage();
            UpdateMoveByKeyboard();
            UpdateRotateByMouse();
            if (hasGun)
            {
                handGun.SetActive(true);
                Shoot();
            }
        }

    }

    private void FixedUpdate()
    {
        PerformMoveAndRotate();
    }

    void PerformMoveAndRotate(){
        //Walk/Move
        if(_velocity!=Vector3.zero)
            rb.MovePosition(rb.position + _velocity * Time.fixedDeltaTime);

        //Rotate
        rb.MoveRotation(rb.rotation * Quaternion.Euler(_rotation));
        if(cam!=null)
            cam.transform.Rotate(-_cameraRotation);
    }

    void UpdateMoveByKeyboard(){
        // Calculate movement velocity as a 3d vector
        float _xMov = Input.GetAxisRaw("Horizontal");
        float _zMov = Input.GetAxisRaw("Vertical");
        Vector3 _movHor = transform.right*_xMov;
        Vector3 _movVer = transform.forward*_zMov;

        // Final movement velocity
        _velocity = (_movHor + _movVer).normalized * moveSpeed;

        // Sound
        if (_zMov < -0.1 || _zMov > 0.1 || _xMov < -0.1 || _xMov > 0.1)
            if (!stepSound.isPlaying)
                stepSound.Play();
        }

    void UpdateRotateByMouse(){
        //Calculate rotation as a 3d vector
        float yRot = Input.GetAxisRaw("Mouse X");
         // Final rotation vector
        _rotation = new Vector3(0f,yRot, 0f) * rotateSpeed;

        //Calculate rotation as a 3d vector
        float xRot = Input.GetAxisRaw("Mouse Y");

         // Final rotation vector
        if(xRot>-90 && xRot<90)
            _cameraRotation = new Vector3(xRot, 0f, 0f) * rotateSpeed;
    }

    void Shoot(){
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)){ //or right mouse click
            RaycastHit hit;
            if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit)){
                if (hit.collider.gameObject != gunTarget)
                {
                    gunTarget.transform.position = hit.point;
                    // Sound
                    if (!ShootSound.isPlaying)
                        ShootSound.Play();
                }                 
            }
            // Animation
            shootingAnimator.Play("ShootingAnimation");
            shootingAnimator.SetBool("shot", false);
        }
        
    }

    public void TakeDamage(){
        rb.AddForce (-transform.forward*gotShotForce, ForceMode.Force);
        this.health -= this.damage;
        // if (this.health <= 0)
            // DestroyMe();
            //Invoke(nameof(DestroyMe), .5f);
        gotShot = false;
    }
}
