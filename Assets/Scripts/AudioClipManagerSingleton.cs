using UnityEngine;

public class AudioClipManagerSingleton
{
    public static AudioClipManagerSingleton Instance
    {
        get
        {
            instance = instance ?? new AudioClipManagerSingleton();
            return instance;
        }
    }

    private static AudioClipManagerSingleton instance;

    private AudioClip bgm;
    private AudioClip abduct;
    private AudioClip result;

    public void Preload()
    {
        bgm = bgm ?? Resources.Load<AudioClip>("Audios/ufo");
        abduct = abduct ?? Resources.Load<AudioClip>("Audios/uku");
        result = result ?? Resources.Load<AudioClip>("Audios/spaceship2");
    }

    public AudioClip Bgm
    {
        get
        {
            bgm = bgm ?? Resources.Load<AudioClip>("Audios/ufo");
            return bgm;
        }
    }

    public AudioClip Abduct
    {
        get
        {
            abduct = abduct ?? Resources.Load<AudioClip>("Audios/uku");
            return abduct;
        }
    }

    public AudioClip Result
    {
        get
        {
            result = result ?? Resources.Load<AudioClip>("Audios/spaceship2");
            return result;
        }
    }
}
