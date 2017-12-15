using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeformationMesh : MonoBehaviour
{
    [Header("UI Элементы")]
    [Tooltip("Переключатель деформации вверх ")] public Toggle UpDeformToggle;
    [Tooltip("Переключатель деформации вниз ")]  public Toggle DownDeformToggle;
    [Tooltip("Переключатель зелёного цвета ")]   public Toggle GreenToggle;
    [Tooltip("Переключатель жёлтого цвета ")]    public Toggle YellowToggle;
    [Tooltip("Переключатель синего цвета ")]     public Toggle BlueToggle;
    [Tooltip("Ползунок радиуса ")]               public Slider RadiusSlider;
    [Tooltip("Ползунок силы ")]                  public Slider PowerSlider;

    // Приватные поля
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
        // Изначальные настройки для устранения возможных ошибок
        if (isColorAccess)
        {
            colors = new Color[vertices.Length];
            // Окрашивание всего меша в зелёный цвет
            for (int i = 0; i < vertices.Length; i++)
            {
                colors[i] = Color.green;
            }
            isColorAccess = false;
        }
        // Получение данных о вершинах и треугольниках меша
        vertices = mesh.vertices;
        triangles = mesh.triangles;
        // Перебор всех вершин меша и манипуляции с ними
        for (int i = 0; i < vertices.Length; i++)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                // Берём координаты по X
                Vector3 vertX = new Vector3(vertices[i].x,0, 0);
                Vector3 hitX = new Vector3(hit.point.x, 0, 0);
                // Берём координаты по Z
                Vector3 vertZ = new Vector3(0, 0, vertices[i].z);
                Vector3 hitZ = new Vector3(0, 0, hit.point.z);
                // Находим ближайшие вершины к указателю мыши
                // Сравнение по X
                if (Vector3.Distance(vertX, hitX) <= RadiusSlider.value)
                {
                    // Сравнение по Z
                    if (Vector3.Distance(vertZ, hitZ) <= RadiusSlider.value)
                    {
                        // Изменение цвета меша
                        if (GreenToggle.isOn)
                            colors[i] = Color.green;
                        if(YellowToggle.isOn)
                            colors[i] = Color.yellow;
                        if (BlueToggle.isOn)
                            colors[i] = Color.blue;
                        // Деформация вершин по оси Y
                        if (UpDeformToggle.isOn)
                            vertices[i] += new Vector3(0, PowerSlider.value, 0) * Time.deltaTime;
                        else if (DownDeformToggle.isOn)
                            vertices[i] += new Vector3(0, -PowerSlider.value, 0) * Time.deltaTime;
                    }
                }
               

            }
            // Ограничение высоты вершин
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