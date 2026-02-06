using UnityEngine;

public class FreeCamera : MonoBehaviour{

	public float movementSpeed = 5.0f;
	bool isLocked = true;

    public bool IsLocked { get => isLocked;
		set {
			if (value)
			{
				Cursor.lockState = CursorLockMode.Locked;
			}
			else
			{
				Cursor.lockState = CursorLockMode.None;				
			}
			isLocked = value;
		} }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
		if (Input.GetKeyDown(KeyCode.LeftControl))
		{
			IsLocked = !isLocked;
		}
    }

    void FixedUpdate()
	{
		if (!isLocked) return;
		var horizontalAxis = Input.GetAxis("Horizontal") * Time.fixedDeltaTime * movementSpeed ;
		var verticalAxis = Input.GetAxis("Vertical") * Time.fixedDeltaTime * movementSpeed;
		var lookX = Input.GetAxis ("Mouse X");
		var lookY = Input.GetAxis ("Mouse Y");

		transform.Translate(horizontalAxis, 0, 0);
		transform.Translate(0, 0, verticalAxis);
		transform.eulerAngles += new Vector3(-lookY, lookX, 0);
	}
}