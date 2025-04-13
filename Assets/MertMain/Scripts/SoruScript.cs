using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoruScript : MonoBehaviour

  
{

    public GameObject optionA;
    public GameObject optionB;
    public GameObject optionC;
    public GameObject optionD;

    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        
    }

    public void CorrectAnswer()
    {
        Debug.Log("doğru cevap verdin");
    }
    public void WrongAnswer(){
        Debug.Log("yanlış cevap verdin");
    }
}
