using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class HealthSystem : MonoBehaviour
{
    public Animator anim;

    [SerializeField]
    private CapsuleCollider PlayerCollider;

    private int lives = 3;
    private int loopTime = 5;

    public AudioSource takeDamage;

    private bool Death;
    private bool canLoseLife = true;

    [SerializeField]
    private GameObject Player;

    public List<GameObject> hearts = new List<GameObject>();
    public List<Renderer> playerRenderers = new List<Renderer>();

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (lives == 2)
        {
            hearts[0].SetActive(false);
        }
        if (lives == 1)
        {
            hearts[1].SetActive(false);
        }
        if (lives == 0)
        {
            hearts[2].SetActive(false);
        }

        if (lives == 0)
        {
            DeathScreen();
            Anims();
            Death = true;
            PlayerCollider.enabled = false;
        }
    }

    public void LoseLife()
    {
        if (canLoseLife)
        {
            takeDamage.Play();
            Anims();
            StartCoroutine("AttackPause");
            StartCoroutine("BlinkEffect");
            lives--;
            Debug.Log("-health");
        }
    }

    public void Die()
    {
        lives = 3;
        hearts[0].SetActive(true);
        hearts[1].SetActive(true);
        hearts[2].SetActive(true);
    }

    private void Anims()
    {
        anim.SetBool("Death", Death);
    }

    private void DeathScreen()
    {
        StartCoroutine("Dying");
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "EnemyWeapon" || col.gameObject.tag == "projectile" || col.gameObject.tag == "Rock")
        {
            LoseLife();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.gameObject.tag == "Player" && other.gameObject.tag == "Rock")
        {
            LoseLife();
        }
    }

    private IEnumerator Dying()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("LoseScene");
    }

    private IEnumerator AttackPause()
    {
        canLoseLife = false;
        yield return new WaitForSeconds(2);
        canLoseLife = true;
    }

    private IEnumerator BlinkEffect()
    {
        for (int i = 0; i < loopTime; i++)
        {
            playerRenderers[0].enabled = false;
            playerRenderers[1].enabled = false;
            playerRenderers[2].enabled = false;
            yield return new WaitForSeconds(0.2f);
            playerRenderers[0].enabled = true;
            playerRenderers[1].enabled = true;
            playerRenderers[2].enabled = true;
            yield return new WaitForSeconds(0.2f);
        }
    }
}