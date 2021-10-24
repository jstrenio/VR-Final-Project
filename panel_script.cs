using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class panel_script : MonoBehaviour
{
    float timer;
    bool player_moving;
    GameObject player;
    

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        player = GameObject.Find("player");
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (player.transform.position.x > 1.5f || player.transform.position.z < -40)
        {
            player_moving = true;
        }
        if (timer > 20 || player_moving)
        {
            Destroy(gameObject);
        }
    }
}
