namespace Assets.CameraOffAxisProjection.Scripts.Utils
{
  using Assets.CameraOffAxisProjection.Scripts;

  using UnityEngine;

  [ExecuteInEditMode]
  [RequireComponent(typeof(CameraOffAxisProjection))]
  public class CameraPointOfViewController : MonoBehaviour
  {
    [SerializeField]
    private Transform pointOfViewTransform;
    [SerializeField]
    private float smooth;

    public string eye;

    private CameraOffAxisProjection cameraOffAxisProjection;

    public Transform PointOfViewTransform
    {
      get
      {
        return this.pointOfViewTransform;
      }

      set
      {
        this.pointOfViewTransform = value;
      }
    }

    public CameraOffAxisProjection CameraOffAxisProjection
    {
      get
      {
        return this.cameraOffAxisProjection;
      }
    }

    protected void Awake()
    {
      this.cameraOffAxisProjection = this.GetComponent<CameraOffAxisProjection>();
    }

    protected void LateUpdate()
    {
      if (this.pointOfViewTransform != null)
      {
        Vector3 smoothedPosition = Vector3.Lerp (this.cameraOffAxisProjection.WorldPointOfView, this.pointOfViewTransform.position, smooth);
        this.cameraOffAxisProjection.WorldPointOfView = smoothedPosition;
        //this.cameraOffAxisProjection.WorldPointOfView = this.pointOfViewTransform.position;
      }
      else if(eye != null)
      {
        Debug.Log("Find...");
          if(GameObject.Find(eye))
          {
            PointOfViewTransform = GameObject.Find(eye).transform;
          
          }
      }
    }

    protected void OnValidate()
    {
      if (this.pointOfViewTransform == null)
      {
        Debug.LogError(
          string.Format(
            "The variable '{0}' of '{1}' of the game object '{2}' has not been assigned.",
            " Point Of View Transform",
            this.GetType().Name,
            this.name),
          this.gameObject);
      }
    }
  }
}
