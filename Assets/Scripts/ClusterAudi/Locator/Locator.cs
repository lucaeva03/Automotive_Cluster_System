using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

// Classe che definisce struttura base che funziona con qualsiasi tipo al posto di T (Service o Feature) - evita duplicazione codice per feature e service
public class Locator<TLocatedType>
{
    // Dizionario interno, associa ogni tipo di servizio alla sua implementazione concreta
    private Dictionary<Type, TLocatedType> _myLocatedObjects;

    // Impostazione dizionario vuoto
    public Locator()
    {
        _myLocatedObjects = new Dictionary<Type, TLocatedType>();
    }

    // Registrazione servizi di tipo T
    public void Add<T>(TLocatedType instance) where T : TLocatedType
    {
        Type type = typeof(T);
        _myLocatedObjects[type] = instance;
    }

	// Cerca e ottiene servizi di tipo T
	public T Get<T>() where T : TLocatedType
    {
        Type type = typeof(T);
        
        return (T)_myLocatedObjects[type];
    }

    // LINQ  - interroga collezioni
    public List<TLocatedType> GetAll()
    {
        return _myLocatedObjects.Values.ToList(); // ToList converte valori del dizionario in lista
    }
}
