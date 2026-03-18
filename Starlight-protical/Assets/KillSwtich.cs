using UnityEngine;

public class KillSwtich : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Enemy1.Instance.Die();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            Enemy2.Instance.Die();
        }
        else if(Input.GetKeyDown(KeyCode.M))
        {
            Enemy3_Controller.Instance.Die();
        }
        else if(Input.GetKeyDown(KeyCode.N))
        {
            Enemy4_Controller.Instance.Die();
        }
    }
}
