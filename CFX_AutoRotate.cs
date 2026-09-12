using UnityEngine;

public class CFX_AutoRotate : MonoBehaviour
{
	public Vector3 rotation;

	public Space space;

	private void Update()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.Rotate(rotation * Time.deltaTime, space);
	}

	public CFX_AutoRotate()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		space = (Space)1;
		
	}
}
