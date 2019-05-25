using System.Collections.Generic;
using UnityEngine;

public class Grille : MonoBehaviour
{
    public enum DIRECTION { Haut, Bas, Gauche, Droite };
    public int dimension=4;
    public Case[,] Cases = new Case[4, 4]; //tableau à 2 dimensions de cases rangées à leurs position
    public GameObject Case; //prefab à instancier
    public Partie partie;


    /// <summary>
    ///     Génère une Grille de taille de l'attribut dimension 
    /// </summary>
    public void GenerationGrille(Partie p, int d)
    {
        //intilisation variables de la classe
        dimension = d;
        partie = p;

        //positionnement et ajustement taille du fond de la grille selon la dimension donnée
        float espace = 0.2f;
        float tailleGrille = dimension + espace * (dimension + 1);
        float positionGrille = (1 + espace) * ((float)dimension - 1) / 2;
        transform.localScale = new Vector3(tailleGrille, tailleGrille, 0);
        transform.position = new Vector3(positionGrille, -positionGrille, 0);

        //On positionne la caméra de façon à ce que, peut importe les dimensions, la grille soit à la même position et on règle le zoom
        //pour que la grille occupe toujours le même espace
        Camera.main.transform.position = new Vector3(positionGrille, -positionGrille, -10);
        Camera.main.orthographicSize = (3 * dimension / 4);
        Cases = new Case[dimension, dimension];

        //On instancie les cases
        for (int y = 0; y < dimension; y++)
        {
            for (int x = 0; x < dimension; x++)
            {
                //positions réelles dans le monde
                float realX = x * (1 + espace);
                float realY = y * (1 + espace);

                //quaternion.identity veut dire "on ne change pas la rotation"
                GameObject c = Instantiate<GameObject>(Case, new Vector3(realX, -realY, -1), Quaternion.identity) as GameObject;
                c.name = "Case" + y + "/" + x;
                Cases[x, y] = c.GetComponent<Case>();
            }
        }
        GenererCaseDebut();
    }

    public void DecalerSousTableau(int index, DIRECTION dir)
    {
        /*Droite*/
        if (dir == DIRECTION.Droite)
        {
            for (int xref = dimension - 1; xref > 0; xref--)
            {
                for (int x = xref - 1; x >= 0; x--)
                {
                    if (Cases[xref, index].GetPuis() == 0 && Cases[x, index].GetPuis() != 0)
                    {
                        Cases[xref, index].SetPuis(Cases[x, index].GetPuis());
                        Cases[x, index].Reset();
                    }
                }
            }
        }
        /*Gauche*/
        if (dir == DIRECTION.Gauche)
        {
            for (int xref = 0; xref < dimension - 1; xref++)
            {
                for (int x = xref + 1; x <= dimension - 1; x++)
                {
                    if (Cases[xref, index].GetPuis() == 0 && Cases[x, index].GetPuis() != 0)
                    {
                        Cases[xref, index].SetPuis(Cases[x, index].GetPuis());
                        Cases[x, index].Reset();
                    }
                }
            }
        }
        /*Bas*/
        if (dir == DIRECTION.Bas)
        {
            for (int yref = dimension - 1; yref > 0; yref--)
            {
                for (int y = yref - 1; y >= 0; y--)
                {
                    if (Cases[index, yref].GetPuis() == 0 && Cases[index, y].GetPuis() != 0)
                    {
                        Cases[index, yref].SetPuis(Cases[index, y].GetPuis());
                        Cases[index, y].Reset();
                    }
                }
            }
        }
        /*Haut*/
        if (dir == DIRECTION.Haut)
        {
            for (int yref = 0; yref < dimension - 1; yref++)
            {
                for (int y = yref + 1; y <= dimension - 1; y++)
                {
                    if (Cases[index, yref].GetPuis() == 0 && Cases[index, y].GetPuis() != 0)
                    {
                        Cases[index, yref].SetPuis(Cases[index, y].GetPuis());
                        Cases[index, y].Reset();
                    }
                }
            }
        }
    }


    public void FusionSousTableau(int index, DIRECTION dir)
    {
        /*Droite*/
        if (dir == DIRECTION.Droite)
        {
            for (int x = dimension - 1; x > 0; x--)
            {
                if (Cases[x, index].GetPuis() != 0 && Cases[x, index].GetPuis() == Cases[x - 1, index].GetPuis())
                {
                    if (partie != null && partie.scoreObject != null)
                    {
                        partie.scoreObject.AddScore(Cases[x, index].GetPuis());
                    }
                    Cases[x, index].AddPuis();
                    Cases[x - 1, index].Reset();
                }
            }
        }
        /*Gauche*/
        if (dir == DIRECTION.Gauche)
        {
            for (int x = 0; x < dimension - 1; x++)
            {
                if (Cases[x, index].GetPuis() != 0 && Cases[x, index].GetPuis() == Cases[x + 1, index].GetPuis())
                {
                    if (partie != null && partie.scoreObject != null)
                    {
                        partie.scoreObject.AddScore(Cases[x, index].GetPuis());  // Ajoute cette fusion au score.  
                    }
                    Cases[x, index].AddPuis();
                    Cases[x + 1, index].Reset();
                }
            }
        }
        /*Bas*/
        if (dir == DIRECTION.Bas)
        {
            for (int y = dimension - 1; y > 0; y--)
            {
                if (Cases[index, y].GetPuis() != 0 && Cases[index, y].GetPuis() == Cases[index, y - 1].GetPuis())
                {
                    if (partie != null && partie.scoreObject!=null)
                    {
                        partie.scoreObject.AddScore(Cases[index, y].GetPuis());
                    }                    
                    Cases[index, y].AddPuis();
                    Cases[index, y - 1].Reset();
                }
            }
        }
        /*Haut*/
        if (dir == DIRECTION.Haut)
        {
            for (int y = 0; y < dimension - 1; y++)
            {
                if (Cases[index, y].GetPuis() != 0 && Cases[index, y].GetPuis() == Cases[index, y + 1].GetPuis())
                {
                    if (partie != null && partie.scoreObject != null)
                    {
                        partie.scoreObject.AddScore(Cases[index, y].GetPuis());
                    }
                    Cases[index, y].AddPuis();
                    Cases[index, y + 1].Reset();
                }
            }
        }
    }

    public void DeplacerGrille(DIRECTION dir)
    {
        for (int index = 0; index < dimension; index++)
        {
            DecalerSousTableau(index, dir);
            FusionSousTableau(index, dir);
            DecalerSousTableau(index, dir);
        }
    }

    // Fonctions de vérification de la possibilité de déplacement et/ou fusion de la grille
    public bool DecalagePossible(DIRECTION dir)
    {
        for (int index = 0; index < dimension; index++)
        {
            /*Droite*/
            if (dir == DIRECTION.Droite)
            {
                for (int xref = dimension - 1; xref > 0; xref--)
                {
                    for (int x = xref - 1; x >= 0; x--)
                    {
                        if (Cases[xref, index].GetPuis() == 0 && Cases[x, index].GetPuis() != 0)
                        {
                            return true;
                        }
                    }
                }
            }
            /*Gauche*/
            if (dir == DIRECTION.Gauche)
            {
                for (int xref = 0; xref < dimension - 1; xref++)
                {
                    for (int x = xref + 1; x <= dimension - 1; x++)
                    {
                        if (Cases[xref, index].GetPuis() == 0 && Cases[x, index].GetPuis() != 0)
                        {
                            return true;
                        }
                    }
                }
            }
            /*Bas*/
            if (dir == DIRECTION.Bas)
            {
                for (int yref = dimension - 1; yref > 0; yref--)
                {
                    for (int y = yref - 1; y >= 0; y--)
                    {
                        if (Cases[index, yref].GetPuis() == 0 && Cases[index, y].GetPuis() != 0)
                        {
                            return true;
                        }
                    }
                }
            }
            /*Haut*/
            if (dir == DIRECTION.Haut)
            {
                for (int yref = 0; yref < dimension - 1; yref++)
                {
                    for (int y = yref + 1; y <= dimension - 1; y++)
                    {
                        if (Cases[index, yref].GetPuis() == 0 && Cases[index, y].GetPuis() != 0)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool FusionPossible(DIRECTION dir)
    {
        for (int index = 0; index < dimension; index++)
        {
            /*Droite*/
            if (dir == DIRECTION.Droite)
            {
                for (int xref = dimension - 1; xref > 0; xref--)
                {
                    if (Cases[xref, index].GetPuis() != 0 &&
                        Cases[xref, index].GetPuis() == Cases[xref - 1, index].GetPuis())
                    {
                        return true;
                    }
                }
            }
            /*Gauche*/
            if (dir == DIRECTION.Gauche)
            {
                for (int xref = 0; xref < dimension - 1; xref++)
                {
                    if (Cases[xref, index].GetPuis() != 0 &&
                        Cases[xref, index].GetPuis() == Cases[xref + 1, index].GetPuis())
                    {
                        return true;
                    }
                }
            }
            /*Bas*/
            if (dir == DIRECTION.Bas)
            {
                for (int yref = dimension - 1; yref > 0; yref--)
                {
                    if (Cases[index, yref].GetPuis() != 0 &&
                        Cases[index, yref].GetPuis() == Cases[index, yref - 1].GetPuis())
                    {
                        return true;
                    }
                }
            }
            /*Haut*/
            if (dir == DIRECTION.Haut)
            {
                for (int yref = 0; yref < dimension - 1; yref++)
                {
                    if (Cases[index, yref].GetPuis() != 0 &&
                        Cases[index, yref].GetPuis() == Cases[index, yref + 1].GetPuis())
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    public int CountFusionsPossible(DIRECTION dir)
    {
        int nb=0;
        for (int index = 0; index < dimension; index++)
        {
            /*Droite*/
            if (dir == DIRECTION.Droite)
            {
                for (int xref = dimension - 1; xref > 0; xref--)
                {
                    if (Cases[xref, index].GetPuis() != 0 &&
                        Cases[xref, index].GetPuis() == Cases[xref - 1, index].GetPuis())
                    {
                        nb+=1;
                    }
                }
            }
            /*Gauche*/
            if (dir == DIRECTION.Gauche)
            {
                for (int xref = 0; xref < dimension - 1; xref++)
                {
                    if (Cases[xref, index].GetPuis() != 0 &&
                        Cases[xref, index].GetPuis() == Cases[xref + 1, index].GetPuis())
                    {
                        nb += 1;
                    }
                }
            }
            /*Bas*/
            if (dir == DIRECTION.Bas)
            {
                for (int yref = dimension - 1; yref > 0; yref--)
                {
                    if (Cases[index, yref].GetPuis() != 0 &&
                        Cases[index, yref].GetPuis() == Cases[index, yref - 1].GetPuis())
                    {
                        nb += 1;
                    }
                }
            }
            /*Haut*/
            if (dir == DIRECTION.Haut)
            {
                for (int yref = 0; yref < dimension - 1; yref++)
                {
                    if (Cases[index, yref].GetPuis() != 0 &&
                        Cases[index, yref].GetPuis() == Cases[index, yref + 1].GetPuis())
                    {
                        nb += 1;
                    }
                }
            }
        }
        return nb;
    }


    public bool DeplacementPossible(DIRECTION dir)
    {
        if (DecalagePossible(dir) || FusionPossible(dir))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Fonction de contrôle du déplacement
    public void Deplacement(DIRECTION dir)
    {
        
        DeplacerGrille(dir);
        GenererCaseAleatoire();
    }

    // Fonctions pour la génération des cases 

    /// <summary>
    ///  retourne une liste de toutes les cases vides (puissanceDe2=0) 
    /// </summary>
    /// <returns></returns>
    public List<Case> CasesLibres()
    {
        List<Case> CasesLib = new List<Case>();
        for (int y = 0; y < dimension; y++)
        {
            for (int x = 0; x < dimension; x++)
            {
                if (Cases[x, y].GetPuis() == 0)
                {
                    CasesLib.Add(Cases[x, y]);
                }
            }
        }
        return CasesLib;
    }

    /// <summary>
    /// génère deux case aléatoirement dans la grille qui valent soit : 
    /// -  4 (puissanceDe2=2) : une seule des deux cases au maximum
    /// -  2 (puissanceDe2=1) 
    /// avec une plus importante probabilité qu'il s'agisse d'une case valant 2
    /// </summary>
    public void GenererCaseDebut()
    {
        GenererCaseAleatoire();
        GenererCaseAleatoire();
    }

    /// <summary>
    ///  génère une case à un emplacment vide de valeur 2 ou 4
    /// </summary>
    public void GenererCaseAleatoire()
    {
        List<Case> casesLib = CasesLibres();
        int x = Random.Range(0, casesLib.Count-1);
        if (Random.Range(1, 10) < 9)
        {
            casesLib[x].SetPuis(1);
        }
        else
        {
            casesLib[x].SetPuis(2);
        }
        casesLib.RemoveAt(x);
    }

    public bool CheckValue(int v)
    {
        foreach (Case c in Cases)
        {
            if (c.GetPuis() == v)
            {
                return true;
            }
        }
        return false;
    }

    private void OnDestroy()
    {
        foreach (Case c in Cases)
        {
            if (c != null)
            {
                Destroy(c.gameObject);
            }
        }
    }

    public Grille Simulate()
    {
        Grille toret = new GameObject().AddComponent<Grille>();
        toret.Cases = new Case[dimension, dimension];
        for (int i =0; i<dimension;i++)
        {
            for (int j=0; j<dimension;j++)
            {
                toret.Cases[i,j]=Instantiate<GameObject>(Cases[i, j].gameObject).GetComponent<Case>();
            }
        }        
        toret.Case = Case;
        toret.dimension = dimension;
        return toret;
    }

    public int MaxCaseGrille()
    {
        int max = Cases[0,0].GetPuis();
        for(int x = 0 ; x < dimension ; x++)
        {
            for(int y = 0; y < dimension ; y++)
            {
                if (max < Cases[x, y].GetPuis())
                {
                    max = Cases[0, 0].GetPuis();
                }
            }
        }
        return max;
    }

    public int[] PositionMaxCaseGrille()
    {
        int[] max = new int[2];
        max[0] = 0;
        max[1] = 0;
        for (int x = 0; x < dimension; x++)
        {
            for (int y = 0; y < dimension; y++)
            {
                if (Cases[max[0],max[1]].GetPuis() < Cases[x, y].GetPuis())
                {
                    max[0] = x;
                    max[1] = y;
                }
            }
        }
        return max;
    }

    public int[,] ConvertToInt()
    {
        int[,] toret= new int[dimension,dimension];
        for (int x = 0; x < dimension; x++)
        {
            for (int y = 0; y < dimension; y++)
            {
                toret[x, y] = Cases[x, y].GetPuis();
            }
        }
        return toret;
    }

    public void RegarderDeplacement(int[,] d)
    {
        for (int x = 0; x < dimension; x++)
        {
            for (int y = 0; y < dimension; y++)
            {
                Cases[x, y].SetPuis(d[x,y]);
            }
        }
    }
}
