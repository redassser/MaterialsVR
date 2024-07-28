using Oculus.Interaction;
using Oculus.Interaction.Grab;
using UnityEngine;

public class CustomGrabInteractor : GrabInteractor
{
    /// <summary>
    /// This is a custom override to fix a bug with deleted Objects causing errors
    /// </summary>
    protected override GrabInteractable ComputeCandidate() {
        Vector3 position = Rigidbody.transform.position;
        GrabInteractable closestInteractable = null;
        GrabPoseScore bestScore = GrabPoseScore.Max;

        var interactables = GrabInteractable.Registry.List(this);
        foreach (GrabInteractable interactable in interactables) {
            Collider[] colliders = interactable.Colliders;
            foreach(Collider col in colliders) {
                if (col == null) return null;
            }
            GrabPoseScore score = GrabPoseHelper.CollidersScore(position, interactable.Colliders, out Vector3 hit);
            if (score.IsBetterThan(bestScore)) {
                bestScore = score;
                closestInteractable = interactable;
            }
        }

        return closestInteractable;
    }
}
