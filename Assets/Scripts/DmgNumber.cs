using System.Collections;
using Player;
using UnityEngine;
using Random = System.Random;

public class DmgNumber : MonoBehaviour
{
    private Vector3? _moveTarget;

    private void Start()
    {
        UpdateRotation();
    }

    public void StartAnim(int value, Color color)
    {
        _moveTarget = transform.position;

        GetComponent<TextMesh>().color = color;
        GetComponent<TextMesh>().text = value.ToString();
        StartCoroutine(AnimCoroutine());
    }

    private Random _random = new Random();
    
    public double GetRandomNumberInRange(double minNumber, double maxNumber)
    {
        return _random.NextDouble() * (maxNumber - minNumber) + minNumber;
    }
    
    private IEnumerator AnimCoroutine()
    {
        float offsetA = (float)GetRandomNumberInRange(-2.5, 2.5);

        var transform1 = transform;
        var position = transform1.position;
        var up = transform1.up;
        var right = transform1.right;
        
        _moveTarget = position + up * 5.0f + right * offsetA;
        yield return new WaitForSeconds(2f);
        _moveTarget = position + -up * 3.5f + right * offsetA;
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    private void UpdateRotation()
    {
        // Autorotate to camera
        GameObject go = PlayerController.Instance.GetMainCamera().gameObject;
        var position = transform.position;
        Vector3 heading = go.transform.position - position;
        transform.LookAt(position - heading);
    }

    void Update()
    {
        UpdateRotation();
        
        if(_moveTarget != null)
            transform.position = Vector3.Lerp(transform.position, _moveTarget.Value, Time.deltaTime * 0.4f);
    }
}
