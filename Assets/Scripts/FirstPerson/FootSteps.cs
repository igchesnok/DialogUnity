using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class FootSteps : MonoBehaviour
{
    public enum StepsOn { Beton, Wood, Metal, Ground };
    public ResourceRequest audioList_;

    private string mainFolder = "FootSteps";// mainFolder - родительская папка в Resources
    private string betonFolder = "Beton";// betonFolder и т.д. дочерние
    private string woodFolder = "Wood";
    private string metalFolder = "Metal";
    private string groundFolder = "Ground";
    private AudioClip[] beton;
    private AudioClip[] wood;
    private AudioClip[] metal;
    private AudioClip[] ground;
    private AudioSource source;
	private AudioClip clip;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.mute = false;
        source.loop = false;
        LoadSounds();
    }

    void LoadSounds()
    {
        beton = Resources.LoadAll<AudioClip>(mainFolder + "/" + betonFolder);
        wood = Resources.LoadAll<AudioClip>(mainFolder + "/" + woodFolder);
        metal = Resources.LoadAll<AudioClip>(mainFolder + "/" + metalFolder);
        ground = Resources.LoadAll<AudioClip>(mainFolder + "/" + groundFolder);
    }

    public List<AudioClip> GetAllObjectsOnlyInScene()
    {
        List<AudioClip> objectsInScene = new List<AudioClip>();
        foreach (AudioClip go in Resources.FindObjectsOfTypeAll(typeof(AudioClip)) as AudioClip[])
        {
            objectsInScene.Add(go);
        }
        return objectsInScene;
    }

    public void PlayStep(StepsOn stepsOn, float volume)
    {
        switch (stepsOn)
        {
            case StepsOn.Beton:
                clip = beton[Random.Range(0, beton.Length)];
                break;
            case StepsOn.Wood:
                clip = wood[Random.Range(0, wood.Length)];
                break;
            case StepsOn.Metal:
                clip = metal[Random.Range(0, metal.Length)];
                break;
            case StepsOn.Ground:
                clip = ground[Random.Range(0, ground.Length)];
                break;
        }
        source.PlayOneShot(clip, volume);
    }
}
