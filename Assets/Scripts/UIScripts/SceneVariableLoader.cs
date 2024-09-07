using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneVariableLoader : MonoBehaviour
{
    private Scene scene;
    public GameManager gameManager;
    public MirrorPlacement mirrorPlacement;
    
    private int mirrorsLeft;
    public MenuLoader menuLoader;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVariable()
    {
        scene = SceneManager.GetActiveScene();

        if (scene.name == "Help Scene")
        {
            gameManager.totalEnemies = 1;
            mirrorPlacement.maxMirrors = 3;
            mirrorsLeft = mirrorPlacement.maxMirrors - mirrorPlacement.mirrorsPlaced;
            gameManager.shotsRemaining = 3;
            Debug.Log(gameManager.totalEnemies);
            Debug.Log(mirrorsLeft);
            Debug.Log(gameManager.shotsRemaining);
            Cursor.visible = true;
            gameManager.shotNumber.text = "" + gameManager.shotsRemaining;
            gameManager.enemyNumber.text = "" + gameManager.totalEnemies;
            gameManager.mirrorNumber.text = "" + mirrorsLeft;
            menuLoader.UpdateSceneMenu();

        }
        else if (scene.name == "LevelOne")
        {
            gameManager.totalEnemies = 2;
            mirrorPlacement.maxMirrors = 3;
            mirrorsLeft = mirrorPlacement.maxMirrors - mirrorPlacement.mirrorsPlaced;
            gameManager.shotsRemaining = 3;
            Debug.Log("LevelOne Loaded");
            menuLoader.UpdateSceneMenu();
            Cursor.visible = true;
            gameManager.shotNumber.text = "" + gameManager.shotsRemaining;
            gameManager.enemyNumber.text = "" + gameManager.enemiesRemaining;
            gameManager.mirrorNumber.text = "" + mirrorsLeft;
        }
        else if (scene.name == "LevelTwo")
        {
            gameManager.totalEnemies = 3;
            mirrorPlacement.maxMirrors = 3;
            mirrorsLeft = mirrorPlacement.maxMirrors - mirrorPlacement.mirrorsPlaced;
            gameManager.shotsRemaining = 3;
            Debug.Log("LevelTwo Loaded");
            menuLoader.UpdateSceneMenu();
            Cursor.visible = true;
            gameManager.shotNumber.text = "" + gameManager.shotsRemaining;
            gameManager.enemyNumber.text = "" + gameManager.enemiesRemaining;
            gameManager.mirrorNumber.text = "" + mirrorsLeft;
        }
        else
        {
            Cursor.visible = false;
            Debug.Log("Mouse Hidden");
        }
    }
}
