using UnityEngine;
using System.Collections;

public class Animal : MonoBehaviour
{
    Animator anim;
    public string ClipName, NewName;
    public int Index;
    public bool isDead = false;

    Vector3 oldPos;

    public GameObject Effect;

    Animal target;
    PuzzleManager manager; // PuzzleManager Script

    void Awake()
    {
        // if velocity is zero could be destroy when appear
        rigidbody.velocity = new Vector3(0f, -5f, 0f);
    }

	void Start()
	{
        anim = GetComponent<Animator>();
        anim.Play(ClipName);
        manager = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();
	}

    void Update()
    {
        if (!isDead)
        {
            if (Input.GetMouseButton(0))
            {
                if (manager.TouchedAnimal == null)
                {
                    Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    if (rigidbody.velocity.magnitude <= 0.3f) // animal has stopped
                    {
                        RaycastHit hit;

                        if (Physics.Raycast(pos, Vector3.forward, out hit, 100f))
                        {
                            if (hit.collider.gameObject == gameObject)
                            {
                                manager.TouchedAnimal = gameObject; // save player touched animal
                            }
                        }
                    }
                }
            }
            else if (manager.TouchedAnimal == gameObject) // if this is touched object when mouse button up
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                float xdist = Mathf.Abs(pos.x - transform.position.x);
                float ydist = Mathf.Abs(pos.y - transform.position.y);

                if (xdist > ydist) // player drag direction is horizontal
                {
                    if (xdist >= 0.4f)
                    {
                        int x = Index; // animal's row
                        int y = manager.Block[x].IndexOf(gameObject); // animal's column

                        if (pos.x < transform.position.x && x >= 1) // left move
                        {
                            manager.isMoving = true; // disable gravity for all animals
                            target = ((GameObject)manager.Block[x - 1][y]).GetComponent<Animal>();
                            MoveX(target.transform.localPosition.x, target.ClipName);
                            target.MoveX(transform.localPosition.x, ClipName);
                        }
                        else if (pos.x > transform.position.x && x <= 4) // right move
                        {
                            manager.isMoving = true; // disable gravity for all animals
                            target = ((GameObject)manager.Block[x + 1][y]).GetComponent<Animal>();
                            MoveX(target.transform.localPosition.x, target.ClipName);
                            target.MoveX(transform.localPosition.x, ClipName);
                        }
                    }
                }
                else // player drag direction is vertical
                {
                    if (ydist >= 0.4f)
                    {
                        int x = Index; // animal's row
                        int y = manager.Block[x].IndexOf(gameObject); // animal's column

                        if (pos.y > transform.position.y && y <= 8) // move up
                        {
                            manager.isMoving = true; // disable gravity for all animals
                            target = ((GameObject)manager.Block[x][y + 1]).GetComponent<Animal>();
                            MoveY(target.transform.localPosition.y, target.ClipName);
                            target.MoveY(transform.localPosition.y, ClipName);
                        }
                        else if (pos.y < transform.position.y && y >= 1) // move down
                        {
                            manager.isMoving = true; // disable gravity for all animals
                            target = ((GameObject)manager.Block[x][y - 1]).GetComponent<Animal>();
                            MoveY(target.transform.localPosition.y, target.ClipName);
                            target.MoveY(transform.localPosition.y, ClipName);
                        }
                    }
                }
                manager.TouchedAnimal = null;
            }
        }
        rigidbody.useGravity = !manager.isMoving;
    }

    public void MoveX(float x, string name)
    {
        NewName = name;
        oldPos = transform.localPosition;
        LeanTween.moveLocalX(gameObject, x, 0.2f).setOnComplete(MoveComplete);
    }

    public void MoveY(float y, string name)
    {
        NewName = name;
        oldPos = transform.localPosition;
        LeanTween.moveLocalY(gameObject, y, 0.2f).setOnComplete(MoveComplete);
    }

    public void Move(Vector3 pos, string name)
    {
        NewName = name;
        LeanTween.moveLocal(gameObject, pos, 0.2f).setOnComplete(MoveComplete);
    }

    void MoveComplete()
    {
        ClipName = NewName; // change animal's clip name

        if (target != null) // when animal moved
        {
            target.ClipName = target.NewName; // change target's animal's clip name

            // check 3matched animals
            if (manager.CheckMatch(this) || manager.CheckMatch(target))
            {
                anim.Play(ClipName);
                target.anim.Play(target.ClipName);
                transform.localPosition = oldPos;
                target.transform.localPosition = target.oldPos;
                manager.isMoving = false; // enable gravity
            }
            else // nothing matched animals
            {
                // back to original position
                target.Move(target.oldPos, ClipName);
                Move(oldPos, target.ClipName);
            }
            target = null;
        }
    }

    public void DestroyAnimal(float hideDelay, float removeDelay)
    {
        if (!isDead)
        {
            Invoke("Hide", hideDelay);
            Invoke("Remove", removeDelay);
            isDead = true;
        }
    }

    void Hide()
    {
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        Instantiate(Effect, transform.localPosition, Quaternion.identity);
    }

    void Remove()
    {
        manager.DeleteAnimal(gameObject);
        manager.RebornAnimal(gameObject);
        Destroy(gameObject);
    }
}
