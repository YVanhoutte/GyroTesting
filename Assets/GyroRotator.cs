using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroRotator : MonoBehaviour
{
    private Quaternion refRot;
    private float deltaTime = 0.0f;

    private Quaternion cameraBase = Quaternion.identity;
    private Quaternion referenceRotation = Quaternion.identity;
    private Quaternion calibration = Quaternion.identity;
    private Quaternion baseOrientation = Quaternion.Euler(90, 0, 0);
    private Quaternion baseOrientationRotationFix = Quaternion.identity;
    private readonly Quaternion baseIdentity = Quaternion.Euler(90, 0, 0);

    private float initialYAngle = 0f;
    private float appliedGyroYAngle = 0f;
    private float calibrationYAngle = 0f;

    private Quaternion initialRot, gyroInitialRot;

    //private IEnumerator Start()
    //{
    //    Screen.orientation = ScreenOrientation.Portrait;
    //    Screen.sleepTimeout = SleepTimeout.NeverSleep;
    //    if (SystemInfo.supportsGyroscope)
    //        Input.gyro.enabled = true;
    //    yield return null;

    //    ResetBaseOrientation();
    //    calibration = Input.gyro.attitude;
    //    cameraBase = transform.rotation;
    //    RecalculateReferenceRotation();

    //    transform.localRotation = cameraBase * (ConvertRotation(referenceRotation * Input.gyro.attitude) * Quaternion.identity);
    //}

    void Start()
    {
        Input.gyro.enabled = true;
        Application.targetFrameRate = 60;
        initialYAngle = transform.eulerAngles.y;

        initialRot = transform.rotation;
        gyroInitialRot = Input.gyro.attitude;
    }

    //void Update()
    //{
    //    Debug.Log("Blah");
    //    Quaternion offsetRotation = Quaternion.Inverse(gyroInitialRot) * Input.gyro.attitude;
    //    transform.localRotation = initialRot * offsetRotation;

    //    //ApplyGyroRotation();
    //    //ApplyCalibration();
    //}

    public void CalibrateYAngle()
    {
        calibrationYAngle = appliedGyroYAngle - initialYAngle; // Offsets the y angle in case it wasn't 0 at edit time.
    }

    void ApplyGyroRotation()
    {
        transform.rotation = Input.gyro.attitude;
        transform.Rotate(0f, 0f, 180f, Space.Self); // Swap "handedness" of quaternion from gyro.
        transform.Rotate(90f, 180f, 0f, Space.World); // Rotate to make sense as a camera pointing out the back of your device.
        appliedGyroYAngle = transform.eulerAngles.y; // Save the angle around y axis for use in calibration.
    }

    void ApplyCalibration()
    {
        transform.Rotate(0f, -calibrationYAngle, 0f, Space.World); // Rotates y angle back however much it deviated when calibrationYAngle was saved.
    }

    void Update()
    {
        if (deltaTime >= 0.5f)
        {
            Debug.Log("Rotation of Device: " + Input.gyro.attitude.eulerAngles);
            deltaTime = 0;
        }
        deltaTime += Time.deltaTime;

        if (Input.gyro.enabled)
        {
            transform.rotation *= Quaternion.Euler(new Vector3(-Input.gyro.rotationRateUnbiased.x, -Input.gyro.rotationRateUnbiased.y, Input.gyro.rotationRateUnbiased.z));
        }
    }

    private void AlignView()
    {
        Quaternion startOrient = Input.gyro.attitude;
        Vector3 startOrientVec = startOrient.eulerAngles;

        Debug.Log("Aligning view to " + startOrientVec);
        transform.rotation.SetLookRotation(startOrientVec, Vector3.up);
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
        referenceRotation = Quaternion.Inverse(baseOrientation) * Quaternion.Inverse(calibration);
    }
}
