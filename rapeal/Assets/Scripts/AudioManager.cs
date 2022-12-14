using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource attackSource;
    public AudioSource explodeSource;
    public AudioSource reloadSource;

    public Health health;
    public AttackControl attackControl;

    private bool onlyOnce;
    private bool attackOnce;
    private bool reloadOnce;


    void Update()
    {
        if (health.health <= 0f && !onlyOnce)
        {
            onlyOnce = true;
            explodeSource.Play();
        }

        if (attackControl.attack == 1 && !attackOnce)
        {
            attackOnce = true;
            reloadOnce = false;
            attackSource.Play();
        }

        if (attackControl.attack == 0 && !reloadOnce)
        {
            attackOnce = false;
            reloadOnce = true;
            reloadSource.Play();
            attackSource.Stop();
        }
    }
}
