﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace LudumDare38.Sprites
{
    public class FramesList
    {
        public int Delay { get; set; }
        public List<FrameInfo> Frames { get; set; }
        public List<Color[]> FramesTextureData { get; set; }
        public bool Loop { get; set; }
        public bool Reset { get; set; }
        public SpriteCollider Collider { get; set; }
        public List<int> FramesToAttack { get; set; }
        public FramesList(int delay)
        {
            Frames = new List<FrameInfo>();
            FramesToAttack = new List<int>();
            FramesTextureData = new List<Color[]>();

            Delay = delay;
            Loop = Delay > 0;
            Reset = Loop;
        }
    }
}
