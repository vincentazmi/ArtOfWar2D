using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private GameObject myTroop;

    // Health attributes
    private float healthPercent;

    // Bar
    public GameObject bar;
    public Sprite greenBar;
    public Sprite amberBar;
    public Sprite redBar;

    public void setTroop(GameObject t){
        myTroop = t;
        updatePosition();
    }

    public void updatePosition(){
        Vector3 troopPos = myTroop.transform.position;
        transform.position = new Vector3(troopPos.x, troopPos.y-70, troopPos.z);
    }

    public void setHealth(int h){
        healthPercent = h;
        updateHealth(h);
    }

    private void hide(){
        gameObject.SetActive(false);
        bar.SetActive(false);
    }

    public void updateHealth(float healthPercent){
        // healthPercent = 0.2f;
        // Debug.Log("healthPercent="+healthPercent);
        if(healthPercent >= 0.8f){
            bar.GetComponent<SpriteRenderer>().sprite = greenBar;
            bar.transform.localScale = new Vector3(healthPercent, 1, 0);
            Debug.Log("green");
        }

        if(healthPercent < 0.8f && healthPercent >= 0.4f){
            bar.GetComponent<SpriteRenderer>().sprite = amberBar;
            bar.transform.localScale = new Vector3(healthPercent, 1, 0);
            Debug.Log("amber");
        }

        if(healthPercent < 0.4f){
            bar.GetComponent<SpriteRenderer>().sprite = redBar;
            bar.transform.localScale = new Vector3(healthPercent, 1, 0);
            Debug.Log("red");
        }

        if(healthPercent <= 0.0f){
            hide();
        }
    }


    
}
