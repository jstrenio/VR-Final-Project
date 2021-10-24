
using UnityEngine;
using UnityEngine.UI;
 
public class health_bar_script : MonoBehaviour
{
    private static Image HealthBarImage;
    public GameObject player;
    public bool attacked;
    public float health;
    

    private void Start()
    {
        HealthBarImage = GetComponent<Image>();
        player = GameObject.Find("player");
	    attacked = player.GetComponent<player_script>().attacked;
        health = 1;
        SetHealthBarValue(health);
    }
    
    void Update() 
    {
        if (player.GetComponent<player_script>().attacked)
        {
            health -= .35f;
            SetHealthBarValue(health);
            Debug.Log("received hit");
            player.GetComponent<player_script>().attacked = false;
        }
        if (health <= 0)
        {
            player.GetComponent<player_script>().reset_scene = true;
        }
    }

    public static void SetHealthBarValue(float value)
    {
        HealthBarImage.fillAmount = value;
        if(HealthBarImage.fillAmount < 0.35f)
        {
            SetHealthBarColor(Color.red);
        }
        else if(HealthBarImage.fillAmount < 0.70f)
        {
            SetHealthBarColor(Color.yellow);
        }
        else
        {
            SetHealthBarColor(Color.green);
        }
    }

    public static float GetHealthBarValue()
    {
        return HealthBarImage.fillAmount;
    }

    public static void SetHealthBarColor(Color healthColor)
    {
        HealthBarImage.color = healthColor;
    }


    
}