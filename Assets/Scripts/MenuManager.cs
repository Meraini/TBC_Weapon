using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


public class MenuManager : MonoBehaviour
{

    //  public GameObject flashText;

 


    // Start is called before the first frame update
    void Start() {
      //  InvokeRepeating("flashTheText", 0f, 0.4f);
    }

    // Update is called once per frame
    void Update()
    {
      /* if(Input.anyKey)
        {
            SceneManager.LoadScene("Game");
        }*/
    }


    public void menuLoad()
    {
        SceneManager.LoadScene("Menu");
    }
    public void creditsLoad()
    {
        SceneManager.LoadScene("Credits");
    }
    public void gameLoad()
    {
        SceneManager.LoadScene("Game");
    }
        public void historyLoad()
    {
        SceneManager.LoadScene("History");
    }
        public void natureLoad()
    {
        SceneManager.LoadScene("Nature");
    }
        public void matLoad()
    {
        SceneManager.LoadScene("Mat");
    }
        public void funLoad()
    {
        SceneManager.LoadScene("Fun");
    }
    public void memoryLoad()
    {
        SceneManager.LoadScene("Memory");
    }

   /* public void credits()
    {
        SceneManager.LoadScene("Menu");
    }*/

    /* void flashTheText()
    {
        if (flashText.activeInHierarchy)
        {
            flashText.SetActive(false);
        }
        else
        {
            flashText.SetActive(true);
        }
    }*/
}
