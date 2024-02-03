using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject UIManager;

    // Scenes
    public Scene mainMenuScene;
    public Scene gameScene;

    // State
    private State[] stateList = new State[]{State.Menu, State.Start, State.Roll, State.Buy, State.Move, State.Attack};
    private int stateIndex = 0;
    private State currentState;

    // Players
    public GameObject playerPrefab;
    private GameObject p1;
    private GameObject p2;
    private GameObject currentPlayer;

    // Troops
    public GameObject[] troopsPrefabs;

    // Start is called before the first frame update
    void Start()
    {
        UIManager = Instantiate(UIManager);
        UIManager.GetComponent<UIController>().setGameController(this);
        changeState(State.Menu);
    }

    


    public void newGame(){
        // Debug.Log("game started");
        createPlayers();
        UIManager.GetComponent<UIController>().startGame();
        changeState(State.Start);
    }

    private void createPlayers(){
        p1 = Instantiate(playerPrefab);
        p1.transform.position = new Vector3(-869.0f, 0.0f, 0.0f);
        p1.GetComponent<Player>().setName("Player 1");
        p1.GetComponent<Player>().setMoney(1000);
        p1.GetComponent<Player>().setSpawnX(-720);
        p1.GetComponent<Player>().setUIController(UIManager.GetComponent<UIController>());
        p1.GetComponent<Player>().setGameController(this);
        

        p2 = Instantiate(playerPrefab);
        p2.transform.position = new Vector3(870.0f, 0.0f, 0.0f);
        p2.GetComponent<Player>().setName("Player 2");
        p2.GetComponent<Player>().setMoney(1000);
        p2.GetComponent<Player>().setSpawnX(720);
        p2.GetComponent<Player>().setUIController(UIManager.GetComponent<UIController>());
        p2.GetComponent<Player>().setGameController(this);

        currentPlayer = p2;
        // UIManager.GetComponent<UIController>().setCurrentPlayer(currentPlayer);
    }

    public void changeState(State s){
        currentState = s;
        stateIndex = (int) currentState;

        UIManager.GetComponent<UIController>().changeState(currentState);

        switch(currentState){
            case State.Menu:
                break;

            case State.Start:
                StartState();
                break;

            case State.Roll:
                RollState();
                break;

            case State.Buy:
                BuyState();
                break;

            case State.Move:
                MoveState();
                break;
            
            case State.Attack:
                AttackState();
                break;
        }
    }

    public void nextState(){ // ingame states
        stateIndex++;
        if(stateIndex > 5) stateIndex = 1;
        currentState = stateList[stateIndex];
        UIManager.GetComponent<UIController>().changeState(currentState);

        switch(currentState){
            case State.Start:
                StartState();
                break;

            case State.Roll:
                RollState();
                break;

            case State.Buy:
                BuyState();
                break;

            case State.Move:
                MoveState();
                break;
            
            case State.Attack:
                AttackState();
                break;
        }
    }

    private void StartState(){
        // Debug.Log("Start state");
        // Debug.Log(currentPlayer.GetComponent<Player>().getName() + " " + p1.GetComponent<Player>().getName());
        if(currentPlayer.GetComponent<Player>().getName() == p1.GetComponent<Player>().getName())
            currentPlayer = p2;
        else currentPlayer = p1;
        Debug.Log(currentPlayer.GetComponent<Player>().getName()+"'s turn");
        UIManager.GetComponent<UIController>().setCurrentPlayer(currentPlayer);
    }

    private void RollState(){
        Debug.Log("Roll state");
    }

    private void BuyState(){
        Debug.Log("Buy state");
    }

    private void MoveState(){
        Debug.Log("Move state");
    }

    private void AttackState(){
        Debug.Log("Attack state");
    }

    public GameObject getCurrentPlayer(){
        return currentPlayer;
    }

    public bool canAfford(int index){
        return currentPlayer.GetComponent<Player>().getMoney() >= troopsPrefabs[index].GetComponent<Troop>().getCost();
    }

    public GameObject buyTroop(int index){
        GameObject newTroop = Instantiate(troopsPrefabs[index]);
        currentPlayer.GetComponent<Player>().addMoney(-newTroop.GetComponent<Troop>().getCost());
        // newTroop.GetComponent<Troop>().createHealthBar();
        if(currentPlayer.GetComponent<Player>().getName() == p1.GetComponent<Player>().getName())
            newTroop.GetComponent<Troop>().setPlayer(currentPlayer);
        else newTroop.GetComponent<Troop>().setPlayer(currentPlayer);
        newTroop.GetComponent<Troop>().hide();
        currentPlayer.GetComponent<Player>().addTroop(newTroop);
        return newTroop;
    }

    public GameObject getEnemyPlayer(){
        if(currentPlayer.GetComponent<Player>().getName() == p1.GetComponent<Player>().getName())
            return p2;
        else return p1;
    }

    public void lostGame(GameObject player){
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(currentPlayer);
        DontDestroyOnLoad(UIManager);

        if(player.GetComponent<Player>().getName() == p1.GetComponent<Player>().getName()){
            // display p2 wins
            SceneManager.LoadScene("Win");
            SceneManager.sceneLoaded += OnLoadWinScene;
        } else {
            // display p1 wins
            SceneManager.LoadScene("Win");
            SceneManager.sceneLoaded += OnLoadWinScene;
            
        }
    }

    private void OnLoadWinScene(Scene scene, LoadSceneMode mode){
        if(scene.name == "Win"){
            UIManager.GetComponent<UIController>().showWinner();
            quitGame();
        }
    }

    public void quitGame(){
        Application.Quit();
    }

    public void lostTroop(GameObject player, GameObject troop){
        // reward money for defeating troop
        if(player.GetComponent<Player>().getName() == p1.GetComponent<Player>().getName()){
            p2.GetComponent<Player>().addMoney(troop.GetComponent<Troop>().getCost()); 
        } else {
            p1.GetComponent<Player>().addMoney(troop.GetComponent<Troop>().getCost());
        }
    }
}
