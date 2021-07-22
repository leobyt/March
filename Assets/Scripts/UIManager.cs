using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using DigitalRuby.Tween;
using TMPro;

public class UIManager : MonoBehaviour
{
    public enum Volume { mute, min, max };
    public enum Sensitivity { fast, normal, slow };
    public enum Menu { main, options, play, tween };
    Sensitivity currentSensitivity = Sensitivity.normal;
    Volume currentVolume = Volume.max;
    Menu currentMenu;
    float deltaCounter = 0;
    Image volumeImage;
    TMP_Text volumeText;
    public AudioClip menuSelect;
    public AudioClip menuClick;
    public Sprite volumeMute;
    public Sprite volumeMin;
    public Sprite volumeMax;
    public EventSystem eventSystem;
    public Button play, options, quit, back, playersButton, volumeButton, sensitivityButton;
    AudioSource audioSource;

    void Start()
    {
        currentMenu = Menu.main;
        volumeImage = transform.GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetComponent<Image>();
        volumeText = transform.GetChild(0).GetChild(2).GetChild(2).GetChild(0).GetComponent<TMP_Text>();
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = Settings.volume;

        if (Settings.speed == 0.5f)
        {
            currentSensitivity = Sensitivity.fast;
            volumeText.text = "SENSITIVITY\nfast";
        }
        else if (Settings.speed == 1f)
        {
            currentSensitivity = Sensitivity.normal;
            volumeText.text = "SENSITIVITY\nnormal";
        }
        else if (Settings.speed == 2f)
        {
            currentSensitivity = Sensitivity.slow;
            volumeText.text = "SENSITIVITY\nslow";
        }

        if (Settings.volume == 0f)
        {
            currentVolume = Volume.mute;
            volumeImage.sprite = volumeMute;
        }
        else if (Settings.volume == 0.5f)
        {
            currentVolume = Volume.min;
            volumeImage.sprite = volumeMin;
        }
        else if (Settings.volume == 1f)
        {
            currentVolume = Volume.max;
            volumeImage.sprite = volumeMax;
        }
    }

    void Update()
    {
        var selectedButton = eventSystem.currentSelectedGameObject;
        if (Input.GetButtonDown("Vertical"))
        {
            audioSource.PlayOneShot(menuSelect, 0.25f);
            if (selectedButton.name == "PlayersButton"
                && selectedButton != null)
            {
                if (Input.GetAxis("Vertical") > 0)
                {
                    ChangePlayers();
                }
                if (Input.GetAxis("Vertical") < 0)
                {
                    ChangePlayers(true);
                }
            }
        }
        if (Input.GetButtonDown("Submit"))
        {
            audioSource.PlayOneShot(menuClick, 0.25f);
            if (selectedButton.name == "PlayersButton"
                && selectedButton != null)
            {
                StartGame();
            }
        }
        if (Input.GetButton("Switch1"))
        {
            deltaCounter += Time.deltaTime;
        }
        if (Input.GetButtonUp("Switch1"))
        {
            if (deltaCounter < Settings.speed)
            {
                audioSource.PlayOneShot(menuSelect, 0.25f);
                if (selectedButton.name == "PlayersButton"
                    && selectedButton != null)
                {
                    ChangePlayers();
                }
                else
                {
                    EventSystem.current.SetSelectedGameObject(selectedButton.GetComponent<Button>().FindSelectableOnDown().gameObject);
                }
            }
            else
            {
                audioSource.PlayOneShot(menuClick, 0.25f);
                switch (currentMenu)
                {
                    case Menu.main:
                        switch (selectedButton.name)
                        {
                            case "PlayButton":
                                Play();
                                break;
                            case "OptionsButton":
                                Options();
                                break;
                            case "QuitButton":
                                Quit();
                                break;
                        }
                        break;
                    case Menu.options:
                        switch (selectedButton.name)
                        {
                            case "BackButton":
                                OptionsBack();
                                break;
                            case "VolumeButton":
                                ChangeVolume();
                                break;
                            case "SensitivityButton":
                                ChangeSensitivity();
                                break;
                        }
                        break;
                    case Menu.play:
                        switch (selectedButton.name)
                        {
                            case "PlayersButton":
                                StartGame();
                                break;
                        }
                        break;
                }
            }

            deltaCounter = 0;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        int random = Random.Range(1, 8);
        SceneManager.LoadScene(random);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ChangePlayers(bool decrease = false)
    {
        if (decrease)
        {
            Settings.numberOfPlayers--;
        }
        else
        {
            Settings.numberOfPlayers++;
        }

        if (Settings.numberOfPlayers > 4)
        {
            Settings.numberOfPlayers = 2;
        }

        if (Settings.numberOfPlayers < 2)
        {
            Settings.numberOfPlayers = 4;
        }

        playersButton.transform.GetChild(0).GetComponent<TMP_Text>().text = Settings.numberOfPlayers.ToString();
    }

    public void ChangeSensitivity()
    {
        switch (currentSensitivity)
        {
            case Sensitivity.slow:
                currentSensitivity = Sensitivity.normal;
                volumeText.text = "SENSITIVITY\nnormal";
                Settings.speed = 1;
                break;
            case Sensitivity.normal:
                currentSensitivity = Sensitivity.fast;
                volumeText.text = "SENSITIVITY\nfast";
                Settings.speed = 0.5f;
                break;
            case Sensitivity.fast:
                currentSensitivity = Sensitivity.slow;
                volumeText.text = "SENSITIVITY\nslow";
                Settings.speed = 2;
                break;
        }
    }

    public void ChangeVolume()
    {
        switch (currentVolume)
        {
            case Volume.mute:
                currentVolume = Volume.min;
                volumeImage.sprite = volumeMin;
                Settings.volume = 0.5f;
                audioSource.volume = Settings.volume;
                break;
            case Volume.min:
                currentVolume = Volume.max;
                volumeImage.sprite = volumeMax;
                Settings.volume = 1f;
                audioSource.volume = Settings.volume;
                break;
            case Volume.max:
                currentVolume = Volume.mute;
                volumeImage.sprite = volumeMute;
                Settings.volume = 0f;
                audioSource.volume = Settings.volume;
                break;
        }
    }

    public void Options()
    {
        currentMenu = Menu.tween;
        Vector3 newPosition = new Vector3(transform.GetChild(0).position.x, transform.GetChild(0).position.y + Screen.height, transform.GetChild(0).position.z);
        gameObject.Tween("moveMenuUp", transform.GetChild(0).position, newPosition, 0.33f, TweenScaleFunctions.QuadraticEaseInOut, (t) =>
        {
            // Progress
            transform.GetChild(0).position = t.CurrentValue;

        }, (t) =>
        {
            // Completion
            currentMenu = Menu.options;
            EventSystem.current.SetSelectedGameObject(sensitivityButton.gameObject);
        });
    }

    public void OptionsBack()
    {
        currentMenu = Menu.tween;
        Vector3 newPosition = new Vector3(transform.GetChild(0).position.x, transform.GetChild(0).position.y - Screen.height, transform.GetChild(0).position.z);
        gameObject.Tween("moveMenuDown", transform.GetChild(0).position, newPosition, 0.33f, TweenScaleFunctions.QuadraticEaseInOut, (t) =>
        {
            // Progress
            transform.GetChild(0).position = t.CurrentValue;

        }, (t) =>
        {
            // Completion
            currentMenu = Menu.main;
            EventSystem.current.SetSelectedGameObject(options.gameObject);
        });
    }

    public void Play()
    {
        currentMenu = Menu.tween;
        Vector3 newPosition = new Vector3(transform.GetChild(0).position.x - Screen.width, transform.GetChild(0).position.y, transform.GetChild(0).position.z);
        gameObject.Tween("moveMenuLeft", transform.GetChild(0).position, newPosition, 0.33f, TweenScaleFunctions.QuadraticEaseInOut, (t) =>
        {
            // Progress
            transform.GetChild(0).position = t.CurrentValue;

        }, (t) =>
        {
            // Completion
            currentMenu = Menu.play;
            EventSystem.current.SetSelectedGameObject(playersButton.gameObject);
        });
    }
}
