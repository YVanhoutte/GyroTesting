using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroRotator : MonoBehaviour
{
    private Quaternion refRot;
    private float deltaTime = 0.0f;

    private IEnumerator Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        if (SystemInfo.supportsGyroscope)
            Input.gyro.enabled = true;
        yield return null;
        //Input.gyro.attitude
        Quaternion startOrient = Input.gyro.attitude;
        Vector3 startOrientVec = startOrient.eulerAngles;
        Vector3 swappedOrient = new Vector3(startOrientVec.z, startOrientVec.y, startOrientVec.x);

        Vector3 grav = Input.gyro.gravity.normalized;
        //grav = new Vector3(grav.x, grav.z, grav.y);
        Quaternion difference = Quaternion.FromToRotation(Vector3.down, grav);
        //transform.forward = grav;
        //transform.forward = Vector3.down;
        //transform.rotation *= difference;
        //transform.rotation = startOrient;
        Debug.Log("Rotation of Device: " + Input.gyro.attitude.eulerAngles);
        //Debug.Log(string.Format("Gravity Vector: {0} Rotation for aligning: {1}", grav, difference.eulerAngles));
        //transform.rotation = Quaternion.Euler(swappedOrient);
        //transform.rotation = new Quaternion(Input.gyro.attitude.y, Input.gyro.attitude.x, Input.gyro.attitude.z, Input.gyro.attitude.w);
    }

    void Update ()
    {
        if(deltaTime >= 0.5f)
        {
            Debug.Log("Rotation of Device: " + Input.gyro.attitude.eulerAngles);
            deltaTime = 0;
        }
        deltaTime += Time.deltaTime;

        if (Input.gyro.enabled)
        {
            transform.rotation *= Quaternion.Euler(  new Vector3(-Input.gyro.rotationRateUnbiased.x, -Input.gyro.rotationRateUnbiased.y,  Input.gyro.rotationRateUnbiased.z));
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

    ///// <summary>
    ///// Recalculates reference system.
    ///// </summary>
    //private void ResetBaseOrientation()
    //{
    //    baseOrientationRotationFix = Quaternion.identity;
    //    baseOrientation = baseOrientationRotationFix * baseIdentity;
    //}

    ///// <summary>
    ///// Recalculates reference rotation.
    ///// </summary>
    //private void RecalculateReferenceRotation()
    //{
    //    referenceRotation = Quaternion.Inverse(baseOrientation) * Quaternion.Inverse(calibration);
    //}
}
