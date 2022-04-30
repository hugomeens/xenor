using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCMap : MonoBehaviour
{
    private int mapSize; 

    private PCTile[][] maze; 

    private List<(int, int)> startsAndEnds = new List<(int, int)>();

    // Start is called before the first frame update
    void Start()
    {
        mapSize = Random.Range(10, 50);
        maze = new PCTile[mapSize][];

        Debug.Log("TestStart");
        //Initialisation du labyrinthe
        InitMaze();

            //Debug.Log("TestInter");
            //for (int i = 0; i < maze.Length; i++)
            //{
            //    Debug.Log(i);
            //    for (int j = 0; j < maze[i].Length; j++)
            //    {
            //        Debug.Log(maze[i][j]);
            //    }
            //}

        //Cr�ation du labyrinthe de produit chimique
        GenerateMaze();

        Debug.Log("TestFin");
        //Instantie de toutes les tiles avec la bonne image

        // Instantie porte
        //Prochaine salle dans porte

        //Instantie les murs
    }

    // Initialise un labyrinthe vide
    private void InitMaze()
    {
        //Initialisation du labyrinthe
        //Partie Haute du laby
        for (int i = 0; i < mapSize/2; i++)
        {
            maze[i] = new PCTile[mapSize];
            for (int j = 0; j < mapSize; j++)
            {
                maze[i][j] = new PCTile();
            }
        }

        maze[mapSize / 2] = new PCTile[mapSize];
        //S�paration du milieu
        for (int j = 0; j < mapSize; j++)
        {
            maze[mapSize / 2][j] = new PCTile(PCTile.PCTileType.Strait, PCTile.PCFluidDirection.Down);
        }

        //Partie Basse du laby
        for (int i = mapSize / 2 +1; i < mapSize; i++)
        {
            maze[i] = new PCTile[mapSize];
            for (int j = 0; j < mapSize; j++)
            {
                maze[i][j] = new PCTile();
            }
        }
    }

    /**
     * <summary>Renvoie toutes le dirrections possible depuis cette case/summary>
     * 
     * <param name="oldDir">Direction sortant de la tuile pr�cendente</param>
     * <param name="i">ligne de l'actuel tuile � cr�er/modifier</param>
     * <param name="j">colone de l'actuelle tuile � cr�er/modifier</param>
     * 
     * <returns>List des directions possibles</returns>
     */
    private List<PCTile.PCFluidDirection> GetPossibleDirections(PCTile.PCFluidDirection oldDir, int i, int j)
    {
        List<PCTile.PCFluidDirection> result = new List<PCTile.PCFluidDirection>();

        PCTile tileActu = maze[i][j];
        if (tileActu.TileType == PCTile.PCTileType.Cross)
        {
            //dans ce cas on a pas le choix, on peut pas tourner
            return new List<PCTile.PCFluidDirection>() { oldDir };
        }

        if (j-1 >= 0 && oldDir != PCTile.PCFluidDirection.Right)
        {
            //y a un tuile a gauche donc on paut check
            PCTile tileGauche = maze[i][j-1];
            if (tileGauche.TileType == PCTile.PCTileType.None)
            {
                result.Add(PCTile.PCFluidDirection.Left);
            }
            else if ((tileGauche.TileType == PCTile.PCTileType.Strait && (int)oldDir % 2 != (int)tileGauche.FluidDirection % 2) && (
                tileGauche.FluidDirection != PCTile.PCFluidDirection.Left && tileGauche.FluidDirection != PCTile.PCFluidDirection.Right))
            {
                if (j - 2 >= 0)
                {
                    result.Add(PCTile.PCFluidDirection.Left);
                }
            }
        }

        if (j+1 < mapSize && oldDir != PCTile.PCFluidDirection.Left)
        {
            PCTile tileDroite = maze[i][j + 1];
            if (tileDroite.TileType == PCTile.PCTileType.None)
            {
                result.Add(PCTile.PCFluidDirection.Right);
            }
            else if (tileDroite.TileType == PCTile.PCTileType.Strait && (int)oldDir % 2 != (int)tileDroite.FluidDirection % 2 && (
                tileDroite.FluidDirection != PCTile.PCFluidDirection.Left && tileDroite.FluidDirection != PCTile.PCFluidDirection.Right))
            {
                if (j+2 < mapSize)
                {
                    result.Add(PCTile.PCFluidDirection.Right);
                }
            }
        }
        

        //regardez la new dir aussi
        if (i+1 < mapSize)
        {
            PCTile tileBas = maze[i+1][j];
            if (tileBas.TileType == PCTile.PCTileType.None)
            {
                result.Add(PCTile.PCFluidDirection.Down);
            }
            else if (tileBas.TileType == PCTile.PCTileType.Strait && (int)oldDir % 2 != (int)tileBas.FluidDirection % 2 && tileBas.FluidDirection != PCTile.PCFluidDirection.Down)
            {
                if (i+2 <=mapSize)
                {
                    result.Add(PCTile.PCFluidDirection.Down);
                }
            }
        }
        else
        {
            result.Add(PCTile.PCFluidDirection.End);
        }

        return result;
    }

    /**
     * <summary>G�n�re le chemin pour ce tuyaux</summary>
     * 
     * <param name="oldDir">Direction sortant de la tuile pr�cendente</param>
     * <param name="i">ligne de l'actuel tuile � cr�er/modifier</param>
     * <param name="j">colone de l'actuelle tuile � cr�er/modifier</param>
     * 
     * <returns>Integers de la colone du de la fin du tuyaux ou un NULL si chemin impossible</returns>
     */
    private int? GeneratePath(PCTile.PCFluidDirection oldDir, int i, int j)
    {
        PCTile oldTile = maze[i][j];

        List<PCTile.PCFluidDirection> possibles = GetPossibleDirections(oldDir, i, j);

        if (possibles.Count == 0)
        {
            return null;
        }

        int nbAlea = Random.Range(0, possibles.Count);

        Debug.Log(possibles.Count);
        Debug.Log(nbAlea);
        PCTile.PCFluidDirection choosenOne = possibles[nbAlea];
        possibles.RemoveAt(nbAlea);

        //Changement de la Tuile
        if (choosenOne == PCTile.PCFluidDirection.End)
        {
            //c la fin des haricots
            return j;
        }
        else
        {
            Debug.Log(maze[i][j].TileType);
            maze[i][j].AddDirection(oldDir, choosenOne);
        }

        //Calcul des nouvelles valeurs
        int newI = i;
        int newJ = j;
        if (choosenOne == PCTile.PCFluidDirection.Down)
        {
            newI++;
        }
        else if (choosenOne == PCTile.PCFluidDirection.Left)
        {
            newJ--;
        }
        else
        {
            newJ++;
        }
        

        //R�cursions
        int? result = GeneratePath(choosenOne, newI, newJ);
        while (result == null)
        {
            if (possibles.Count == 0)
            {
                maze[i][j] = oldTile;
                return null;
            }
            nbAlea = Random.Range(0, possibles.Count);
            choosenOne = possibles[nbAlea];
            possibles.RemoveAt(nbAlea);
            newI = i;
            newJ = j;
            result = GeneratePath(choosenOne, newI, newJ);
        }

        return result;
    }

    private void GenerateMaze()
    {
        //On tire 3 nombre al�atoire diff�rents pour les 3 d�buts
        List<int> starts = new List<int>() {Random.Range(0, mapSize)};
        for (int i = 0; i < 2; i++)
        {
            int nbAlea = Random.Range(0, mapSize);
            while (starts.Contains(nbAlea))
            {
                nbAlea = Random.Range(0, mapSize);
            }
            starts.Add(nbAlea);
        }

        //On demarre a chauqe fois d'un pont avec un direction vers le bas et on va chercher le chemin
        //On retroune la fin et on l'ajoute
        List<int> ends = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            int? toAdd = GeneratePath(PCTile.PCFluidDirection.Down, 0, starts[i]);
            if (toAdd == null)
            {
                //Impossible de faire ce PC
                InitMaze();
                GenerateMaze();
                return;
            }
            ends.Add((int)toAdd);
        }

        //On a joute � startsAndEnds
        foreach (int start in starts)
        {
            startsAndEnds.Add((-1, start));
        }
        foreach (int end in ends)
        {
            startsAndEnds.Add((mapSize, end));
        }
    }
}
