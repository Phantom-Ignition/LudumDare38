using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LudumDare38.Managers
{
    public static class SoundManager
    {
        //--------------------------------------------------
        // Content Manager

        private static Dictionary<string, SoundEffect> _seCache = new Dictionary<string, SoundEffect>();
        private static ContentManager _contentManager = new ContentManager(SceneManager.Instance.Content.ServiceProvider, "Content");

        //--------------------------------------------------
        // BGM Name

        private static string _bgmName;

        //--------------------------------------------------
        // SEs

        private static SoundEffect _cancelSe;
        private static SoundEffect _confirmSe;
        private static SoundEffect _selectSe;

        private static Dictionary<string, SoundEffect> _seQueue;
        private static List<SoundEffect> _seToRemove;

        //--------------------------------------------------
        // BGMs

        private static Dictionary<string, Song> _songs;

        //--------------------------------------------------
        // Volumes

        private static float _bgmVolume;
        private static float _seVolume;

        //--------------------------------------------------
        // For debug purposes

        private static bool _soundOn = false;

        //----------------------//------------------------//

        public static void Initialize()
        {
            _cancelSe = LoadSe("Cancel");
            _confirmSe = LoadSe("Confirm");
            _selectSe = LoadSe("Cursor");

            _songs = new Dictionary<string, Song>
            {
                { "Rhinoceros", LoadBgm("Rhinoceros") },
                { "SpaceFighterLoop", LoadBgm("SpaceFighterLoop") }
            };

            MediaPlayer.IsRepeating = true;

            _seQueue = new Dictionary<string, SoundEffect>();
            _seToRemove = new List<SoundEffect>();

            _bgmVolume = 1.0f;
            _seVolume = 1.0f;
        }

        public static void SetBgmVolume(float volume)
        {
            _bgmVolume = volume;
            MediaPlayer.Volume = _bgmVolume;
        }

        public static void SetSeVolume(float volume)
        {
            _seVolume = volume;
        }

        public static Song LoadBgm(string filename)
        {
            return _contentManager.Load<Song>("sounds/bgm/" + filename);
        }

        public static SoundEffect LoadSe(string filename)
        {
            if (!_seCache.ContainsKey(filename))
            {
                try
                {
                    _seCache[filename] = _contentManager.Load<SoundEffect>("sounds/se/" + filename);
                }
                catch (Exception ex)
                {
                    _seCache[filename] = null;
                    Debug.WriteLine(ex.ToString());
                }
            }
            return _seCache[filename];
        }

        public static void StartBgm(string bgmName)
        {
            if (_soundOn)
            {
                if (bgmName != _bgmName)
                {
                    if (MediaPlayer.State == MediaState.Playing)
                    {
                        MediaPlayer.Stop();
                    }
                    MediaPlayer.Play(_songs[bgmName]);
                    _bgmName = bgmName;
                }
                MediaPlayer.Volume = _bgmVolume;
            }
        }

        public static void PlaySafe(this SoundEffect se)
        {
            if (_soundOn && se != null && !_seQueue.ContainsKey(se.Name))
            {
                _seQueue.Add(se.Name, se);
            }
        }

        public static void PlaySafe(this SoundEffectInstance seInstance)
        {
            if (_soundOn && seInstance != null)
            {
                seInstance.Volume -= 1 - _seVolume;
                seInstance.Volume = MathHelper.Clamp(seInstance.Volume, 0.0f, 1.0f);
                seInstance.Play();
            }
        }

        public static void PlayCancelSe()
        {
            _cancelSe.PlaySafe();
        }

        public static void PlayConfirmSe()
        {
            _confirmSe.PlaySafe();
        }

        public static void PlaySelectSe()
        {
            _selectSe.PlaySafe();
        }

        public static void Update()
        {
            foreach (var se in _seQueue)
            {
                var instance = se.Value.CreateInstance();
                instance.Volume = _seVolume;
                instance.Play();
                _seToRemove.Add(se.Value);
            }
            _seToRemove.ForEach(se => _seQueue.Remove(se.Name));
            _seToRemove.Clear();
        }

        public static void Dispose()
        {
            _cancelSe.Dispose();
            _confirmSe.Dispose();
            _selectSe.Dispose();
        }
    }
}
