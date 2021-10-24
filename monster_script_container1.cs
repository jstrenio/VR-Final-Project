using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class monster_script_container1 : MonoBehaviour
{
	private Animator anim;
	public AudioSource audio;
	private CharacterController controller;
	private int battle_state = 0;
	public float speed = 6.0f;
	public float runSpeed = 3.0f;
	public float turnSpeed = 60.0f;
	public float gravity = 20.0f;
	public float spotted_timer = 0;
	public float chase_timer;
	private Vector3 moveDirection = Vector3.zero;
	private float w_sp = 0.0f;
	private float r_sp = 0.0f;
	public NavMeshAgent agent;
	public GameObject player;
	public GameObject waypoint1;
	public GameObject waypoint2;
	public GameObject target;
	public GameObject container, crane_container;
	public GameObject neck;
	public enum state { idle, patrol, chase, attack, search, die}
	public enum action {stand, walk, run, jump, attack, howl, die}
	public state monster_state;
	action monster_action;
	public float timer;
	public bool spotted;
	public bool hidden, attacked;
	private bool container_sequence;
	public float ctimer;
	public AudioClip fx_walk;
	public AudioClip fx_howl;
	public AudioClip fx_attack;
	public AudioClip fx_clicks;
	public AudioClip fx_run;
	public AudioClip fx_metal;
	public GameObject arms;
	public AudioSource arms_audio;
    public int monster;
	private bool squirt;
	
	// Use this for initialization
	void Start () 
	{			
		squirt = GameObject.Find("mblood").GetComponent<blood_script>().squirt;			
		audio = GetComponent<AudioSource>();
		audio.clip = fx_walk;
		anim = GetComponent<Animator>();
		controller = GetComponent<CharacterController> ();
		agent = GetComponent<NavMeshAgent>();
		w_sp = speed; //read walk speed
		r_sp = runSpeed; //read run speed
		battle_state = 0;
		runSpeed = 1;
		player = GameObject.Find("player");
		hidden = player.GetComponent<player_script>().hidden;
		attacked = player.GetComponent<player_script>().attacked;
		container = GameObject.Find("waypoint_container");
		crane_container = GameObject.Find("crane_container");
//		neck = GameObject.Find("neck1c");
		target = waypoint1;
		timer = 0;
        if (monster == 1)
        {
            monster_state = state.idle;
        }
        else
        {
            monster_state = state.patrol;
        }
		
		monster_action = action.stand;
		anim.SetInteger("action", 0);
		spotted = false;
		container_sequence = false;
		ctimer = 0f;
		arms = GameObject.Find("arms");
		arms_audio = arms.GetComponent<AudioSource>();
		spotted_timer = 0;
		chase_timer = 0;
		
	}

	private void FixedUpdate() 
	{
		look();
	}
	
	// Update is called once per frame
	void Update () 
	{		

		hidden = player.GetComponent<player_script>().hidden;

		if (monster_state == state.chase || monster_state == state.die)
		{
			chase_timer += Time.deltaTime;
			chase();
		}

		else if ((Vector3.Distance(player.transform.position, transform.position) < 1f || spotted == true) && monster_state != state.chase)
		{
            if (agent.enabled == false)
            {
                agent.enabled = true;
            }
			monster_action = action.run;
			monster_state = state.chase;
			target = player;
		}

		if (monster_state == state.chase && (hidden == true || chase_timer > 5))
		{
			agent.speed = 2;
			chase_timer = 0;
			anim.SetInteger("action", 0);
			monster_state = state.patrol;
			monster_action = action.walk;
			target = waypoint1;
		}

		else if (monster_state == state.patrol)
		{
			patrol();	
		}

		else if (monster_state == state.idle)
		{
			stand();
		}

		if (transform.position.z > -191 && transform.position.z < -184)
		{
			if (crane_container.transform.localPosition.y < 23.5 && !player.GetComponent<player_script>().container_down)
			{
				GameObject.Find("mblood").GetComponent<blood_script>().squirt = true;
				GameObject.Find("splatter").GetComponent<Renderer>().enabled = true;
				Destroy(gameObject);
			}
		}

		void walk()
		{
			anim.speed = 2;
			anim.SetInteger("action", 1);
			agent.SetDestination(target.transform.position);
			audio.clip = fx_walk;
			if (!audio.isPlaying)
			{
				audio.Play();
			}
			
		}
		
		void stand()
		{
			anim.speed = 1;
			timer += Time.deltaTime;
			anim.SetInteger("action", 0);
			audio.clip = fx_clicks;
			if (!audio.isPlaying)
			{
				audio.Play();
			}
		}

		void run()
		{
			anim.SetInteger("action", 2);
			agent.SetDestination(target.transform.position);
			agent.speed = 5;
			audio.clip = fx_run;
			if (!audio.isPlaying)
			{
				audio.Play();
			}
		}

		void howl()
		{
			anim.speed = 1;
			timer += Time.deltaTime;
			anim.SetInteger("action", 4);
			audio.clip = fx_howl;
			if (!audio.isPlaying)
			{
				audio.Play();
			}
		}

		void attack()
		{
			anim.speed = 1;
			anim.SetInteger("action", 3);
			audio.clip = fx_attack;
			if (!audio.isPlaying)
			{
				audio.Play();
			}
		}

		void reset_waypoint()
		{
			timer = 0;
			if (target == waypoint1)
			{
				target = waypoint2;
			}
			else
			{
				target = waypoint1;
			}
		}

		void patrol()
		{
			if (Vector3.Distance(transform.position, target.transform.position) > .5f)
			{
				monster_action = action.walk;
				walk();
			}
			else if (timer < 2.0f)
			{
				stand();
			}
			else if (timer < 5.0f)
			{
				howl();
			}
			else
			{
				reset_waypoint();
			}
		}

		void chase()
		{
			if (Vector3.Distance(transform.position, target.transform.position) > 2.5 && monster_action == action.run)
			{
				timer = 0;
				run();
			}
			else if (monster_action == action.run)
			{
				monster_action = action.die;
				monster_state = state.die;
				//player.transform.position = transform.position + transform.forward * 2 + transform.up * 2;
				
			}
			else if (timer < 4)
			{
				monster_action = action.die;
				GameObject.Find("camera").transform.LookAt(transform.position + transform.up * 2);
				player.transform.parent = neck.transform;
				anim.speed = .7f;
				agent.enabled = false;
				
				attack();
				Vector3 target2_pos = new Vector3(.08f,-0.69f, 0.38f);
				arms.transform.localPosition = target2_pos; // = Vector3.MoveTowards(transform.localPosition, target2_pos, 1);

				
				if (timer > 3)
				{
					arms.GetComponent<Animator>().SetBool("grabbed", false);
					
					player.transform.parent = null;
					player.transform.rotation = Quaternion.identity;
					
					
				}
				else if (timer > 1)
				{
					arms.GetComponent<Animator>().SetBool("grabbed", true);
					
					if (timer > 1.9f && !arms_audio.isPlaying)
					{
						arms_audio.Play();
						player.GetComponent<player_script>().attacked = true;
					}
					
				}
			}
			else if (timer < 14)
			{
				anim.speed = .8f;
				Vector3 target_pos = new Vector3(.08f,-1.45f, 0.38f);
				arms.transform.localPosition = target_pos; // = Vector3.MoveTowards(transform.localPosition, target_pos, 1);
			}
			if (timer > 14)
			{
				agent.enabled = true;
				agent.speed = 2;
				anim.SetInteger("action", 0);
				monster_state = state.patrol;
				monster_action = action.walk;
				target = waypoint1;
				timer = 0;
				anim.speed = 2;
			}
			timer += Time.deltaTime;
		}

		void jump_container()
		{
			agent.enabled = true;
			target = container;
			ctimer += Time.deltaTime;
			if (Vector3.Distance(transform.position, target.transform.position) > 1f && ctimer < 3)
			{
				anim.SetInteger("action", 2);
				agent.SetDestination(target.transform.position);
				agent.speed = 5;
				ctimer = 0;
				if (Vector3.Distance(transform.position, GameObject.Find("waypoint_jump").transform.position) < .5f)
				{
					audio.clip = fx_metal;
					if (!audio.isPlaying)
					{
						audio.Play();
					}
				}
			}
			else if (ctimer < 2f)
			{
				anim.speed = 1;
				timer += Time.deltaTime;
				anim.SetInteger("action", 0);
			}
			else if (ctimer < 6)
			{
				howl();
			}	
			else
			{
				monster_state = state.patrol;
				monster_action = action.walk;
				target = waypoint1;
				container_sequence = true;

			}
		}
	}

	void look()
	{
		// raycast object from player
		RaycastHit hit;
		RaycastHit hit2;
		Vector3 origin = transform.position + Vector3.up * 2;
		Vector3 direction = player.transform.position - origin;
		Vector3 direction2 = transform.forward * 3;

		if (Physics.Raycast(origin, direction, out hit, 20) && hit.transform.tag == "Player" && Vector3.Angle(transform.forward, direction) < 60)
		{
			if ( spotted_timer > 1.5f)
			{
				spotted = true;
			}
			else 
			{
				spotted_timer += Time.deltaTime;
			}
		}
		else
		{
			spotted = false;
			spotted_timer = 0;
		}

        if (Physics.Raycast(origin, direction2, out hit2, 3) && hit2.transform.tag == "crane_container")
        {
            monster_state = state.idle;
            agent.enabled = false;
        }

        // else
        // {
        //     Debug.Log("hitting nothing");
        // }
        else if (monster_state == state.idle && !agent.enabled)
        {
            monster_state = state.patrol;
            agent.enabled = true;
        }

		Debug.DrawRay(origin, direction * 2, Color.red);
		//Debug.DrawRay(origin, direction2, Color.blue);
	}
}