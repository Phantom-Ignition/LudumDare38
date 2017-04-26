using LudumDare38.Characters;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using System;
using System.Collections.Generic;

namespace LudumDare38.Managers
{
    class EnemiesSpawnManager
    {
        //--------------------------------------------------
        // Enemy Model

        public struct EnemyModel
        {
            public EnemyType Type;
            public Vector2 Position;
        }

        //--------------------------------------------------
        // Queue

        private List<EnemyModel> _queue;
        public List<EnemyModel> Queue => _queue;

        //--------------------------------------------------
        // Time stuff

        private float _spawnInterval;
        public float SpawnInterval => _spawnInterval;
        private float _currentSpawnInterval;

        //--------------------------------------------------
        // Spawn Rules

        private List<EnemyType> _waveSpawnQueue;
        private float _kamikazeCount;
        public float KamikazeCount => _kamikazeCount;
        private float _shooterCount;
        public float ShooterCount => _shooterCount;
        private int _bossCount;
        public int BossCount => _bossCount;

        //--------------------------------------------------
        // Waves

        private int _currentWave;
        public int CurrentWave => _currentWave;
        private bool _waveCompleted;
        public bool WaveCompleted => _waveCompleted;

        //--------------------------------------------------
        // Random

        private Random _rand;

        //--------------------------------------------------
        // Active

        private bool _active;
        public bool Active => _active;

        //----------------------//------------------------//

        public EnemiesSpawnManager()
        {
            _queue = new List<EnemyModel>();
            _waveSpawnQueue = new List<EnemyType>();
            _rand = new Random();
            _spawnInterval = 200.0f;
        }

        public void Start()
        {
            _active = true;
            GenerateWave();
        }

        public void StartNextWave()
        {
            _currentWave++;
            _active = true;
            _waveCompleted = false;
            GenerateWave();
        }

        public EnemyModel ShiftModelFromQueue()
        {
            var model = _queue[0];
            _queue.RemoveAt(0);
            return model;
        }

        public void Update(GameTime gameTime)
        {
            if (!_active) return;

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            _currentSpawnInterval += deltaTime;
            if (_currentSpawnInterval >= _spawnInterval)
            {
                Spawn();
                _currentSpawnInterval = 0;
            }
        }

        private void Spawn()
        {
            var model = GetNextModel();
            _queue.Add(model);
        }

        private EnemyModel GetNextModel()
        {
            return new EnemyModel
            {
                Type = GetNextWaveEnemy(),
                Position = GetRandomPosition()
            };
        }

        private void CompleteWave()
        {
            _waveCompleted = true;
            _active = false;
        }

        private EnemyType GetNextWaveEnemy()
        {
            var enemy = _waveSpawnQueue[0];
            _waveSpawnQueue.RemoveAt(0);
            if (_waveSpawnQueue.Count == 0)
            {
                CompleteWave();
            }
            return enemy;
        }

        private void GenerateWave()
        {
            IncreaseDifficulty();
            _waveSpawnQueue.Clear();
            if (_bossCount > 0)
            {
                for (var i = 0; i < _bossCount; i++)
                    _waveSpawnQueue.Add(EnemyType.Boss);
            }
            else
            {
                for (var i = 0; i < (int)_kamikazeCount; i++)
                    _waveSpawnQueue.Add(EnemyType.Kamikaze);
                for (var i = 0; i < (int)_shooterCount; i++)
                    _waveSpawnQueue.Add(EnemyType.Shooter);
            }
            _waveSpawnQueue.Shuffle(_rand);
        }

        private void IncreaseDifficulty()
        {
            _spawnInterval = MathHelper.Max(_spawnInterval * 0.95f, 300.0f);

            var wave = _currentWave + 1;

            if (wave <= 7)
            {
                switch (wave)
                {
                    case 1:
                        _kamikazeCount = 1;
                        _shooterCount = 0;
                        break;
                    case 2:
                        _kamikazeCount = 2;
                        _shooterCount = 0;
                        break;
                    case 3:
                        _kamikazeCount = 4;
                        _shooterCount = 0;
                        break;
                    case 4:
                        _kamikazeCount = 0;
                        _shooterCount = 1;
                        break;
                    case 5:
                        _kamikazeCount = 2;
                        _shooterCount = 1;
                        break;
                    case 6:
                        _kamikazeCount = 0;
                        _shooterCount = 0;
                        _bossCount = 1;
                        break;
                    case 7:
                        _kamikazeCount = 4;
                        _shooterCount = 2;
                        _bossCount = 0;
                        break;
                }
            }
            else
            {
                if (wave % 6 == 0)
                {
                    _bossCount = wave / 6;
                }
                else
                {
                    _bossCount = 0;
                    _kamikazeCount *= 1.4f;
                    _shooterCount *=  1.4f;
                }
            }
        }

        private Vector2 GetRandomPosition()
        {
            var size = SceneManager.Instance.VirtualSize;
            var side = _rand.Next(4); // Top, right, bottom and left
            var x = 0;
            var y = 0;
            switch (side)
            {
                case 0:
                    x = _rand.Next(0, (int)size.X);
                    y = 0;
                    break;
                case 1:
                    x = (int)size.X;
                    y = _rand.Next(0, (int)size.Y);
                    break;
                case 2:
                    x = _rand.Next(0, (int)size.X);
                    y = (int)size.Y;
                    break;
                case 3:
                    x = 0;
                    y = _rand.Next(0, (int)size.Y);
                    break;
            }
            return new Vector2(x, y);
        }
    }
}
