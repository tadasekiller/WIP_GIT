using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleRotate : MonoBehaviour
{
    public GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(Player.transform.rotation.x,Player.transform.rotation.y,
            Player.transform.rotation.z);
    }
}
