using UnityEngine;
using System.Collections;

public class PuzzleManager : MonoBehaviour
{
    public GameObject Animal;
    public ArrayList[] Block = new ArrayList[6]; // animal's array 6x10

    public GameObject TouchedAnimal = null;
    public bool isMoving = false;

	void Start()
	{
        for (int i = 0; i < 6; i++)
        {
            Block[i] = new ArrayList(); // array's array
        }

        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Block[x].Add(CreateRandomAnimal(x, new Vector3(-2.07f + (x * 0.82f), 5f + (y * 1.2f), 0f)));
            }
        }
	}

    void Update()
    {
        DestroyMatchedBlock();
    }

    void DestroyMatchedBlock()
    {
        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Animal target = ((GameObject)Block[x][y]).GetComponent<Animal>();
                if (target.rigidbody.velocity.magnitude > 0.3f || target.isDead) return; // if any animal moving
            }
        }

        // check 3matched animal's for vertical
        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Animal first = ((GameObject)Block[x][y]).GetComponent<Animal>();
                Animal second = ((GameObject)Block[x][y + 1]).GetComponent<Animal>();
                Animal third = ((GameObject)Block[x][y + 2]).GetComponent<Animal>();

                if (first.ClipName == second.ClipName && second.ClipName == third.ClipName)
                {
                    first.DestroyAnimal(0f, 0.2f);
                    second.DestroyAnimal(0f, 0.2f);
                    third.DestroyAnimal(0f, 0.2f);
                }
            }
        }

        // check 3matched animal's for horizontal
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Animal first = ((GameObject)Block[x][y]).GetComponent<Animal>();
                Animal second = ((GameObject)Block[x + 1][y]).GetComponent<Animal>();
                Animal third = ((GameObject)Block[x + 2][y]).GetComponent<Animal>();

                if (first.ClipName == second.ClipName && second.ClipName == third.ClipName)
                {
                    first.DestroyAnimal(0f, 0.5f);
                    second.DestroyAnimal(0f, 0.5f);
                    third.DestroyAnimal(0f, 0.5f);
                }
            }
        }
    }

    public bool CheckMatch(Animal animal)
    {
        int x = animal.Index; // animal's row
        int y = Block[x].IndexOf(animal.gameObject); // animal's column

        for (int i = 0; i < 8; i++)
        {
            Animal first = ((GameObject)Block[x][i]).GetComponent<Animal>();
            Animal second = ((GameObject)Block[x][i + 1]).GetComponent<Animal>();
            Animal third = ((GameObject)Block[x][i + 2]).GetComponent<Animal>();

            if (first.ClipName == second.ClipName && second.ClipName == third.ClipName)
            {
                if (first == animal || second == animal || third == animal) return true;
            }
        }

        for (int i = 0; i < 4; i++)
        {
            Animal first = ((GameObject)Block[i][y]).GetComponent<Animal>();
            Animal second = ((GameObject)Block[i + 1][y]).GetComponent<Animal>();
            Animal third = ((GameObject)Block[i + 2][y]).GetComponent<Animal>();

            if (first.ClipName == second.ClipName && second.ClipName == third.ClipName)
            {
                if (first == animal || second == animal || third == animal) return true;
            }
        }
        return false;
    }

    public GameObject CreateRandomAnimal(int idx, Vector3 pos)
    {
        GameObject temp = Instantiate(Animal) as GameObject;
        temp.transform.parent = transform;

        temp.GetComponent<Animal>().ClipName = string.Format("char{0:00}", Random.Range(1, 6));
        temp.GetComponent<Animal>().Index = idx;
        temp.transform.localPosition = pos;
        temp.name = "Animal";
        return temp;
    }

    public void DeleteAnimal(GameObject animal)
    {
        int x = animal.GetComponent<Animal>().Index;
        Block[x].Remove(animal);
    }

    public void RebornAnimal(GameObject animal)
    {
        int x = animal.GetComponent<Animal>().Index;
        float y = Mathf.Max(5.0f, ((GameObject)Block[x][Block[x].Count - 1]).transform.position.y + 1.2f);
        Block[x].Add(CreateRandomAnimal(x, new Vector3(-2.07f + (x * 0.82f), y, 0f)));
    }
}
