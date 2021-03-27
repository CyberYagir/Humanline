using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    public TMP_InputField seedField;
    public TMP_InputField townName;
    public TMP_Text placeholder;
    public Slider slider;
    private void Start()
    {
        slider.value = PlayerPrefs.GetFloat("Volume", 1);
    }
    private void Update()
    {
        placeholder.text = $"World seed... [{DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + DateTime.Now.Day.ToString("00") + DateTime.Now.Month.ToString("00") + DateTime.Now.Year.ToString("0000")}]";
    }
    public void ChangeV()
    {
        AudioListener.volume = slider.value;
        PlayerPrefs.SetFloat("Volume", slider.value);
    }

    public void Play()
    {
        PlayerPrefs.DeleteAll();
        Application.LoadLevel(1);
        if (seedField.text != "")
        {
            PlayerPrefs.SetString("Seed", long.Parse(seedField.text).ToString("0000000000"));
            PlayerPrefs.SetString("Town", townName.text);
        }
        else
        {
            PlayerPrefs.SetString("Seed", DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + DateTime.Now.Day.ToString("00") + DateTime.Now.Month.ToString("00") + DateTime.Now.Year.ToString("0000"));
            PlayerPrefs.SetString("Town", townName.text);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
