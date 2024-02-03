using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private GameController GameController;
    private UIController UIController;
    private string playerName;
    private int money;
    private int spawnX;


    // Health bar
    public int maxHealth;
    private int currentHealth;
    public GameObject healthBarPrefab;
    private GameObject healthBar;

    private List<GameObject> troops = new List<GameObject>();
    private int troopCount = 0;

    public void setName(string n){
        playerName = n;
    }

    public void setUIController(UIController uic){
        UIController = uic;
    }

    public void setGameController(GameController gc){
        GameController = gc;
    }

    void Start(){
        setHealth();
    }

    private void createHealthBar(){
        healthBar = Instantiate(healthBarPrefab);
        healthBar.GetComponent<HealthBar>().setTroop(gameObject);
        healthBar.GetComponent<HealthBar>().setHealth(currentHealth/maxHealth);
        healthBar.GetComponent<HealthBar>().updatePosition();
    }

    public void setMoney(int m){
        money = m;
    }

    public void setHealth(){
        currentHealth = maxHealth;
        createHealthBar();
    }

    public int getHealth(){
        return currentHealth;
    }

    private float getHealthPercent(){
        return (float) currentHealth / (float) maxHealth;
    }

    public void updateHealth(){
        healthBar.GetComponent<HealthBar>().updateHealth(getHealthPercent());
    }

    public int getMoney(){
        return money;
    }

    public string getName(){
        return playerName;
    }

    public void setSpawnX(int x){
        spawnX = x;
    }

    public int getSpawnX(){
        return spawnX;
    }

    public List<GameObject> getTroops(){
        return troops;
    }

    public void addTroop(GameObject newTroop){
        troops.Add(newTroop);
        troopCount++;
    }

    void OnMouseDown(){
        UIController.attackBase(gameObject);
    }

    public void takeDamage(int dmg){
        currentHealth -= dmg;
        if(currentHealth <= 0) GameController.lostGame(gameObject);
        updateHealth();
    }

    public void removeTroop(GameObject t){
        troops.Remove(t);
    }

    public void addMoney(int m){
        money += m;
    }

    public int getRewardMoney(){
        return 40;
    }
}
