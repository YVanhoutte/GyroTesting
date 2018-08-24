// ***********************************************************
// Written by Heyworks Unity Studio http://unity.heyworks.com/
// Reference: http://blog.heyworks.com/how-to-write-gyroscope-controller-with-unity3d/
// ***********************************************************
using UnityEngine;

/// <summary>
/// Gyroscope controller that works with any device orientation.
/// </summary>
public class GyroController : MonoBehaviour 
{
	#region [Private fields]
	private const float lowPassFilterFactor = 0.2f;

	private readonly Quaternion baseIdentity =  Quaternion.Euler(90, 0, 0);
	
	private Quaternion cameraBase =  Quaternion.identity;
	private Quaternion calibration =  Quaternion.identity;
	private Quaternion baseOrientation =  Quaternion.Euler(90, 0, 0);
	private Quaternion baseOrientationRotationFix =  Quaternion.identity;

	private Quaternion referenceRotation = Quaternion.identity;
	[SerializeField] private bool debug = false;
	#endregion
	#region [Unity events]
	protected void Start () 
	{
        if (SystemInfo.supportsGyroscope)
            Input.gyro.enabled = true;

        ResetBaseOrientation();
        UpdateCalibration(true);
        UpdateCameraBaseRotation(true);
        RecalculateReferenceRotation();
    }

	protected void Update() 
	{
        transform.localRotation = Quaternion.Slerp(transform.localRotation,
            cameraBase * (ConvertRotation(referenceRotation * Input.gyro.attitude) * Quaternion.identity), lowPassFilterFactor);
        //transform.localRotation = Quaternion.Slerp(transform.localRotation, Input.gyro.attitude, lowPassFilterFactor); //This results in a widely misaligned rotation, it also slowly reverts back to the start like the method above
    }
	#endregion
	#region [Private methods]
	/// <summary>
	/// Update the gyro calibration.
	/// </summary>
	private void UpdateCalibration(bool onlyHorizontal)
	{
		if (onlyHorizontal)
		{
			var fw = (Input.gyro.attitude) * (-Vector3.forward);
			fw.z = 0;
			if (fw == Vector3.zero)
			{
				calibration = Quaternion.identity;
			}
			else
			{
				calibration = (Quaternion.FromToRotation(baseOrientationRotationFix * Vector3.up, fw));
			}
		}
		else
		{
			calibration = Input.gyro.attitude;
		}
	}
	
	/// <summary>
	/// Update the camera base rotation.
	/// </summary>
	/// <param name='onlyHorizontal'>
	/// Only y rotation.
	/// </param>
	private void UpdateCameraBaseRotation(bool onlyHorizontal)
	{
		if (onlyHorizontal)
		{
			var fw = transform.forward;
			fw.y = 0;
			if (fw == Vector3.zero)
			{
				cameraBase = Quaternion.identity;
			}
			else
			{
				cameraBase = Quaternion.FromToRotation(Vector3.forward, fw);
			}
		}
		else
		{
			cameraBase = transform.rotation;
		}
	}
	
	/// <summary>
	/// Converts the rotation from right handed to left handed.
	/// </summary>
	/// <returns>
	/// The result rotation.
	/// </returns>
	/// <param name='q'>
	/// The rotation to convert.
	/// </param>
	private static Quaternion ConvertRotation(Quaternion q)
	{
		return new Quaternion(q.x, q.y, -q.z, -q.w);	
	}
	
	/// <summary>
	/// Recalculates reference system.
	/// </summary>
	private void ResetBaseOrientation()
	{
		baseOrientationRotationFix = Quaternion.identity;
		baseOrientation = baseOrientationRotationFix * baseIdentity;
	}

	/// <summary>
	/// Recalculates reference rotation.
	/// </summary>
	private void RecalculateReferenceRotation()
	{
		referenceRotation = Quaternion.Inverse(baseOrientation)*Quaternion.Inverse(calibration);
	}
	#endregion
}
