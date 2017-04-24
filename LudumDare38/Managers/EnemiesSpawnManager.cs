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

        private List<List<EnemyModel>> _spawnRules;

        private List<EnemyType> _waveSpawnQueue;
        private int _kamizakeCount;
        private int _shooterCount;
        private int _increaseTurn;

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

            _currentSpawnInterval = 1500.0f; // TODO REMOVE THIS

            CreateSpawnRules();
            InitEnemiesCount();
        }

        private void CreateSpawnRules()
        {
            var virtualSize = SceneManager.Instance.VirtualSize;
            _spawnRules = new List<List<EnemyModel>>
            {
                // Wave #1
                new List<EnemyModel> {
                    new  EnemyModel
                    {
                        Type = EnemyType.Kamikaze,
                        Position = new Vector2(virtualSize.X, virtualSize.Y / 2)
                    },/*
                    new  EnemyModel
                    {
                        Type = EnemyType.Shooter,
                        Position = new Vector2(-10, virtualSize.Y / 4)
                    },
                    new  EnemyModel
                    {
                        Type = EnemyType.Shooter,
                        Position = new Vector2(virtualSize.X / 2, 0)
                    },
                    new  EnemyModel
                    {
                        Type = EnemyType.Shooter,
                        Position = new Vector2(virtualSize.X / 2, virtualSize.Y)
                    }*/
                },
                /*
                // Wave #2
                new List<EnemyModel> {
                    new  EnemyModel
                    {
                        Type = EnemyType.Kamikaze,
                        Position = new Vector2(0, virtualSize.Y / 2)
                    },
                }
                */
            };
        }

        private void InitEnemiesCount()
        {
            _kamizakeCount = 3;
            _shooterCount = 2;
        }

        public void Start()
        {
            _active = true;
        }

        public void StartNextWave()
        {
            _currentWave++;
            _active = true;
            _waveCompleted = false;
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
            if (_spawnRules.Count > 0)
            {
                var model = _spawnRules[0][0];
                _spawnRules[0].RemoveAt(0);
                if (_spawnRules[0].Count == 0)
                {
                    _spawnRules.RemoveAt(0);
                    CompleteWave();
                }
                return model;
            }

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
            if (_spawnRules.Count == 0)
            {
                GenerateWave();
            }
        }

        private EnemyType GetNextWaveEnemy()
        {
            var enemy = _waveSpawnQueue[0];
            _waveSpawnQueue.RemoveAt(0);
            if (_waveSpawnQueue.Count == 0)
            {
                CompleteWave();
                GenerateWave();
            }
            return enemy;
        }

        private void GenerateWave()
        {
            IncreaseDifficulty();
            _waveSpawnQueue.Clear();
            for (var i = 0; i < _kamizakeCount; i++)
                _waveSpawnQueue.Add(EnemyType.Kamikaze);
            for (var i = 0; i < _shooterCount; i++)
                _waveSpawnQueue.Add(EnemyType.Shooter);
            _waveSpawnQueue.Shuffle(_rand);
        }

        private void IncreaseDifficulty()
        {
            _spawnInterval = MathHelper.Max(_spawnInterval * 0.95f, 300.0f);
            if (_increaseTurn == 0) _kamizakeCount++;
            else _shooterCount++;
            _increaseTurn = _increaseTurn == 0 ? 1 : 0;
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
