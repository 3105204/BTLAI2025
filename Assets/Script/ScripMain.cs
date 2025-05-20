using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class ScripMain : MonoBehaviour
{
    public GameObject MeshSize;
    public InputField InputMeshSizeLine;
    public InputField InputMeshSizeGrow;
    public InputField InputStatus;
    public int MsizeLine;
    public int MsizeGrow;
    public GameObject LineMesh;
    public int[,] Mesh;
    public int Msize;
    public int x, y;
    public GameObject result;
    static readonly int[] dx = { -1, 0, 1, 0 };
    static readonly int[] dy = { 0, 1, 0, -1 };
    (int x, int y) start = (0, 0);
    (int x, int y) end = (3, 3);
    public GameObject PosStartEnd;
    public InputField StartX;
    public InputField StartY;
    public InputField EndX;
    public InputField EndY;
    public Text pathLength;
    public Text pathDetail;
    public GameObject Notification;
    class Node
    {
        public int X { get; set; }
        public int Y { get; set; }
        public List<(int, int)> Path { get; set; }

        public Node(int x, int y, List<(int, int)> path)
        {
            X = x;
            Y = y;
            Path = new List<(int, int)>(path);
            Path.Add((x, y));
        }
    }
    static (int distance, List<(int, int)> path) FindShortestPath(int[,] grid, (int x, int y) start, (int x, int y) end)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        if (grid[start.x, start.y] == 1 || grid[end.x, end.y] == 1)
            return (-1, null);

        bool[,] visited = new bool[rows, cols];
        Queue<Node> queue = new Queue<Node>();

        queue.Enqueue(new Node(start.x, start.y, new List<(int, int)>()));
        visited[start.x, start.y] = true;

        while (queue.Count > 0)
        {
            Node current = queue.Dequeue();

            if (current.X == end.x && current.Y == end.y)
                return (current.Path.Count - 1, current.Path);

            for (int i = 0; i < 4; i++)
            {
                int newX = current.X + dx[i];
                int newY = current.Y + dy[i];

                if (newX >= 0 && newX < rows && newY >= 0 && newY < cols
                    && !visited[newX, newY] && grid[newX, newY] == 0)
                {
                    visited[newX, newY] = true;
                    queue.Enqueue(new Node(newX, newY, current.Path));
                }
            }
        }

        return (-1, null);
    }
    void Start()
    {
        MeshSize.SetActive(true);
    }
    public void ClickAfterMeshSize()
    {
        MsizeLine = int.Parse(InputMeshSizeLine.text);
        MsizeGrow = int.Parse(InputMeshSizeGrow.text);
        InputMeshSizeGrow.text = "";
        InputMeshSizeLine.text = "";
        Msize = MsizeGrow * MsizeLine;
        Mesh = new int[MsizeLine,MsizeGrow];
        MeshSize.SetActive(false);
        PosStartEnd.SetActive(true);
    }
    public void ClickPosStartEnd()
    {
        start.x = int.Parse(StartX.text);
        start.y = int.Parse(StartY.text);
        end.x = int.Parse(EndX.text);
        end.y = int.Parse(EndY.text);
        Debug.Log(start+" "+end);
        if (start.x < 0 || start.x >= MsizeLine)
        {
            Notification.SetActive(true);
        }
        else if (start.y < 0 || start.y >= MsizeGrow)
        {
            Notification.SetActive(true);
        }
        else if (end.x < 0 || end.x >= MsizeLine)
        {
            Notification.SetActive(true);
        }
        else if (end.y < 0 || end.y >= MsizeGrow)
        {
            Notification.SetActive(true);
        }
        else
        {
            StartX.text = "";
            StartY.text = "";
            EndX.text = "";
            EndY.text = "";
            PosStartEnd.SetActive(false);
            LineMesh.SetActive(true);
        }
    }
    public void ClickAfterStatus()
    {
        if (InputStatus.text != "0" && InputStatus.text != "1")
        {
            Notification.SetActive(true);
        }
        else
        {
            if (Msize > 0)
            {
                Mesh[x, y] = int.Parse(InputStatus.text);
                Debug.Log(Mesh[x, y]);
                y++;
                if (y >= MsizeGrow)
                {
                    x++;
                    y = 0;
                }
                InputStatus.text = "";
                Msize--;
            }
            if (Msize == 0)
            {
                String res = "";
                var (distance, path) = FindShortestPath(Mesh, start, end);
                Debug.Log(distance + " " + path);
                if (distance > 0)
                {

                    pathLength.text = distance.ToString();
                    foreach (var point in path)
                    {
                        if (point.Item1 == end.x && point.Item2 == end.y)
                        {
                            res += $"({point.Item1}, {point.Item2})";
                        }
                        else
                            res += $"({point.Item1}, {point.Item2})->";
                    }
                    pathDetail.text = res;
                }
                else
                {
                    pathLength.text = "NULL";
                    pathDetail.text = "NULL";
                }
                LineMesh.SetActive(false);
                result.SetActive(true);
            }
        }
    }
    public void ClickNew()
    {
        result.SetActive(false);
        MeshSize.SetActive(true);
    }
    public void QuitApplication()
    {
        Debug.Log("Đang thoát ứng dụng...");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    public void ClickClose()
    {
        Notification.SetActive(false);
    }
}
