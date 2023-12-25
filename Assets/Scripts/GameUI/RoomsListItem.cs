using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomsListItem : MonoBehaviour
{
    public Text GetTitleText()
    {
        return GetComponentsInChildren<Text>()[0];
    }
    
    public Text GetInfoeText()
    {
        return GetComponentsInChildren<Text>()[1];
    }
}