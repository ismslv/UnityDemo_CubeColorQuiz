using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreScene : MonoBehaviour
{
    public static CoreScene a;

    //Prefabs
    public GameObject spotPrefab;
    public GameObject cubePrefab;
    public ColorsData lightColors;

    //Objects
    public Transform lightCone;
    public Light lightConeLight;
    public TextMesh text;

    //Options
    public float spotsGap;
    public float rotationSmooth;

    //Inner
    [HideInInspector]
    public Spot[] spots;
    [HideInInspector]
    public bool canChoose;
    Quaternion rotationGoal;
    [HideInInspector]
    public Spot selectedSpot;

    void Awake()
    {
        if (CoreScene.a == null)
            a = this;
    }

    void Start()
    {
        rotationGoal = lightCone.rotation;
        lightConeLight.color = lightColors.Get("neutral");
        CreateSpots();
    }

    void Update()
    {
        UpdateLightCone();
        if (canChoose) {
            CheckMouse();
            if (Input.GetMouseButtonDown(0))
                CheckClick();
        }
        UpdateSpots();
    }

    #region Spots

    void CreateSpots()
    {
        spots = new Spot[3];
        for (int i = 0; i < 3; i++)
        {
            spots[i] = CreateSpot(i);
        }
    }

    Spot CreateSpot(int point)
    {
        GameObject newSpot = Instantiate(spotPrefab);
        newSpot.transform.position = new Vector3((point - 1) * spotsGap, 0, 0);
        newSpot.name = "Spot " + (point + 1).ToString();
        var cube = CreateCube();
        cube.SetParent(newSpot.transform);

        return new Spot()
        {
            spotPlace = newSpot.transform,
            cube = cube,
            cubeRenderer = cube.GetComponent<MeshRenderer>(),
            cubeCollider = cube.GetComponent<Collider>()
        };
    }

    Transform CreateCube()
    {
        GameObject newCube = Instantiate(cubePrefab);
        newCube.name = "Cube";
        return newCube.transform;
    }

    void ForeachSpot(System.Action<Spot> action) {
        if (spots != null && spots.Length > 0) {
            foreach (var s in spots) {
                action.Invoke(s);
            }
        }
    }

    public void ResetScene() {
        UnselectSpots();
        ForeachSpot(s => {
            s.cube.localPosition = new Vector3(0, 0.7f, 0);
            s.cube.localEulerAngles = Vector3.zero;
        });
        lightConeLight.color = lightColors.Get("neutral");
    }

    public void ColorizeSpots() {
        ForeachSpot(s => {
            var color = CoreQuiz.a.RandomColor;
            s.color = color;
            s.cubeRenderer.material.color = color.color;
        });
    }

    #endregion

    #region Lightspot

    void UpdateLightCone()
    {
        lightCone.rotation = Quaternion.Lerp(lightCone.rotation, rotationGoal, Time.deltaTime * rotationSmooth);
    }

    void FocusOnSpot(int num)
    {
        FocusOnSpot(spots[num]);
    }

    void FocusOnSpot(Spot spot)
    {
        rotationGoal = Quaternion.LookRotation(spot.spotPlace.position - lightCone.position, Vector3.up);
    }

    void SelectSpot(Spot spot) {
        selectedSpot = spot;
    }

    public void UnselectSpots() {
        selectedSpot = null;
    }

    #endregion

    void CheckMouse() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 20f))
        {
            var spot = GetSpotByCube(hit.collider);
            if (spot != null) {
                FocusOnSpot(spot);
            }
        }
    }

    void CheckClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 20f))
        {
            var spot = GetSpotByCube(hit.collider);
            if (spot != null) {
                SelectSpot(spot);
                bool isRight = CoreQuiz.a.CheckAnswer(spot);
                if (isRight) {
                    lightConeLight.color = lightColors.Get("right");
                    text.text = "Exactement!";
                } else {
                    lightConeLight.color = lightColors.Get("wrong");
                    text.text = "Pas du tout.";
                }
            }
        }
    }

    Spot GetSpotByCube(Collider cube) {
        return spots.First(c => {return c.cubeCollider == cube;});
    }

    void UpdateSpots() {
        if (selectedSpot != null)
            selectedSpot.cube.Rotate(new Vector3(0, 5, 0), Space.Self);
    }

    [System.Serializable]
    public class Spot
    {
        public Transform spotPlace;
        public Transform cube;
        public MeshRenderer cubeRenderer;
        public Collider cubeCollider;
        public ColorItem color;
    }

}
