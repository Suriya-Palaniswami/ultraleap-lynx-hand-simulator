
//   ==============================================================================
//   | Lynx HandTracking Sample : LynxInterfaces (2022)                           |
//   | Author : GC                                                                |
//   |======================================                                      |
//   | LynxLayout Script                                                          |
//   | Script to align child buttons.                                             |
//   ==============================================================================

using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LynxLayout : MonoBehaviour
{
    #region SCRIPT ATTRIBUTES
    // Public attributes
    public float lineLenght;
    public float lineSpacing;
    public float columnSpacing;

    // Private attributes
    private List<GameObject> gameObjects = new List<GameObject>();
    #endregion


    #region UNITY API
    // Update is called once per frame
    void Update()
    {
        PlaceElements();
    }
    #endregion


    #region PRIVATE METHODS
    private void SetupList()
    {
        gameObjects.Clear();
        foreach (Transform child in transform)
        {
            gameObjects.Add(child.gameObject);
        }     
    }

    private void DebugLogList()
    {
        int i = 0;
        foreach (GameObject gameObject in gameObjects)
        {
            Debug.Log("N°" + i + " : " + gameObject + " de taille " + gameObject.transform.lossyScale);
            i++;
        }
    }

    private void PlaceElements()
    {
        SetupList();
        Vector3 difference = new Vector3(0, 0, 0); //0.10
        Vector3 distance = new Vector3(columnSpacing, lineSpacing, 1); //0.15

        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.transform.localPosition = (Vector3.Scale(gameObject.transform.localPosition, new Vector3(0, 0, 1))) + Vector3.Scale(difference, distance);

            if (difference.x >= lineLenght - 1)
            {
                difference.x = 0;
                difference.y -= 1f;
            }
            else 
            {
                difference.x += 1f;
            }
        }
    }
    #endregion


    #region PUBLIC METHODS
    public void AddElement(GameObject element)
    {
        GameObject.Instantiate(element, this.transform);
    }

    public void DeleteElement(int element)
    {
        GameObject.Destroy(gameObjects[element]);
    }

    public void ClearElements()
    {
        foreach (GameObject gameObject in gameObjects)
        {
            GameObject.Destroy(gameObject);
        }
    }
    #endregion
}
