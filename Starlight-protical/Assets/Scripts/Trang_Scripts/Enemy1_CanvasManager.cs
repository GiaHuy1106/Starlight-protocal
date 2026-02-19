using UnityEngine;
using UnityEngine.UI;

public class Enemy1_CanvasManager : MonoBehaviour
{
    public GameObject profileDetail;
    public GameObject miniProfile;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OpenProfile()
    {
        profileDetail.SetActive(true);
        Time.timeScale = 0f;
    }    
    public void closeProfile()
    {
        Time.timeScale = 1f;
        profileDetail.SetActive(false);
    }    
}
