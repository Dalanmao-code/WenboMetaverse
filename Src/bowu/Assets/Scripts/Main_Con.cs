using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main_Con : MonoBehaviour
{
    public static Main_Con Instance;

    public GameObject main_Inter;

    public Button quit_Button;

    public Button setting_Button;

    public GameObject audio_Inter;
    [HideInInspector]
    public AudioSource bg_AudioSource;

    public Slider bg_Slider;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        main_Inter.SetActive(true);
        audio_Inter.SetActive(false);
        bg_AudioSource = GetComponent<AudioSource>();
        quit_Button.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        setting_Button.onClick.AddListener(() =>
        {
            audio_Inter.SetActive(true);
        });
        bg_Slider.value = bg_AudioSource.volume;
        bg_Slider.onValueChanged.AddListener(OnValueChange);
    }

    private void OnValueChange(float volume)
    {
        bg_AudioSource.volume = volume;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!main_Inter.activeSelf)
            {
                main_Inter.SetActive(true);
            }
            else
            {
                Application.Quit();
            }
        }
    }
}
