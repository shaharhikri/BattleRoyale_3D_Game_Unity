using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class RandomGuns : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int numOfGuns = this.transform.childCount;
        GameObject[] guns = new GameObject[numOfGuns];
        for (int i = 0; i < numOfGuns; i++)
        {
            guns[i] = this.transform.GetChild(i).gameObject;
            guns[i].SetActive(false);

        }
        Shuffler shuffler = new Shuffler();
        shuffler.Shuffle(guns);
        for (int i = 0; i < numOfGuns / 3; i++)
        {
            guns[i].SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public class Shuffler
    {
        /// <summary>Creates the shuffler with a <see cref="MersenneTwister"/> as the random number generator.</summary>

        public Shuffler()
        {
            _rng = new System.Random();
        }

        /// <summary>Shuffles the specified array.</summary>
        /// <typeparam name="T">The type of the array elements.</typeparam>
        /// <param name="array">The array to shuffle.</param>

        public void Shuffle<T>(IList<T> array)
        {
            for (int n = array.Count; n > 1;)
            {
                int k = _rng.Next(n);
                --n;
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }

        private System.Random _rng;
    }
}

