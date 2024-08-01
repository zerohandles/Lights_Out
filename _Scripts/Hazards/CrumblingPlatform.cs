using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumblingPlatform : MonoBehaviour
{
    BoxCollider2D platformCol;
    EdgeCollider2D bottomEdgeCol;
    List<Material> materials = new List<Material>();

    [SerializeField] ParticleSystem crumbleParticles;
    [SerializeField] float crumbleDelay;
    [SerializeField] float solidifyDelay;
    [SerializeField] float crumbleSpeed = 1;
    [SerializeField] AudioClip crumbleSound;
    AudioSource audioSource;
    float fade = 1;
    bool isCrumbling;

    void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
        platformCol = GetComponent<BoxCollider2D>();
        // Use a child object with an edge collider to prevent player from crumbling platforms from below
        bottomEdgeCol = GetComponentInChildren<EdgeCollider2D>();
        int childCount = transform.childCount;
        
        // Populate the list of materials in child objects to control the shader graph
        for (int i = 0; i < childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<SpriteRenderer>() != null )
            {
                var mat = transform.GetChild(i).GetComponent<SpriteRenderer>().material;
                materials.Add(mat);
            }
        }

        // Prevent crumble variables from being set too high or too low, breaking the crumble effect
        crumbleSpeed = Mathf.Clamp(crumbleSpeed, 0, 1);
        solidifyDelay = Mathf.Clamp(solidifyDelay, crumbleSpeed, Mathf.Infinity);
    }

 
    void Update()
    {
        // Change the shader graph fade amount depending on the platform's state.
        if (isCrumbling)
        {
            fade -= Time.deltaTime / crumbleSpeed;
            if (fade <= 0.3f)
            {
                platformCol.enabled = false;
                bottomEdgeCol.enabled = false;
            }
        }
        else
        {
            fade += Time.deltaTime;
            if (fade >= 0.5f)
            {
                platformCol.enabled = true;
                bottomEdgeCol.enabled = true;
            }
        }
        fade = Mathf.Clamp(fade, 0, 1);

        foreach (var material in materials)
            material.SetFloat("_Fade", fade);
    }

    // Play crumbling animation and set crumbling bools
    private IEnumerator CrumblePlatform()
    {
        if (isCrumbling)
            yield return null;

        audioSource.PlayOneShot(crumbleSound);
        yield return new WaitForSeconds(crumbleDelay);
        isCrumbling = true;
        crumbleParticles.Play();

        yield return new WaitForSeconds(solidifyDelay);
        isCrumbling = false;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            audioSource.pitch = Random.Range(1, 4f);
            StartCoroutine(CrumblePlatform());
        }
    }
}
