using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DisplayImage : MonoBehaviour
{
    [SerializeField] private Transform baseTransform;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float duration = 1f;
    private Vector3 basePosition;
    private Quaternion baseRotation;

    private void Start()
    {
        basePosition = baseTransform.localPosition;
        baseRotation = baseTransform.localRotation;
    }

    public void Animate()
    {
        Debug.Log("Animating Display Image");
        transform.DOLocalMove(targetTransform.localPosition, duration).SetTarget(this);
        transform.DOLocalRotateQuaternion(targetTransform.localRotation, duration).SetTarget(this);
    }

    public void Reset()
    {
        DOTween.Kill(transform);
        transform.localPosition = basePosition;
        transform.localRotation = baseRotation;
    }
}
