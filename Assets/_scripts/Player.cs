using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private float maxHealth = 100;

    public Animator animator;

    private float currentHealth;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    AudioSource[] audioSources;
    AudioSource rock1;
    AudioSource rock2;

    [SerializeField]
    Camera[] cameras;
    private int currentCamera;

    private void Start()
    {
        audioSources = GetComponents<AudioSource>();
        rock1 = audioSources[0];
        rock2 = audioSources[1];
    }

    public void Setup()
    {
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }

        this.SetDefaults();
    }

    public void CmdSetHealth()
    {
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(float _amount)
    {
        currentHealth -= _amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void RpcTakeDamage(float _amount)
    {
        Debug.Log("in RpcTakeDamage for " + transform.name);
        if(isDead)
        {
            return;
        }
        currentHealth -= _amount;

        if ( currentHealth < maxHealth )
        {
            if( currentHealth < maxHealth/3f )
            {
                rock1.Stop();
                if ( !rock2.isPlaying )
                    rock2.Play();
            }
            else
            {
                if (!rock1.isPlaying)
                    rock1.Play();
            }
            
        }


        Debug.Log(transform.name + " now has " + currentHealth + " health");
        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        currentHealth = 0f;
        animator.SetBool("Death", true);

        // disable components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        Collider _col = GetComponent<Collider>();
        if (_col != null)
        {
            _col.enabled = false;
        }

        // death cam
        SwitchCamera(1, true);
        

        Debug.Log(transform.name + "is dead");

        // enable respawn mode
    }

    public void SetDefaults()
    {
        isDead = false;
        currentHealth = 100f;
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        Collider _col = GetComponent<Collider>();
        if( _col != null )
        {
            _col.enabled = true;
        }

    }

    //Respawn function. Switches to camera 1
    void OnRespawn()
    {
        SwitchCamera(0, true);
    }

    private void SwitchCamera(int num, bool direct)
    {
        Debug.Log("Switch camera " + num);
        if (direct) currentCamera = num;
        else currentCamera = (currentCamera + num) % cameras.Length;

        for (var i = 0; i < cameras.Length; i++)
        {
            cameras[i].enabled = (i == currentCamera);
            //audioListeners[i].enabled = (i == currentCamera);
        }
    }


}
