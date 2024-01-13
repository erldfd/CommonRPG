using UnityEditor;
using UnityEngine;
using System.Collections.Generic; // List�� ����ϱ� ���� �߰�

public class CustomWindow : EditorWindow
{
    [MenuItem("Window/My CustomWindow")]
    public static void ShowWindow()
    {
        // â�� ���� �Լ�
        GetWindow(typeof(CustomWindow));
    }

    // �������� ��ġ�� ũ�⸦ ������ ����
    private Rect window1 = new Rect(10, 10, 300, 200); // �������� ũ�⸦ ����
    private Rect window2 = new Rect(300, 10, 100, 100);

    // �Է� �ʵ��� �ؽ�Ʈ�� ������ ����Ʈ
    private List<string> inputTexts = new List<string>();

    private void OnGUI()
    {
        // â�� GUI ��Ҹ� �׸��� �Լ�
        // BeginWindows�� EndWindows ���̿� �����츦 �׸�
        BeginWindows();
        // ���� ������ ��� ������ ���������� ����
        GUI.backgroundColor = Color.red;
        // ���� ������ �ڽ��� �׸�
        // �ڽ��� ������ �� ���ڿ�
        GUI.Box(new Rect(0, 0, position.width / 2, position.height), "");
        // ���� ������ ù ��° �����츦 �׸�
        // �������� ��ġ�� ũ��� window1 ������ ����� ���� ����
        // �������� ������ "Window 1"
        // �������� ������ �׸��� �Լ��� WindowFunction1
        window1 = GUI.Window(0, window1, WindowFunction1, "Window 1");
        // ������ ������ ��� ������ �Ķ������� ����
        GUI.backgroundColor = Color.blue;
        // ������ ������ �ڽ��� �׸�
        // �ڽ��� ������ �� ���ڿ�
        GUI.Box(new Rect(position.width / 2, 0, position.width / 2, position.height), "");
        // ������ ������ �� ��° �����츦 �׸�
        // �������� ��ġ�� ũ��� window2 ������ ����� ���� ����
        // �������� ������ "Window 2"
        // �������� ������ �׸��� �Լ��� WindowFunction2
        window2 = GUI.Window(1, window2, WindowFunction2, "Window 2");
        
        EndWindows();
    }

    // ù ��° �������� ������ �׸��� �Լ�
    private void WindowFunction1(int windowID)
    {
        
        // ������ �ȿ� �ؽ�Ʈ�� �׸�
        GUI.Label(new Rect(10, 20, 80, 20), "Hello, world!");
        // ������ �ȿ� ��ǲ �ʵ带 �߰��ϴ� ��ư�� �׸�
        // ��ư�� ������ ����Ʈ�� ���ο� �ؽ�Ʈ�� �߰���
        if (GUI.Button(new Rect(10, 80, 40, 20), "+"))
        {
            inputTexts.Add("Enter text here");
        }
        // ������ �ȿ� ��ǲ �ʵ带 �����ϴ� ��ư�� �׸�
        // ��ư�� ������ ����Ʈ�� ������ �ؽ�Ʈ�� ������
        if (GUI.Button(new Rect(50, 80, 40, 20), "-"))
        {
            if (inputTexts.Count > 0)
            {
                inputTexts.RemoveAt(inputTexts.Count - 1);
            }
        }
        // ����Ʈ�� ����� �ؽ�Ʈ���� ��ǲ �ʵ�� �׸�
        // ��ǲ �ʵ��� �ؽ�Ʈ�� ����Ǹ� ����Ʈ�� �ݿ���
        for (int i = 0; i < inputTexts.Count; i++)
        {
            inputTexts[i] = GUI.TextField(new Rect(10, 110 + i * 40, 80, 20), inputTexts[i]); // ��ǲ�ʵ��� ������ �ø�
        }
        // �������� �ּ� ũ��� �ִ� ũ�⸦ ������
        minSize = new Vector2(100, 100);
        //maxSize = new Vector2(500, 500);
        // �������� ���� �ϴ� �𼭸��� ũ�� ���� �ڵ��� �׸�
        // �ڵ��� ������ �巡���ϸ� �������� ũ�Ⱑ �����
        float mouseDeltaX = Event.current.delta.x;
        float mouseDeltaY = Event.current.delta.y;

        if (GUI.RepeatButton(new Rect(window1.width - 20, window1.height - 20, 20, 20), "��"))
        {
            Debug.Log($"{Event.current.delta} : {mouseDeltaX}, {mouseDeltaY}");
            window1.width += 20;
            window1.height += 40;
        }
        // �����츦 �巡���� �� �ֵ��� ��
        GUI.DragWindow(new Rect(0, 0, position.width / 2, position.height));
    }

    // �� ��° �������� ������ �׸��� �Լ�
    private void WindowFunction2(int windowID)
    {
        GUI.Button(new Rect(10, 80, 40, 20), "+");
        // ������ �ȿ� ��ư�� �׸�
        // ��ư�� ������ �ֿܼ� �޽����� �����
        if (GUI.Button(new Rect(10, 20, 80, 20), "Click me"))
        {
            Debug.Log("Clicked!");
        }
        // �����츦 �巡���� �� �ֵ��� ��
        GUI.DragWindow();
    }
}
