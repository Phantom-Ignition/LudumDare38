using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;

namespace LudumDare38.Sprites
{
    //--------------------------------------------------
    // Sprite direction

    public enum SpriteDirection
    {
        Left,
        Right
    }

    //----------------------//------------------------//

    public class CharacterSprite : Sprite
    {
        //--------------------------------------------------
        // Frames stuff

        private int _currentFrame;
        public int CurrentFrame => _currentFrame;

        private string _currentFrameList;
        public string CurrentFrameList => _currentFrameList;

        private bool _looped;
        public bool Looped => _looped;

        private Dictionary<string, FramesList> _framesList;

        //--------------------------------------------------
        // Animation delay

        private int _delayTick;

        //--------------------------------------------------
        // Battle System visual stuff

        private bool _dyingAnimation;
        private bool _skipDyingAnimationFrames;
        private bool _dyingAnimationEnded;
        public bool DyingAnimationEnded => _dyingAnimationEnded;

        //--------------------------------------------------
        // Collider

        public SpriteCollider Collider => GetCurrentFramesList().Collider;

        //----------------------//------------------------//

        public CharacterSprite(Texture2D file) : base(file)
        {
            _currentFrame = 0;
            _currentFrameList = "stand";
            _delayTick = 0;
            _framesList = new Dictionary<string, FramesList>();
            _looped = false;

            _dyingAnimation = false;
            _dyingAnimationEnded = false;

            Origin = Vector2.Zero;
        }

        public void CreateFrameList(string name, int delay)
        {
            _framesList[name] = new FramesList(delay);
        }

        public void CreateFrameList(string name, int delay, bool reset)
        {
            _framesList[name] = new FramesList(delay);
            _framesList[name].Reset = reset;
        }

        public void ResetCurrentFrameList()
        {
            _currentFrame = 0;
            _looped = false;
            _delayTick = 0;
        }

        public void AddFrames(string name, List<Rectangle> frames, int[] offsetX, int[] offsetY)
        {
            for (var i = 0; i < frames.Count; i++)
            {
                _framesList[name].Frames.Add(new FrameInfo(frames[i], offsetX[i], offsetY[i]));
            }
        }

        public void AddFrames(string name, List<Rectangle> frames)
        {
            var offsetX = new int[frames.Count];
            var offsetY = new int[frames.Count];
            AddFrames(name, frames, offsetX, offsetY);
        }

        public void AddCollider(string name, Rectangle rectangle)
        {
            var collider = new SpriteCollider(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            collider.Type = SpriteCollider.ColliderType.Block;
            _framesList[name].Collider = collider;
        }

        public void AddAttackCollider(string name, List<List<Rectangle>> rectangleFrames, int attackWidth)
        {
            for (var i = 0; i < rectangleFrames.Count; i++)
            {
                for (var j = 0; j < rectangleFrames[i].Count; j++)
                {
                    var collider = new SpriteCollider(rectangleFrames[i][j].X, rectangleFrames[i][j].Y, rectangleFrames[i][j].Width, rectangleFrames[i][j].Height);
                    collider.Type = SpriteCollider.ColliderType.Attack;
                    collider.AttackWidth = attackWidth;
                    _framesList[name].Frames[i].AttackColliders.Add(collider);
                }
            }
        }

        public void AddFramesToAttack(string name, params int[] frames)
        {
            for (var i = 0; i < frames.Length; i++)
                _framesList[name].FramesToAttack.Add(frames[i]);
        }

        public void GenerateTextureData()
        {
            foreach (var pair in _framesList)
            {
                var frameList = pair.Value;
                for (var i = 0; i < frameList.Frames.Count; i++)
                {
                    var frame = frameList.Frames[i];
                    var frameRect = frame.SpriteSheetInfo;
                    var textureData = new Color[frameRect.Width * frameRect.Height];
                    TextureRegion.Texture.GetData(0,
                        new Rectangle(frameRect.X, frameRect.Y, frameRect.Width, frameRect.Height),
                        textureData,
                        0,
                        textureData.Length);
                    frameList.FramesTextureData.Add(textureData);
                }
            }
        }

        public void SetFrameList(string name)
        {
            if (_currentFrameList != name)
            {
                _currentFrame = 0;
                _delayTick = 0;
                _currentFrameList = name;
                _looped = false;
                if (!_framesList[_currentFrameList].Reset)
                {
                    _framesList[_currentFrameList].Loop = true;
                }
            }
        }

        public void SetFrameListOnly(string name)
        {
            if (_currentFrameList != name)
            {
                _currentFrameList = name;
                _looped = false;
            }
        }

        public void SetIfFrameListExists(string name)
        {
            if (_framesList.ContainsKey(name))
                SetFrameList(name);
        }
        
        public void SetPosition(Vector2 position)
        {
            Position = new Vector2((int)position.X, (int)position.Y);

            for (var i = 0; i < GetCurrentFramesList().Frames.Count; i++)
            {
                for (var j = 0; j < GetCurrentFramesList().Frames[i].AttackColliders.Count; j++)
                {
                    var collider = GetCurrentFramesList().Frames[i].AttackColliders[j];
                    var offsetX = 0;
                    if (Effect == SpriteEffects.FlipHorizontally)
                        offsetX = 2 * (collider.OffsetX - GetBlockCollider().OffsetX) - GetBlockCollider().Width + collider.Width;
                    collider.Position = new Vector2(position.X - offsetX, position.Y);
                }
            }

            GetCurrentFramesList().Collider.Position = position;
        }

        public void SetDirection(SpriteDirection direction)
        {
            if (direction == SpriteDirection.Left)
                Effect = SpriteEffects.FlipHorizontally;
            else
                Effect = SpriteEffects.None;
        }

        public void RequestDyingAnimation()
        {
            _dyingAnimation = true;
            if (!_framesList.ContainsKey("dying"))
                _skipDyingAnimationFrames = true;
        }

        public Rectangle GetCurrentFrameRectangle()
        {
            return GetCurrentFramesList().Frames[_currentFrame].SpriteSheetInfo;
        }

        public FramesList GetCurrentFramesList()
        {
            return _framesList[_currentFrameList];
        }

        public SpriteCollider GetBlockCollider()
        {
            return _framesList[_currentFrameList].Collider;
        }

        public int GetFrameWidth()
        {
            return GetCurrentFrameRectangle().Width;
        }

        public int GetFrameHeight()
        {
            return GetCurrentFrameRectangle().Height;
        }

        public int GetColliderWidth()
        {
            return _framesList[_currentFrameList].Collider.Width;
        }

        public int GetColliderHeight()
        {
            return _framesList[_currentFrameList].Collider.Height;
        }

        public Color[] GetCurrentFrameTextureData()
        {
            return _framesList[_currentFrameList].FramesTextureData[_currentFrame];
        }

        public void Update(GameTime gameTime)
        {
            if (_dyingAnimation)
                UpdateDying(gameTime);

            if (_framesList[_currentFrameList].Loop)
            {
                _delayTick += gameTime.ElapsedGameTime.Milliseconds;
                if (_delayTick > _framesList[_currentFrameList].Delay)
                {
                    _delayTick -= _framesList[_currentFrameList].Delay;
                    _currentFrame++;
                    if (_currentFrame == GetCurrentFramesList().Frames.Count)
                    {
                        if (!_framesList[_currentFrameList].Reset)
                        {
                            _currentFrame--;
                            _framesList[_currentFrameList].Loop = false;
                        }
                        else _currentFrame = 0;
                        if (!_looped) _looped = true;
                    }
                }
            }
        }

        public void UpdateDying(GameTime gameTime)
        {
            if ((_framesList.ContainsKey("dying") && _currentFrameList == "dying" && _looped) || _skipDyingAnimationFrames)
            {
                Alpha -= 0.05f;
                if (Alpha <= 0.0f)
                {
                    _dyingAnimationEnded = true;
                    _dyingAnimation = false;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            if (!IsVisible) return;

            if (Effect == SpriteEffects.FlipHorizontally)
                position.X -= GetCurrentFrameRectangle().Width - (GetBlockCollider().OffsetX + GetBlockCollider().Width) + GetCurrentFramesList().Frames[_currentFrame].OffsetX;
            else
                position.X -= GetBlockCollider().OffsetX - GetCurrentFramesList().Frames[_currentFrame].OffsetX;

            position.Y -= GetBlockCollider().OffsetY - GetCurrentFramesList().Frames[_currentFrame].OffsetY;
            spriteBatch.Draw(TextureRegion.Texture, position, GetCurrentFrameRectangle(),
                Color * Alpha, Rotation, Origin, Scale, Effect, 0);
        }
    }
}
