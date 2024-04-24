using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    public List<Collider> detected = new List<Collider>();
    public int len = 0;
    private void OnTriggerEnter(Collider other) {
        detected.Add(other);
        len++;
    }
    private void OnTriggerExit(Collider other) {
        if(detected.Contains(other)) {
            detected.Remove(other);
            len--;
        }
    }
    public GameObject getAnswer() {
        if (detected.Count == 0) return null;
        return detected[0].gameObject;
    }
    public void clearList() {
        detected.Clear();
        len = 0;
    }
}
