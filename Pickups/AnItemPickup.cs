using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RetroGlad
{
    public class AnItemPickup : MonoBehaviour, IPickUp
    {
        public AudioClip clip;

        //public int manaCrystalValue;
        protected void OnTriggerEnter2D(Collider2D other)
        {
            var pickedUp = other.attachedRigidbody.GetComponent<iPickerUpper>();
            if (pickedUp != null && pickedUp.Team == Team.Blue)
            {
                pickedUp.OnPickup(this);
                Destroy(gameObject);
            }
        }
    }
}
