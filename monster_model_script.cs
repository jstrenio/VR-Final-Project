using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class monster_model_script : MonoBehaviour {
	
	private Animator anim;
    public Rigidbody rb;
	public int speed;
	public GameObject player;
	public float start_x, start_z, end_z;
	public int action_type;
	
	
	// Use this for initialization
	void Start () 
	{						
		player = GameObject.Find("player");
		rb = GetComponent<Rigidbody>();
		anim = GetComponent<Animator>();
		
		//run
		if (action_type == 1)
		{
			speed = 15;
			anim.SetInteger("action", 2);
			anim.speed = 1.5f;
		}

		//howl
		else if (action_type == 2)
		{
			speed = 0;
			anim.SetInteger("action", 4);
			anim.speed = 1;
		}

		//idle
		else if (action_type == 3)
		{
			speed = 0;
			anim.SetInteger("action", 0);
			anim.speed = 1;
		}

		//walk
		else if (action_type == 4)
		{
			speed = 1;
			anim.SetInteger("action", 1);
			anim.speed = 1.2f;
		}
		
        
		
	}
	
	// Update is called once per frame
	void Update () 
	{		
		if (player.transform.position.x > start_x && player.transform.position.z < start_z)
		{
			Vector3 movement = transform.forward * speed * Time.deltaTime;
        	rb.MovePosition(transform.position + movement);
		}
        
        if (transform.position.z < end_z)
        {
            Destroy(gameObject);   
        }
		
	}
}
