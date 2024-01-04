using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vaz.Constants.Databank.Foes
{   
    public class DatabankFoes : MonoBehaviour
    {
        [SerializeField] private static Foe[] foeDatabank;

        // Start is called before the first frame update
        void Start()
        {
            foeDatabank = new Foe[99];

            // BLOSSOM I:

                // Spectre              - Light
                foeDatabank[0] = Resources.Load<Foe>("Cheshire");

                // DUNGEON I:

                    // Red Minimental       - Air
                    foeDatabank[1] = Resources.Load<Foe>("Red Minimental");
                    // Orange Minimental    - Fire
                    foeDatabank[2] = Resources.Load<Foe>("Orange Minimental");
                    // Yellow Minimental    - Earth
                    foeDatabank[3] = Resources.Load<Foe>("Yellow Minimental");
                    // Green Minimental     - Life
                    foeDatabank[4] = Resources.Load<Foe>("Green Minimental");
                    // Blue Minimental      - Ice
                    foeDatabank[5] = Resources.Load<Foe>("Blue Minimental");
                    // Purple Minimental    - Lightning
                    foeDatabank[6] = Resources.Load<Foe>("Purple Minimental");
                    // Pink Minimental      - Noise
                    foeDatabank[7] = Resources.Load<Foe>("Pink Minimental");
                    // Papier Planeswarm    - Air
                    foeDatabank[8] = Resources.Load<Foe>("Papier Planeswarm");
                    // Imp                  - Fire
                    foeDatabank[8] = Resources.Load<Foe>("Imp");
                    // Mischievimp          - Earth
                    foeDatabank[9] = Resources.Load<Foe>("Mischievimp");
                    // Shallow Visage       - Life
                    foeDatabank[11] = Resources.Load<Foe>("Shallow Visage");
                    // Eyeshadow            - Lightning
                    foeDatabank[10] = Resources.Load<Foe>("Eyeshadow");
                    // Logarythym           - Noise
                    foeDatabank[13] = Resources.Load<Foe>("Logarythym");
                    // Nuckelavee           - BOSS OF DUNGEON 1 - FIRE/ICE
                    foeDatabank[14] = Resources.Load<Foe>("Nuckelavee");

                // DUNGEON II:

                    // Bogillbear           - BOSS OF DUNGEON 2 - AIR

                // DUNGEON III:

                    // Black Beast          - BOSS OF DUNGEON 3 - ICE

                // DUNGEON IV:

                    // Lady Raynham         - BOSS OF DUNGEON 4 - LIGHTNING

                // DUNGEON V:

                    // Vacant Medraut       - BOSS OF DUNGEON 5 - AIR

                // DUNGEON VI:

                    // UNIT_1023            - BOSS OF DUNGEON 6 - LIFE

            

        }

        void SeeDatabank()
        {
            for (int i = 0; i < foeDatabank.Length; i += 1)
            {
                print("#" + i + ": " + foeDatabank[i]);
            }
        }
    }
}