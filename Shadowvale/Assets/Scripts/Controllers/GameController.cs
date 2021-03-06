using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;
using System.IO;
public class GameController : MonoSingleton<GameController>
{
    public enum GameState
    { 
        select,
        build,
        direct,
        destroy
    }
    public bool loadGame = true, dayCycle = false;
    public GameState gameState;
    private Spawner spawner;
    private Save save;
    private Load load;
    public Camera gameCam;
    public float camSpeed = 50, camDist = 10, camMaxZoom = 20, camMinZoom = 5;
    public LayerMask tileMask, selectMask, directMask;
    public bool camFocused = false;
    public Cooldown dayTimer = new Cooldown(30);
    public Light2D worldLight;
    public bool day = true, paused = false;
    public GameObject pauseMenu;
    void Start()
    {
        Debug.Log("STARTED");
        load = GetComponent<Load>();
        spawner = Spawner.Instance;
        if (!loadGame || !load.LoadGame())
        {
            GridBuilder.Instance.Generate();
            spawner.Setup();
            spawner.SpawnFollower(new Vector3(Grid.startPos.x, Grid.startPos.y - 1, 0));
            spawner.SpawnHome(Grid.tiles[Grid.startPos.x, Grid.startPos.y]);
            gameCam.transform.position = new Vector3(Grid.startPos.x, Grid.startPos.y, gameCam.transform.position.z);
        }

        //Cursor.lockState = CursorLockMode.Confined;
        save = GetComponent<Save>();
    }

    private void Update()
    {
        if (!paused)
        {
            ClickControl();
            CameraControl();
            if (dayCycle && dayTimer.Tick())
            {
                dayTimer.Reset();
                StartCoroutine(DayFade());
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                Unpause();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Exit()
    {
        save.SaveGame();
        ResetStatics();
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void Pause()
    {
        Time.timeScale = 0;
        paused = true;
        pauseMenu.SetActive(true);
    }

    public void Unpause()
    {
        Time.timeScale = 1;
        paused = false;
        pauseMenu.SetActive(false);
    }

    void ResetStatics()
    {
        Buildings.Reset();
        Creatures.Reset();
        Enemies.Reset();
        Followers.Reset();
        Grid.Reset();
        Resources.Reset();
    }

    IEnumerator DayFade()
    {
        Debug.Log("STARTING FADE");
        for (int i = 0; i < Followers.followers.Count; i++)
        {
            Followers.followers[i].StartCoroutine(Followers.followers[i].LightFade(day));
        }
        while ((day) ? worldLight.intensity > 0.05f : worldLight.intensity < 1)
        {
            worldLight.intensity = (day) ? worldLight.intensity - (0.25f * Time.deltaTime) : worldLight.intensity + (0.25f * Time.deltaTime);
            yield return null;
        }
        day = !day;
    }

    void ClickControl()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        if (gameState == GameState.build || gameState == GameState.destroy)
        {
            Cursor.visible = false;
            // Select tile below cursor
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, 0, tileMask);

            if (hit && !Grid.IsSelected(hit.transform.position))
            {
                if (gameState == GameState.build && spawner.buildings[spawner.selectedTemplate].type == Build.Type.multi)
                {
                    Grid.SelectTiles(hit.transform.position, spawner.buildings[spawner.selectedTemplate]);
                }
                else
                {
                    Grid.SelectTile(hit.transform.position, spawner.buildings[spawner.selectedTemplate]);
                }
            }
        }
        else
        {
            Cursor.visible = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (gameState == GameState.build)
            {
                // Place building on selected tile
                spawner.BuildStructure();
                Grid.SelectTile(Grid.selectedTiles[0].transform.position, spawner.buildings[spawner.selectedTemplate]);
            }
            else if (gameState == GameState.destroy)
            {
                if (Grid.selectedTiles[0].structure is Building && !(Grid.selectedTiles[0].structure is HomeBase))
                {
                    (Grid.selectedTiles[0].structure as Building).Destroy();
                    Destroy(Grid.selectedTiles[0].structure.gameObject);
                }
            }
            else
            {
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, 0, selectMask);
                if (hit.collider == null)
                {
                    if (!Inspector.MouseOver())
                    {
                        Followers.Deselect();
                        Buildings.Deselect();
                        gameState = GameState.select;
                        Inspector.Disable();
                    }
                }
                else
                {
                    Interaction target = hit.collider.GetComponent<Interaction>();
                    Inspector.Enable(target);
                    // Select either the follower or building in clicked position
                    if (target is Follower)
                    {
                        Followers.Select(target as Follower);
                        Buildings.Deselect();
                        gameState = GameState.direct;
                        return;
                    }
                    else if (target is Building)
                    {
                        Buildings.Select(hit.collider.gameObject);
                        return;
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (gameState == GameState.build || gameState == GameState.destroy)
            {
                gameState = GameState.select;
                Grid.DeselectTile();
            }
            else if (gameState == GameState.direct)
            {
                // Direct target follower
                GameObject targetObj = null;
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, 0, directMask);

                if (hit.collider != null)
                {
                    targetObj = hit.collider.gameObject;
                }

                Followers.Direct(mousePos2D, targetObj);
            }
        }
    }

    void CameraControl()
    {
        Vector2 mousePos = Input.mousePosition;

        if (Input.GetKeyDown(KeyCode.H) && !camRecentering)
        {
            StartCoroutine(RecenterCam());
        }

        if (Input.GetKeyDown(KeyCode.F) && !camRecentering && Followers.selected != null)
        {
            StartCoroutine(FocusCam());
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f && gameCam.orthographicSize > camMinZoom) // forward
        {
            gameCam.orthographicSize--;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && gameCam.orthographicSize < camMaxZoom) // backwards
        {
            gameCam.orthographicSize++;
        }

        if ((mousePos.x <= 10 && Input.GetAxis("Mouse X") < 0) || (mousePos.x >= Screen.width - 10 && Input.GetAxis("Mouse X") > 0))
        {
            Vector3 newPos = new Vector3(gameCam.transform.position.x + (Input.GetAxis("Mouse X") * (camSpeed * Time.deltaTime)), gameCam.transform.position.y, -camDist);
            gameCam.transform.position = newPos;
            camFocused = false;
        }

        if ((mousePos.y <= 10 && Input.GetAxis("Mouse Y") < 0) || (mousePos.y >= Screen.height - 10 && Input.GetAxis("Mouse Y") > 0))
        {
            Vector3 newPos = new Vector3(gameCam.transform.position.x, gameCam.transform.position.y + (Input.GetAxis("Mouse Y") * (camSpeed * Time.deltaTime)), -camDist);
            gameCam.transform.position = newPos;
            camFocused = false;
        }

        if (camFocused && Followers.selected != null)
        {
            gameCam.transform.position = Vector3.MoveTowards(gameCam.transform.position, new Vector3(Followers.selected.transform.position.x, Followers.selected.transform.position.y, gameCam.transform.position.z), Time.deltaTime * camSpeed);
        }
    }

    bool camRecentering = false;
    IEnumerator RecenterCam()
    {
        camRecentering = true;
        Vector3 targetPos = new Vector3(Grid.startPos.x, Grid.startPos.y, gameCam.transform.position.z);
        while (gameCam.transform.position != targetPos)
        {
            gameCam.transform.position = Vector3.MoveTowards(gameCam.transform.position, targetPos, (camSpeed * 2) * Time.deltaTime);
            yield return null;
        }
        camRecentering = false;
    }

    IEnumerator FocusCam()
    {
        camRecentering = true;
        Vector3 targetPos = new Vector3(Followers.selected.transform.position.x, Followers.selected.transform.position.y, gameCam.transform.position.z);
        while (gameCam.transform.position != targetPos)
        {
            gameCam.transform.position = Vector3.MoveTowards(gameCam.transform.position, targetPos, (camSpeed * 2) * Time.deltaTime);
            yield return null;
        }
        camRecentering = false;
        camFocused = true;
    }

    public void GameOver()
    {
        if (System.IO.File.Exists(Application.persistentDataPath + "/" + Save.file + ".json"))
        {
            Debug.Log("DELETING");
            File.Delete(Application.persistentDataPath + "/" + Save.file + ".json");
        }
        ResetStatics();
        SceneManager.LoadScene(0);
    }
}
