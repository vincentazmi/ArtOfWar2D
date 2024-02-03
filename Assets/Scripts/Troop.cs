using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Troop : MonoBehaviour
{
    private UIController UIController;

    // Troop sprites
    public Sprite player1Sprite;
    public Sprite player2Sprite;
    public Sprite selectedSprite;
    public Sprite attackingSprite;
    public Sprite defaultSprite;

    private GameObject player;


    // Troop attributes
    public int cost;
    public int maxHealth;
    private int currentHealth;
    public int movementMultiplier;
    public int attackPoints;
    public int attackRange;

    private bool attackingEnabled = true;
    private bool beingAttacked;

    public GameObject healthBarPrefab;
    private GameObject healthBar;

    void Awake(){
        currentHealth = maxHealth;
        // Debug.Log("currentHealth= "+currentHealth);
        createHealthBar();
    }

    public void show(){
        gameObject.SetActive(true);
        healthBar.SetActive(true);
        updateHealthBarPosition();
    }

    public void hide(){
        gameObject.SetActive(false);
        healthBar.SetActive(false);
    }

    private void createHealthBar(){
        healthBar = Instantiate(healthBarPrefab);
        healthBar.GetComponent<HealthBar>().setTroop(gameObject);
        healthBar.GetComponent<HealthBar>().setHealth(currentHealth/maxHealth);
        updateHealthBarPosition();
    }

    public void updateHealthBarPosition(){
        healthBar.GetComponent<HealthBar>().updatePosition();
    }

    public void setUIController(UIController uic){
        UIController = uic;
    }

    public void setPlayer(GameObject p){
        player = p;
    }

    public GameObject getPlayer(){
        return player;
    }

    public int getCost(){
        return cost;
    }

    public Sprite getGhostSprite(){
        // Debug.Log(defaultSprite == null);
        return defaultSprite;
    }

    public int getMovementMultiplier(){
        return movementMultiplier;
    }

    public int getAttackRange(){
        return attackRange;
    }

    public void selectMe(){
        beingAttacked = false;
        // Debug.Log("select me");
        GetComponent<SpriteRenderer>().sprite = selectedSprite;
    }

    public void attackSelectMe(){
        beingAttacked = true;
        // Debug.Log("attack me");
        GetComponent<SpriteRenderer>().sprite = attackingSprite;
    }

    public void deselectMe(){
        beingAttacked = false;
        // Debug.Log("deselect me");
        if(player.GetComponent<Player>().getName() == "Player 1") GetComponent<SpriteRenderer>().sprite = player1Sprite;
        else GetComponent<SpriteRenderer>().sprite = player2Sprite;
    }

    public bool isBeingAttacked(){
        return beingAttacked;
    }

    void OnMouseDown(){
        UIController.selectTroop(gameObject);
        // Debug.Log("clicked me");
    }

    public bool isAttackingEnabled(){
        return attackingEnabled;
    }

    public void enableAttacking(){
        attackingEnabled = true;
    }

    public void disableAttacking(){
        attackingEnabled = false;
    }

    public int getAttackPoints(){
        return attackPoints;
    }

    public void takeDamage(GameObject troop){
        currentHealth -= troop.GetComponent<Troop>().getAttackPoints();
        if(currentHealth <= 0) {
            player.GetComponent<Player>().removeTroop(gameObject);
            hide();
            return;
        }
        healthBar.GetComponent<HealthBar>().updateHealth(getHealthPercent());
        
    }

    public int getHealth(){
        return currentHealth;
    }

    public float getHealthPercent(){
        // Debug.Log(currentHealth+" "+maxHealth+" "+(currentHealth/maxHealth*100));
        return (float) currentHealth / (float) maxHealth;
    }

    public int getRewardMoney(){
        return (int) (cost*0.1);
    }

}
