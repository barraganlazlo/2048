using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Case : MonoBehaviour
{

    private int puissanceDe2;

    public void SetPuis(int a)
    {
        puissanceDe2 = a;
        gameObject.GetComponent<SpriteRenderer>().sprite = Partie.images[puissanceDe2];
    }

    public int GetPuis()
    {
        return puissanceDe2;
    }

    public void AddPuis()
    {
        puissanceDe2 += 1;
        gameObject.GetComponent<SpriteRenderer>().sprite = Partie.images[puissanceDe2];
    }

    public void Reset()
    {
        SetPuis(0);
        gameObject.GetComponent<SpriteRenderer>().sprite = Partie.images[puissanceDe2];
    }

}
