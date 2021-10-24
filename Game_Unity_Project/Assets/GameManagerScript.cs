using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    public SoldierController player,friend, enemy1, enemy2;
    public Text playerHealthText;
    public GameObject enemyHasGunText_wrapper;
    public GameObject friendIsDeadText_wrapper;
    public GameObject pauseState;
    public GameObject resumeState;
    public GameObject endGamePanel;
    public GameObject gamePanel;
    public Text winLoseText;
    private bool pause = false;
    private int playerStartingHealth;

    // Start is called before the first frame update
    void Start()
    {
        playerStartingHealth = player.health;
        updatePlayerHealthText();
        ResumeGame();
        enemyHasGunText_wrapper.SetActive(false);
        friendIsDeadText_wrapper.SetActive(false);
        endGamePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        pauseResumeHandle();
        updatePlayerHealthText();
        UpdateEnemiesHasGunText();
        UpdateFriendIsDeadGunText();

        GameOverHandle();
    }

    //GameOver funcs
    private void GameOverHandle(){
        if(player.health<=0 || player.transform.position.y<=-50){
            SetupEndGamePanel(false);
        }
        else if(enemy1==null && enemy2==null){
            SetupEndGamePanel(true);
        }
    }

    private void SetupEndGamePanel(bool won){
        endGamePanel.SetActive(true);
        winLoseText.text = won==true?"You Won!":"You Lost!";
        winLoseText.color = won==true?Color.green:Color.red;
        Time.timeScale = 0;
        gamePanel.SetActive(false);
    }

    //Player Texts funcs
    private void UpdateEnemiesHasGunText(){
        if(enemy1.hasGun)
            enemyHasGunText_wrapper.SetActive(true);
    }

    private void UpdateFriendIsDeadGunText(){
        if(friend==null)
            friendIsDeadText_wrapper.SetActive(true);
    }

    private void updatePlayerHealthText(){
        int life = (int)(( ((float)(player.health)) / playerStartingHealth)*100);
        playerHealthText.text = "HP: "+(life>0?life:0)+"%";
    }

    //Pause&Resume funcs
    private void pauseResumeHandle(){
        if(Input.GetKeyDown(KeyCode.P) && !endGamePanel.activeSelf){
            if(pause)
                ResumeGame();
            else
                PauseGame();
            }
    }
    private void PauseGame(){
        pause = true;
        pauseState.SetActive(true);
        resumeState.SetActive(false);
        Time.timeScale = 0;
    }

    private void ResumeGame(){
        pause = false;
        resumeState.SetActive(true);
        pauseState.SetActive(false);
        Time.timeScale = 1;
    }
}
