using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class player_script : MonoBehaviour
{
    public MeshRenderer red_switch, blue_switch, green_switch;
    public GameObject player, cam, object_script, switch_obj, container;
    public float movespeed, switch_timer, container_timer;
    public Rigidbody rb, container_rb;
    public object_interact cube;
    public AudioSource con_audio;
    public AudioSource audio;
    public AudioClip container_fx, container_fall_fx, footsteps;
    public bool hidden, switch_down, container_action, container_down, attacked, reset_scene, squirt;
    public int switch_state;
    private Vector3 switch_down_pos, switch_up_pos, container_down_pos, container_up_pos;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("player");
        cam = GameObject.Find("camera");
        object_script = GameObject.Find("script_object");
        movespeed = 5.0f;
        rb = player.GetComponent<Rigidbody>();
        cube = object_script.GetComponent<object_interact>();
        cam.transform.position += Vector3.up;
        hidden = false;
        switch_down = true;
        switch_timer = 0.0f;
        switch_state = 0;
        switch_obj = GameObject.Find("red_switch");
        red_switch = switch_obj.GetComponent<MeshRenderer>();
        green_switch = GameObject.Find("green_switch").GetComponent<MeshRenderer>();
        blue_switch = GameObject.Find("blue_switch").GetComponent<MeshRenderer>();
        switch_down_pos = switch_obj.transform.position;
        switch_up_pos = switch_down_pos - switch_obj.transform.forward * .2f;
        container = GameObject.Find("crane_container");
        con_audio = container.GetComponent<AudioSource>();
        audio = GetComponent<AudioSource>();
        container_action = false;
        container_down = true;
        container_timer = 0;
        container_down_pos = container.transform.position;
        container_up_pos = container.transform.position + container.transform.up * 7;
        container_rb = container.GetComponent<Rigidbody>();
        container_rb.useGravity = false;
        attacked = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (switch_state == 1)
        {
            switch_timer += Time.deltaTime;
        }
        else if ( switch_state == 0)
        {
            switch_timer = 0;
        }
        else if (switch_state == 2)
        {
            move_switch();
        }

        if (container_action)
        {
            move_container();
        }

    }

    private void FixedUpdate() 
    {
        // raycast object from player
        RaycastHit hit;
        LayerMask target = LayerMask.GetMask("target");
        LayerMask finish = LayerMask.GetMask("finish");
        if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, 100f, target))
        {
            // if its a cube move towards it
            if (hit.transform.gameObject.tag == "finish")
            {
                if (SceneManager.GetActiveScene().name == "scene0")
                {
                    SceneManager.LoadScene("scene1");
                }
                if (SceneManager.GetActiveScene().name == "scene1")
                {
                    SceneManager.LoadScene("scene2");
                }
                else if (SceneManager.GetActiveScene().name == "scene2")
                {
                    Debug.Log("scene3!");
                    SceneManager.LoadScene("scene3");
                }
                else if (SceneManager.GetActiveScene().name == "scene3")
                {
                    Debug.Log("scene3!");
                    SceneManager.LoadScene("scene4");
                }
            }

            if (reset_scene)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            if (switch_timer > 3 && blue_switch.enabled)
            {
                switch_state = 0;
                red_switch.enabled = true;
                green_switch.enabled = false;
                blue_switch.enabled = false;
                
        }

            // if its a switch
            if (hit.transform.gameObject.tag == "switch")
            {
                // if the switch is not being interacted with
                if (switch_state == 0)
                {
                    // set switch to gaze
                    switch_state = 1;
                    red_switch.enabled = false;
                    green_switch.enabled = false;
                    blue_switch.enabled = true;
                }
            }


            else if (hit.transform.gameObject.tag == "target")
            {
                approach();
            }

            else if (audio.isPlaying)
            {
                audio.Stop();
            }

            // if the switch is up and they just swiped down
            else if (hit.transform.gameObject.tag == "look_down_zone" && switch_state == 1 && switch_timer < 1 && !container_action
                    && !switch_down && Vector3.Distance(player.transform.position, switch_obj.transform.position) < 2.1f)
            {
                // turn switch green, move it down and drop the container
                switch_state = 2;
                red_switch.enabled = false;
                green_switch.enabled = true;
                blue_switch.enabled = false;

            }

            // if the switch is down and they just swiped up
            else if (hit.transform.gameObject.tag == "look_up_zone" && switch_state == 1 && switch_timer < 1 && !container_action
                    && switch_down && Vector3.Distance(player.transform.position, switch_obj.transform.position) < 2f)
            {
                switch_state = 2;
                red_switch.enabled = false;
                green_switch.enabled = true;
                blue_switch.enabled = false;

            }

            
        }

        // if they aren't looking at the switch and its been more than the 1 sec they have to swipe up or down deactive switch
        else if (switch_state == 1 && switch_timer > 1)
        {
            switch_state = 0;
            red_switch.enabled = true;
            green_switch.enabled = false;
            blue_switch.enabled = false;
            
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "grass")
        {
            transform.localScale *= 0.5f;
            movespeed = 2f;
            hidden = true;
        }
    }

        private void OnTriggerExit(Collider other) {
        if (other.tag == "grass")
        {
            transform.localScale *= 2f;
            movespeed = 5.0f;
            hidden = false;
        }
    }

   public void approach()
   {
        Vector3 movement = cam.transform.forward * movespeed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);
        if (!audio.isPlaying)
        {
            audio.Play();
        }

   }

    public void move_container()
    {
        container_timer += Time.deltaTime;

       if (container_down)
       {
           container.transform.position = Vector3.MoveTowards(container.transform.position, container_up_pos, 2f * Time.deltaTime);

           con_audio.clip = container_fx;
			if (!con_audio.isPlaying)
			{
				con_audio.Play();
			}
       }
       else
       {
           container_rb.useGravity = true;
           con_audio.clip = container_fall_fx;
           if (!con_audio.isPlaying)
			{
				con_audio.Play();
			}
            
       }

        if (container_timer > 4)
        {
            if (container_down)
            {
                container_down = false;
            }
            else
            {
                container_down = true;
                container_rb.useGravity = false;
                GameObject.Find("mblood").GetComponent<blood_script>().squirt = false;
                Debug.Log("squirt false");
            }
            container_action = false;
            container_timer = 0;
        }
    }

   public void move_switch()
   {
       switch_timer += Time.deltaTime;

       if (switch_down)
       {
           switch_obj.transform.position = Vector3.MoveTowards(switch_obj.transform.position, switch_up_pos, .2f * Time.deltaTime);
       }
       else
       {
           switch_obj.transform.position = Vector3.MoveTowards(switch_obj.transform.position, switch_down_pos, .2f * Time.deltaTime);
       }

       if (switch_timer > 2f)
       {
           switch_state = 0;
            red_switch.enabled = true;
            green_switch.enabled = false;
            blue_switch.enabled = false;
            container_action = true;
            if (switch_down)
            {
                switch_down = false;
            }
            else
            {
                switch_down = true;
            }
       }
   }
}
