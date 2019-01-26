using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeformationMesh : MonoBehaviour
{
    [Header("UI Элементы")]
    public Toggle UpDeformToggle;
    public Toggle DownDeformToggle;
    public Toggle GreenToggle;
    public Toggle YellowToggle;
    public Toggle BlueToggle;
    public Slider RadiusSlider;
    public Slider PowerSlider;

    Color[] colors;
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    float power = 0.4f;
    float Radius;
    bool isColorAccess=true;

    private void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = new Vector3[mesh.vertices.Length];
        triangles = new int[mesh.triangles.Length];
    }

    private void Update()
    {

        if (Input.GetMouseButton(0))
        {
            // Проверка на наличие UI элементов в точке
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                // Деформация меша
                Deform();
                // Обновление меша
                UpdateMesh();
            }
        }
    }

    void Deform()
    {
        if (isColorAccess)
        {
            colors = new Color[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                colors[i] = Color.green;
            }
            isColorAccess = false;
        }
        vertices = mesh.vertices;
        triangles = mesh.triangles;
        for (int i = 0; i < vertices.Length; i++)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 vertX = new Vector3(vertices[i].x,0, 0);
                Vector3 hitX = new Vector3(hit.point.x, 0, 0);
                
                Vector3 vertZ = new Vector3(0, 0, vertices[i].z);
                Vector3 hitZ = new Vector3(0, 0, hit.point.z);

                if (Vector3.Distance(vertX, hitX) <= RadiusSlider.value)
                {
                    if (Vector3.Distance(vertZ, hitZ) <= RadiusSlider.value)
                    {
                        if (GreenToggle.isOn)
                            colors[i] = Color.green;
                        if(YellowToggle.isOn)
                            colors[i] = Color.yellow;
                        if (BlueToggle.isOn)
                            colors[i] = Color.blue;

                        if (UpDeformToggle.isOn)
                            vertices[i] += new Vector3(0, PowerSlider.value, 0) * Time.deltaTime;
                        else if (DownDeformToggle.isOn)
                            vertices[i] += new Vector3(0, -PowerSlider.value, 0) * Time.deltaTime;
                    }
                }
               

            }
            if (vertices[i].y > 1)
                vertices[i].y = 1;
            if (vertices[i].y < -1)
                vertices[i].y = -1;
        }
    }
    void UpdateMesh()
    {
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}
