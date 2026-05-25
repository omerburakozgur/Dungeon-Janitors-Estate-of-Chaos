/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using UnityEngine.Animations.Rigging;

[System.Serializable]
public class FingerPose
{
    [Tooltip("The actual bone in the character's skeleton.")]
    public Transform realBone;

    [Tooltip("The ghost/dummy bone on the tool that the real bone should mimic.")]
    public Transform targetBone;
}

/// <summary>
/// Snaps player hand IK targets to specific grip points on held tools and applies finger poses.
/// </summary>
public class IKTargetSnapper : MonoBehaviour
{
    [Header("Grip Points")]
    public Transform rightHandGrip;
    public Transform leftHandGrip;

    [Header("Elbow Hints")]
    public Transform rightElbowHint;
    public Transform leftElbowHint;

    [Header("Finger Poses")]
    public FingerPose[] rightFingers;
    public FingerPose[] leftFingers;

    [Header("Player IK References")]
    public TwoBoneIKConstraint rightArmIK;
    public TwoBoneIKConstraint leftArmIK;

    [Header("Transition Settings")]
    public float weightLerpSpeed = 15f;

    private void LateUpdate()
    {
        if (rightArmIK != null)
        {
            if (rightHandGrip != null)
            {
                rightArmIK.weight = Mathf.Lerp(rightArmIK.weight, 1f, Time.deltaTime * weightLerpSpeed);
                rightArmIK.data.target.position = rightHandGrip.position;
                rightArmIK.data.target.rotation = rightHandGrip.rotation;

                if (rightElbowHint != null) rightArmIK.data.hint.position = rightElbowHint.position;

                if (rightArmIK.weight > 0.1f)
                {
                    foreach (var finger in rightFingers)
                    {
                        if (finger.realBone != null && finger.targetBone != null)
                        {
                            finger.realBone.localRotation = Quaternion.Slerp(
                                finger.realBone.localRotation,
                                finger.targetBone.localRotation,
                                rightArmIK.weight
                            );
                        }
                    }
                }
            }
            else
            {
                rightArmIK.weight = Mathf.Lerp(rightArmIK.weight, 0f, Time.deltaTime * weightLerpSpeed);
            }
        }

        if (leftArmIK != null)
        {
            if (leftHandGrip != null)
            {
                leftArmIK.weight = Mathf.Lerp(leftArmIK.weight, 1f, Time.deltaTime * weightLerpSpeed);
                leftArmIK.data.target.position = leftHandGrip.position;
                leftArmIK.data.target.rotation = leftHandGrip.rotation;

                if (leftElbowHint != null) leftArmIK.data.hint.position = leftElbowHint.position;

                if (leftArmIK.weight > 0.1f)
                {
                    foreach (var finger in leftFingers)
                    {
                        if (finger.realBone != null && finger.targetBone != null)
                        {
                            finger.realBone.localRotation = Quaternion.Slerp(
                                finger.realBone.localRotation,
                                finger.targetBone.localRotation,
                                leftArmIK.weight
                            );
                        }
                    }
                }
            }
            else
            {
                leftArmIK.weight = Mathf.Lerp(leftArmIK.weight, 0f, Time.deltaTime * weightLerpSpeed);
            }
        }
    }
}