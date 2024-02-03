using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIController : MonoBehaviour
{    
    // ** Startup ** //
    private GameController GameController;
    private GameObject currentPlayer;
    
    // Menu
    public GameObject backgroundImagePrefab;
    private GameObject backgroundImage;
    public Sprite menuBackgroundImage;
    public Sprite ingameBackgroundImage;
    public GameObject mainMenuButtonPrefab;
    private List<GameObject> mainMenuButtons = new List<GameObject>();
    public Sprite[] mainMenuButtonSprites;
    
    // Controls
    public GameObject controlButtonPrefab;
    public Sprite[] controlsImages;
    public Sprite[] controlsButtonImages;
    private int controlsIndex = 0;
    private GameObject controlsMainMenu;
    private GameObject controlsNext;
    private GameObject controlsPrev;

    // ** Game ** //

    // Pause Menu
    public GameObject pauseMenuPrefab;
    private GameObject pauseMenu;
    private GameObject resumeButton;
    private GameObject quitButton;
    private bool gameIsPaused = false;


    // Next state button
    public Sprite[] nextStateButtonImages;
    public GameObject nextStateButtonPrefab;
    private GameObject nextStateButton = null;
    private State currentState;
    
    // Dice
    public Sprite[] diceImages;
    public GameObject dicePrefab;
    private GameObject dice;
    private int diceRollNumber;
    private int movementPower;

    // Buy Troops
    public TMP_Text moneyTextPrefab;
    private TMP_Text moneyText;
    public Sprite[] buyImages;
    public GameObject buyButtonPrefab;
    private GameObject[] buyOptions;

    // Ghost Troop
    public GameObject ghostTroopPrefab;
    private GameObject ghostTroop;
    private bool ghostToMouse = false;
    private bool ghostToX = false;

    // Troops
    private GameObject selectedTroop;

    // Ranges
    public GameObject movementRangePrefab;
    public GameObject attackRangePrefab;
    private GameObject movementRange;
    private GameObject attackRange;

    // Win text
    public TMP_Text winTextPrefab;

    // ** Menu and Startup methods: ** //

    public void setGameController(GameController gm){
        GameController = gm;
    }

    public void setCurrentPlayer(GameObject p){
        currentPlayer = p;
    }

    void Start(){
        createMenuButtons();
        createControlsButtons();
    }

    private void createBackgroundImage(){
        backgroundImage = Instantiate(backgroundImagePrefab);
        backgroundImage.GetComponent<SpriteRenderer>().sprite = menuBackgroundImage;
    }

    private void createMenuButtons(){
        int index = 0;
        foreach(Sprite s in mainMenuButtonSprites){
            GameObject button = Instantiate(mainMenuButtonPrefab, GameObject.Find("Canvas").transform);
            int buttonIndex = index;
            button.GetComponent<Button>().onClick.AddListener(() => menuButtonClicked(buttonIndex));
            button.GetComponent<Image>().sprite = mainMenuButtonSprites[index];
            button.SetActive(true);
            mainMenuButtons.Add(button);
            index++;
        }

        Vector3 newPosition = new Vector3(0, -360, 0); // Lowest position
        Vector3 incrementVector = new Vector3(0, 230, 0); // Increase height by 200 (image is 189px in height)
        mainMenuButtons.Reverse();
        foreach(GameObject o in mainMenuButtons){
            o.GetComponent<RectTransform>().anchoredPosition = newPosition;
            newPosition += incrementVector;
        }
        mainMenuButtons.Reverse();
    }

    private void createControlsButtons(){
        controlsMainMenu = Instantiate(controlButtonPrefab, GameObject.Find("Canvas").transform);
        controlsMainMenu.transform.position = new Vector3(-830.0f, 400.0f, 0.0f);
        controlsMainMenu.GetComponent<Image>().sprite = controlsButtonImages[0];
        controlsMainMenu.GetComponent<Button>().onClick.AddListener(() => hideControls());
        
        controlsPrev = Instantiate(controlButtonPrefab, GameObject.Find("Canvas").transform);
        controlsPrev.transform.position = new Vector3(740.0f, 250.0f, 0.0f);
        controlsPrev.GetComponent<Image>().sprite = controlsButtonImages[2];
        controlsPrev.GetComponent<Button>().onClick.AddListener(() => prevControls());
        
        controlsNext = Instantiate(controlButtonPrefab, GameObject.Find("Canvas").transform);
        controlsNext.transform.position = new Vector3(886.0f, 250.0f, 0.0f);
        controlsNext.GetComponent<Image>().sprite = controlsButtonImages[1];
        controlsNext.GetComponent<Button>().onClick.AddListener(() => nextControls());
        
        hideControls();
    }

    private void menuButtonClicked(int index){
        switch(index){
            case 0: // New Game
                newGame();
                break;
            // case 1: // Load Game
            //     loadGame();
            //     break;
            case 1: // Controls
                showControls();
                break;
            case 2: // Quit
                quitGame();
                break;
        }
    }

    private void showMenu(){
        foreach(GameObject btn in mainMenuButtons){
            btn.SetActive(true);
        }
    }

    private void hideMenu(){
        foreach(GameObject btn in mainMenuButtons){
            btn.SetActive(false);
        }
    }

    private void newGame(){
        GameController.newGame();
        backgroundImage.GetComponent<SpriteRenderer>().sprite = ingameBackgroundImage;
    }

    private void showControls(){
        hideMenu();
        backgroundImage.GetComponent<SpriteRenderer>().sprite = controlsImages[controlsIndex];
        controlsMainMenu.SetActive(true);
        controlsNext.SetActive(true);
        controlsPrev.SetActive(true);
    }

    private void hideControls(){
        showMenu();
        backgroundImage.GetComponent<SpriteRenderer>().sprite = menuBackgroundImage;
        controlsMainMenu.SetActive(false);
        controlsNext.SetActive(false);
        controlsPrev.SetActive(false);
    }

    private void nextControls(){
        controlsIndex++;
        if(controlsIndex > controlsImages.Length-1) controlsIndex = controlsImages.Length-1;
        backgroundImage.GetComponent<SpriteRenderer>().sprite = controlsImages[controlsIndex];
    }

    private void prevControls(){
        controlsIndex--;
        if(controlsIndex < 0) controlsIndex = 0;
        backgroundImage.GetComponent<SpriteRenderer>().sprite = controlsImages[controlsIndex];
    }
    

    private void quitGame(){
        Debug.Log("game ended");
        Application.Quit();
    }

    // ** In-Game methods: ** //

    public void startGame(){ // run after scene changes to game scene
        hideMenu();
        createNextStateButton();
        createDice();
        createMoneyText();
        createBuyOptions();
        createGhostTroop();
        createRanges();
        changeState(State.Start);
        createPauseMenu();
    }

    private void createPauseMenu(){
        pauseMenu = Instantiate(pauseMenuPrefab, GameObject.Find("Canvas").transform);
        resumeButton = pauseMenu.transform.GetChild(0).gameObject;
        resumeButton.GetComponent<Button>().onClick.AddListener(() => unPause());
        quitButton = pauseMenu.transform.GetChild(1).gameObject;
        quitButton.GetComponent<Button>().onClick.AddListener(() => GameController.quitGame());
        unPause();
    }

    private void pause(){
        gameIsPaused = true;
        pauseMenu.SetActive(true);
        nextStateButton.SetActive(false);
        if(currentState == State.Buy) hideBuyOptions();
    }

    private void unPause(){
        gameIsPaused = false;
        pauseMenu.SetActive(false);
        nextStateButton.SetActive(true);
        if(currentState == State.Buy) showBuyOptions();
    }

    private void createNextStateButton(){
        // Debug.Log("creating button");
        nextStateButton = Instantiate(nextStateButtonPrefab, GameObject.Find("Canvas").transform);
        nextStateButton.GetComponent<Button>().onClick.AddListener(() => GameController.nextState());
    }

    public void changeState(State state){
        currentState = state;
        if(currentState != State.Menu)
            nextStateButton.GetComponent<Image>().sprite = nextStateButtonImages[(int) currentState-1]; // -1 because menu state at 0

        switch(currentState){
            case State.Menu:
                menuState();
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

    private void menuState(){
        createBackgroundImage();
        showMenu();
    }

    private void StartState(){
        // Debug.Log("Start state");
        if(selectedTroop != null) selectTroop(selectedTroop); // remove seleciton from ending your turn (pot. from prev player attack state)
    }

    private void RollState(){
        // Debug.Log("Roll state");
        StartCoroutine(rollDice());
    }

    private void BuyState(){
        // Debug.Log("Buy state");
        showMoneyText();
        showBuyOptions();
    }

    private void MoveState(){
        // Debug.Log("Move state");
        hideMoneyText();
        hideBuyOptions();
        if(selectedTroop != null) selectTroop(selectedTroop); // remove seleciton from buy state
        
    }

    private void AttackState(){
        // Debug.Log("Attack state");
        dice.SetActive(false); // TODO wat?
        hideMovementRange();
        enableAttackForAllTroops();
    }

    private void enableAttackForAllTroops(){
        foreach(GameObject t in currentPlayer.GetComponent<Player>().getTroops()){
            t.GetComponent<Troop>().enableAttacking();
        }
    }

    private void createDice(){
        dice = Instantiate(dicePrefab);
        dice.SetActive(false);
    }

    private void createMoneyText(){
        moneyText = Instantiate(moneyTextPrefab, GameObject.Find("Canvas").transform);
        hideMoneyText();
    }

    private void showMoneyText(){
        moneyText.text = "Money: " + currentPlayer.GetComponent<Player>().getMoney();
        moneyText.gameObject.SetActive(true);
        // Debug.Log("money="+currentPlayer.GetComponent<Player>().getMoney());
    }

    private void hideMoneyText(){
        moneyText.gameObject.SetActive(false);
    }

    private IEnumerator rollDice(){
        nextStateButton.SetActive(false); // Disable changing state during dice roll

        // move dice to center
        dice.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        dice.transform.localScale += new Vector3(0.8f, 0.8f, 0.0f);
        dice.SetActive(true);
        // Debug.Log("active");

        int randomCount = Random.Range(10,30);
        int randomRoll = 1;
        int prevRoll = 1;
        for(int i = 0; i < randomCount; i++){
            randomRoll = Random.Range(1,7);
            dice.GetComponent<SpriteRenderer>().sprite = diceImages[randomRoll-1];
            if(randomRoll != prevRoll) yield return new WaitForSeconds(0.05f); // Only delay if new image is being shown
            // Debug.Log(randomRoll);
        }
        diceRollNumber = randomRoll;
        movementPower = diceRollNumber;
        if(diceRollNumber == 6){
            currentPlayer.GetComponent<Player>().addMoney(50);
            StartCoroutine(showRewardMoney(50));
        } 
        yield return new WaitForSeconds(1);
        
        // move dice to bottom
        dice.transform.position = new Vector3(0.0f, -490.0f, 0.0f);
        dice.transform.localScale -= new Vector3(0.8f, 0.8f, 0.0f);
        
        // Debug.Log("done");

        nextStateButton.SetActive(true); // Enable changing state after dice roll
    }

    private void updateDiceImage(){
        if(diceRollNumber > 0) // if greater than 0
            dice.GetComponent<SpriteRenderer>().sprite = diceImages[diceRollNumber-1];
        else dice.SetActive(false);
    }

    private void increaseMovementPower(){
        movementPower++;
        if(movementPower > diceRollNumber) movementPower = diceRollNumber;
    }

    private void decreaseMovementPower(){
        movementPower--;
        if(movementPower < 1) movementPower = 1;
    }

    private void createRanges(){
        movementRange = Instantiate(movementRangePrefab);
        attackRange = Instantiate(attackRangePrefab);
        movementRange.SetActive(false);
        attackRange.SetActive(false);
    }

    private void setMovementRange(int multiplier){
        movementRange.transform.localScale = new Vector3(multiplier, multiplier, 0);
    }

    private void setAttackRange(int multiplier){
        attackRange.transform.localScale = new Vector3(multiplier, multiplier, 0);
    }

    private void showMovementRange(){
        movementRange.SetActive(true);
    }

    private void hideMovementRange(){
        movementRange.SetActive(false);
    }

    private void showAttackRange(){
        attackRange.SetActive(true);
        highlightEnemyWithinRange();
    }

    private void hideAttackRange(){
        attackRange.SetActive(false);
        unHighlightAllEnemies();
    }

    private void updateMovementRange(){
        if(movementPower < 1) {
            // if used up all movement power
            hideMovementRange();
            return;
        }
        movementRange.transform.position = selectedTroop.transform.position;
        setMovementRange(selectedTroop.GetComponent<Troop>().getMovementMultiplier()*movementPower);
        showMovementRange();
    }

    private void updateAttackRange(){
        attackRange.transform.position = selectedTroop.transform.position;
        setAttackRange(selectedTroop.GetComponent<Troop>().getAttackRange());
        showAttackRange();
    }

    private void updateAttackRange(GameObject t){
        attackRange.transform.position = t.transform.position;
        setAttackRange(selectedTroop.GetComponent<Troop>().getAttackRange());
        showAttackRange();
    }

    private void createGhostTroop(){
        ghostTroop = Instantiate(ghostTroopPrefab);
        ghostTroop.GetComponent<GhostTroop>().setMaterial();
    }

    private void createBuyOptions(){
        buyOptions = new GameObject[buyImages.Length];
        int index = 0;
        foreach(Sprite s in buyImages){
            GameObject newObject = Instantiate(buyButtonPrefab, GameObject.Find("Canvas").transform);
            newObject.GetComponent<Image>().sprite = s;

            // need to create a new variable equal to index otherwise clickedBuyOption always recieves the last index
            int objectIndex = index;
            newObject.GetComponent<Button>().onClick.AddListener(() => clickedBuyOption(objectIndex));
            
            buyOptions[index] = newObject;

            newObject = null;
            index++;
        }

        Vector3 newPosition = new Vector3(0, -370, 0); // Lowest position
        Vector3 incrementVector = new Vector3(0, 110, 0); // Increase height by 110 (image is 100px in height)
        System.Array.Reverse(buyOptions);
        foreach(GameObject o in buyOptions){
            o.GetComponent<RectTransform>().anchoredPosition = newPosition;
            newPosition += incrementVector;
        }
        System.Array.Reverse(buyOptions);
        hideBuyOptions();
    }

    private void showBuyOptions(){
        foreach(GameObject o in buyOptions){
            o.SetActive(true);
        }
    }

    private void hideBuyOptions(){
        foreach(GameObject o in buyOptions){
            o.SetActive(false);
        }
    }

    private void clickedBuyOption(int option){
        if(!GameController.canAfford(option)) return; // cant buy what you cant afford
        hideBuyOptions();
        selectedTroop = GameController.buyTroop(option);
        showMoneyText(); // updates money after buying troop
        selectedTroop.GetComponent<Troop>().setUIController(this);
        ghostTroop.GetComponent<GhostTroop>().updateSprite(selectedTroop.GetComponent<Troop>().getGhostSprite());
        ghostToMouse = true;
        ghostToX = true;
    }

    private IEnumerator showRewardMoney(int reward){
        
        moneyText.text = "Money: +" + reward;
        moneyText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1);
        
        hideMoneyText();
    }

    public void selectTroop(GameObject troop){
        if(selectedTroop != null && selectedTroop == troop){ // deselect troop
            hideAttackRange();
            hideMovementRange();
            selectedTroop.GetComponent<Troop>().deselectMe();
            selectedTroop = null;
        }
        else if(troop.GetComponent<Troop>().getPlayer() != currentPlayer){
            // if troop is opponent troop
            if(troop.GetComponent<Troop>().isBeingAttacked() && selectedTroop.GetComponent<Troop>().isAttackingEnabled()){
                // if troop is within attack range and ally troop has not attacked yet
                troop.GetComponent<Troop>().takeDamage(selectedTroop);
                int reward = 0;
                if(troop.GetComponent<Troop>().getHealth() <= 0)reward = troop.GetComponent<Troop>().getCost();
                else reward = troop.GetComponent<Troop>().getRewardMoney();
                currentPlayer.GetComponent<Player>().addMoney(reward);
                StartCoroutine(showRewardMoney(reward));
                selectedTroop.GetComponent<Troop>().disableAttacking(); // disable attacking

                // after attacking, deselect troop and unhighlight all
                hideAttackRange();
                hideMovementRange();
                selectedTroop.GetComponent<Troop>().deselectMe();
                selectedTroop = null;
            }
            
        }
        else{ // troop must be ally
            if(currentState == State.Attack && !troop.GetComponent<Troop>().isAttackingEnabled())
                return; // Cannot select troop that has already attacked in attack state.

            if(currentState == State.Move || currentState == State.Attack){
                if(selectedTroop != null){
                    if(selectedTroop == troop){
                        hideAttackRange();
                        hideMovementRange();
                        selectedTroop.GetComponent<Troop>().deselectMe();
                        selectedTroop = null;
                        
                    } else {
                        selectedTroop.GetComponent<Troop>().deselectMe();
                        selectedTroop = troop;
                        selectedTroop.GetComponent<Troop>().selectMe();
                    }
                }
                else {
                    selectedTroop = troop;
                    selectedTroop.GetComponent<Troop>().selectMe();
                }
            }
            else {
                // to remove selection if enableSelectTroop is false? currentstate move or attack?
                if(selectedTroop != null && selectedTroop == troop){
                    hideAttackRange();
                    hideMovementRange();
                    selectedTroop.GetComponent<Troop>().deselectMe();
                    selectedTroop = null;
                }
            }
        }
        
    }

    private bool mouseWithinTroopRadius(){
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseXZ = new Vector2(mousePos.x, mousePos.y);

        Vector3 troopPos = selectedTroop.transform.position;
        Vector2 troopXZ = new Vector2(troopPos.x,troopPos.y);

        // 125 is distance between troop and edge of circle when movement power and movement multiplier is 1
        int circleSize = 125*movementPower*selectedTroop.GetComponent<Troop>().getMovementMultiplier();

        // distance between troop and mouse has to be within circle size
        return Vector2.Distance(mouseXZ, troopXZ) < circleSize;
    }

    private void highlightEnemyWithinRange(){
        foreach(GameObject t in GameController.getEnemyPlayer().GetComponent<Player>().getTroops()){
            Vector3 troopPos = attackRange.transform.position;
            Vector2 troopXZ = new Vector2(troopPos.x,troopPos.y);

            Vector3 enemyPos = t.transform.position;
            Vector2 enemyXZ = new Vector2(enemyPos.x,enemyPos.y);

            // 125 is distance between troop and edge of circle when movement power and movement multiplier is 1
            int circleSize = 125*selectedTroop.GetComponent<Troop>().getAttackRange();

            // distance between troop and mouse has to be within circle size
            if(Vector2.Distance(troopXZ, enemyXZ)-50 < circleSize){ // -50 to account for size of troop
                t.GetComponent<Troop>().attackSelectMe();
            } else {
                t.GetComponent<Troop>().deselectMe();
            }
        }
    }

    private void unHighlightAllEnemies(){
        foreach(GameObject t in GameController.getEnemyPlayer().GetComponent<Player>().getTroops()){
            t.GetComponent<Troop>().deselectMe();
        }
    }

    public void attackBase(GameObject player){
        if(currentState == State.Attack && selectedTroop != null){
            if(currentPlayer.GetComponent<Player>().getName() != player.GetComponent<Player>().getName())
                player.GetComponent<Player>().takeDamage(selectedTroop.GetComponent<Troop>().getAttackPoints());
                if(player.GetComponent<Player>().getHealth() <= 0) return; // if player died dont continue
                StartCoroutine(showRewardMoney(player.GetComponent<Player>().getRewardMoney()));
                selectedTroop.GetComponent<Troop>().disableAttacking();
                selectTroop(selectedTroop); // deselect troop
        }
        
    }

    public void showWinner(){
        gameIsPaused = true; // disables void Update
        TMP_Text winText = Instantiate(winTextPrefab, GameObject.Find("Canvas").transform);
        winText.text = currentPlayer.GetComponent<Player>().getName() + " wins!";
    }

    void Update(){
        if(currentState != State.Menu){
            if(Input.GetKeyDown(KeyCode.Escape)){
                if(gameIsPaused){
                    unPause();
                } else {
                    pause();
                }
            }
            if(!gameIsPaused){
                if(ghostToMouse){
                    ghostTroop.GetComponent<GhostTroop>().show();
                    if(ghostToX) ghostTroop.GetComponent<GhostTroop>().moveToMouse(currentPlayer.GetComponent<Player>().getSpawnX());
                    else ghostTroop.GetComponent<GhostTroop>().moveToMouse();

                    updateAttackRange(ghostTroop);

                    if(Input.GetMouseButtonDown(0)){
                        // If left clicked during ghost troop enabled
                        selectedTroop.transform.position = ghostTroop.transform.position; // move selected troop to ghost position

                        // disable ghost
                        ghostToMouse = false;
                        ghostToX = false;
                        ghostTroop.GetComponent<GhostTroop>().hide();

                        if(currentState == State.Buy) {
                            // if ghost to buy was on
                            selectedTroop.GetComponent<Troop>().show(); // show newly bought troop
                            selectTroop(selectedTroop); // deselect troop setting its sprite colour to player1/2
                            showBuyOptions(); // back to buy menu
                        }

                        if(currentState == State.Move){
                            diceRollNumber -= movementPower; // remove dictance traveled from moving in move state
                            increaseMovementPower(); // set movement power to diceRollNumber i.e what is left (can be 0)
                            updateDiceImage();
                            selectedTroop.GetComponent<Troop>().updateHealthBarPosition();
                        }
                    }
                } else {
                    ghostTroop.GetComponent<GhostTroop>().hide();
                    hideAttackRange();
                }

                if(currentState == State.Move){
                    // during move state
                    if(selectedTroop != null) {
                        // if troop selected
                        updateMovementRange(); // display movement cirlce - move to select troop instead of update?
                        if((Input.GetKey("left shift") || Input.GetKey("right shift")) && mouseWithinTroopRadius()){
                            // if holding shift
                            ghostTroop.GetComponent<GhostTroop>().updateSprite(selectedTroop.GetComponent<Troop>().getGhostSprite());
                            ghostToMouse = true;
                        } else {
                            ghostToMouse = false;
                        }

                        // Increase / decrease movement power
                        if(Input.GetKeyDown("=") || Input.GetKeyDown("+"))
                            increaseMovementPower();
                        else if(Input.GetKeyDown("-"))
                            decreaseMovementPower();
                    }

                }
                if(currentState == State.Attack){
                    // during move state
                    if(selectedTroop != null) {
                        // if troop selected
                        updateAttackRange(); // display attack cirlce
                        if(Input.GetMouseButtonDown(0)){
                            // if clicked on screen during attack state, when a troop is selected
                            // only legal options of attack are opponent troops

                        }
                    }
                }
            }

        } else {
            // menu state
        }
    }
        
}
