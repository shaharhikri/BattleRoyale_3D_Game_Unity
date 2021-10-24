using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpGun : MonoBehaviour
{
    [SerializeField]
    public GameObject player;
    private SoldierController playerController;
    // Start is called before the first frame update
    void Start()
    {
        if(player!=null){
             playerController = player.GetComponent<SoldierController>();
        }
    }

    private void OnMouseDown()
    {
        if(playerController!=null && !playerController.hasGun){
            playerController.hasGun = true;
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject==player && !playerController.hasGun && Input.GetKey(KeyCode.Space)){
            playerController.hasGun = true;
            Destroy(this.gameObject);
        }
    }
}
