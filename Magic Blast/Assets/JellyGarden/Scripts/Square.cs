using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum FindSeparating
{
    NONE = 0,
    HORIZONTAL,
    VERTICAL,
	ALL
}

public enum SquareTypes
{
    NONE = 0,
    EMPTY,
	BLOCK, // Мыльные шары - лопнуть. Недвижимые(кубы движутся вниз, а они на месте) и поверх предмета.   . Нужно лопнуть комбинацией кубиков на котором он находится 
	WIREBLOCK, //Клетка - разрушить. Недвижимая. Внутри цветной кубик. Разрушаем с комбинацией цвета кубика внутри.
	SOLIDBLOCK, // Двойной блок - разрушить. На обычном блоке стоит маленький темный блок- их может быть до 4. Соответственно нужно разрушить черные блоки, чтоб уничтожить простой блок. В условии победы идет уничтожение обычного блока
    DOUBLEBLOCK, // time bomb
	UNDESTROYABLE,// Копилка - разрушить. Движимые. Можно разрушить только паверапами. Роторами
	THRIVING, // Кубики льда - разрушить. Расширяются с каждым ходом если не разрушить. Разрушаются комбинацией кубов рядом.
	BEACH_BALLS, //Пляжные мячи - их нужно лопнуть комбинацией кубиков. движимые
	COLOR_CUBE, //Цветной блок - разрушить. Можно разрушить только кубиками рядом такого же цвета.
	TOY, // for toys
	STATIC_COLOR, // for static color on field
	STATIC_POWER // for power up on field
}



public class Square : MonoBehaviour
{
	public Item item;
    public int row;
    public int col;
	public int colorCube;
	public int toyToGen;
	public int colorToGen;
	public int blockLevel;
	public int BombTime;
	public int lastColorValue;
    public SquareTypes type;
	public SquareTypes additiveType;

	public GameObject[] hidenLevelObjects;

    public List< GameObject> block = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        // GenItem();
        

    }

	public void updateHidenLevel()
	{
		if (type == SquareTypes.SOLIDBLOCK) {
			/*foreach (GameObject go in hidenLevelObjects) {
				go.SetActive (false);
			}*/
			if (blockLevel > 0) {
				for (int i = 0; i < blockLevel; i++) {
					hidenLevelObjects [i].SetActive (true);
				}

			}
		}
	}

    public Item GenItem(bool falling = true)
    {
		if (type == SquareTypes.STATIC_COLOR && colorToGen == 0) {
			type = SquareTypes.EMPTY;
			return null;
		}
        if (IsNone() && !CanGoInto()) return null;
        GameObject item = Instantiate(LevelManager.THIS.itemPrefab) as GameObject;
        item.transform.localScale = Vector2.one * 0.6f;
        item.GetComponent<Item>().square = this;
        //if (!falling)
        //    item.GetComponent<Item>().anim.SetTrigger("reAppear");

        item.transform.SetParent(transform.parent);
        if (falling)
        {
            item.transform.position = transform.position + Vector3.back * 0.2f + Vector3.up * 3f;
            item.GetComponent<Item>().justCreatedItem = true;
        }
        else
            item.transform.position = transform.position + Vector3.back * 0.2f;
        this.item = item.GetComponent<Item>();
        return this.item;
    }

    public Square GetNeighborLeft(bool safe = false)
    {
		if (col == 0 && !safe) return null;
        return LevelManager.THIS.GetSquare(col - 1, row, safe);
    }
    public Square GetNeighborRight(bool safe = false)
    {
        if (col >= LevelManager.THIS.maxCols && !safe) return null;
        return LevelManager.THIS.GetSquare(col + 1, row, safe);
    }
    public Square GetNeighborTop(bool safe = false)
    {
        if (row == 0 && !safe) return null;
        return LevelManager.THIS.GetSquare(col, row - 1, safe);
    }
    public Square GetNeighborBottom(bool safe = false)
    {
        if (row >= LevelManager.THIS.maxRows && !safe) return null;
        return LevelManager.THIS.GetSquare(col, row + 1, safe);
    }

    Hashtable FindMoreMatches(int spr_COLOR, Hashtable countedSquares, FindSeparating separating, Hashtable countedSquaresGlobal = null)
    {
        bool globalCounter = true;
        if (countedSquaresGlobal == null)
        {
            globalCounter = false;
            countedSquaresGlobal = new Hashtable();
        }

        if (this.item == null) return countedSquares;
        if (this.item.destroying) return countedSquares;
    //    if (LevelManager.THIS.countedSquares.ContainsValue(this.item) && globalCounter) return countedSquares;
        if (this.item.color == spr_COLOR && !countedSquares.ContainsValue(this.item) && this.item.currentType != ItemsTypes.INGREDIENT)
        {
            if (LevelManager.THIS.onlyFalling && this.item.justCreatedItem)
                countedSquares.Add(countedSquares.Count - 1, this.item);
            else if (!LevelManager.THIS.onlyFalling)
                countedSquares.Add(countedSquares.Count - 1, this.item);
            else return countedSquares;

             if (separating == FindSeparating.VERTICAL)
            {
                if (GetNeighborTop() != null)
                    countedSquares = GetNeighborTop().FindMoreMatches(spr_COLOR, countedSquares, FindSeparating.VERTICAL);
                if (GetNeighborBottom() != null)
                    countedSquares = GetNeighborBottom().FindMoreMatches(spr_COLOR, countedSquares, FindSeparating.VERTICAL);
            }
            else if (separating == FindSeparating.HORIZONTAL)
            {
                if (GetNeighborLeft() != null)
                    countedSquares = GetNeighborLeft().FindMoreMatches(spr_COLOR, countedSquares, FindSeparating.HORIZONTAL);
                if (GetNeighborRight() != null)
                    countedSquares = GetNeighborRight().FindMoreMatches(spr_COLOR, countedSquares, FindSeparating.HORIZONTAL);
            }
        }
        return countedSquares;
    }

    public List<Item> FindMatchesAround(FindSeparating separating = FindSeparating.NONE, int matches = 3, Hashtable countedSquaresGlobal = null)
    {
        bool globalCounter = true;
        List<Item> newList = new List<Item>();
        if (countedSquaresGlobal == null)
        {
            globalCounter = false;
            countedSquaresGlobal = new Hashtable();
        }
        Hashtable countedSquares = new Hashtable();
        countedSquares.Clear();
        if (this.item == null) return newList;

		if (separating != FindSeparating.HORIZONTAL || separating == FindSeparating.ALL)
        {
            countedSquares = this.FindMoreMatches(this.item.color, countedSquares, FindSeparating.VERTICAL, countedSquaresGlobal);
        }

        foreach (DictionaryEntry de in countedSquares)
        {
            LevelManager.THIS.countedSquares.Add(LevelManager.THIS.countedSquares.Count - 1, de.Value);
        }

        if (countedSquares.Count < matches) countedSquares.Clear();

		if (separating != FindSeparating.VERTICAL || separating == FindSeparating.ALL)
        {
            countedSquares = this.FindMoreMatches(this.item.color, countedSquares, FindSeparating.HORIZONTAL, countedSquaresGlobal);
        }

        foreach (DictionaryEntry de in countedSquares)
        {
            LevelManager.THIS.countedSquares.Add(LevelManager.THIS.countedSquares.Count - 1, de.Value);
        }

        if (countedSquares.Count < matches) countedSquares.Clear();

        foreach ( DictionaryEntry de in countedSquares)
        {
            newList.Add((Item)de.Value);
        }
        // print(countedSquares.Count);
        return newList;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FallInto()
    {
        //if(item == null)
    }

    public void FallOut()
    {
		if (type == SquareTypes.WIREBLOCK) {
			//gameObject.SetActive (false);
		}
		if (item != null)
        {
            Square nextSquare = GetNeighborBottom();
            if (nextSquare != null)
            {
                if (nextSquare.IsNone())
                {
                    for (int i = row + 1; i < LevelManager.THIS.maxRows; i++)
                    {
                        if (LevelManager.THIS.GetSquare(col, i) != null)
                        {
                            if (!LevelManager.THIS.GetSquare(col, i).IsNone())
                            {
                                nextSquare = LevelManager.THIS.GetSquare(col, i);
                                break;
                            }
                        }
                    }
                }
				if (nextSquare.CanGoInto() )
                {
                    if (nextSquare.item == null)
                    {
                        item.CheckNeedToFall(nextSquare);
                    }
                }
            }
        }
    }

    public bool IsNone()
    {
        return type == SquareTypes.NONE;
    }

    public bool IsHaveDestroybleObstacle()
    {
		return type == SquareTypes.SOLIDBLOCK || type == SquareTypes.THRIVING || type == SquareTypes.COLOR_CUBE || type == SquareTypes.UNDESTROYABLE;

    }

    public bool CanGoOut()
    {
        return type != SquareTypes.WIREBLOCK;
		//return true;
    }

	public bool CanGenItem()
	{
		return type != SquareTypes.SOLIDBLOCK && type != SquareTypes.NONE && type != SquareTypes.THRIVING && type != SquareTypes.COLOR_CUBE;
	}

    public bool CanGoInto()
    {
		return type != SquareTypes.SOLIDBLOCK && type != SquareTypes.UNDESTROYABLE && type != SquareTypes.NONE && type != SquareTypes.THRIVING && type != SquareTypes.WIREBLOCK && type != SquareTypes.COLOR_CUBE;
    }

	public void checkBlockedBlocks(int _color)
	{
		if (type != SquareTypes.NONE)
			return;
		List<Square> sqList = GetAllNeghbors();
		foreach (Square sq in sqList)
		{
			if (sq.type == SquareTypes.WIREBLOCK) {
				if (sq.item != null) {
					if (sq.item.COLORView == _color) {
						sq.item.DestroyItem ();
						sq.DestroyBlock();
					}
				}
			}
		}
	}


	public void startDestroyBlockDelayed (float delay = 0f,bool canCheck = false)
	{
		StartCoroutine (DestroyBlockDelayed(delay,canCheck));
	}

	IEnumerator DestroyBlockDelayed(float delay = 0f,bool canCheck = false)
	{
		yield return new WaitForSeconds (delay);
		DestroyBlock (canCheck);
	}

	public void DestroyBlock(bool canCheck = false)
    {
		if (type == SquareTypes.THRIVING) {
			LevelManager.THIS.IceShow (gameObject);
		}
		if (type == SquareTypes.WIREBLOCK) {
			LevelManager.THIS.SolidChainShow (gameObject);
		}
		if (type == SquareTypes.SOLIDBLOCK && blockLevel > 0) {
			
			//hidenLevelObjects[blockLevel-1].GetComponent<Animation>().Play("BrickRotate");
			//hidenLevelObjects[blockLevel-1].GetComponent<SpriteRenderer>().sortingOrder = 100;
			//hidenLevelObjects[blockLevel-1].AddComponent<Rigidbody2D>();
			//hidenLevelObjects[blockLevel-1].GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(Random.insideUnitCircle.x * Random.Range(30, 200), Random.Range(100, 150)), ForceMode2D.Force);
			GameObject.Destroy (hidenLevelObjects [blockLevel - 1]);
			LevelManager.THIS.ColorChainParticlesShow (gameObject, blockLevel - 1);
			blockLevel--;
			updateHidenLevel ();
			return;
		} else if (blockLevel < 1 && type == SquareTypes.SOLIDBLOCK) {
			LevelManager.THIS.SimpleShieldShow (gameObject);
		}
			
        //if (type == SquareTypes.UNDESTROYABLE) return;
		if (type == SquareTypes.BLOCK || additiveType == SquareTypes.BLOCK) {
			Debug.Log ("destroy block");
			if (LevelManager.THIS.blocksCount [0] > 0 && LevelManager.THIS.isContainTarget(Target.BLOCKS)) {
				LevelManager.THIS.blocksCount [0]--;
				if (LevelManager.THIS.blocksCount [0] < 0) {
					LevelManager.THIS.blocksCount [0] = 0;
				}
				LevelManager.THIS.animateDownBlocks (gameObject, LevelManager.THIS.blocksSprites [0], SquareTypes.BLOCK);
			} else {
				Debug.Log ("bubble anim");
				// анимация взрыва
				LevelManager.THIS.BubbleShow(gameObject);
			}
			type = SquareTypes.NONE;
			additiveType = SquareTypes.NONE;
		}

		if (type == SquareTypes.COLOR_CUBE) {
			//LevelManager.THIS.ColorShieldParticlesShow(gameObject,colorCube);
			Debug.Log ("destroy block");
			if (LevelManager.THIS.blocksCount [2] > 0 && LevelManager.THIS.isContainTarget(Target.BLOCKS)) {
				LevelManager.THIS.blocksCount [2]--;
				if (LevelManager.THIS.blocksCount [2] < 0) {
					LevelManager.THIS.blocksCount [2] = 0;
				}
				LevelManager.THIS.animateDownBlocks (gameObject, LevelManager.THIS.ColorCubePrefabs[colorCube].GetComponent<SpriteRenderer>().sprite, SquareTypes.COLOR_CUBE);
			} else {
				Debug.Log ("color shield anim");
				// анимация взрыва
				LevelManager.THIS.ColorShieldParticlesShow(gameObject,colorCube);
			}

		}

		if (type == SquareTypes.UNDESTROYABLE) {
			//LevelManager.THIS.TargetBlocks--;
			LevelManager.THIS.blocksCount[6]--;
			if (LevelManager.THIS.blocksCount [6] < 0) {
				LevelManager.THIS.blocksCount [6] = 0;
			} else {
				LevelManager.THIS.animateDownBlocks (gameObject, LevelManager.THIS.blocksSprites [6], SquareTypes.UNDESTROYABLE);
			}

			LevelManager.THIS.PinataShow (gameObject);
		}
		if (type != SquareTypes.SOLIDBLOCK && type != SquareTypes.THRIVING && type != SquareTypes.COLOR_CUBE)
        {
            List<Square> sqList = GetAllNeghbors();
            foreach (Square sq in sqList)
            {
				if (canCheck) 
				{
					if (sq.type == SquareTypes.SOLIDBLOCK || sq.type == SquareTypes.THRIVING) {
						if (sq.type == SquareTypes.SOLIDBLOCK) {
							Debug.Log ("find solid");
							if (sq.blockLevel == 0) {
								sq.DestroyBlock();
							} else {
								Debug.Log ("updateSolid solid");
								//sq.hidenLevelObjects[sq.blockLevel-1].GetComponent<Animation>().Play("BrickRotate");
								//sq.hidenLevelObjects[sq.blockLevel-1].GetComponent<SpriteRenderer>().sortingOrder = 100;
								//sq.hidenLevelObjects[sq.blockLevel-1].AddComponent<Rigidbody2D>();
								//sq.hidenLevelObjects[sq.blockLevel-1].GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(Random.insideUnitCircle.x * Random.Range(30, 200), Random.Range(100, 150)), ForceMode2D.Force);
								if (sq.blockLevel > 0) {
									GameObject.Destroy(sq.hidenLevelObjects[sq.blockLevel-1]);
									LevelManager.THIS.ColorChainParticlesShow (sq.gameObject, sq.blockLevel - 1);
									sq.blockLevel--;
									sq.updateHidenLevel ();
								}

							}
						} 
						else {
							sq.DestroyBlock(true);
						}
					}
				}
            }
        }
        if (block.Count > 0)
        {
            if (type == SquareTypes.BLOCK)
            {
                LevelManager.THIS.CheckCollectedTarget(gameObject.transform.Find("Block(Clone)").gameObject);
                LevelManager.THIS.PopupScore(LevelManager.THIS.scoreForBlock, transform.position, 0); 
                LevelManager.THIS.TargetBlocks--;
				/*LevelManager.THIS.blocksCount[0]--;
				if (LevelManager.THIS.blocksCount [0] < 0) {
					LevelManager.THIS.blocksCount [0] = 0;
				}
				LevelManager.THIS.animateDownBlocks (gameObject, LevelManager.THIS.blocksSprites [0], SquareTypes.BLOCK);*/
                block[block.Count - 1].GetComponent<SpriteRenderer>().enabled = false;
            }
            if (type == SquareTypes.WIREBLOCK)
            {
                LevelManager.THIS.PopupScore(LevelManager.THIS.scoreForWireBlock, transform.position, 0);
				//LevelManager.THIS.TargetBlocks--;
            }
            if (type == SquareTypes.SOLIDBLOCK)
            {
                LevelManager.THIS.PopupScore(LevelManager.THIS.scoreForSolidBlock, transform.position, 0);
				//LevelManager.THIS.TargetBlocks--;
				LevelManager.THIS.blocksCount[4]--;
				if (LevelManager.THIS.blocksCount [4] < 0) {
					LevelManager.THIS.blocksCount [4] = 0;
				}
				LevelManager.THIS.animateDownBlocks (gameObject, LevelManager.THIS.blocksSprites [4], SquareTypes.SOLIDBLOCK);
            }
            if (type == SquareTypes.THRIVING)
            {
                LevelManager.THIS.PopupScore(LevelManager.THIS.scoreForThrivingBlock, transform.position, 0);
                LevelManager.Instance.thrivingBlockDestroyed = true;
				//LevelManager.THIS.TargetBlocks--;
				LevelManager.THIS.blocksCount[5]--;
				if (LevelManager.THIS.blocksCount [5] < 0) {
					LevelManager.THIS.blocksCount [5] = 0;
				}
				LevelManager.THIS.animateDownBlocks (gameObject, LevelManager.THIS.blocksSprites [5], SquareTypes.THRIVING);
            }
            //Destroy( block[block.Count-1]);
            if (type != SquareTypes.BLOCK)
            {
                SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.block_destroy);

                //block[block.Count - 1].GetComponent<Animation>().Play("BrickRotate");
                //block[block.Count - 1].GetComponent<SpriteRenderer>().sortingOrder = 100;
                //block[block.Count - 1].AddComponent<Rigidbody2D>();
                //block[block.Count - 1].GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(Random.insideUnitCircle.x * Random.Range(30, 200), Random.Range(100, 150)), ForceMode2D.Force);


            }
			if (type == SquareTypes.COLOR_CUBE) {

			} else {

			}
            //GameObject.Destroy(block[block.Count - 1], 1.5f);
			GameObject.Destroy(block[block.Count - 1]);
            if (block.Count > 1) type = SquareTypes.BLOCK;
            block.Remove(block[block.Count - 1]);

            if (block.Count == 0)
                type = SquareTypes.EMPTY;
        }

    }

    public void GenThriveBlock(Square newSquare)
    {

    }

    public List<Square> GetAllNeghbors()
    {
        List<Square> sqList = new List<Square>();
        Square nextSquare = null;
        nextSquare = GetNeighborBottom();
        if (nextSquare != null)
            sqList.Add(nextSquare);
        nextSquare = GetNeighborTop();
        if (nextSquare != null)
            sqList.Add(nextSquare);
        nextSquare = GetNeighborLeft();
        if (nextSquare != null)
            sqList.Add(nextSquare);
        nextSquare = GetNeighborRight();
        if (nextSquare != null)
            sqList.Add(nextSquare);
        return sqList;
    }

    public bool IsHaveSolidAbove()
    {
        for (int i = row; i >= 0; i--)
        {
			if (LevelManager.THIS.GetSquare(col, i).type == SquareTypes.SOLIDBLOCK || LevelManager.THIS.GetSquare(col, i).type == SquareTypes.UNDESTROYABLE || LevelManager.THIS.GetSquare(col, i).type == SquareTypes.THRIVING || LevelManager.THIS.GetSquare(col, i).type == SquareTypes.COLOR_CUBE || LevelManager.THIS.GetSquare(col, i).type == SquareTypes.WIREBLOCK)
                return true;
        }
        return false;
    }
}
