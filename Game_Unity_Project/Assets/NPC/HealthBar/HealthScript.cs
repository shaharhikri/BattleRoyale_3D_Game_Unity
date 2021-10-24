using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    public GameObject soldier;
    public GameObject redBar;
    public GameObject text;
    public GameObject player;
    private SoldierController soldierController;
    private TextMesh textMesh;

    private float startingLen;

    private float startingHealth;

    

    // Start is called before the first frame update
    void Start()
    {
        soldierController = soldier.GetComponent<SoldierController>();
        startingLen = redBar.transform.localScale.x;
        startingHealth = soldierController.health;
        textMesh = text.GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        if(soldierController.health==0)
            Destroy(this.gameObject);
        else{
            float c = ((float)(soldierController.health))/startingHealth;
            c = c>0?c:0;
            float lastX = redBar.transform.localScale.x;
            float x = startingLen*c;
            float y = redBar.transform.localScale.y;
            float z = redBar.transform.localScale.z;
            redBar.transform.localScale = new Vector3(x,y,z);

            // float x_p = redBar.transform.position.x-(0.5f*Mathf.Abs(lastX-x));
            // float y_p = redBar.transform.position.y;
            // float z_p = redBar.transform.position.z;
            // redBar.transform.position += new Vector3((0.5f*Mathf.Abs(lastX-x)),0,0);

            this.gameObject.transform.LookAt(player.transform);

            textMesh.text = (int)(c*startingHealth)+"%";
        }
    }
}
