using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vaz.Constants.Databank.Heroes
{   
    public class DatabankHeroes : MonoBehaviour
    {
        [SerializeField] private static Hero[] heroDatabank;

        // Start is called before the first frame update
        void Start()
        {
            heroDatabank = new Hero[4];

            // BLOSSOM I:

                // Lorelei Stavrou
                heroDatabank[0] = Resources.Load<Hero>("Lorelei");
                // Oscar Flint
                heroDatabank[1] = Resources.Load<Hero>("Oscar");
                // Jasmine Trisul
                heroDatabank[2] = Resources.Load<Hero>("Jasmine");
                // Cloud Delapore
                heroDatabank[3] = Resources.Load<Hero>("Cloud");

            // BLOSSOM II:
            /*
                // Lorelei Stavrou
                heroDatabank[0] = Resources.Load<Hero>("Lorelei");
                // Kaito Katawi
                heroDatabank[1] = Resources.Load<Hero>("Kaito");
                // Oscar Flint
                heroDatabank[2] = Resources.Load<Hero>("Oscar");
                // Ali Cooper
                heroDatabank[3] = Resources.Load<Hero>("Ali");
                // Jasmine Trisul
                heroDatabank[4] = Resources.Load<Hero>("Jasmine");
                // Penny Carde
                heroDatabank[5] = Resources.Load<Hero>("Penny");
                // Cloud Delapore
                heroDatabank[6] = Resources.Load<Hero>("Cloud");
            */
            // BLOSSOM III:
            /*
                // Lorelei Stavrou
                heroDatabank[0] = Resources.Load<Hero>("Lorelei");
                // Kaito Katawi
                heroDatabank[1] = Resources.Load<Hero>("Kaito");
                // Oscar Flint
                heroDatabank[2] = Resources.Load<Hero>("Oscar");
                // Ali Cooper
                heroDatabank[3] = Resources.Load<Hero>("Ali");
                // Jasmine Trisul
                heroDatabank[4] = Resources.Load<Hero>("Jasmine");
                // Penny Carde
                heroDatabank[5] = Resources.Load<Hero>("Penny");
                // Cloud Delapore
                heroDatabank[6] = Resources.Load<Hero>("Cloud");
                // Azriel/Azran, disappears with the destruction of the Boltzmann Brain

            */
            // BLOSSOM IV:
            /*
                // Lorelei Stavrou
                heroDatabank[0] = Resources.Load<Hero>("Lorelei");
                // Kaito Katawi
                heroDatabank[1] = Resources.Load<Hero>("Kaito");
                // Oscar Flint
                heroDatabank[2] = Resources.Load<Hero>("Oscar");
                // Ali Cooper
                heroDatabank[3] = Resources.Load<Hero>("Ali");
                // Jasmine Trisul
                heroDatabank[4] = Resources.Load<Hero>("Jasmine");
                // Penny Carde
                heroDatabank[5] = Resources.Load<Hero>("Penny");
                // Cloud Delapore
                heroDatabank[6] = Resources.Load<Hero>("Cloud");
                // Wilde Carde, assists with some dungeons and bosses

                // Siddh Trisul, assists on final day

            */
            // BLOSSOM V:
            /*
                // Lorelei Stavrou
                heroDatabank[0] = Resources.Load<Hero>("Lorelei");
                // Kaito Katawi
                heroDatabank[1] = Resources.Load<Hero>("Kaito");
                // Oscar Flint
                heroDatabank[2] = Resources.Load<Hero>("Oscar");
                // Ali Cooper
                heroDatabank[3] = Resources.Load<Hero>("Ali");
                // Jasmine Trisul
                heroDatabank[4] = Resources.Load<Hero>("Jasmine");
                // Penny Carde
                heroDatabank[5] = Resources.Load<Hero>("Penny");
                // Cloud Delapore
                heroDatabank[6] = Resources.Load<Hero>("Cloud");
                // Wilde Carde, assists with some dungeons and bosses

                // Siddh Trisul, assists with some dungeons and bosses.

                // Emma Delapore, assists with some dungeons and bosses.
                    
                // Momiji Katawi, assists with some dungeons and bosses

            */
            // BLOSSOM VI:
            /*
                // Lorelei Stavrou
                heroDatabank[0] = Resources.Load<Hero>("Lorelei");
                // Kaito Katawi
                heroDatabank[1] = Resources.Load<Hero>("Kaito");
                // Oscar Flint
                heroDatabank[2] = Resources.Load<Hero>("Oscar");
                // Ali Cooper
                heroDatabank[3] = Resources.Load<Hero>("Ali");
                // Jasmine Trisul
                heroDatabank[4] = Resources.Load<Hero>("Jasmine");
                // Penny Carde
                heroDatabank[5] = Resources.Load<Hero>("Penny");
                // Cloud Delapore
                heroDatabank[6] = Resources.Load<Hero>("Cloud");
                // Wilde Carde, assists with some dungeons and bosses

                // Siddh Trisul, assists with some dungeons and bosses.

                // Emma Delapore, assists with some dungeons and bosses.

                // ...

            */
            // BLOSSOM NULLA
            /*
                // Duran Godwin

                // UNIT-1023

            */

        }

        void SeeDatabank()
        {
            for (int i = 0; i < heroDatabank.Length; i += 1)
            {
                print("#" + i + ": " + heroDatabank[i]);
            }
        }
    }
}

