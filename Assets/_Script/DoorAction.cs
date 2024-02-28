using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public enum DoorState
{
    Close,
    Open
}

public class DoorAction : MonoBehaviour
{
    private DoorState state;
    [SerializeField] private GameObject objectMask;

    private void Awake()
    {
        state = DoorState.Close;
        objectMask.SetActive(false);
    }

    public void SetChestState(DoorState _state)
    {
        if (DoorState.Open == _state)
        {
            state = _state;
            objectMask.SetActive(true);
            StartCoroutine(MoveDoorUp());
        }
    }

    IEnumerator MoveDoorUp()
    {
        Vector3 destination = transform.position;
        destination.y += 8;
        float moveSpeed = 1f;

        while (Vector3.Distance(transform.position, destination) > 0.1f)
        {
            Vector3 direction = (destination - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            yield return null;
        }
        transform.position = destination;
    }

}
