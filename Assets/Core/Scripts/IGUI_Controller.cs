using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IGUI_Controller : MonoBehaviour
{

    public GameObject characterPanel, spellbook, inventory;



    // Start is called before the first frame update
    void Start()
    {
        //Panels[].SetActive(false);

        characterPanel.SetActive(false);
        spellbook.SetActive(false);
        inventory.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C) )
        {
            characterPanel.SetActive(!characterPanel.activeSelf);
        }
    }
}
