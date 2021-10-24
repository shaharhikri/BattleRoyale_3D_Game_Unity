using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    int showFramesCount = 0;
    public int framesBeforeDissapearing = 30;
    private string otherTeamTag = "";

    //Line
    public GameObject handGun;
    private GameObject muzzle;

    private LineRenderer line;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = Vector3.zero;
        if(this.gameObject.tag.CompareTo("RedTeamBullet")==0)
            otherTeamTag = "BlueTeam";
        else if(this.gameObject.tag.CompareTo("BlueTeamBullet")==0)
            otherTeamTag = "RedTeam";
        
        line = handGun.GetComponent<LineRenderer>();
        muzzle = this.gameObject;
        //Remove line
        line.SetPosition(0,Vector3.zero);
        line.SetPosition(1,Vector3.zero);
    }

    private void FixedUpdate()
    {
        showFramesCount++;
        if(showFramesCount>=framesBeforeDissapearing){
            showFramesCount=0;
            transform.position = Vector3.zero;
            //Remove line
            if(line!=null){
                line.SetPosition(0,Vector3.zero);
                line.SetPosition(1,Vector3.zero);
            }
        }
        else{
            if(this.gameObject.transform.position!=Vector3.zero){
                //Draw Line
                if(line!=null){
                    line.SetPosition(0,handGun.transform.position);
                    line.SetPosition(1,muzzle.transform.position);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject target = other.gameObject;
        SoldierController soldier = target.GetComponent<SoldierController>();

        if(soldier!=null && target.tag.CompareTo(otherTeamTag)==0)
            soldier.gotShot = true;
    }
}
 
