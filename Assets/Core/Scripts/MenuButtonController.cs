using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtonController : MonoBehaviour
{

    public int index;
    [SerializeField] int maxIndex;
    [SerializeField] bool keyDown;
    public AudioSource audioSource;

    public string startScene;

    public GameObject SettingsPanel;


    // Start is called before the first frame update
    void Start()
    {
        SettingsPanel.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxis ("Vertical") != 0 && SettingsPanel.activeSelf == false)
        {
            if (!keyDown)
            {
                if (Input.GetAxis ("Vertical") < 0)
                {
                    if(index < maxIndex)
                    {
                        index++;
                    }
                    else
                        index = 0;
                }
                else if(Input.GetAxis ("Vertical") > 0)
                {
                    if(index > 0)
                    {
                        index--;
                    }
                    else
                    {
                        index = maxIndex;
                    }
                }
                keyDown = true;
                
            }
        }
        else { keyDown = false;
        }

        if (Input.GetButton ("Submit"))
        {
            if(index == 0)
            {
                StartGame();
            }
            else if(index == 1)
            {
                Options();
            }
            else if(index == 2)
            {
                QuitGame();
            }
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            EscapeToMainMenu();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(startScene);
    }

    public void Options()
    {
        Debug.Log("Option button clicked");

        if (SettingsPanel.activeSelf == false)
        {
            SettingsPanel.SetActive(true);
        }
    }

    public void EscapeToMainMenu()
    {
        if (SettingsPanel.activeSelf == true)
        {
            SettingsPanel.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quiting Game");
    }


}
