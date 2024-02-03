using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTroop : MonoBehaviour
{

    public void show(){
        gameObject.SetActive(true);
    }

    public void hide(){
        gameObject.SetActive(false);
    }

    public void setMaterial(){
        Material mat = gameObject.GetComponent<SpriteRenderer>().material;

        // Get colour and set new colour to the same with 30% opacity
        Color oldColour = mat.color;
        Color newColour = new Color(oldColour.r, oldColour.g, oldColour.b, 0.3f);
        // Set colour, give to this game object
        mat.SetColor("_Color",newColour);
        GetComponent<SpriteRenderer>().material = mat;
    }

    public void updateSprite(Sprite s){
        gameObject.GetComponent<SpriteRenderer>().sprite = s;
    }

    public void moveToMouse(float lockx = 9999.0f){
        Vector3 mousePos = Input.mousePosition;
        transform.position = Camera.main.ScreenToWorldPoint(mousePos);
        float actualX;
        if(lockx == 9999.0f) actualX = transform.position.x;
        else actualX = lockx;
        transform.position = new Vector3(actualX, transform.position.y, 0);
    }
}
