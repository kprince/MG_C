using MiddleGround.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiddleGround.Audio
{
    public class MG_AudioManager : MonoBehaviour
    {
        public static MG_AudioManager Instance;
        static readonly Dictionary<int, string> dic_type_Path = new Dictionary<int, string>()
        {
            {(int)MG_PlayAudioType.Button,"MG_AudioClips/MG_AC_Button" },
            {(int)MG_PlayAudioType.SpinDice,"MG_AudioClips/MG_AC_SpinDice" },
            {(int)MG_PlayAudioType.SpinSlots,"MG_AudioClips/MG_AC_SpinSlots" },
            {(int)MG_PlayAudioType.Fly,"MG_AudioClips/MG_AC_Fly" },
        };
        readonly Dictionary<int, AudioClip> dic_type_ac = new Dictionary<int, AudioClip>();
        readonly List<AudioSource> as_all = new List<AudioSource>();
        GameObject go_root;
        public void Init(GameObject asRoot)
        {
            Instance = this;
            go_root = asRoot;
        }
        public AudioSource PlayOneShot(MG_PlayAudioType audioType)
        {
            int _typeIndex = (int)audioType;
            if(dic_type_ac.TryGetValue(_typeIndex,out AudioClip tempAC))
            {
                if(tempAC is null)
                {
                    Debug.LogError("Play MG_ShotAudio Error : loadedDic has key , but content is null.");
                    return null;
                }
                bool hasPlayed = false;
                int asCount = as_all.Count;
                for(int i = 0; i < asCount; i++)
                {
                    if (!as_all[i].isPlaying)
                    {
                        AudioSource tempAS = as_all[i];
                        tempAS.clip = tempAC;
                        tempAS.loop = false;
                        tempAS.playOnAwake = false;
                        tempAS.mute = !MG_Manager.Instance.Get_Save_SoundOn();
                        tempAS.Play();
                        hasPlayed = true;
                        return tempAS;
                    }
                }
                if (!hasPlayed)
                {
                    AudioSource tempAS = go_root.AddComponent<AudioSource>();
                    tempAS.clip = tempAC;
                    tempAS.loop = false;
                    tempAS.playOnAwake = false;
                    tempAS.mute = !MG_Manager.Instance.Get_Save_SoundOn();
                    tempAS.Play();
                    as_all.Add(tempAS);
                    return tempAS;
                }
            }
            else
            {
                if(dic_type_Path.TryGetValue(_typeIndex,out string tempPath))
                {
                    if (string.IsNullOrEmpty(tempPath))
                    {
                        Debug.LogError("Play MG_ShotAudio Error : audioPathDic has key , but content is null.");
                        return null;
                    }
                    tempAC = Resources.Load<AudioClip>(tempPath);
                    dic_type_ac.Add(_typeIndex, tempAC);
                    return PlayOneShot(audioType);
                }
                else
                {
                    Debug.LogError("Play MG_ShotAudio Error : audioPathDic not has this audio. type : " + audioType);
                    return null;
                }
            }
            return null;
        }
        public AudioSource PlayLoop(MG_PlayAudioType audioType)
        {
            AudioSource result = PlayOneShot(audioType);
            result.loop = true;
            return result;
        }
        public void SetMusicState(bool oldState)
        {
        }
        public void SetSoundState(bool oldState)
        {
            foreach(AudioSource temp in as_all)
            {
                temp.mute = oldState;
            }
        }
        public void PauseBgm(bool pause)
        {
        }
    }
    public enum MG_PlayAudioType
    {
        Button,
        SpinDice,
        SpinSlots,
        Fly
    }
}
