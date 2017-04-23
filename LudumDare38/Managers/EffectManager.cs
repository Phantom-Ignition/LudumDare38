using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudumDare38.Managers
{
    class EffectManager
    {
        //--------------------------------------------------
        // Cache & Content Manager
        
        private static ContentManager _contentManager = new ContentManager(SceneManager.Instance.Content.ServiceProvider, "Content");

        //----------------------//------------------------//

        public static Effect Load(string filename)
        {
            return _contentManager.Load<Effect>("effects/" + filename).Clone();
        }
    }
}
