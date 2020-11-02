using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CurveData
{
    public string name;
    public CurvePath position;
    public CurvePath direction;
    public float moveSpeed = 1;
    public float rotaSpeed = 1;
    public AnimationCurve angleCurve = new AnimationCurve(new Keyframe[] { new Keyframe(-1f, 0.75f), new Keyframe(0, 1), new Keyframe(1, 0.24f) });
    public Vector2 offsetLimit = new Vector2(-2.5f, 2.5f);
    public Vector2 angleLimit = new Vector2(-90f, 90f);
}

public class CurveLookAt : MonoBehaviour
{

    public bool active = true;

    public int curveAddress;
    private CurveData curveData;
    private CurvePath positionCurve { get { return curveData.position; } }
    private CurvePath directionCurve { get { return curveData.direction; } }
    private AnimationCurve angleCurve { get { return curveData.angleCurve; } }
    private float moveSpeed { get { return curveData.moveSpeed; } }
    private float rotaSpeed { get { return curveData.rotaSpeed; } }
    private Vector2 offsetLimit { get { return curveData.offsetLimit; } }
    private Vector2 angleLimit { get { return curveData.angleLimit; } }

    public Transform target;
    public float elasticity = 0;

    private Vector2 controllerDirection = Vector2.zero;
    private Vector2 controllerElasticity = Vector2.zero;
    [Space]
    public float process;
    public float offset = 0;
    public float angle = 0;

    [Space]
    public CurveData[] curveDatas = new CurveData[] { };

    private void OnEnable()
    {
        SetCurveAddress(curveDatas[0]);
    }
    public void SetCurveAddress(CurveData data) {

        curveData = data;
        process = 0;
        offset = 0;
        angle = 0;
    }

    private bool manaul = true;

    private bool inTransition = false;
    private CurveData transitionCurve;
    Vector3 nearPosition;
    Vector3 nearDirection;

    Vector3 toPosition;
    Vector3 toDirection;
    float toProcess;
    [Space]
    public float toTime = 1;

    private Action toEndAction;
    public void ToCurveAddress(CurveData data,Action action = null)
    {
        if (data != null && data != curveData) {
            manaul = false;
            inTransition = true;
            transitionCurve = data;
            

            nearPosition = target.position;
            nearDirection = directionCurve.Evaluate((process / positionCurve.length) * directionCurve.length + offset);

            float minDis = float.MaxValue;
            float length = 0;
            toProcess = 0;
            for (int i = 0; i < data.position.smoothCount - 1; i++)
            {
                length += Vector3.Distance(data.position.GetSmoothPoint(i), data.position.GetSmoothPoint(i + 1));
                float distance = Vector3.Distance(nearPosition, data.position.GetSmoothPoint(i));
                if (distance < minDis)
                {
                    minDis = distance;
                    toProcess = length;
                }
            }
            toPosition = data.position.Evaluate(toProcess);
            toDirection = data.direction.Evaluate(toProcess / data.position.length * data.direction.length);

            toEndAction = action;
            StartCoroutine(InTransition());
        }
    }

    public float bei = 0.5f;
    private Vector3 mouseDownPosition;

    private void Update()
    {
        if (!active) return;

        Debug.DrawLine(nearPosition, toPosition);
        Debug.DrawLine(nearDirection, toDirection);

        if (manaul)
        {
            controllerDirection.x = Input.GetAxis("Horizontal") * moveSpeed;
            controllerDirection.y = Input.GetAxis("Vertical") * rotaSpeed;
            float qe = Input.GetAxis("qe") * moveSpeed;


            if (Input.GetMouseButtonDown(1)) {
                mouseDownPosition = Input.mousePosition;
            }
            if (Input.GetMouseButton(1)) {
                Vector2 dir = Input.mousePosition - mouseDownPosition;
                controllerDirection.x = -dir.x * moveSpeed * Time.deltaTime * bei;
                controllerDirection.y = -dir.y * rotaSpeed * Time.deltaTime * bei;
            }

            if (elasticity > 0)
            {
                if (controllerDirection != Vector2.zero)
                {
                    controllerElasticity = controllerDirection;
                }
                else if (controllerElasticity.magnitude > 0.001f)
                {
                    controllerElasticity *= (1 - 1 / elasticity);
                }
                else {

                    controllerElasticity = Vector2.zero;
                }
            }

            process -= (controllerDirection.x + controllerElasticity.x);

            if ((angle + controllerDirection.y + controllerElasticity.y) > angleLimit.x && (angle + controllerDirection.y + controllerElasticity.y) < angleLimit.y) angle += controllerDirection.y + controllerElasticity.y;
            if ((offset - qe) > offsetLimit.x && (offset - qe) < offsetLimit.y) offset -= qe;


            if (process < 0)
            {
                process = positionCurve.length - Mathf.Abs(process) % positionCurve.length;
            }
            else
            {
                process = process % positionCurve.length;
            }

            Vector3 pos = positionCurve.Evaluate(process);
            Vector3 tar = directionCurve.Evaluate((process / positionCurve.length) * directionCurve.length + offset);

            float sub = Vector3.Distance(pos, tar);

            target.position = pos;
            target.LookAt(tar, Vector3.up);
            target.position = target.position + target.forward * sub;
            target.localEulerAngles = new Vector3(target.localEulerAngles.x + angle, target.localEulerAngles.y, target.localEulerAngles.z);
            target.position = target.position - target.forward * sub * angleCurve.Evaluate(angle / 90f);
        }
    }

    IEnumerator InTransition() {

        float nowTime = 0;
        float value = 0;

        while (nowTime <= toTime)
        {


            value = nowTime / toTime;
            target.position = Vector3.Lerp(nearPosition, toPosition, value * value);
            target.LookAt(Vector3.Lerp(nearDirection, toDirection, value), Vector3.up);

            nowTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        offset = 0;
        angle = 0;
        process = toProcess;
        curveData = transitionCurve;
        inTransition = false;
        manaul = true;

        if (toEndAction != null) {
            Action action = toEndAction;
            action.Invoke();
            toEndAction = null;
        }
    }

    public bool gizmos = false;
    private void OnDrawGizmos()
    {
        if (!gizmos) return;
        if (!active) return;
        if (target == null) return;


        if (curveAddress >= 0 && curveAddress < curveDatas.Length && curveData != curveDatas[curveAddress]) {
            
            SetCurveAddress(curveDatas[curveAddress]);
        }

        if (process < 0)
        {
            process = positionCurve.length - Mathf.Abs(process) % positionCurve.length;
        }
        else
        {
            process = process % positionCurve.length;
        }

        angle = Mathf.Max(angleLimit.x, Mathf.Min(angle, angleLimit.y));
        offset = Mathf.Max(offsetLimit.x, Mathf.Min(offset, offsetLimit.y));

        Vector3 pos = positionCurve.Evaluate(process);
        Vector3 tar = directionCurve.Evaluate((process / positionCurve.length) * directionCurve.length + offset);

        float sub = Vector3.Distance(pos , tar);

        target.position = pos;
        target.LookAt(tar, Vector3.up);
        target.position = target.position + target.forward * sub;
        target.localEulerAngles = new Vector3(target.localEulerAngles.x + angle, target.localEulerAngles.y, target.localEulerAngles.z);
        target.position = target.position - target.forward * sub * angleCurve.Evaluate(angle/90f);



        Gizmos.DrawLine(target.position, target.position + target.forward * sub * angleCurve.Evaluate(angle / 90f));
        Gizmos.DrawLine(pos,tar);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(50, 50, 100, 50), "Star")) {
            ToCurveAddress(curveDatas[0]);
        }
        if (GUI.Button(new Rect(50, 100, 100, 50), "Modules"))
        {
            ToCurveAddress(curveDatas[1]);
        }
    }
}
