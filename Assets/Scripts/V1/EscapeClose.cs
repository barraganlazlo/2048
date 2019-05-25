using UnityEngine;
using System.Collections;

public class EscapeClose : MonoBehaviour
{

    void Update()
    {
        if (Input.GetButtonUp("Cancel"))
        {
            gameObject.SetActive(false);
        }
    }
}
