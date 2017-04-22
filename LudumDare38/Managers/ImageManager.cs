using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LudumDare38.Managers
{
    public static class ImageManager
    {
        //--------------------------------------------------
        // Cache & Content Manager

        private static Dictionary<string, Texture2D> _cache = new Dictionary<string, Texture2D>();
        private static ContentManager _contentManager = new ContentManager(SceneManager.Instance.Content.ServiceProvider, "Content");

        //----------------------//------------------------//

        public static Texture2D Load(string filename)
        {
            return LoadBitmap("imgs/" + filename);
        }

        public static Texture2D LoadHud(string filename)
        {
            return LoadBitmap("imgs/hud/" + filename);
        }

        public static Texture2D LoadGun(string filename)
        {
            return LoadBitmap("imgs/guns/" + filename);
        }

        public static Texture2D LoadProjectile(string filename)
        {
            return LoadBitmap("imgs/projectiles/" + filename);
        }

        public static Texture2D LoadEnemy(string filename)
        {
            return LoadBitmap("imgs/enemies/" + filename);
        }

        public static Texture2D LoadScene(string scene, string filename)
        {
            return LoadBitmap(String.Format("imgs/scenes/{0}/{1}", scene, filename));
        }

        public static Texture2D LoadBitmap(string filename)
        {
            if (!_cache.ContainsKey(filename))
            {
                try
                {
                    _cache[filename] = _contentManager.Load<Texture2D>(filename);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
            return _cache[filename];
        }
    }
}
