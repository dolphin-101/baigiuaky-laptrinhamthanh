using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menuicon : MonoBehaviour
{
    [Header("Kéo 5 Panels vào đây theo thứ tự (Shop, Cup, Home, Team, Book)")]
    public GameObject[] panels;

    void Start()
    {
        OpenPanel(2);
    }
    public void OpenPanel(int panelIndex)
    {
        for (int i = 0; i < panels.Length; i++)
        {
 
            if (i == panelIndex)
            {
                panels[i].SetActive(true);
            }
            else
            {
                panels[i].SetActive(false);
            }
        }
    }
}
