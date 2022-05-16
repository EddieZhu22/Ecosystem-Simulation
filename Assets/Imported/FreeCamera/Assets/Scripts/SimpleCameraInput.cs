using UnityEngine;

public class SimpleCameraInput : MonoBehaviour
{
    public Axis xAxis = new Axis(KeyCode.D, KeyCode.A);
    public Axis yAxis = new Axis(KeyCode.W, KeyCode.S);
    public Axis zAxis = new Axis(KeyCode.E, KeyCode.Q);
    public KeyCode BoostSpeedKey = KeyCode.LeftShift;
    public KeyCode SwitchOnKey = KeyCode.F;

    public int GetAxis(Axis axisType)
    {
        if (Input.GetKey(axisType.Positive)) return 1;
        if (Input.GetKey(axisType.Negative)) return -1;
        return 0;
    }
}

[System.Serializable]
public class Axis
{
   public KeyCode Positive;
   public KeyCode Negative;
    public Axis(KeyCode positive, KeyCode negative)
    {
        Positive = positive;
        Negative = negative;
    }
}
