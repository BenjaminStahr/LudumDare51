using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(BoxCollider2D))]
public class Spikey : MonoBehaviour
{
    bool up;
    float timer;
    Animator anim;
    BoxCollider2D trigger;

    [SerializeField]
    private float uptime = 3;

    [SerializeField]
    private float downtime = 5;

    bool triggerEnabled;

    // Start is called before the first frame update
    void Start()
    {
        timer = Time.realtimeSinceStartup;
        up = false;

        anim = GetComponent<Animator>();
        trigger = GetComponent<BoxCollider2D>();
        triggerEnabled = trigger.enabled;

        anim.SetBool("up", up);
    }

    // Update is called once per frame
    void Update()
    {
        float time = up ? uptime : downtime;
        if (Time.realtimeSinceStartup > timer + time)
        {
            timer = Time.realtimeSinceStartup;
            up = !up;
            anim.SetBool("up", up);
            trigger.enabled = triggerEnabled && up ;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.GetComponent<Controller>().Death();
        }
    }
}
