using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseScene : MonoBehaviour
{
    public Camera main_camera;
    public Sprite[] numbers;
    public Image image;
    private int time = 0;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ChangeNumber());
    }

    IEnumerator ChangeNumber()
    {
        main_camera.transform.position = Camera_move.camera_position;

        int i = 0;
        while (time < 3)
        {
            if (i > 2)
                i = 2;
            time += 1;

            image.sprite = numbers[i];
            i++;
            yield return new WaitForSecondsRealtime(1f);
        }
        SceneManager.UnloadSceneAsync("Test");
    }
}
