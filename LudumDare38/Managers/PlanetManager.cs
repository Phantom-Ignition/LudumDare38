using LudumDare38.Objects.Guns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudumDare38.Managers
{
    //--------------------------------------------------
    // OrbitField

    public struct OrbitField
    {
        public int OrbitLevel;
        public float Angle;
        public bool Available;
    }

    class PlanetManager
    {
        //--------------------------------------------------
        // Singleton

        private static PlanetManager _instance = null;
        private static readonly object _padlock = new object();

        public static PlanetManager Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                        _instance = new PlanetManager();
                    return _instance;
                }
            }
        }

        //--------------------------------------------------
        // Gold

        public int Gold { get; set; } = 200;

        //--------------------------------------------------
        // Orbits

        private OrbitField[] _orbits;
        public OrbitField[] Orbits => _orbits;
        public OrbitField[] AvailableOrbits => _orbits.Where(of => of.Available).ToArray();

        private float[] _possibleAngles;        
        public int NumPossibleAngles => _possibleAngles.Length;

        //--------------------------------------------------
        // Guns queue (collected by ScenePlanet)

        private List<GameGunBase> _guns;
        public List<GameGunBase> GunsQueue =>_guns;

        //--------------------------------------------------
        // Paused

        public bool Paused { get; set; }

        //----------------------//------------------------//

        private PlanetManager()
        {
            _possibleAngles = new float[8];
            var angleStep = (float)Math.PI * 2 / _possibleAngles.Length;
            var anglesLenght = _possibleAngles.Length;
            _orbits = new OrbitField[3 * anglesLenght];
            for (var o = 0; o < 3; o++)
            {
                for (var a = 0; a < anglesLenght; a++)
                {
                    _possibleAngles[a] = angleStep * a;
                    _orbits[a + o * anglesLenght] = new OrbitField() { OrbitLevel = o + 1, Angle = angleStep * a, Available = true };
                }
            }

            _guns = new List<GameGunBase>();
        }

        public GameGunBase CreateGun(GameGunBase gun)
        {
            _guns.Add(gun);
            var index = Array.IndexOf(_orbits, gun.OrbitField);
            _orbits[index].Available = false;
            return gun;
        }

        public int AngleIndex(float angle)
        {
            return Array.IndexOf(_possibleAngles, angle);
        }
    }
}
