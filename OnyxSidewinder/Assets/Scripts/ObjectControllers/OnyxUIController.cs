using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OnyxUIController : MonoBehaviour
{
    Text instruction;
    //public GameObject ProgressUI;

    public bool Initialize()
    {
        
        instruction = GetComponent<Text>();

        return true;
    }



    // Update is called once per frame
    void Update()
    {

    }

    public void setMessageText(string s)
    {
        instruction.text = s;
    }
}
