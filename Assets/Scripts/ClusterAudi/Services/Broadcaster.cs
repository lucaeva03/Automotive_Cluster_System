using System;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

namespace ClusterAudi
{
    // Sistema eventi centralizzato
    public class Broadcaster : IBroadcaster
    {
        // Dizionario che memorizza chi ascolta che cosa
        private Dictionary<Type, List<object>> _eventsDictionary;

        // Impostazione dizionario vuoto
        public Broadcaster()
        {
            _eventsDictionary = new Dictionary<Type, List<object>>();
        }

        // Registrazione di una chiamata (Feature) per un tipo di evento
        public void Add<T>(Action<T> arg)
        {
            Type type = typeof(T);

            // Creazione lista se primo elemento
            if (!_eventsDictionary.ContainsKey(type))
            {
                _eventsDictionary[type] = new List<object>();
            }

            _eventsDictionary[type].Add(arg);
        }

        // Rimozione di una chiamata (Feature) per un tipo di evento
        public void Remove<T>(Action<T> arg)
        {
            Type type = typeof(T);
            if (_eventsDictionary.ContainsKey(type))
            {
                _eventsDictionary[type].Remove(arg);
            }
        }

        // Trasmissione di un evento a tutti gli ascoltatori registrati
        public void Broadcast<T>(T arg)
        {
            Type type = typeof(T);
            if (_eventsDictionary.ContainsKey(type))
            {
                for (int i = 0; i < _eventsDictionary[type].Count; i++)
                {
                    try
                    {
                        (_eventsDictionary[type][i] as Action<T>).Invoke(arg);
                    }
                    catch (Exception e)
                    {
						// Impedisce eccezione di bloccare l'intera operazione
						Debug.LogError(e.Message);
                    }
                }
            }
        }
    }
}