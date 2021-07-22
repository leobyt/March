using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public bool gameover = false;
    [HideInInspector] public bool nextTurn = false;
    public GameObject[] players;
    public GameObject[] playersUI;
    public enum GamePlay { turn, arena };
    public GamePlay gamePlay;
    public UIToast toast;
    int currentPlayer = 0;
    Coroutine coroutine;
    SwitchManager switchManager;
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = Settings.volume;
        switchManager = GetComponent<SwitchManager>();
        gameover = false;

        for (int i = Settings.numberOfPlayers; i < players.Length; i++)
        {
            players[i].SetActive(false);
            playersUI[i].SetActive(false);
        }

        switch (gamePlay)
        {
            case GamePlay.turn:
                for (int i = 0; i < Settings.numberOfPlayers; i++)
                {
                    players[i].GetComponent<Robot>().switchKey = "Switch1";
                    players[i].GetComponent<Robot>().playerIndex = i;
                }
                switchManager.SpawnBattery();
                Turn(players[currentPlayer]);
                break;
            case GamePlay.arena:
                for (int i = 0; i < Settings.numberOfPlayers; i++)
                {
                    players[i].GetComponent<Robot>().switchKey = "Switch" + (i + 1).ToString();
                    players[i].GetComponent<Robot>().playerIndex = i;
                }
                switchManager.SpawnBattery();
                ArenaTurn();
                break;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            SceneManager.LoadScene("Game");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Menu");
        }

        if (Input.GetButtonDown("Switch1"))
        {
            coroutine = StartCoroutine("Quit", 4);
        }

        if (Input.GetButtonUp("Switch1"))
        {
            StopCoroutine(coroutine);
        }

        switch (gamePlay)
        {
            case GamePlay.turn:
                if (nextTurn)
                {
                    if (gameover == false && switchManager.battery.transform.position.y <= -2)
                    {
                        switchManager.SpawnBattery();
                    }

                    nextTurn = false;
                    currentPlayer++;

                    if (currentPlayer == players.Length
                    || players[currentPlayer].activeSelf == false)
                    {
                        currentPlayer = 0;
                    }

                    Turn(players[currentPlayer]);
                }
                break;
            case GamePlay.arena:
                if (NextTurn())
                {
                    if (gameover == false && switchManager.battery.transform.position.y <= -2)
                    {
                        switchManager.SpawnBattery();
                    }

                    ArenaTurn();
                }
                if (PlayersReady())
                {
                    for (int i = 0; i < Settings.numberOfPlayers; i++)
                    {
                        var phase = players[i].GetComponent<Robot>().phase;
                        players[i].GetComponent<Robot>().phase = NextPhase(phase);
                    }
                }
                break;

            default:
                break;
        }
    }

    public void SetText(string text, int playerIndex)
    {
        playersUI[playerIndex].transform.GetChild(2).GetComponent<Text>().text = text;
    }

    public void Turn(GameObject player)
    {
        foreach (var p in players)
        {
            if (p.name != player.name)
            {
                p.GetComponent<Robot>().phase = Robot.Phase.wait;
            }
        }

        player.GetComponent<Robot>().phase = Robot.Phase.direction;
    }

    public void ArenaTurn()
    {
        foreach (var p in players)
        {
            p.GetComponent<Robot>().phase = Robot.Phase.direction;
        }
    }

    public bool NextTurn()
    {
        var count = 0;

        for (int i = 0; i < Settings.numberOfPlayers; i++)
        {
            if (players[i].GetComponent<Robot>().phase == Robot.Phase.wait)
            {
                count++;
            }
        }

        if (count == Settings.numberOfPlayers)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool PlayersReady()
    {
        var count = 0;

        for (int i = 0; i < Settings.numberOfPlayers; i++)
        {
            if (players[i].GetComponent<Robot>().isReady)
            {
                count++;
            }
        }

        if (count == Settings.numberOfPlayers)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Robot.Phase NextPhase(Robot.Phase phase)
    {
        switch (phase)
        {
            case Robot.Phase.direction:
                return Robot.Phase.power;
            case Robot.Phase.power:
                if (gamePlay == GamePlay.arena)
                {
                    return Robot.Phase.ready;
                }
                else
                {
                    return Robot.Phase.roll;
                }
            case Robot.Phase.ready:
                return Robot.Phase.roll;
            case Robot.Phase.roll:
                return Robot.Phase.stop;
            case Robot.Phase.stop:
                return Robot.Phase.wait;
            case Robot.Phase.wait:
                return Robot.Phase.direction;

            default:
                return Robot.Phase.wait;
        }
    }

    IEnumerator Quit(int timer)
    {
        yield return new WaitForSeconds(1);
        timer--;

        if (timer > 0)
        {
            coroutine = StartCoroutine("Quit", timer);
        }
        else
        {
            SceneManager.LoadScene("Menu");
        }
    }

    IEnumerator SpawnBattery()
    {
        yield return new WaitForSeconds(3);
        switchManager.SpawnBattery();
    }

    public IEnumerator LoadMenu()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Menu");
    }
}

public static class Settings
{
    public static int numberOfPlayers = 2;
    public static float speed = 1;
    public static float volume = 1;
}
