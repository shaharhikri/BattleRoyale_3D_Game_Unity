using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Controller : SoldierController
{
    public NavMeshAgent agent;
    public GameObject masterSoldier=null;
    private SoldierController masterSoldierController=null;
    public float distanceFromMaster = 5f;
    private string otherTeamTag="BlueTeam";
    // public Transform player;
    private GameObject otherTeamSoldier;

    //Patroling
    public Vector3 walkPoint = new Vector3(1f,2f,3f);
    public bool walkPointSet;
    public float walkPointRange;
    private int walkPointFramesCount = 0;

    //Atacking
    public GameObject gunTarget; //bullet
    public float timeBetweenShooting = 1.5f;

    //States
    public float sightRange, attackRange;
    private bool playerInSightRange, playerInAttackRange;


    //Health
    // public int health = 100;
    // public int damage = 20;
    // public bool gotShot = false;
    public float gotShotForce = 500f;
    private Rigidbody rb;

    //GunSearching
    // public bool hasGun;
    private bool gunInSightRange;

    public GameObject handGun;

    public float pickUpDistance = 0.5f;

    private LineRenderer line;

    //Animator
    private Animator animator;
    private int moveState = 3;

    public GameObject debugTarget;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        hasGun = false;
        handGun.SetActive(false);
        line = handGun.GetComponent<LineRenderer>();
        if(masterSoldier!=null)
            masterSoldierController = masterSoldier.GetComponent<SoldierController>();
        if(this.gameObject.tag.CompareTo("BlueTeam")==0)
            otherTeamTag = "RedTeam";
        animator = GetComponentInChildren<Animator>();
        walkPoint = GetNavMeshRandomPoint(walkPoint, walkPointRange);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if(moveState!=2)
            removeMyShootingLine();
        animator.SetInteger("MoveState", moveState);

        if (gotShot)
        {
            TakeDamage();
            return;
        }
        else if(masterSoldier==null || masterSoldier==this.gameObject)
            NonSecondaryUpdate();
        else
            SecondaryUpdate();

    }

    private void NonSecondaryUpdate(){
        if(!hasGun){
            GameObject pickUpGun = getInSightRangeObjByTag("Gun");
            if(pickUpGun!=null){
                if((gameObject.transform.position-pickUpGun.transform.position).magnitude > pickUpDistance)
                    GoToGun(pickUpGun);
                else{
                    PickGun(pickUpGun);
                    hasGun = true;
                }
            }
            else
                Patroling();
        }
        else{
            if(otherTeamSoldier!=null){
                playerInSightRange = true;
                if((gameObject.transform.position-otherTeamSoldier.transform.position).magnitude <= attackRange)
                    playerInAttackRange = true;
                else
                    playerInAttackRange = false;
            }
            else{
                otherTeamSoldier = getInSightRangeObjByTag(otherTeamTag);
                playerInSightRange = false;
                playerInAttackRange = false;
            }

            if (!playerInSightRange && !playerInAttackRange)
                Patroling();
            else if(playerInSightRange && !playerInAttackRange)
                Chase(otherTeamSoldier);
            else if(playerInSightRange && playerInAttackRange)
                Attack(otherTeamSoldier);    
            
        }
    }

    private void SecondaryUpdate(){
        if(masterSoldierController!=null){
            if(!masterSoldierController.hasGun){
                FollowMaster();
            }

            else{
                hasGun = true;
                handGun.SetActive(true);
                if(otherTeamSoldier!=null && (gameObject.transform.position-otherTeamSoldier.transform.position).magnitude <= attackRange){
                    Attack(otherTeamSoldier);
                }
                else{
                    otherTeamSoldier = getInSightRangeObjByTag(otherTeamTag);
                    FollowMaster();
                }   
            }
        }
    }

    /*Before fiding a Gun*/

    void GoToGun(GameObject pickUpGun){
        agent.SetDestination(pickUpGun.transform.position);
        //moveState=?
    }

    void PickGun(GameObject pickUpGun){
        Destroy(pickUpGun);
        handGun.SetActive(true);
    }

    /*After fiding a Gun*/
    private void Patroling(){
        walkPointFramesCount++;
        
        Vector3 distanceToWalkPoint = gameObject.transform.position - walkPoint;
        if(distanceToWalkPoint.magnitude < 2f || walkPointFramesCount>=740){
            walkPoint = GetNavMeshRandomPoint(walkPoint, walkPointRange);
           
            //debug
            if(debugTarget!=null)
                debugTarget.transform.position = walkPoint;
            
            walkPointFramesCount = 0;
        }
        agent.SetDestination(walkPoint);

        moveState = 3;
    }

    private void Chase(GameObject player){
        agent.SetDestination(player.transform.position);
        moveState = 3;
    }

    private void Attack(GameObject player){
        
        if(health>0){
            //dont move
            agent.SetDestination(transform.position);
            transform.LookAt(player.transform);
            Invoke(nameof(Shoot), timeBetweenShooting);
            moveState = 2;
        }
    }

    //Secondary Soldier Chase Master
    void FollowMaster(){
            //follow master
            if((gameObject.transform.position-masterSoldier.transform.position).magnitude > distanceFromMaster){
                Chase(masterSoldier);
                moveState = 3;
            }
            else{
                agent.SetDestination(transform.position);
                moveState = 0;
            }
            
    }

    //===== 
    private Vector3 GetNavMeshRandomPoint(Vector3 center, float maxDistance) {
        // Get Random Point inside Sphere which position is center, radius is maxDistance
        Vector3 randomPos = Random.insideUnitSphere * maxDistance + center;

        NavMeshHit hit; // NavMesh Sampling Info Container

        // from randomPos find a nearest point on NavMesh surface in range of maxDistance
        NavMesh.SamplePosition(randomPos, out hit, maxDistance, NavMesh.AllAreas);

        Vector3 res = hit.position;
        //Debug.Log(res);
        if (res.x == Mathf.Infinity || res.y == Mathf.Infinity || res.z == Mathf.Infinity)
        {
            Debug.Log("######################### "+res + " #########################");
            res = GetNavMeshRandomPoint(center, maxDistance);
        }
        return res;
    }

    void Shoot(){
        RaycastHit hit;
        if(Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out hit)){
            if(hit.collider.gameObject!=gunTarget && hit.collider.gameObject.tag.CompareTo(this.gameObject.tag)!=0)
                gunTarget.transform.position = hit.point;
        }
    }

    public void TakeDamage(){
        //Remove Line
        this.health -= this.damage;
        rb.AddForce(-transform.forward*gotShotForce, ForceMode.Force);
        moveState = -1;
        if (this.health <= 0){
            DestroyMe();
        }
        gotShot = false;
    }

    private void  DestroyMe(){
        moveState = -2;
        Destroy(this.gameObject, 4);
    }

    private GameObject getInSightRangeObjByTag(string tagName){
        Collider[] colliders = Physics.OverlapSphere(transform.position, sightRange);
        foreach(Collider c in colliders){
            if(c.gameObject.tag.CompareTo(tagName)==0){
                return c.gameObject;
            }
        }
        return null;
    }

    public void removeMyShootingLine(){
        line.SetPosition(0,Vector3.zero);
        line.SetPosition(1,Vector3.zero);
    }
}