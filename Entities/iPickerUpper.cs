using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RetroGlad
{
    public interface iPickerUpper
    {
        Team Team { get; }
        void OnPickup(IPickUp pickup);

    }
}
