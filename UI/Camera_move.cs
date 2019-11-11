using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_move : MonoBehaviour
{
    public float ShakeAmount;

    [HideInInspector]
    public GameObject[] player;

    [HideInInspector]
    public bool my_character_dead = false;

    private Vector3 player_1_pos;
    //private Vector3 player_2_pos;
    private Vector3 camera_target;

    private Vector3 player_1_movement;
    //private Vector3 player_2_movement;

    private Vector3 temp;

    private float dist_player;

    //private bool is_one_player_camera;
    //private bool is_camera_player1;


    private int camera_state=0;

    Camera _camera;

    private float ShakeTime;
    private Vector3 initialPosition;

    public static Vector3 camera_position;

    private float CameraSpeed = 3f;


    private GameObject skill_player;

    private float Camera_view;

    public BoxCollider2D Camera_bound;

    private Vector2 minBound;
    private Vector2 maxBound;

    private float halfWidth;
    private float halfHieght;

    private float clampedX;
    private float clampedY;

    private Vector3 vec3_temp;

    public void VivrateForTime(float time, float power = 0.05f)
    {
        ShakeTime = time;
        ShakeAmount = power;
    }

    void Start()
    {
        _camera = GetComponent<Camera>();
        player = GameObject.FindGameObjectsWithTag("Player");
        player_1_pos.z = 0;
        //player_2_pos.z = 0;
        //is_one_player_camera = false;
        Camera_view = 1.5f;

        minBound = Camera_bound.bounds.min;
        maxBound = Camera_bound.bounds.max;
        halfHieght = _camera.orthographicSize;
        halfWidth = halfHieght * Screen.width / Screen.height;

        //StartCoroutine(StopGame());
    }

    // Update is called once per frame
    void Update()
    {
        camera_position = transform.position;
        initialPosition = _camera.transform.position;
        initialPosition.z = -990f;
        player_1_movement.x = player[0].transform.position.x - player_1_pos.x;
        player_1_movement.y = player[0].transform.position.y - player_1_pos.y;
        //player_2_movement.x = player[1].transform.position.x - player_2_pos.x;
        //player_2_movement.y = player[1].transform.position.y - player_2_pos.y;

        player_1_pos.x = player[0].transform.position.x;
        player_1_pos.y = player[0].transform.position.y;
        //player_2_pos.x = player[1].transform.position.x;
        //player_2_pos.y = player[1].transform.position.y;

        if( ShakeTime > 0)
        {
            _camera.transform.position = Random.insideUnitSphere * ShakeAmount + initialPosition;
            ShakeTime -= Time.deltaTime;
        }

        if(NetworkManager.instance.is_multi && my_character_dead)
        {
            camera_target = BossStatus.instance.transform.position;
            camera_target.z = -1000;//camera 원래 z축;

            transform.position = Vector3.Lerp(transform.position, camera_target, Time.deltaTime * CameraSpeed);
        }
        else
        {
            camera_target = player_1_pos;
            camera_target.z = -1000;//camera 원래 z축;
            dist_player = Mathf.Sqrt(((Vector2)player_1_pos - (Vector2)BossStatus.instance.transform.position).sqrMagnitude);

            transform.position = Vector3.Lerp(transform.position, camera_target, Time.deltaTime * CameraSpeed);



            minBound = Camera_bound.bounds.min;
            maxBound = Camera_bound.bounds.max;
            halfHieght = _camera.orthographicSize;
            halfWidth = halfHieght * Screen.width / Screen.height;

            clampedX = Mathf.Clamp(this.transform.position.x, minBound.x + halfWidth, maxBound.x - halfWidth);
            clampedY = Mathf.Clamp(this.transform.position.y, minBound.y + halfHieght, maxBound.y - halfHieght);


            vec3_temp.x = clampedX;
            vec3_temp.y = clampedY;
            vec3_temp.z = transform.position.z;

            transform.position = vec3_temp;



            if (dist_player > 7f && camera_state == 0) //작에서 커짐
            {
                _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, 6.0f + Camera_view, Time.deltaTime * CameraSpeed);
                if (_camera.orthographicSize >= 5.9f + Camera_view) camera_state = 1;
            }
            else if (dist_player < 7f && camera_state == 1) //커짐에서 작
            {
                _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, 5.0f + Camera_view, Time.deltaTime * CameraSpeed);
                if (_camera.orthographicSize <= 5.1f + Camera_view) camera_state = 0;
            }
            else if (dist_player > 10f && camera_state == 1) //작에서 커짐
            {
                _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, 7.0f + Camera_view, Time.deltaTime * CameraSpeed);
                if (_camera.orthographicSize >= 6.9f + Camera_view) camera_state = 2;
            }
            else if (dist_player < 10f && camera_state == 2)//커에서 작
            {
                _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, 6.0f + Camera_view, Time.deltaTime * CameraSpeed);
                if (_camera.orthographicSize <= 6.1f + Camera_view) camera_state = 1;
            }
            else if (dist_player > 13f && camera_state == 2) //작에서 커짐
            {
                _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, 8.0f + Camera_view, Time.deltaTime * CameraSpeed);
                if (_camera.orthographicSize >= 7.9f + Camera_view) camera_state = 3;
            }
            else if (dist_player < 13f && camera_state == 3)//커에서 작
            {
                _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, 7.0f + Camera_view, Time.deltaTime * CameraSpeed);
                if (_camera.orthographicSize <= 7.1f + Camera_view) camera_state = 2;
            }
        }
    }

}
